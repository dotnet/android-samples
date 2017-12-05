using Android.Gms.Tasks;
using Android.Widget;

namespace ActivityRecognition.Listeners
{
    public class RequestOnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public MainActivity Activity { get; set; }

        public void OnSuccess(Java.Lang.Object result)
        {
            Toast.MakeText(Activity, Activity.GetString(Resource.String.activity_updates_enabled), ToastLength.Short).Show();
            Activity.SetUpdatesRequestedState(true);
            Activity.UpdateDetectedActivitiesList();
        }
    }
}