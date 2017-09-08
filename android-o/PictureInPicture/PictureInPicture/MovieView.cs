using System;
using Android.Widget;
using Android.Views;
using Android.Media;
using Android.Content;
using Android.Util;
using Android.Graphics;
using Android.Content.Res;
using Android.Transitions;
using Android.Runtime;

namespace PictureInPicture
{
    [Register("com.xamarin.PictureInPicture.MovieView")]
    public class MovieView : RelativeLayout
    {
        const string TAG = "MovieView";

        /** Shows the video playback. */
        SurfaceView surfaceView;

        // Controls
        ImageButton toggle;
        View shade;
        ImageButton fastForwardButton;
        ImageButton fastRewindButton;
        ImageButton minimize;

        /** This plays the video. This will be null when no video is set. */
        public MediaPlayer MVMediaPlayer { get; private set; }

        /** The resource ID for the video to play. */
        int videoResourceId;
        int VideoResourceId
        {
            get => videoResourceId;
            set
            {
                if (videoResourceId == value)
                {
                    return;
                }

                videoResourceId = value;
                Surface surface = surfaceView.Holder.Surface;
                if (surface != null && surface.IsValid)
                {
                    CloseVideo();
                    OpenVideo(surface);
                }
            }
        }

        /** Whether we adjust our view bounds or we fill the remaining area with black bars */
        bool shouldAdjustViewBounds;
        public bool ShouldAdjustViewBounds
        {
            get => shouldAdjustViewBounds;
            set
            {
                if (shouldAdjustViewBounds == value)
                {
                    return;
                }

                shouldAdjustViewBounds = value;
                if (shouldAdjustViewBounds)
                {
                    Background = null;
                }
                else
                {
                    SetBackgroundColor(Color.Black);
                }

                RequestLayout();
            }
        }

        /** Handles timeout for media controls. */
        TimeoutHandler timeoutHandler;

        /** The listener for all the events we publish. */
        public IMovieListener MVMovieListener { get; set; }

        public int SavedCurrentPosition { get; set; }

        public MovieView(Context context) : this(context, null)
        {
        }

        public MovieView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public MovieView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            SetBackgroundColor(Color.Black);

            // Inflate the content
            Inflate(context, Resource.Layout.view_movie, this);
            surfaceView = (SurfaceView)FindViewById(Resource.Id.surface);
            shade = FindViewById(Resource.Id.shade);
            toggle = (ImageButton)FindViewById(Resource.Id.toggle);
            fastForwardButton = (ImageButton)FindViewById(Resource.Id.fast_forward);
            fastRewindButton = (ImageButton)FindViewById(Resource.Id.fast_rewind);
            minimize = (ImageButton)FindViewById(Resource.Id.minimize);

            // Attributes
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.MovieView,
                    defStyleAttr, Resource.Style.Widget_PictureInPicture_MovieView);
            VideoResourceId = a.GetResourceId(Resource.Styleable.MovieView_android_src, 0);
            ShouldAdjustViewBounds = a.GetBoolean(Resource.Styleable.MovieView_android_adjustViewBounds, false);
            a.Recycle();

            surfaceView.Click += (sender, e) => OnClick(sender);
            toggle.Click += (sender, e) => OnClick(sender);
            fastForwardButton.Click += (sender, e) => OnClick(sender);
            fastRewindButton.Click += (sender, e) => OnClick(sender);
            minimize.Click += (sender, e) => OnClick(sender);

            surfaceView.Holder.AddCallback(new MovieViewSurfaceHolderCallback(this));
        }

        public void OnClick(Object view)
        {
            switch (((View)view).Id)
            {
                case Resource.Id.surface:
                    ToggleControls();
                    break;
                case Resource.Id.toggle:
                    Toggle();
                    break;
                case Resource.Id.fast_forward:
                    FastForward();
                    break;
                case Resource.Id.fast_rewind:
                    FastRewind();
                    break;
                case Resource.Id.minimize:
                    if (MVMovieListener != null)
                    {
                        MVMovieListener.OnMovieMinimized();
                    }
                    break;
            }
            // Start or reset the timeout to hide controls
            if (MVMediaPlayer != null)
            {
                if (timeoutHandler == null)
                {
                    timeoutHandler = new TimeoutHandler(this);
                }
                timeoutHandler.RemoveMessages(Constants.MESSAGE_HIDE_CONTROLS);
                if (MVMediaPlayer.IsPlaying)
                {
                    timeoutHandler.SendEmptyMessageDelayed(Constants.MESSAGE_HIDE_CONTROLS, Constants.TIMEOUT_CONTROLS);
                }
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (MVMediaPlayer != null)
            {
                int videoWidth = MVMediaPlayer.VideoWidth;
                int videoHeight = MVMediaPlayer.VideoHeight;
                if (videoWidth != 0 && videoHeight != 0)
                {
                    float aspectRatio = (float)videoHeight / videoWidth;
                    int width = MeasureSpec.GetSize(widthMeasureSpec);
                    MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
                    int height = MeasureSpec.GetSize(heightMeasureSpec);
                    MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
                    if (ShouldAdjustViewBounds)
                    {
                        if (widthMode == MeasureSpecMode.Exactly && heightMode != MeasureSpecMode.Exactly)
                        {
                            base.OnMeasure(widthMeasureSpec,
                                MeasureSpec.MakeMeasureSpec((int)(width * aspectRatio), MeasureSpecMode.Exactly));
                        }
                        else if (widthMode != MeasureSpecMode.Exactly && heightMode == MeasureSpecMode.Exactly)
                        {
                            base.OnMeasure(MeasureSpec.MakeMeasureSpec((int)(height / aspectRatio),
                                                                        MeasureSpecMode.Exactly), heightMeasureSpec);
                        }
                        else
                        {
                            base.OnMeasure(widthMeasureSpec,
                                MeasureSpec.MakeMeasureSpec((int)(width * aspectRatio), MeasureSpecMode.Exactly));
                        }
                    }
                    else
                    {
                        float viewRatio = (float)height / width;
                        if (aspectRatio > viewRatio)
                        {
                            int padding = (int)((width - height / aspectRatio) / 2);
                            SetPadding(padding, 0, padding, 0);
                        }
                        else
                        {
                            int padding = (int)((height - width * aspectRatio) / 2);
                            SetPadding(0, padding, 0, padding);
                        }
                        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                    }
                    return;
                }
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnDetachedFromWindow()
        {
            if (timeoutHandler != null)
            {
                timeoutHandler.RemoveMessages(Constants.MESSAGE_HIDE_CONTROLS);
                timeoutHandler = null;
            }

            base.OnDetachedFromWindow();
        }

        /// <summary>
        /// Shows all the controls.
        /// </summary>
        public void ShowControls()
        {
            TransitionManager.BeginDelayedTransition(this);
            shade.Visibility = ViewStates.Visible;
            toggle.Visibility = ViewStates.Visible;
            fastForwardButton.Visibility = ViewStates.Visible;
            fastRewindButton.Visibility = ViewStates.Visible;
            minimize.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Hides all the controls.
        /// </summary>
        public void HideControls()
        {
            TransitionManager.BeginDelayedTransition(this);
            shade.Visibility = ViewStates.Invisible;
            toggle.Visibility = ViewStates.Invisible;
            fastForwardButton.Visibility = ViewStates.Invisible;
            fastRewindButton.Visibility = ViewStates.Invisible;
            minimize.Visibility = ViewStates.Invisible;
        }

        /// <summary>
        /// Fast-forward the video.
        /// </summary>
        public void FastForward()
        {
            if (MVMediaPlayer == null)
            {
                return;
            }
            MVMediaPlayer.SeekTo(MVMediaPlayer.CurrentPosition + Constants.FAST_FORWARD_REWIND_INTERVAL);
        }

        /// <summary>
        /// Fast-rewind the video.
        /// </summary>
        public void FastRewind()
        {
            if (MVMediaPlayer == null)
            {
                return;
            }
            MVMediaPlayer.SeekTo(MVMediaPlayer.CurrentPosition - Constants.FAST_FORWARD_REWIND_INTERVAL);
        }

        public bool IsPlaying => MVMediaPlayer != null && MVMediaPlayer.IsPlaying;

        public void Play()
        {
            if (MVMediaPlayer == null)
            {
                return;
            }
            MVMediaPlayer.Start();
            AdjustToggleState();
            KeepScreenOn = true;
            if (MVMovieListener != null)
            {
                MVMovieListener.OnMovieStarted();
            }
        }

        public void Pause()
        {
            if (MVMediaPlayer == null)
            {
                AdjustToggleState();
                return;
            }
            MVMediaPlayer.Pause();
            AdjustToggleState();
            KeepScreenOn = false;
            if (MVMovieListener != null)
            {
                MVMovieListener.OnMovieStopped();
            }
        }

        public void OpenVideo(Surface surface)
        {
            if (videoResourceId == 0)
            {
                return;
            }
            MVMediaPlayer = new MediaPlayer();

            MVMediaPlayer.SetSurface(surface);
            try
            {
                using (AssetFileDescriptor fd = Resources.OpenRawResourceFd(videoResourceId))
                {
                    MVMediaPlayer.SetDataSource(fd);
                    MVMediaPlayer.SetOnPreparedListener(new MovieViewMediaPlayerOnPreparedListener(this));
                    MVMediaPlayer.SetOnCompletionListener(new MovieViewMediaPlayerOnCompletionListener(this));
                    MVMediaPlayer.Prepare();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Failed to open video", ex);
            }
        }

        public void CloseVideo()
        {
            if (MVMediaPlayer != null)
            {
                MVMediaPlayer.Release();
                MVMediaPlayer = null;
            }
        }

        void Toggle()
        {
            if (MVMediaPlayer == null)
            {
                return;
            }
            if (MVMediaPlayer.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        void ToggleControls()
        {
            if (shade.Visibility == ViewStates.Visible)
            {
                HideControls();
            }
            else
            {
                ShowControls();
            }
        }

        public void AdjustToggleState()
        {
            if (IsPlaying)
            {
                toggle.ContentDescription = Resources.GetString(Resource.String.pause);
                toggle.SetImageResource(Resource.Drawable.ic_pause_64dp);
            }
            else
            {
                toggle.ContentDescription = Resources.GetString(Resource.String.play);
                toggle.SetImageResource(Resource.Drawable.ic_play_arrow_64dp);
            }
        }
    }
}
