
namespace PictureInPicture
{
    public class Constants
    {
        /** The amount of time we are stepping forward or backward for fast-forward and fast-rewind. */
        public const int FAST_FORWARD_REWIND_INTERVAL = 5000; // ms

        /** The amount of time until we fade out the controls. */
        public const int TIMEOUT_CONTROLS = 3000; // ms

        /** Intent action for media controls from Picture-in-Picture mode. */
        public const string ACTION_MEDIA_CONTROL = "media_control";

        /** Intent extra for media controls from Picture-in-Picture mode. */
        public const string EXTRA_CONTROL_TYPE = "control_type";

        /** The request code for play action PendingIntent. */
        public const int REQUEST_PLAY = 1;

        /** The request code for pause action PendingIntent. */
        public const int REQUEST_PAUSE = 2;

        /** The request code for info action PendingIntent. */
        public const int REQUEST_INFO = 3;

        /** The intent extra value for play action. */
        public const int CONTROL_TYPE_PLAY = 1;

        /** The intent extra value for pause action. */
        public const int CONTROL_TYPE_PAUSE = 2;

        /** The TimeoutHandler's Message.What value. */
        public const int MESSAGE_HIDE_CONTROLS = 1;
    }
}
