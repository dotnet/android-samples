using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Annotation;

namespace PictureInPicture
{
    public class PIPMovieListener : IMovieListener
    {
        MainActivity mainActivity;
        string play, pause;

        public PIPMovieListener(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;

            // Prepare string resources for Picture-in-Picture actions.
            play = mainActivity.GetString(Resource.String.play);
            pause = mainActivity.GetString(Resource.String.pause);
        }

        public void OnMovieStarted()
        {
            // We are playing the video now. In PiP mode, we want to show an action item to pause
            // the video.
            mainActivity.UpdatePictureInPictureActions(Resource.Drawable.ic_pause_24dp, pause, Constants.CONTROL_TYPE_PAUSE, Constants.REQUEST_PAUSE);
        }

        public void OnMovieStopped()
        {
            // The video stopped or reached its end. In PiP mode, we want to show an action item
            // to play the video.
            mainActivity.UpdatePictureInPictureActions(Resource.Drawable.ic_play_arrow_24dp, play, Constants.CONTROL_TYPE_PLAY, Constants.REQUEST_PLAY);
        }

        public void OnMovieMinimized()
        {
            // The MovieView wants us to minimize it. We enter Picture-in-Picture mode now.
            mainActivity.Minimize();
        }
    }
}
