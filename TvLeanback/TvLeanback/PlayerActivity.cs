using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content;
using Android.Media;

using Java.Util;
using Java.Util.Concurrent;

namespace TvLeanback
{
	[Activity (Label = "PlayerActivity", Exported = true, Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
	public class PlayerActivity : Activity, MediaPlayer.IOnErrorListener, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, View.IOnClickListener
	{ 
		private static readonly String TAG = "PlayerActivity";

		private static readonly int HIDE_CONTROLLER_TIME = 5000;
		private static readonly int SEEKBAR_DELAY_TIME = 100;
		private static readonly int SEEKBAR_INTERVAL_TIME = 1000;
		private static readonly int MIN_SCRUB_TIME = 3000;
		private static readonly int SCRUB_SEGMENT_DIVISOR = 30;
		private static readonly double MEDIA_BAR_TOP_MARGIN = 0.8;
		private static readonly double MEDIA_BAR_RIGHT_MARGIN = 0.2;
		private static readonly double MEDIA_BAR_BOTTOM_MARGIN = 0.0;
		private static readonly double MEDIA_BAR_LEFT_MARGIN = 0.2;
		private static readonly double MEDIA_BAR_HEIGHT = 0.1;
		private static readonly double MEDIA_BAR_WIDTH = 0.9;

		protected VideoView mVideoView;
		private TextView mStartText;
		private TextView mEndText;
		private SeekBar mSeekbar;
		private ImageView mPlayPause;
		private ProgressBar mLoading;
		private View mControllers;
		private View mContainer;
		private Timer mSeekbarTimer;
		private Timer mControllersTimer;
		private PlaybackState mPlaybackState;
		private readonly Handler mHandler = new Handler ();
		private Movie mSelectedMovie;
		private bool mShouldStartPlayback;
		private bool mControllersVisible;
		private int mDuration;
		private DisplayMetrics mMetrics;

		/*
	     * List of various states that we can be in
	     */
		public enum PlaybackState
		{
			PLAYING,
			PAUSED,
			BUFFERING,
			IDLE
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.player_activity);
			Log.Debug (TAG, "Created");
			mMetrics = new DisplayMetrics ();
			WindowManager.DefaultDisplay.GetMetrics (mMetrics);

			LoadViews ();
			SetupController ();
			SetupControlsCallbacks ();
			StartVideoPlayer ();
			UpdateMetadata (true);
		}

		private void StartVideoPlayer ()
		{
			var b = this.Intent.Extras;
			mSelectedMovie = Utils.Deserialize(this.Intent.GetStringExtra (
				Resources.GetString (Resource.String.movie)));
			if (null != b) {
				mShouldStartPlayback = b.GetBoolean (Resources.GetString (Resource.String.should_start));
				int startPosition = b.GetInt (Resources.GetString (Resource.String.start_position), 0);
				mVideoView.SetVideoPath (mSelectedMovie.VideoUrl);
				if (mShouldStartPlayback) {
					mPlaybackState = PlaybackState.PLAYING;
					UpdatePlayButton (mPlaybackState);
					if (startPosition > 0) {
						mVideoView.SeekTo (startPosition);
					}
					mVideoView.Start ();
					mPlayPause.RequestFocus ();
					StartControllersTimer ();
				} else {
					UpdatePlaybackLocation ();
					mPlaybackState = PlaybackState.PAUSED;
					UpdatePlayButton (mPlaybackState);
				}
			}
		}

		private void UpdatePlaybackLocation ()
		{
			if (mPlaybackState == PlaybackState.PLAYING ||
			    mPlaybackState == PlaybackState.BUFFERING) {
				StartControllersTimer ();
			} else {
				StopControllersTimer ();
			}
		}

		private void Play (int position)
		{
			StartControllersTimer ();
			mVideoView.SeekTo (position);
			mVideoView.Start ();
			RestartSeekBarTimer ();
		}

		private void StopSeekBarTimer ()
		{
			if (null != mSeekbarTimer) {
				mSeekbarTimer.Cancel ();
			}
		}

		private void RestartSeekBarTimer ()
		{
			StopSeekBarTimer ();
			mSeekbarTimer = new Timer ();
			mSeekbarTimer.ScheduleAtFixedRate (new UpdateSeekbarTask (this), SEEKBAR_DELAY_TIME,
				SEEKBAR_INTERVAL_TIME);
		}

		private void StopControllersTimer ()
		{
			if (null != mControllersTimer) {
				mControllersTimer.Cancel ();
			}
		}

		private void StartControllersTimer ()
		{
			if (null != mControllersTimer) {
				mControllersTimer.Cancel ();
			}
			mControllersTimer = new Timer ();
			mControllersTimer.Schedule (new HideControllersTask (this), HIDE_CONTROLLER_TIME);
		}

		private void UpdateControllersVisibility (bool show)
		{
			if (show) {
				mControllers.Visibility = ViewStates.Visible;
			} else {
				mControllers.Visibility = ViewStates.Invisible;
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			Log.Debug (TAG, "onPause() was called");
			if (null != mSeekbarTimer) {
				mSeekbarTimer.Cancel ();
				mSeekbarTimer = null;
			}
			if (null != mControllersTimer) {
				mControllersTimer.Cancel ();
			}
			mVideoView.Pause ();
			mPlaybackState = PlaybackState.PAUSED;
			UpdatePlayButton (PlaybackState.PAUSED);
		}

		protected override void OnStop ()
		{
			Log.Debug (TAG, "onStop() was called");
			base.OnStop ();
		}

		protected override void OnDestroy ()
		{
			Log.Debug (TAG, "onDestroy() is called");
			StopControllersTimer ();
			StopSeekBarTimer ();
			base.OnDestroy ();
		}

		protected override void OnStart ()
		{
			Log.Debug (TAG, "onStart() was called");
			base.OnStart ();
		}

		protected override void OnResume ()
		{
			Log.Debug (TAG, "onResume() was called");
			base.OnResume ();
		}

		private void SetupController ()
		{
			int w = (int)(mMetrics.WidthPixels * MEDIA_BAR_WIDTH);
			int h = (int)(mMetrics.HeightPixels * MEDIA_BAR_HEIGHT);
			int marginLeft = (int)(mMetrics.WidthPixels * MEDIA_BAR_LEFT_MARGIN);
			int marginTop = (int)(mMetrics.HeightPixels * MEDIA_BAR_TOP_MARGIN);
			int marginRight = (int)(mMetrics.WidthPixels * MEDIA_BAR_RIGHT_MARGIN);
			int marginBottom = (int)(mMetrics.HeightPixels * MEDIA_BAR_BOTTOM_MARGIN);
			var lp = new RelativeLayout.LayoutParams (w, h);
			lp.SetMargins (marginLeft, marginTop, marginRight, marginBottom);
			mControllers.LayoutParameters = lp;
			mStartText.SetText (Resource.String.init_text);
			mEndText.SetText (Resource.String.init_text);
		}

		private void SetupControlsCallbacks ()
		{
			mVideoView.SetOnErrorListener (this);

			mVideoView.SetOnPreparedListener (this);
			mVideoView.SetOnCompletionListener (this);
		}
		public bool OnError(MediaPlayer mp, MediaError error, int what){
			String msg = "";
			if (error == MediaError.TimedOut) {
				msg = GetString (Resource.String.video_error_media_load_timeout);
			} else if (error == MediaError.ServerDied) {
				msg = GetString (Resource.String.video_error_server_unaccessible);
			} else {
				msg = GetString (Resource.String.video_error_unknown_error);
			}
			Utils.ShowErrorDialog (this, msg);
			mVideoView.StopPlayback ();
			mPlaybackState = PlaybackState.IDLE;
			return false;
		}
		public void OnPrepared(MediaPlayer mp){
			Log.Debug (TAG, "onPrepared is reached");
			mDuration = mp.Duration;
			mEndText.Text = FormatTimeSignature (mDuration);
			mSeekbar.Max = mDuration;
			RestartSeekBarTimer ();
		}
		public void OnCompletion(MediaPlayer mp){
				StopSeekBarTimer ();
				mPlaybackState = PlaybackState.IDLE;
				UpdatePlayButton (PlaybackState.IDLE);
				mControllersTimer = new Timer ();
				mControllersTimer.Schedule (new BackToDetailTask (this), HIDE_CONTROLLER_TIME);
		}
		/*
	     * @Override public bool onKeyDown(int keyCode, KeyEvent event) { return
	     * base.onKeyDown(keyCode, event); }
	     */
		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
			int currentPos = 0;
			int delta = (int)(mDuration / SCRUB_SEGMENT_DIVISOR);
			if (delta < MIN_SCRUB_TIME)
				delta = MIN_SCRUB_TIME;

			Log.Verbose ("keycode", "duration " + mDuration + " delta:" + delta);
			if (!mControllersVisible) {
				UpdateControllersVisibility (true);
			}
			switch (keyCode) {
			case Keycode.DpadCenter:
				return true;
			case Keycode.DpadDown:
				return true;
			case Keycode.DpadLeft:
				currentPos = mVideoView.CurrentPosition;
				currentPos -= delta;
				if (currentPos > 0)
					Play (currentPos);
				return true;
			case Keycode.DpadRight:
				currentPos = mVideoView.CurrentPosition;
				currentPos += delta;
				if (currentPos < mDuration)
					Play (currentPos);
				return true;
			case Keycode.DpadUp:
				return true;
			}
			return base.OnKeyDown (keyCode, e);
		}

		private void UpdateSeekbar (int position, int duration)
		{
			mSeekbar.Progress = position;
			mSeekbar.Max = duration;
			mStartText.Text = FormatTimeSignature (mDuration);
		}

		protected void UpdatePlayButton (PlaybackState state)
		{
			switch (state) {
			case PlaybackState.PLAYING:
				mLoading.Visibility = ViewStates.Invisible;
				mPlayPause.Visibility = ViewStates.Visible;
				mPlayPause.SetImageDrawable (
					Resources.GetDrawable (Resource.Drawable.ic_pause_playcontrol_normal));
				break;
			case PlaybackState.PAUSED:
			case PlaybackState.IDLE:
				mLoading.Visibility = ViewStates.Invisible;
				mPlayPause.Visibility = ViewStates.Visible;
				mPlayPause.SetImageDrawable (
					Resources.GetDrawable (Resource.Drawable.ic_play_playcontrol_normal));
				break;
			case PlaybackState.BUFFERING:
				mPlayPause.Visibility = ViewStates.Invisible;
				mLoading.Visibility = ViewStates.Visible;
				break;
			default:
				break;
			}
		}

		private void UpdateMetadata (bool visible)
		{
			mVideoView.Invalidate ();
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			return true;
		}
		private void LoadViews ()
		{
			mVideoView = FindViewById<VideoView> (Resource.Id.videoView);
			mStartText = FindViewById<TextView> (Resource.Id.startText);
			mEndText = FindViewById<TextView> (Resource.Id.endText);
			mSeekbar = FindViewById<SeekBar> (Resource.Id.seekBar);
			mPlayPause = FindViewById<ImageView> (Resource.Id.playpause);
			mLoading = FindViewById<ProgressBar> (Resource.Id.progressBar);
			mControllers = FindViewById (Resource.Id.controllers);
			mContainer = FindViewById (Resource.Id.container);

			mVideoView.SetOnClickListener (this);
		}
		public void OnClick(View v){
			Log.Debug (TAG, "clicked play pause button");

			if (!mControllersVisible) {
				UpdateControllersVisibility (true);
			}

			if (mPlaybackState == PlaybackState.PAUSED) {
				mPlaybackState = PlaybackState.PLAYING;
				UpdatePlayButton (mPlaybackState);
				mVideoView.Start ();
				StartControllersTimer ();
			} else {
				mVideoView.Pause ();
				mPlaybackState = PlaybackState.PAUSED;
				UpdatePlayButton (PlaybackState.PAUSED);
				StopControllersTimer ();
			}
		}
		private String FormatTimeSignature (int timeSignature)
		{
			return Java.Lang.String.Format (Java.Util.Locale.Us, 
				"%02d:%02d",
				TimeUnit.Milliseconds.ToMinutes (timeSignature),
				TimeUnit.Milliseconds.ToSeconds (timeSignature)
				-
				TimeUnit.Minutes.ToSeconds (TimeUnit.Milliseconds
					.ToMinutes (timeSignature)));
		}

		internal class HideControllersTask : TimerTask
		{
			private PlayerActivity owner;
			public HideControllersTask(PlayerActivity owner)
			{
				this.owner  = owner;
			}
			public override void Run ()
			{
				owner.mHandler.Post (() => {
					owner.UpdateControllersVisibility (false);
					owner.mControllersVisible = false;
				});
			}
		}

		internal class UpdateSeekbarTask : TimerTask
		{
			private PlayerActivity owner;
			public UpdateSeekbarTask(PlayerActivity owner)
			{
				this.owner  = owner;
			}
			public override void Run ()
			{
				owner.mHandler.Post (() => {
					int currentPos = 0;
					currentPos = owner.mVideoView.CurrentPosition;
					owner.UpdateSeekbar (currentPos, owner.mDuration);
				});
			}
		}

		internal class BackToDetailTask : TimerTask
		{
			PlayerActivity owner;
			public BackToDetailTask(PlayerActivity owner)
			{
				this.owner = owner;
			}
			public override void Run ()
			{
				owner.mHandler.Post (() => {
					Intent intent = new Intent (owner, Utils.ToJavaClass (typeof(DetailsActivity)));
					intent.PutExtra (owner.Resources.GetString (Resource.String.movie), Utils.Serialize(owner.mSelectedMovie));
					owner.StartActivity (intent);
				});
			}
		}
	}
}