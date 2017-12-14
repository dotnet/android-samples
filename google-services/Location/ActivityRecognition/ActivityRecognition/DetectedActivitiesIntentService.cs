using Android.App;
using System.Collections.Generic;
using Android.Util;
using Android.Content;
using Android.Gms.Location;
using Android.Preferences;

namespace ActivityRecognition
{
    [Service(Exported = false)]
    public class DetectedActivitiesIntentService : IntentService
    {
        protected const string TAG = "DetectedActivitiesIS";

        public DetectedActivitiesIntentService()
            : base(TAG)
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            var result = ActivityRecognitionResult.ExtractResult(intent);

            IList<DetectedActivity> detectedActivities = result.ProbableActivities;

            PreferenceManager.GetDefaultSharedPreferences(this)
                .Edit()
                .PutString(Constants.KeyDetectedActivities,
                        Utils.DetectedActivitiesToJson(detectedActivities))
                .Apply();

            Log.Info(TAG, "activities detected");
            foreach (DetectedActivity da in detectedActivities)
            {
                Log.Info(TAG, Utils.GetActivityString(
                    ApplicationContext, da.Type) + " " + da.Confidence + "%"
                );
            }
        }
    }
}