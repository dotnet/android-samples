using System;
using Android.Content;

namespace Geofencing
{
	/// <summary>
	/// Storage for geofence values, implemented in SharedPreferences
	/// </summary>
	public class SimpleGeofenceStore
	{
		// The SharedPreferences object in which geofences are stored
		readonly ISharedPreferences mPrefs;
		// The name of the SharedPreferences
		const string SHARED_PREFERENCES = "SharedPrefrences";

		/// <summary>
		/// Create the SharedPreferences storage with private access only.
		/// </summary>
		/// <param name="context">Context.</param>
		public SimpleGeofenceStore (Context context)
		{
			mPrefs = context.GetSharedPreferences (SHARED_PREFERENCES, FileCreationMode.Private);
		}

		/// <summary>
		/// Returns a stored geofence by its ID, or returns null if it's not found
		/// </summary>
		/// <returns>A SimpleGeofence defined by its center and radius, or null if the ID is invalid</returns>
		/// <param name="id">The ID of a stored Geofence</param>
		public SimpleGeofence GetGeofence(String id)
		{
			// Get the latitude for the geofence identified by id, or INVALID_FLOAT_VALUE if it doesn't exist (similarly for the other values that follow)
			double lat = mPrefs.GetFloat (GetGeofenceFieldKey (id, Constants.KEY_LATITUDE), Constants.INVALID_FLOAT_VALUE);
			double lng = mPrefs.GetFloat (GetGeofenceFieldKey (id, Constants.KEY_LONGITUDE), Constants.INVALID_FLOAT_VALUE);
			float radius = mPrefs.GetFloat (GetGeofenceFieldKey (id, Constants.KEY_RADIUS), Constants.INVALID_INT_VALUE);
			long expirationDuration = mPrefs.GetLong (GetGeofenceFieldKey (id, Constants.KEY_EXPIRATION_DURATION), Constants.INVALID_LONG_VALUE);
			int transitionType = mPrefs.GetInt (GetGeofenceFieldKey (id, Constants.KEY_TRANSITION_TYPE), Constants.INVALID_INT_VALUE);

			// If none of the values is incorrect, return the object
			if (lat != Constants.INVALID_FLOAT_VALUE
			    && lng != Constants.INVALID_FLOAT_VALUE
			    && radius != Constants.INVALID_FLOAT_VALUE
			    && expirationDuration != Constants.INVALID_LONG_VALUE
			    && transitionType != Constants.INVALID_INT_VALUE)
				return new SimpleGeofence (id, lat, lng, radius, expirationDuration, transitionType);

			// Otherwise return null
			return null;
		}

		/// <summary>
		/// Save a geofence
		/// </summary>
		/// <param name="id">The ID of the Geofence</param>
		/// <param name="geofence">The SimpleGeofence with the values you want to save in SharedPreferemces</param>
		public void SetGeofence(String id, SimpleGeofence geofence) {
			// Get a SharedPreferences editor instance. Among other things, SharedPreferences ensures that updates are atomic and non-concurrent
			ISharedPreferencesEditor prefs = mPrefs.Edit();
			// Write the geofence values to SharedPreferences 
			prefs.PutFloat(GetGeofenceFieldKey(id, Constants.KEY_LATITUDE), (float) geofence.Latitude);
			prefs.PutFloat(GetGeofenceFieldKey(id, Constants.KEY_LONGITUDE), (float) geofence.Longitude);
			prefs.PutFloat (GetGeofenceFieldKey (id, Constants.KEY_RADIUS), geofence.Radius);
			prefs.PutLong (GetGeofenceFieldKey (id, Constants.KEY_EXPIRATION_DURATION), geofence.ExpirationDuration);
			prefs.PutInt (GetGeofenceFieldKey (id, Constants.KEY_TRANSITION_TYPE), geofence.TransitionType);
			// Commit the changes
			prefs.Commit ();
		}

		/// <summary>
		/// Given a Geofence object's ID and the name of a field (For example, KEY_LATITUDE), return the keyname of the object's values in SharedPreferences
		/// </summary>
		/// <returns>The full key name o a value in SharedPreferences</returns>
		/// <param name="id">The ID of a Geofence object</param>
		/// <param name="fieldName">The field represented by the key</param>
		private string GetGeofenceFieldKey(String id, String fieldName) {
			return Constants.KEY_PREFIX + "_" + id + "_" + fieldName;
		}
	}
}

