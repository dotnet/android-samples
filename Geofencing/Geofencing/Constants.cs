using System;
using Android.Gms.Location;

namespace Geofencing
{
	public static class Constants
	{
		public const String TAG = "ExampleGeofencingApp";

		// Request code to attempt to resolve Google Play services connection failures.
		public const int CONNECTION_FAILURE_RESOLUTION_REQUEST = 9000;
		// Timeout for making a connection to GoogleApiClient (in milliseconds).
		public const long CONNECTION_TIME_OUT_MS = 100;

		// For the purposes of this demo, the geofences are hard-coded and should not expire.
		// An app with dynamically-created geofences would want to include a reasonable expiration time.
		public const long GEOFENCE_EXPIRATION_TIME = Geofence.NeverExpire;

		// Geofence parameters for the Android building on Google's main campus in Mountain View.
		public const String ANDROID_BUILDING_ID = "1";
		public const double ANDROID_BUILDING_LATITUDE = 37.420092;
		public const double ANDROID_BUILDING_LONGITUDE = -122.083648;
		public const float ANDROID_BUILDING_RADIUS_METERS = 60.0f;

		// Geofence parameters for the Yerba Buena Gardens near the Moscone Center in San Francisco.
		public const String YERBA_BUENA_ID = "2";
		public const double YERBA_BUENA_LATITUDE = 37.784886;
		public const double YERBA_BUENA_LONGITUDE = -122.402671;
		public const float YERBA_BUENA_RADIUS_METERS = 72.0f;


		// The constants below are less interesting than those above.

		// Path for the DataItem containing the last geofence id entered.
		public const String GEOFENCE_DATA_ITEM_PATH = "/geofenceid";
		public static readonly Android.Net.Uri GEOFENCE_DATA_ITEM_URI =
			new Android.Net.Uri.Builder().Scheme("wear").Path(GEOFENCE_DATA_ITEM_PATH).Build();
		public const String KEY_GEOFENCE_ID = "geofence_id";

		// Keys for flattened geofences stored in SharedPreferences.
		public const String KEY_LATITUDE = "com.example.wearable.geofencing.KEY_LATITUDE";
		public const String KEY_LONGITUDE = "com.example.wearable.geofencing.KEY_LONGITUDE";
		public const String KEY_RADIUS = "com.example.wearable.geofencing.KEY_RADIUS";
		public const String KEY_EXPIRATION_DURATION = "com.example.wearable.geofencing.KEY_EXPIRATION_DURATION";
		public const String KEY_TRANSITION_TYPE = "com.example.wearable.geofencing.KEY_TRANSITION_TYPE";
		// The prefix for flattened geofence keys.
		public const String KEY_PREFIX = "com.example.wearable.geofencing.KEY";

		// Invalid values, used to test geofence storage when retrieving geofences.
		public const long INVALID_LONG_VALUE = -999l;
		public const float INVALID_FLOAT_VALUE = -999.0f;
		public const int INVALID_INT_VALUE = -999;
	}
}

