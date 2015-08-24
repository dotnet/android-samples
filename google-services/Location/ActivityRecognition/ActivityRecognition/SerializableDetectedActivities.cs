using System;
using System.Collections.Generic;

using Android.Gms.Location;

namespace ActivityRecognition
{
	public class SerializableDetectedActivities : Java.Lang.Object, Java.IO.ISerializable
	{
		List<DetectedActivity> detectedActivities;

		public SerializableDetectedActivities (List<DetectedActivity> detectedActivities)
		{
			this.detectedActivities = detectedActivities;
		}

		public List<DetectedActivity> DetectedActivities {
			get {
				return detectedActivities;
			}
		}
	}
}

