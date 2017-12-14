using Android.Gms.Location;

namespace ActivityRecognition
{
    public static class Constants
    {
        public const string PackageName = "com.xamarin.activityrecognition";
        public const string KeyActivityUpdatesRequested = PackageName + ".ACTIVITY_UPDATES_REQUESTED";
        public const string KeyDetectedActivities = PackageName + ".DETECTED_ACTIVITIES";
        public const long DetectionIntervalInMilliseconds = 30 * 1000;

        internal static readonly int[] MonitoredActivities = {
            DetectedActivity.Still,
            DetectedActivity.OnFoot,
            DetectedActivity.Walking,
            DetectedActivity.Running,
            DetectedActivity.OnBicycle,
            DetectedActivity.InVehicle,
            DetectedActivity.Tilting,
            DetectedActivity.Unknown
        };
    }
}

