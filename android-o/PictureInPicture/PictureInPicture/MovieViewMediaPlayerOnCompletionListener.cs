using System;
using Android.Media;

namespace PictureInPicture
{
    public class MovieViewMediaPlayerOnCompletionListener : Java.Lang.Object, MediaPlayer.IOnCompletionListener
    {
        MovieView self;
        public MovieViewMediaPlayerOnCompletionListener(MovieView self)
        {
            this.self = self;
        }

        public void OnCompletion(MediaPlayer player)
        {
            self.AdjustToggleState();
            self.KeepScreenOn = false;
            if (self.MVMovieListener != null)
            {
                self.MVMovieListener.OnMovieStopped();
            }
        }
    }
}
