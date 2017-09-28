using System;
using Android.Content;
using Android.Locations;
using Android.Preferences;
using Android.Text.Format;
using Java.Util;

namespace LocUpdFgService
{
	public class Utils
	{
		public const string KeyRequestingLocationUpdates = "requesting_locaction_updates";

		/**
	     * Returns true if requesting location updates, otherwise returns false.
	     *
	     * @param context The {@link Context}.
	     */
		public static bool RequestingLocationUpdates(Context context)
		{
			return PreferenceManager.GetDefaultSharedPreferences(context)
				    .GetBoolean(KeyRequestingLocationUpdates, false);
		}

		/**
	     * Stores the location updates state in SharedPreferences.
	     * @param requestingLocationUpdates The location updates state.
	     */
		public static void SetRequestingLocationUpdates(Context context, bool requestingLocationUpdates)
		{
			PreferenceManager.GetDefaultSharedPreferences(context)
					.Edit()
			        .PutBoolean(KeyRequestingLocationUpdates, requestingLocationUpdates)
					.Apply();
		}

		/**
		 * Returns the {@code location} object as a human readable string.
		 * @param location  The {@link Location}.
		 */
		public static string GetLocationText(Location location)
		{
			return location == null ? "Unknown location" :
					"(" + location.Latitude + ", " + location.Longitude + ")";
		}

		public static string GetLocationTitle(Context context)
		{
			return context.GetString(Resource.String.location_updated,
			        DateFormat.GetDateFormat(context).Format(new Date()));
		}
	}
}
