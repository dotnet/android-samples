using System;
using Android.App;
using System.Collections.Generic;
using Android.Util;
using Android.Content;
using Android.Gms.Location;
using Android.Support.V4.Content;
using System.Linq;

namespace ActivityRecognition
{
	[Service (Exported = false)]
	public class DetectedActivitiesIntentService : IntentService
	{
		protected const string TAG = "activity-detection-intent-service";

		public DetectedActivitiesIntentService () 
			: base (TAG)
		{
		}

		protected override void OnHandleIntent (Intent intent)
		{
			var result = ActivityRecognitionResult.ExtractResult(intent);
			var localIntent = new Intent (Constants.BroadcastAction);

			IList<DetectedActivity> detectedActivities = result.ProbableActivities;

			Log.Info (TAG, "activities detected");
			foreach (DetectedActivity da in detectedActivities) {
				Log.Info (TAG, Constants.GetActivityString (
					ApplicationContext, da.Type) + " " + da.Confidence + "%"
				);
			}

			localIntent.PutExtra (Constants.ActivityExtra, detectedActivities.ToArray ());
			LocalBroadcastManager.GetInstance (this).SendBroadcast (localIntent);
		}
	}
}

