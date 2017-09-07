using Android.Content;

namespace PictureInPicture
{
    public class PIPBroadcastReceiver : BroadcastReceiver
    {
        MainActivity self;

        public PIPBroadcastReceiver (MainActivity self)
        {
            this.self = self;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent == null || Constants.ACTION_MEDIA_CONTROL != intent.Action)
            {
                return;
            }

            // This is where we are called back from Picture-in-Picture action items.
            int controlType = intent.GetIntExtra(Constants.EXTRA_CONTROL_TYPE, 0);
            switch (controlType)
            {
                case Constants.CONTROL_TYPE_PLAY:
                    self.PIPMovieView.Play();
                    break;
                case Constants.CONTROL_TYPE_PAUSE:
                    self.PIPMovieView.Pause();
                    break;
            }
        }
    }
}
