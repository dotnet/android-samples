
namespace PictureInPicture
{
    public interface IMovieListener
    {
        /// <summary>
        /// Called when the video is started or resumed.
        /// </summary>
        void OnMovieStarted();

        /// <summary>
        /// Called when the video is paused or finished.
        /// </summary>
        void OnMovieStopped();

        /// <summary>
        /// Called when this view should be minimized.
        /// </summary>
        void OnMovieMinimized();
    }
}
