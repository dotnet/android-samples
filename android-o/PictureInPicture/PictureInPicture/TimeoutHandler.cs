using System;
using Android.OS;

namespace PictureInPicture
{
    public class TimeoutHandler : Handler
    {
        WeakReference<MovieView> mMovieViewRef;

        public TimeoutHandler(MovieView view)
        {
            mMovieViewRef = new WeakReference<MovieView>(view);
        }

        public override void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case Constants.MESSAGE_HIDE_CONTROLS:
                    MovieView movieViewOut;
                    mMovieViewRef.TryGetTarget(out movieViewOut);
                    if (movieViewOut != null)
                    {
                        movieViewOut.HideControls();
                    }
                    break;
                default:
                    base.HandleMessage(msg);
                    break;
            }
        }
    }
}
