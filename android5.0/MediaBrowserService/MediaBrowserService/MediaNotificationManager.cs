using System;
using Android.Content;
using Android.App;
using Android.Media.Session;
using Android.Media;
using Android.Graphics;
using System.Threading.Tasks;

namespace MediaBrowserService
{
	public class MediaNotificationManager : BroadcastReceiver
	{
		static readonly string Tag = LogHelper.MakeLogTag(typeof(MediaNotificationManager));

		const int NotificationId = 412;
		const int RequestCode = 100;

		public const string ActionPause = "com.example.android.mediabrowserservice.pause";
		public const string ActionPlay = "com.example.android.mediabrowserservice.play";
		public const string ActionPrev = "com.example.android.mediabrowserservice.prev";
		public const string ActionNext = "com.example.android.mediabrowserservice.next";

		readonly MusicService service;
		MediaSession.Token sessionToken;
		MediaController controller;
		MediaController.TransportControls transportControls;

		PlaybackState playbackState;
		MediaMetadata metadata;

		readonly NotificationManager notificationManager;

		PendingIntent pauseIntent;
		PendingIntent playIntent;
		PendingIntent previousIntent;
		PendingIntent nextIntent;

		int notificationColor;

		bool started;

		public MediaNotificationManager(MusicService serv) {
			service = serv;
			UpdateSessionToken();

			notificationColor = ResourceHelper.GetThemeColor(service,
				Android.Resource.Attribute.ColorPrimary, Color.DarkGray);

			notificationManager = (NotificationManager) service
				.GetSystemService(Context.NotificationService);

			var pkg = service.PackageName;
			pauseIntent = PendingIntent.GetBroadcast(service, RequestCode,
				new Intent(ActionPause).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
			playIntent = PendingIntent.GetBroadcast(service, RequestCode,
				new Intent(ActionPlay).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
			previousIntent = PendingIntent.GetBroadcast(service, RequestCode,
				new Intent(ActionPrev).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
			nextIntent = PendingIntent.GetBroadcast(service, RequestCode,
				new Intent(ActionNext).SetPackage(pkg), PendingIntentFlags.CancelCurrent);

			notificationManager.CancelAll();

			mCb.OnPlaybackStateChangedImpl = (state) => {
				playbackState = state;
				LogHelper.Debug (Tag, "Received new playback state", state);
				if (state != null && (state.State == PlaybackStateCode.Stopped ||
				    state.State == PlaybackStateCode.None)) {
					stopNotification ();
				} else {
					Notification notification = CreateNotification ();
					if (notification != null) {
						notificationManager.Notify (NotificationId, notification);
					}
				}
			};
			mCb.OnMetadataChangedImpl = (meta) => {
				metadata = meta;
				LogHelper.Debug (Tag, "Received new metadata ", metadata);
				var notification = CreateNotification ();
				if (notification != null) {
					notificationManager.Notify (NotificationId, notification);
				}
			};
			mCb.OnSessionDestroyedImpl = () => {
				LogHelper.Debug (Tag, "Session was destroyed, resetting to the new session token");
				UpdateSessionToken ();
			};
		}

		public void StartNotification() {
			if (!started) {
				metadata = controller.Metadata;
				playbackState = controller.PlaybackState;

				// The notification must be updated after setting started to true
				var notification = CreateNotification();
				if (notification != null) {
					controller.RegisterCallback(mCb);
					var filter = new IntentFilter();
					filter.AddAction(ActionNext);
					filter.AddAction(ActionPause);
					filter.AddAction(ActionPlay);
					filter.AddAction(ActionPrev);
					service.RegisterReceiver(this, filter);

					service.StartForeground(NotificationId, notification);
					started = true;
				}
			}
		}

		public void stopNotification() {
			if (started) {
				started = false;
				controller.UnregisterCallback(mCb);
				try {
					notificationManager.Cancel(NotificationId);
					service.UnregisterReceiver(this);
				} catch (ArgumentException) {
					// ignore if the receiver is not registered.
				}
				service.StopForeground(true);
			}
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var action = intent.Action;
			LogHelper.Debug(Tag, "Received intent with action " + action);
			switch (action) {
			case ActionPause:
				transportControls.Pause();
				break;
			case ActionPlay:
				transportControls.Play();
				break;
			case ActionNext:
				transportControls.SkipToNext();
				break;
			case ActionPrev:
				transportControls.SkipToPrevious();
				break;
			default:
				LogHelper.Warn (Tag, "Unknown intent ignored. Action=", action);
				break;
			}
		}

		void UpdateSessionToken() {
			var freshToken = service.SessionToken;
			if (sessionToken == null || sessionToken != freshToken) {
				if (controller != null) {
					controller.UnregisterCallback(mCb);
				}
				sessionToken = freshToken;
				controller = new MediaController(service, sessionToken);
				transportControls = controller.GetTransportControls();
				if (started) {
					controller.RegisterCallback(mCb);
				}
			}
		}

		PendingIntent CreateContentIntent() {
			var openUI = new Intent(service, typeof(MusicPlayerActivity));
			openUI.SetFlags(ActivityFlags.SingleTop);
			return PendingIntent.GetActivity(service, RequestCode, openUI,
				PendingIntentFlags.CancelCurrent);
		}

		class MediaCallback : MediaController.Callback {
			public Action<PlaybackState> OnPlaybackStateChangedImpl {get;set;}
			public Action<MediaMetadata> OnMetadataChangedImpl {get;set;}
			public Action OnSessionDestroyedImpl {get;set;}
			public override void OnPlaybackStateChanged (PlaybackState state)
			{
				OnPlaybackStateChangedImpl (state);
			}
			public override void OnMetadataChanged (MediaMetadata meta)
			{
				OnMetadataChangedImpl (meta);
			}
			public override void OnSessionDestroyed ()
			{
				base.OnSessionDestroyed();
				OnSessionDestroyedImpl ();
			}
		}

		MediaCallback mCb = new MediaCallback();

		Notification CreateNotification() {
			LogHelper.Debug(Tag, "updateNotificationMetadata. mMetadata=" + metadata);
			if (metadata == null || playbackState == null) {
				return null;
			}

			var notificationBuilder = new Notification.Builder(service);
			int playPauseButtonPosition = 0;

			// If skip to previous action is enabled
			if ((playbackState.Actions & PlaybackState.ActionSkipToPrevious) != 0) {
				notificationBuilder.AddAction(Resource.Drawable.ic_skip_previous_white_24dp,
					service.GetString(Resource.String.label_previous), previousIntent);
				
				playPauseButtonPosition = 1;
			}

			AddPlayPauseAction(notificationBuilder);

			// If skip to next action is enabled
			if ((playbackState.Actions & PlaybackState.ActionSkipToNext) != 0) {
				notificationBuilder.AddAction(Resource.Drawable.ic_skip_next_white_24dp,
					service.GetString(Resource.String.label_next), nextIntent);
			}

			var description = metadata.Description;

			var fetchArtUrl = string.Empty;
			Bitmap art = null;
			if (description.IconUri != null) {
				String artUrl = description.IconUri.ToString();
				art = AlbumArtCache.Instance.GetBigImage(artUrl);
				if (art == null) {
					fetchArtUrl = artUrl;
					art = BitmapFactory.DecodeResource(service.Resources,
						Resource.Drawable.ic_default_art);
				}
			}

			notificationBuilder
				.SetStyle(new Notification.MediaStyle()
					.SetShowActionsInCompactView(
						new []{playPauseButtonPosition})  // show only play/pause in compact view
					.SetMediaSession(sessionToken))
				.SetColor(notificationColor)
				.SetSmallIcon(Resource.Drawable.ic_notification)
				.SetVisibility(NotificationVisibility.Public)
				.SetUsesChronometer(true)
				.SetContentIntent(CreateContentIntent())
				.SetContentTitle(description.Title)
				.SetContentText(description.Subtitle)
				.SetLargeIcon(art);

			SetNotificationPlaybackState(notificationBuilder);
			if (fetchArtUrl != null) {
				FetchBitmapFromURL(fetchArtUrl, notificationBuilder);
			}

			return notificationBuilder.Build();
		}

		void AddPlayPauseAction(Notification.Builder builder) {
			LogHelper.Debug(Tag, "updatePlayPauseAction");
			string label;
			int icon;
			PendingIntent intent;
			if (playbackState.State == PlaybackStateCode.Playing) {
				label = service.GetString(Resource.String.label_pause);
				icon = Resource.Drawable.ic_pause_white_24dp;
				intent = pauseIntent;
			} else {
				label = service.GetString(Resource.String.label_play);
				icon = Resource.Drawable.ic_play_arrow_white_24dp;
				intent = playIntent;
			}
			builder.AddAction(new Notification.Action(icon, label, intent));
		}

		void SetNotificationPlaybackState(Notification.Builder builder) {
			var beginningOfTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			LogHelper.Debug(Tag, "updateNotificationPlaybackState. mPlaybackState=" + playbackState);
			if (playbackState == null || !started) {
				LogHelper.Debug(Tag, "updateNotificationPlaybackState. cancelling notification!");
				service.StopForeground(true);
				return;
			}
			if (playbackState.State == PlaybackStateCode.Playing
				&& playbackState.Position >= 0) {
				var timespan = ((long)(DateTime.UtcNow - beginningOfTime).TotalMilliseconds) - playbackState.Position;
				LogHelper.Debug(Tag, "updateNotificationPlaybackState. updating playback position to ",
					timespan / 1000, " seconds");
				builder
					.SetWhen(timespan)
					.SetShowWhen(true)
					.SetUsesChronometer(true);
			} else {
				LogHelper.Debug(Tag, "updateNotificationPlaybackState. hiding playback position");
				builder
					.SetWhen(0)
					.SetShowWhen(false)
					.SetUsesChronometer(false);
			}

			// Make sure that the notification can be dismissed by the user when we are not playing:
					builder.SetOngoing(playbackState.State == PlaybackStateCode.Playing);
		}

		void FetchBitmapFromURL(string bitmapUrl, Notification.Builder builder) {
			AlbumArtCache.Instance.Fetch (bitmapUrl, new AlbumArtCache.FetchListener () {
				OnFetched = (artUrl, bitmap, icon) => {
					if (metadata != null && metadata.Description != null &&
					    artUrl == metadata.Description.IconUri.ToString ()) {
						LogHelper.Debug (Tag, "fetchBitmapFromURLAsync: set bitmap to ", artUrl);
						builder.SetLargeIcon (bitmap);
						notificationManager.Notify (NotificationId, builder.Build ());
					}
				}
			});
		}
	}
}

