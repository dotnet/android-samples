using System.Collections.Generic;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Widget;

namespace ActivityRecognition.Listeners
{
    public class RemoveOnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public MainActivity Activity { get; set; }
        public void OnSuccess(Java.Lang.Object result)
        {
            Toast.MakeText(Activity, Activity.GetString(Resource.String.activity_updates_removed), ToastLength.Short).Show();
            Activity.SetUpdatesRequestedState(false);
            Activity.mAdapter.UpdateActivities(new List<DetectedActivity>());
        }
    }
}