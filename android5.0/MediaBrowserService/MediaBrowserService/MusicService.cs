using System;
using System.Collections.Generic;
using Android.Media.Session;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Media.Browse;
using Android.Runtime;

namespace MediaBrowserService
{
	[Service (Exported = true)]
	[IntentFilter(new[]{"android.media.browse.MediaBrowserService"})]
	public class MusicService : Android.Service.Media.MediaBrowserService, Playback.ICallback
	{
		public const string ActionCmd = "com.example.android.mediabrowserservice.ACTION_CMD";
		public const string CmdName = "CMD_NAME";
		public const string CmdPause = "CMD_PAUSE";

		static readonly string Tag = LogHelper.MakeLogTag (typeof(MusicService));
		const string CustomActionThumbsUp = "com.example.android.mediabrowserservice.THUMBS_UP";
		const int StopDelay = 30000;

		MusicProvider musicProvider;
		MediaSession session;
		List<MediaSession.QueueItem> playingQueue;
		int currentIndexOnQueue;
		MediaNotificationManager mediaNotificationManager;
		bool serviceStarted;
		DelayedStopHandler delayedStopHandler;
		Playback playback;
		PackageValidator packageValidator;

		public MusicService ()
		{
			delayedStopHandler = new DelayedStopHandler (this);
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			LogHelper.Debug (Tag, "onCreate");

			playingQueue = new List<MediaSession.QueueItem> ();
			musicProvider = new MusicProvider ();
			packageValidator = new PackageValidator (this);

			session = new MediaSession (this, "MusicService");
			SessionToken = session.SessionToken;
			var mediaCallback = new MediaSessionCallback ();
			mediaCallback.OnPlayImpl = () => {
				LogHelper.Debug (Tag, "play");

				if (playingQueue == null || playingQueue.Count != 0) {
					playingQueue = new List<MediaSession.QueueItem> (QueueHelper.GetRandomQueue (musicProvider));
					session.SetQueue (playingQueue);
					session.SetQueueTitle (GetString (Resource.String.random_queue_title));
					currentIndexOnQueue = 0;
				}

				if (playingQueue != null && playingQueue.Count != 0) {
					HandlePlayRequest ();
				}
			};
			mediaCallback.OnSkipToQueueItemImpl = (id) => {
				LogHelper.Debug (Tag, "OnSkipToQueueItem:" + id);

				if (playingQueue != null && playingQueue.Count != 0) {
					currentIndexOnQueue = QueueHelper.GetMusicIndexOnQueue (playingQueue, id);
					HandlePlayRequest ();
				}
			};
			mediaCallback.OnSeekToImpl = (pos) => {
				LogHelper.Debug (Tag, "onSeekTo:", pos);
				playback.SeekTo ((int)pos);
			};
			mediaCallback.OnPlayFromMediaIdImpl = (mediaId, extras) => {
				LogHelper.Debug (Tag, "playFromMediaId mediaId:", mediaId, "  extras=", extras);

				playingQueue = QueueHelper.GetPlayingQueue (mediaId, musicProvider);
				session.SetQueue (playingQueue);
				var queueTitle = GetString (Resource.String.browse_musics_by_genre_subtitle,
					                 MediaIDHelper.ExtractBrowseCategoryValueFromMediaID (mediaId));
				session.SetQueueTitle (queueTitle);

				if (playingQueue != null && playingQueue.Count != 0) {
					currentIndexOnQueue = QueueHelper.GetMusicIndexOnQueue (playingQueue, mediaId);

					if (currentIndexOnQueue < 0) {
						LogHelper.Error (Tag, "playFromMediaId: media ID ", mediaId,
							" could not be found on queue. Ignoring.");
					} else {
						HandlePlayRequest ();
					}
				}
			};
			mediaCallback.OnPauseImpl = () => {
				LogHelper.Debug (Tag, "pause. current state=" + playback.State);
				HandlePauseRequest ();
			};
			mediaCallback.OnStopImpl = () => {
				LogHelper.Debug (Tag, "stop. current state=" + playback.State);
				HandleStopRequest (null);
			};
			mediaCallback.OnSkipToNextImpl = () => {
				LogHelper.Debug (Tag, "skipToNext");
				currentIndexOnQueue++;
				if (playingQueue != null && currentIndexOnQueue >= playingQueue.Count) {
					currentIndexOnQueue = 0;
				}
				if (QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
					HandlePlayRequest ();
				} else {
					LogHelper.Error (Tag, "skipToNext: cannot skip to next. next Index=" +
					currentIndexOnQueue + " queue length=" +
					(playingQueue == null ? "null" : playingQueue.Count.ToString ()));
					HandleStopRequest ("Cannot skip");
				}
			};
			mediaCallback.OnSkipToPreviousImpl = () => {
				LogHelper.Debug (Tag, "skipToPrevious");
				currentIndexOnQueue--;
				if (playingQueue != null && currentIndexOnQueue < 0) {
					currentIndexOnQueue = 0;
				}
				if (QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
					HandlePlayRequest ();
				} else {
					LogHelper.Error (Tag, "skipToPrevious: cannot skip to previous. previous Index=" +
					currentIndexOnQueue + " queue length=" +
					(playingQueue == null ? "null" : playingQueue.Count.ToString ()));
					HandleStopRequest ("Cannot skip");
				}
			};
			mediaCallback.OnCustomActionImpl = (action, extras) => {
				if (CustomActionThumbsUp == action) {
					LogHelper.Info (Tag, "onCustomAction: favorite for current track");
					var track = GetCurrentPlayingMusic ();
					if (track != null) {
						var musicId = track.GetString (MediaMetadata.MetadataKeyMediaId);
						musicProvider.SetFavorite (musicId, !musicProvider.IsFavorite (musicId));
					}
					UpdatePlaybackState (null);
				} else {
					LogHelper.Error (Tag, "Unsupported action: ", action);
				}
			};
			mediaCallback.OnPlayFromSearchImpl = (query, extras) => {
				LogHelper.Debug (Tag, "playFromSearch  query=", query);

				if (string.IsNullOrEmpty (query)) {
					playingQueue = new List<MediaSession.QueueItem> (QueueHelper.GetRandomQueue (musicProvider));
				} else {
					playingQueue = new List<MediaSession.QueueItem> (QueueHelper.GetPlayingQueueFromSearch (query, musicProvider));
				}

				LogHelper.Debug (Tag, "playFromSearch  playqueue.length=" + playingQueue.Count);
				session.SetQueue (playingQueue);

				if (playingQueue != null && playingQueue.Count != 0) {
					currentIndexOnQueue = 0;

					HandlePlayRequest ();
				} else {
					HandleStopRequest (GetString (Resource.String.no_search_results));
				}
			};
			session.SetCallback (mediaCallback);
			session.SetFlags (MediaSessionFlags.HandlesMediaButtons |
			MediaSessionFlags.HandlesTransportControls);

			playback = new Playback (this, musicProvider);
			playback.State = PlaybackStateCode.None;
			playback.Callback = this;
			playback.Start ();

			var context = ApplicationContext;
			var intent = new Intent (context, typeof(MusicPlayerActivity));
			var pi = PendingIntent.GetActivity (context, 99 /*request code*/,
				         intent, PendingIntentFlags.UpdateCurrent);
			session.SetSessionActivity (pi);

			var extraBundle = new Bundle ();
			CarHelper.SetSlotReservationFlags (extraBundle, true, true, true);
			session.SetExtras (extraBundle);

			UpdatePlaybackState (null);

			mediaNotificationManager = new MediaNotificationManager (this);
		}

		[Obsolete ("deprecated")]
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			if (intent != null) {
				var action = intent.Action;
				var command = intent.GetStringExtra (CmdName);
				if (ActionCmd == action) {
					if (CmdPause == command) {
						if (playback != null && playback.IsPlaying) {
							HandlePauseRequest ();
						}
					}
				}
			}
			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			LogHelper.Debug (Tag, "onDestroy");
			HandleStopRequest (null);

			delayedStopHandler.RemoveCallbacksAndMessages (null);
			session.Release ();
		}

		public override BrowserRoot OnGetRoot (string clientPackageName, int clientUid, Bundle rootHints)
		{
			LogHelper.Debug (Tag, "OnGetRoot: clientPackageName=" + clientPackageName,
				"; clientUid=" + clientUid + " ; rootHints=", rootHints);
			
			if (!packageValidator.IsCallerAllowed (this, clientPackageName, clientUid)) {
				LogHelper.Warn (Tag, "OnGetRoot: IGNORING request from untrusted package "
				+ clientPackageName);
				return null;
			}
			if (CarHelper.IsValidCarPackage (clientPackageName)) {
				// Optional: if your app needs to adapt ads, music library or anything else that
				// needs to run differently when connected to the car, this is where you should handle
				// it.
			}
			return new BrowserRoot (MediaIDHelper.MediaIdRoot, null);
		}

		public override async void OnLoadChildren (string parentId, Result result)
		{
			if (!musicProvider.IsInitialized) {
				result.Detach ();

				await musicProvider.RetrieveMediaAsync (success => {
					if (success) {
						LoadChildrenImpl (parentId, result);
					} else {
						UpdatePlaybackState (GetString (Resource.String.error_no_metadata));
						result.SendResult (new JavaList<MediaBrowser.MediaItem>());
					}
				});

			} else {
				LoadChildrenImpl (parentId, result);
			}
		}

		void LoadChildrenImpl (string parentId, Result result)
		{
			LogHelper.Debug (Tag, "OnLoadChildren: parentMediaId=", parentId);

			var mediaItems = new JavaList<MediaBrowser.MediaItem> ();

			if (MediaIDHelper.MediaIdRoot == parentId) {
				LogHelper.Debug (Tag, "OnLoadChildren.ROOT");
				mediaItems.Add (new MediaBrowser.MediaItem (
					new MediaDescription.Builder ()
					.SetMediaId (MediaIDHelper.MediaIdMusicsByGenre)
					.SetTitle (GetString (Resource.String.browse_genres))
					.SetIconUri (Android.Net.Uri.Parse ("android.resource://" +
					"com.example.android.mediabrowserservice/drawable/ic_by_genre"))
					.SetSubtitle (GetString (Resource.String.browse_genre_subtitle))
					.Build (), MediaItemFlags.Browsable
				));

			} else if (MediaIDHelper.MediaIdMusicsByGenre == parentId) {
				LogHelper.Debug (Tag, "OnLoadChildren.GENRES");
				foreach (var genre in musicProvider.Genres) {
					var item = new MediaBrowser.MediaItem (
						           new MediaDescription.Builder ()
						.SetMediaId (MediaIDHelper.CreateBrowseCategoryMediaID (MediaIDHelper.MediaIdMusicsByGenre, genre))
						.SetTitle (genre)
						.SetSubtitle (GetString (Resource.String.browse_musics_by_genre_subtitle, genre))
						.Build (), MediaItemFlags.Browsable
					           );
					mediaItems.Add (item);
				}
			} else if (parentId.StartsWith (MediaIDHelper.MediaIdMusicsByGenre)) {
				var genre = MediaIDHelper.GetHierarchy (parentId) [1];
				LogHelper.Debug (Tag, "OnLoadChildren.SONGS_BY_GENRE  genre=", genre);
				foreach (var track in musicProvider.GetMusicsByGenre(genre)) {
					var hierarchyAwareMediaID = MediaIDHelper.CreateMediaID (
						                            track.Description.MediaId, MediaIDHelper.MediaIdMusicsByGenre, genre);
					var trackCopy = new MediaMetadata.Builder (track)
						.PutString (MediaMetadata.MetadataKeyMediaId, hierarchyAwareMediaID)
						.Build ();
					var bItem = new MediaBrowser.MediaItem (
						            trackCopy.Description, MediaItemFlags.Playable);
					mediaItems.Add (bItem);
				}
			} else {
				LogHelper.Warn (Tag, "Skipping unmatched parentMediaId: ", parentId);
			}
			LogHelper.Debug (Tag, "OnLoadChildren sending ", mediaItems.Count,
				" results for ", parentId);
			result.SendResult (mediaItems);
		}

		class MediaSessionCallback : MediaSession.Callback
		{
			public Action OnPlayImpl { get; set; }

			public Action<long> OnSkipToQueueItemImpl { get; set; }

			public Action<long> OnSeekToImpl { get; set; }

			public Action<string, Bundle> OnPlayFromMediaIdImpl { get; set; }

			public Action OnPauseImpl { get; set; }

			public Action OnStopImpl{ get; set; }

			public Action OnSkipToNextImpl{ get; set; }

			public Action OnSkipToPreviousImpl{ get; set; }

			public Action<string, Bundle> OnCustomActionImpl { get; set; }

			public Action<string, Bundle> OnPlayFromSearchImpl { get; set; }

			public override void OnPlay ()
			{
				OnPlayImpl ();
			}

			public override void OnSkipToQueueItem (long id)
			{
				OnSkipToQueueItemImpl (id);
			}

			public override void OnSeekTo (long pos)
			{
				OnSeekToImpl (pos);
			}

			public override void OnPlayFromMediaId (string mediaId, Bundle extras)
			{
				OnPlayFromMediaIdImpl (mediaId, extras);
			}

			public override void OnPause ()
			{
				OnPauseImpl ();
			}

			public override void OnStop ()
			{
				OnStopImpl ();
			}

			public override void OnSkipToNext ()
			{
				OnSkipToNextImpl ();
			}

			public override void OnSkipToPrevious ()
			{
				OnSkipToPreviousImpl ();
			}

			public override void OnCustomAction (string action, Bundle extras)
			{
				OnCustomActionImpl (action, extras);
			}

			public override void OnPlayFromSearch (string query, Bundle extras)
			{
				OnPlayFromSearchImpl (query, extras);
			}
		}

		void HandlePlayRequest ()
		{
			LogHelper.Debug (Tag, "handlePlayRequest: mState=" + playback.State);

			delayedStopHandler.RemoveCallbacksAndMessages (null);
			if (!serviceStarted) {
				LogHelper.Verbose (Tag, "Starting service");
				// The MusicService needs to keep running even after the calling MediaBrowser
				// is disconnected. Call startService(Intent) and then stopSelf(..) when we no longer
				// need to play media.
				StartService (new Intent (ApplicationContext, typeof(MusicService)));
				serviceStarted = true;
			}

			if (!session.Active)
				session.Active = true;

			if (QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
				UpdateMetadata ();
				playback.Play (playingQueue [currentIndexOnQueue]);
			}
		}

		void HandlePauseRequest ()
		{
			LogHelper.Debug (Tag, "handlePauseRequest: mState=" + playback.State);
			playback.Pause ();
			// reset the delayed stop handler.
			delayedStopHandler.RemoveCallbacksAndMessages (null);
			delayedStopHandler.SendEmptyMessageDelayed (0, StopDelay);
		}

		void HandleStopRequest (String withError)
		{
			LogHelper.Debug (Tag, "handleStopRequest: mState=" + playback.State + " error=", withError);
			playback.Stop (true);
			// reset the delayed stop handler.
			delayedStopHandler.RemoveCallbacksAndMessages (null);
			delayedStopHandler.SendEmptyMessageDelayed (0, StopDelay);

			UpdatePlaybackState (withError);

			// service is no longer necessary. Will be started again if needed.
			StopSelf ();
			serviceStarted = false;
		}

		void UpdateMetadata ()
		{
			if (!QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
				LogHelper.Error (Tag, "Can't retrieve current metadata.");
				UpdatePlaybackState (Resources.GetString (Resource.String.error_no_metadata));
				return;
			}
			var queueItem = playingQueue [currentIndexOnQueue];
			var musicId = MediaIDHelper.ExtractMusicIDFromMediaID (
				              queueItem.Description.MediaId);
			var track = musicProvider.GetMusic (musicId);
			var trackId = track.GetString (MediaMetadata.MetadataKeyMediaId);
			if (musicId != trackId) {
				var e = new InvalidOperationException ("track ID should match musicId.");
				LogHelper.Error (Tag, "track ID should match musicId.",
					" musicId=", musicId, " trackId=", trackId,
					" mediaId from queueItem=", queueItem.Description.MediaId,
					" title from queueItem=", queueItem.Description.Title,
					" mediaId from track=", track.Description.MediaId,
					" title from track=", track.Description.Title,
					" source.hashcode from track=", track.GetString (
					MusicProvider.CustomMetadataTrackSource).GetHashCode (),
					e);
				throw e;
			}
			LogHelper.Debug (Tag, "Updating metadata for MusicID= " + musicId);
			session.SetMetadata (track);

			// Set the proper album artwork on the media session, so it can be shown in the
			// locked screen and in other places.
			if (track.Description.IconBitmap == null &&
			    track.Description.IconUri != null) {
				var albumUri = track.Description.IconUri.ToString ();
				AlbumArtCache.Instance.Fetch (albumUri, new AlbumArtCache.FetchListener {
					OnFetched = (artUrl, bitmap, icon) => {
						var qItem = playingQueue [currentIndexOnQueue];
						var trackItem = musicProvider.GetMusic (trackId);
						trackItem = new MediaMetadata.Builder (trackItem)
							.PutBitmap (MediaMetadata.MetadataKeyAlbumArt, bitmap)
							.PutBitmap (MediaMetadata.MetadataKeyDisplayIcon, icon)
							.Build ();

						musicProvider.UpdateMusic (trackId, trackItem);

						// If we are still playing the same music
						var currentPlayingId = MediaIDHelper.ExtractMusicIDFromMediaID (
							                       qItem.Description.MediaId);
						if (trackId == currentPlayingId) {
							session.SetMetadata (trackItem);
						}
					}
				});
			}
		}

		void UpdatePlaybackState (String error)
		{
			LogHelper.Debug (Tag, "updatePlaybackState, playback state=" + playback.State);
			var position = PlaybackState.PlaybackPositionUnknown;
			if (playback != null && playback.IsConnected) {
				position = playback.CurrentStreamPosition;
			}

			var stateBuilder = new PlaybackState.Builder ()
				.SetActions (GetAvailableActions ());

			SetCustomAction (stateBuilder);
			var state = playback.State;

			// If there is an error message, send it to the playback state:
			if (error != null) {
				// Error states are really only supposed to be used for errors that cause playback to
				// stop unexpectedly and persist until the user takes action to fix it.
				stateBuilder.SetErrorMessage (error);
				state = PlaybackStateCode.Error;
			}
			stateBuilder.SetState (state, position, 1.0f, SystemClock.ElapsedRealtime ());

			// Set the activeQueueItemId if the current index is valid.
			if (QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
				var item = playingQueue [currentIndexOnQueue];
				stateBuilder.SetActiveQueueItemId (item.QueueId);
			}

			session.SetPlaybackState (stateBuilder.Build ());

			if (state == PlaybackStateCode.Playing || state == PlaybackStateCode.Paused) {
				mediaNotificationManager.StartNotification ();
			}
		}

		void SetCustomAction (PlaybackState.Builder stateBuilder)
		{
			MediaMetadata currentMusic = GetCurrentPlayingMusic ();
			if (currentMusic != null) {
				// Set appropriate "Favorite" icon on Custom action:
				var musicId = currentMusic.GetString (MediaMetadata.MetadataKeyMediaId);
				var favoriteIcon = Resource.Drawable.ic_star_off;
				if (musicProvider.IsFavorite (musicId)) {
					favoriteIcon = Resource.Drawable.ic_star_on;
				}
				LogHelper.Debug (Tag, "updatePlaybackState, setting Favorite custom action of music ",
					musicId, " current favorite=", musicProvider.IsFavorite (musicId));
				stateBuilder.AddCustomAction (CustomActionThumbsUp, GetString (Resource.String.favorite),
					favoriteIcon);
			}
		}

		long GetAvailableActions ()
		{
			long actions = PlaybackState.ActionPlay | PlaybackState.ActionPlayFromMediaId |
			               PlaybackState.ActionPlayFromSearch;
			if (playingQueue == null || playingQueue.Count == 0) {
				return actions;
			}
			if (playback.IsPlaying) {
				actions |= PlaybackState.ActionPause;
			}
			if (currentIndexOnQueue > 0) {
				actions |= PlaybackState.ActionSkipToPrevious;
			}
			if (currentIndexOnQueue < playingQueue.Count - 1) {
				actions |= PlaybackState.ActionSkipToNext;
			}
			return actions;
		}

		MediaMetadata GetCurrentPlayingMusic ()
		{
			if (QueueHelper.isIndexPlayable (currentIndexOnQueue, playingQueue)) {
				var item = playingQueue [currentIndexOnQueue];
				if (item != null) {
					LogHelper.Debug (Tag, "getCurrentPlayingMusic for musicId=",
						item.Description.MediaId);
					return musicProvider.GetMusic (
						MediaIDHelper.ExtractMusicIDFromMediaID (item.Description.MediaId));
				}
			}
			return null;
		}

		public void OnCompletion ()
		{
			if (playingQueue != null && playingQueue.Count != 0) {
				// In this sample, we restart the playing queue when it gets to the end:
				currentIndexOnQueue++;
				if (currentIndexOnQueue >= playingQueue.Count) {
					currentIndexOnQueue = 0;
				}
				HandlePlayRequest ();
			} else {
				// If there is nothing to play, we stop and release the resources:
				HandleStopRequest (null);
			}
		}


		public void OnPlaybackStatusChanged (PlaybackStateCode state)
		{
			UpdatePlaybackState (null);
		}

		public void OnError (string error)
		{
			UpdatePlaybackState (error);
		}

		class DelayedStopHandler : Handler
		{
			readonly WeakReference<MusicService> weakReference;

			public DelayedStopHandler (MusicService service)
			{
				weakReference = new WeakReference<MusicService> (service);
			}

			public override void HandleMessage (Message msg)
			{
				MusicService service;
				weakReference.TryGetTarget (out service);
				if (service != null && service.playback != null) {
					if (service.playback.IsPlaying) {
						LogHelper.Debug (Tag, "Ignoring delayed stop since the media player is in use.");
						return;
					}
					LogHelper.Debug (Tag, "Stopping service with delay handler.");
					service.StopSelf ();
					service.serviceStarted = false;
				}
			}
		}
	}
}

