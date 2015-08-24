using System;
using Android.Widget;
using Android.Gms.Location;
using System.Collections.Generic;
using Android.Content;
using Android;
using Android.Views;

namespace ActivityRecognition
{
	public class DetectedActivitiesAdapter : ArrayAdapter<DetectedActivity>
	{
		public DetectedActivitiesAdapter (Context context, IList<DetectedActivity> detectedActivities) 
			: base (context, 0, detectedActivities)
		{
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			DetectedActivity detectedActivity = GetItem (position);
			if (convertView == null) {
				convertView = LayoutInflater.From (Context).Inflate (
					Resource.Layout.detected_activity, parent, false);
			}

			var activityName = convertView.FindViewById<TextView> (Resource.Id.detected_activity_name);
			var activityConfidenceLevel = convertView.FindViewById<TextView> (Resource.Id.detected_activity_confidence_level);
			var progressBar = convertView.FindViewById<ProgressBar> (Resource.Id.detected_activity_progress_bar);

			activityName.Text = Constants.GetActivityString (Context, detectedActivity.Type);
			activityConfidenceLevel.Text = detectedActivity.Confidence + "%";
			progressBar.Progress = detectedActivity.Confidence;
			return convertView;
		}

		internal void UpdateActivities (IList<DetectedActivity> detectedActivities)
		{
			var detectedActivitiesMap = new Dictionary<int, int>();
			foreach (var activity in detectedActivities) {
				detectedActivitiesMap.Add(activity.Type, activity.Confidence);
			}
			var tempList = new List<DetectedActivity>();
			for (int i = 0; i < Constants.MonitoredActivities.Length; i++) {
				int confidence = detectedActivitiesMap.ContainsKey (Constants.MonitoredActivities[i]) ?
					detectedActivitiesMap[Constants.MonitoredActivities[i]] : 0;

				tempList.Add (new DetectedActivity(Constants.MonitoredActivities[i], confidence));
			}
			Clear ();
			AddAll (tempList);
		}
	}
}

