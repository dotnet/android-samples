using Android.Gms.Tasks;
using Android.Util;
using Android.Widget;
using Exception = Java.Lang.Exception;

namespace ActivityRecognition.Listeners
{
    public class RequestOnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public MainActivity Activity { get; set; }
        public void OnFailure(Exception e)
        {
            Log.Warn(Activity.TAG, Activity.GetString(Resource.String.activity_updates_not_enabled));
            Toast.MakeText(Activity, Activity.GetString(Resource.String.activity_updates_not_enabled), ToastLength.Short).Show();
            Activity.SetUpdatesRequestedState(false);
        }
    }
}