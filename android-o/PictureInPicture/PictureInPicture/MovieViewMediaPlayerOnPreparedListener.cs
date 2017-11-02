using System;
using Android.Media;

namespace PictureInPicture
{
    public class MovieViewMediaPlayerOnPreparedListener : Java.Lang.Object, MediaPlayer.IOnPreparedListener
    {
        MovieView self;

        public MovieViewMediaPlayerOnPreparedListener(MovieView self)
        {
            this.self = self;
        }

        public void OnPrepared(MediaPlayer player)
        {
            // Adjust the aspect ratio of this view
            self.RequestLayout();

            if (self.SavedCurrentPosition > 0)
            {
                self.MVMediaPlayer.SeekTo(self.SavedCurrentPosition);
                self.SavedCurrentPosition = 0;
            }
            else
            {
                // Start automatically
                self.Play();
            }
        }
    }
}
