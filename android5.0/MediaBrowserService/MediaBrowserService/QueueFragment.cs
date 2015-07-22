using System;
using Android.App;
using Android.Widget;
using Android.Media.Browse;
using Android.Media.Session;
using System.Collections.Generic;
using Android.Content;
using System.Text;
using Android.Views;
using Android.Runtime;
using System.Linq;

namespace MediaBrowserService
{
	public class QueueFragment : Fragment
	{
		new static readonly string Tag = LogHelper.MakeLogTag (typeof(QueueFragment).Name);

		ImageButton skipNext;
		ImageButton skipPrevious;
		ImageButton playPause;

		MediaBrowser mediaBrowser;
		Android.Media.Session.MediaController.TransportControls transportControls;
		Android.Media.Session.MediaController mediaController;
		PlaybackState playbackState;

		QueueAdapter queueAdapter;

		class ConnectionCallback : MediaBrowser.ConnectionCallback
		{
			public Action OnConnectedImpl { get; set; }

			public Action OnConnectionFailedImpl { get; set; }

			public Action OnConnectionSuspendedImpl { get; set; }

			public override void OnConnected ()
			{
				OnConnectedImpl ();
			}

			public override void OnConnectionFailed ()
			{
				OnConnectionFailedImpl ();
			}

			public override void OnConnectionSuspended ()
			{
				OnConnectionSuspendedImpl ();
			}
		}

		ConnectionCallback connectionCallback = new ConnectionCallback ();

		class SessionCallback : Android.Media.Session.MediaController.Callback
		{
			public Action OnSessionDestroyedImpl { get; set; }

			public Action<PlaybackState> OnPlaybackStateChangedImpl { get; set; }

			public Action<IList<MediaSession.QueueItem>> OnQueueChangedImpl { get; set; }

			public override void OnSessionDestroyed ()
			{
				OnSessionDestroyedImpl ();
			}

			public override void OnPlaybackStateChanged (PlaybackState state)
			{
				OnPlaybackStateChangedImpl (state);
			}

			public override void OnQueueChanged (IList<MediaSession.QueueItem> queue)
			{
				OnQueueChangedImpl (queue);
			}
		}

		SessionCallback sessionCallback = new SessionCallback ();

		public static QueueFragment Create ()
		{
			return new QueueFragment ();
		}

		QueueFragment ()
		{
			connectionCallback.OnConnectedImpl = () => {
				LogHelper.Debug (Tag, "onConnected: session token ", mediaBrowser.SessionToken);

				if (mediaBrowser.SessionToken == null) {
					throw new InvalidOperationException ("No Session token");
				}

				mediaController = new Android.Media.Session.MediaController (Activity,
					mediaBrowser.SessionToken);
				transportControls = mediaController.GetTransportControls ();
				mediaController.RegisterCallback (sessionCallback);

				Activity.MediaController = mediaController;
				playbackState = mediaController.PlaybackState;

				var queue = (JavaList)mediaController.Queue;
				if (queue != null) {
					queueAdapter.Clear ();
					queueAdapter.NotifyDataSetInvalidated ();
					queueAdapter.AddAll(queue.ToArray());
					queueAdapter.NotifyDataSetChanged ();
				}
				OnPlaybackStateChanged (playbackState);
			};
			connectionCallback.OnConnectionFailedImpl = () => LogHelper.Debug (Tag, "onConnectionFailed");
			connectionCallback.OnConnectionSuspendedImpl = () => {
				LogHelper.Debug (Tag, "onConnectionSuspended");
				mediaController.UnregisterCallback (sessionCallback);
				transportControls = null;
				mediaController = null;
				Activity.MediaController = null;
			};
			sessionCallback.OnSessionDestroyedImpl = () => LogHelper.Debug (Tag, "Session destroyed. Need to fetch a new Media Session");
			sessionCallback.OnPlaybackStateChangedImpl = state => {
				if (state == null) {
					return;
				}
				LogHelper.Debug (Tag, "Received playback state change to state ", state.State);
				playbackState = state;
				OnPlaybackStateChanged (state);
			};
			sessionCallback.OnQueueChangedImpl = queue => {
				LogHelper.Debug (Tag, "onQueueChanged ", queue);
				if (queue != null) {
					queueAdapter.Clear ();
					queueAdapter.NotifyDataSetInvalidated ();
					queueAdapter.AddAll (queue.ToArray());
					queueAdapter.NotifyDataSetChanged ();
				}
			};
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate (Resource.Layout.fragment_list, container, false);

			skipPrevious = rootView.FindViewById<ImageButton> (Resource.Id.skip_previous);
			skipPrevious.Enabled = false;
			skipPrevious.Click += OnClick;

			skipNext = rootView.FindViewById<ImageButton> (Resource.Id.skip_next);
			skipNext.Enabled = false;
			skipNext.Click += OnClick;

			playPause = rootView.FindViewById<ImageButton> (Resource.Id.play_pause);
			playPause.Enabled = true;
			playPause.Click += OnClick;

			queueAdapter = new QueueAdapter (Activity);

			var listView = rootView.FindViewById<ListView> (Resource.Id.list_view);
			listView.Adapter = queueAdapter;
			listView.Focusable = true;
			listView.ItemClick += (sender, e) => {
				var item = queueAdapter.GetItem(e.Position);
				transportControls.SkipToQueueItem(item.QueueId);
			};

			mediaBrowser = new MediaBrowser (Activity,
				new ComponentName (Activity, Java.Lang.Class.FromType (typeof(MusicService))),
				connectionCallback, null);

			return rootView;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (mediaBrowser != null)
				mediaBrowser.Connect ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (mediaController != null)
				mediaController.UnregisterCallback (sessionCallback);
			if (mediaBrowser != null)
				mediaBrowser.Disconnect ();
		}

		void OnPlaybackStateChanged (PlaybackState state)
		{
			LogHelper.Debug (Tag, "onPlaybackStateChanged ", state);
			if (state == null) {
				return;
			}
			queueAdapter.ActiveQueueItemId = state.ActiveQueueItemId;
			queueAdapter.NotifyDataSetChanged ();
			var enablePlay = false;
			var statusBuilder = new StringBuilder ();
			switch (state.State) {
			case PlaybackStateCode.Playing:
				statusBuilder.Append ("playing");
				enablePlay = false;
				break;
			case PlaybackStateCode.Paused:
				statusBuilder.Append ("paused");
				enablePlay = true;
				break;
			case PlaybackStateCode.Stopped:
				statusBuilder.Append ("ended");
				enablePlay = true;
				break;
			case PlaybackStateCode.Error:
				statusBuilder.Append ("error: ").Append (state.ErrorMessage);
				break;
			case PlaybackStateCode.Buffering:
				statusBuilder.Append ("buffering");
				break;
			case PlaybackStateCode.None:
				statusBuilder.Append ("none");
				enablePlay = false;
				break;
			case PlaybackStateCode.Connecting:
				statusBuilder.Append ("connecting");
				break;
			default:
				statusBuilder.Append (playbackState);
				break;
			}
			statusBuilder.Append (" -- At position: ").Append (state.Position);
			LogHelper.Debug (Tag, statusBuilder.ToString ());

			if (enablePlay)
				playPause.SetImageDrawable (
					Activity.GetDrawable (Resource.Drawable.ic_play_arrow_white_24dp));
			else
				playPause.SetImageDrawable (Activity.GetDrawable (Resource.Drawable.ic_pause_white_24dp));

			skipPrevious.Enabled = (state.Actions & PlaybackState.ActionSkipToPrevious) != 0;
			skipNext.Enabled = (state.Actions & PlaybackState.ActionSkipToNext) != 0;

			LogHelper.Debug (Tag, "Queue From MediaController *** Title " +
			mediaController.QueueTitle + "\n: Queue: " + mediaController.Queue +
			"\n Metadata " + mediaController.Metadata);
		}

		public void OnClick (object sender, EventArgs e)
		{
			var v = (View)sender;
			var state = playbackState == null ?
				PlaybackStateCode.None : playbackState.State;
			switch (v.Id) {
			case Resource.Id.play_pause:
				LogHelper.Debug (Tag, "Play button pressed, in state " + state);
				if (state == PlaybackStateCode.Paused ||
					state == PlaybackStateCode.Stopped ||
					state == PlaybackStateCode.None) {
					PlayMedia ();
				} else if (state == PlaybackStateCode.Playing) {
					PauseMedia ();
				}
				break;
			case Resource.Id.skip_previous:
				LogHelper.Debug (Tag, "Start button pressed, in state " + state);
				SkipToPrevious ();
				break;
			case Resource.Id.skip_next:
				SkipToNext ();
				break;
			}
		}

		void PlayMedia ()
		{
			if (transportControls != null)
				transportControls.Play ();
		}

		void PauseMedia ()
		{
			if (transportControls != null)
				transportControls.Pause ();
		}

		void SkipToPrevious ()
		{
			if (transportControls != null)
				transportControls.SkipToPrevious ();
		}

		void SkipToNext ()
		{
			if (transportControls != null)
				transportControls.SkipToNext ();
		}
	}
}

