using System;
using Android.Graphics;
using Android.Views;

namespace PictureInPicture
{
    public class MovieViewSurfaceHolderCallback : Java.Lang.Object, ISurfaceHolderCallback
    {
        MovieView self;

        public MovieViewSurfaceHolderCallback(MovieView self)
        {
            this.self = self;
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            self.OpenVideo(holder.Surface);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            // Do nothing
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            if (self.MVMediaPlayer != null)
            {
                self.SavedCurrentPosition = self.MVMediaPlayer.CurrentPosition;
            }

            self.CloseVideo();
        }
    }
}
