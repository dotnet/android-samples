using System.Collections.Generic;
using Android.Gms.Maps.Model;

namespace Geofencing
{
	public static class Constants
	{
		public const string PACKAGE_NAME = "com.xamarin.geofencing";
		public const string SHARED_PREFERENCES_NAME = PACKAGE_NAME + ".SHARED_PREFERENCES_NAME";
		public const string GEOFENCES_ADDED_KEY = PACKAGE_NAME + ".GEOFENCES_ADDED_KEY";

		public const long GEOFENCE_EXPIRATION_IN_HOURS = 12;
		public const long GEOFENCE_EXPIRATION_IN_MILLISECONDS =	GEOFENCE_EXPIRATION_IN_HOURS * 60 * 60 * 1000;
		public const float GEOFENCE_RADIUS_IN_METERS = 1609;

		public static readonly Dictionary<string, LatLng> BAY_AREA_LANDMARKS = new Dictionary<string, LatLng> {
			{ "SFO", new LatLng (37.621313, -122.378955) },
			{ "GOOGLE", new LatLng (37.422611, -122.0840577) }
		};
	}
}

