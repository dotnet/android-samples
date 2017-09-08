using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content;
using Android.Support.Annotation;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Content.Res;
using Android.Views;

namespace PictureInPicture
{
    [Activity(Label = "PictureInPicture", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/AppTheme",
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize
              | ConfigChanges.ScreenLayout | ConfigChanges.Orientation, SupportsPictureInPicture = true)]
    public class MainActivity : AppCompatActivity
    {
        /** This shows the video. */
        public MovieView PIPMovieView { get; private set; }

        /** The arguments to be used for Picture-in-Picture mode. */
        PictureInPictureParams.Builder pictureInPictureParamsBuilder = new PictureInPictureParams.Builder();

        /** The bottom half of the screen; hidden on landscape */
        ScrollView scrollView;

        /** A {@link BroadcastReceiver} to receive action item events from Picture-in-Picture mode. */
        BroadcastReceiver receiver;

        /// <summary>Updat
        /// Update the state of pause/resume action item in Picture-in-Picture mode.
        /// </summary>
        /// <param name="iconId">The icon to be used.</param>
        /// <param name="title">The title text.</param>
        /// <param name="controlType">The type of action.</param>
        /// <param name="requestCode">The request code for the pending intent.</param>
        public void UpdatePictureInPictureActions([DrawableRes]int iconId, string title, int controlType, int requestCode)
        {
            var actions = new List<RemoteAction>();

            // This is the PendingIntent that is invoked when a user clicks on the action item.
            // You need to use distinct request codes for play and pause, or the PendingIntent won't
            // be properly updated.
            PendingIntent intent = PendingIntent.GetBroadcast(this, requestCode, new Intent(Constants.ACTION_MEDIA_CONTROL)
                                                              .PutExtra(Constants.EXTRA_CONTROL_TYPE, controlType), 0);
            Icon icon = Icon.CreateWithResource(this, iconId);
            actions.Add(new RemoteAction(icon, title, title, intent));

            // Another action item. This is a fixed action.
            actions.Add(new RemoteAction(
                Icon.CreateWithResource(this, Resource.Drawable.ic_info_24dp),
                GetString(Resource.String.info), GetString(Resource.String.info_description),
                PendingIntent.GetActivity(this, Constants.REQUEST_INFO,
                                          new Intent(Intent.ActionView, Android.Net.Uri.Parse(GetString(Resource.String.info_uri))), 0)));

            pictureInPictureParamsBuilder.SetActions(actions).Build();

            // This is how you can update action items (or aspect ratio) for Picture-in-Picture mode.
            // Note this call can happen even when the app is not in PiP mode. In that case, the
            // arguments will be used for at the next call of #enterPictureInPictureMode.
            SetPictureInPictureParams(pictureInPictureParamsBuilder.Build());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            // View references
            PIPMovieView = (MovieView)FindViewById(Resource.Id.movie);
            scrollView = (ScrollView)FindViewById(Resource.Id.scroll);

            // Set up the video; it automatically starts.
            PIPMovieView.MVMovieListener = new PIPMovieListener(this);
            FindViewById(Resource.Id.pip).Click += (sender, e) =>
            {
                switch (((View)sender).Id)
                {
                    case Resource.Id.pip:
                        Minimize();
                        break;
                }
            };
        }

        protected override void OnStop()
        {
            // On entering Picture-in-Picture mode, onPause is called, but not onStop.
            // For this reason, this is the place where we should pause the video playback.
            PIPMovieView.Pause();
            base.OnStop();
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            if (!IsInPictureInPictureMode)
            {
                // Show the video controls so the video can be easily resumed.
                PIPMovieView.ShowControls();
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            AdjustFullScreen(newConfig);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                AdjustFullScreen(Resources.Configuration);
            }
        }

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);

            if (isInPictureInPictureMode)
            {
                // Starts receiving events from action items in PiP mode.
                receiver = new PIPBroadcastReceiver(this);
                RegisterReceiver(receiver, new IntentFilter(Constants.ACTION_MEDIA_CONTROL));
            }
            else
            {
                // We are out of PiP mode. We can stop receiving events from it.
                UnregisterReceiver(receiver);
                receiver = null;

                // Show the video controls if the video is not playing
                if (PIPMovieView != null && !PIPMovieView.IsPlaying)
                {
                    PIPMovieView.ShowControls();
                }
            }
        }

        /// <summary>
        /// Enters Picture-in-Picture mode.
        /// </summary>
        public void Minimize()
        {
            if (PIPMovieView == null)
            {
                return;
            }

            // Hide the controls in picture-in-picture mode.
            PIPMovieView.HideControls();
            // Calculate the aspect ratio of the PiP screen.
            var aspectRatio = new Rational(PIPMovieView.Width, PIPMovieView.Height);
            pictureInPictureParamsBuilder.SetAspectRatio(aspectRatio).Build();
            EnterPictureInPictureMode(pictureInPictureParamsBuilder.Build());
        }

        /// <summary>
        /// Adjusts immersive full-screen flags depending on the screen orientation.
        /// </summary>
        /// <param name="config">The current configuration.</param>
        void AdjustFullScreen(Configuration config)
        {
            if (config.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                SystemUiFlags uiVisibilityFlags = SystemUiFlags.LayoutStable
                          | SystemUiFlags.LayoutHideNavigation
                          | SystemUiFlags.LayoutFullscreen
                          | SystemUiFlags.HideNavigation
                          | SystemUiFlags.Fullscreen
                          | SystemUiFlags.ImmersiveSticky;

                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiVisibilityFlags;
                scrollView.Visibility = ViewStates.Gone;
                PIPMovieView.ShouldAdjustViewBounds = false;
            }
            else
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LayoutStable;
                scrollView.Visibility = ViewStates.Visible;
                PIPMovieView.ShouldAdjustViewBounds = true;
            }
        }

    }
}

