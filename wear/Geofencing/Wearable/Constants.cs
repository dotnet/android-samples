using System;

namespace Wearable
{
	public static class Constants
	{
		public const String TAG = "ExampleGeofencingApp";

		// Timeout for making a connection to GoogleApiClient (in milliseconds).
		public const long CONNECTION_TIME_OUT_MS = 100;

		public const int NOTIFICATION_ID = 1;
		public const String ANDROID_BUILDING_ID = "1";
		public const String YERBA_BUENA_ID = "2";

		public const String ACTION_CHECK_IN = "check_in";
		public const String ACTION_DELETE_DATA_ITEM = "delete_data_item";
		public const String KEY_GEOFENCE_ID = "geofence_id";

	}
}

