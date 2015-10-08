using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Util;

using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;

namespace Geofencing
{
	[Activity (
		Icon = "@drawable/ic_launcher", 
		ExcludeFromRecents = true, 
		Theme = "@android:style/Theme.Translucent.NoTitleBar",
		Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : Activity, 
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		// Internal List of Geofence objects. In a real app, these migth be provided by an API based on locations within the user's proximity
		List<IGeofence> mGeofenceList;

		// These will LocalDataStoreSlot hard-coded geofences in this sample app
		private SimpleGeofence mAndroidBuildingGeofence;
		private SimpleGeofence mYerbaBuenaGeofence;

		// Persistent storage for geofences
		SimpleGeofenceStore mGeofenceStorage;

		GoogleApiClient apiClient;
		// Stores the PendingIntent used to request geofence monitoring
		PendingIntent mGeofenceRequestIntent;

		// Defines the allowable request types (in this example, we only add geofences)
		enum RequestType { Add }
		RequestType mRequestType;
		// Flag that indicates if a request is underway
		bool mInProgress;

		bool IsGooglePlayServicesAvailable
		{
			get {
				int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
				if (resultCode == ConnectionResult.Success) {
					if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
						Log.Debug (Constants.TAG, "Google Play services is available");
					}
					return true;
				} else {
					Log.Error (Constants.TAG, "Google Play services is unavailable");
					return false;
				}
			}
		}

		PendingIntent GeofenceTransitionPendingIntent {
			get {
				var intent = new Intent (this, typeof(GeofenceTransitionsIntentService));
				return PendingIntent.GetService (this, 0, intent, PendingIntentFlags.UpdateCurrent);
			}
		}

		protected override void OnCreate (Bundle bundle)
		{

			base.OnCreate (bundle);
			// Rather than displaying this activity, simply display a toast indicating that the geofence
			// service is being created. This should happen in less than a second.
			Toast.MakeText (this, GetString (Resource.String.start_geofence_service), ToastLength.Long).Show ();

			// Instantiate a new geofence storage area.
			mGeofenceStorage = new SimpleGeofenceStore (this);
			// Instntiate the current List of geofences
			mGeofenceList = new List<IGeofence> ();
			// Start with the request flag set to false
			mInProgress = false;

			CreateGeofences ();
			AddGeofences ();

			Finish ();
		}

		public void CreateGeofences ()
		{
			// Create internal "flattened" objects containing the geofence data
			mAndroidBuildingGeofence = new SimpleGeofence (
				Constants.ANDROID_BUILDING_ID, // geofenceId
				Constants.ANDROID_BUILDING_LATITUDE,
				Constants.ANDROID_BUILDING_LONGITUDE,
				Constants.ANDROID_BUILDING_RADIUS_METERS,
				Constants.GEOFENCE_EXPIRATION_TIME,
				Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit
			);
			mYerbaBuenaGeofence = new SimpleGeofence (
				Constants.YERBA_BUENA_ID, // geofenceId
				Constants.YERBA_BUENA_LATITUDE,
				Constants.YERBA_BUENA_LONGITUDE,
				Constants.YERBA_BUENA_RADIUS_METERS,
				Constants.GEOFENCE_EXPIRATION_TIME,
				Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit
			);

			// Store these flat versions in SharedPreferences and add them to the geofence list
			mGeofenceStorage.SetGeofence (Constants.ANDROID_BUILDING_ID, mAndroidBuildingGeofence);
			mGeofenceStorage.SetGeofence (Constants.YERBA_BUENA_ID, mYerbaBuenaGeofence);
			mGeofenceList.Add (mAndroidBuildingGeofence.ToGeofence ());
			mGeofenceList.Add (mYerbaBuenaGeofence.ToGeofence ());
		}

		public void AddGeofences()
		{
			// Start a request to add geofences
			mRequestType = RequestType.Add;
			// Test for Google Play services after setting the request type
			if (!IsGooglePlayServicesAvailable) {
				Log.Error (Constants.TAG, "Unable to add geofences - Google Play services unavailable.");
				return;
			}
			// Create a new location client object. Since this activity implements ConnectionCallbacks and OnConnectionFailedListener,
			// it can be used as the listener for both parameters
			apiClient = new GoogleApiClient.Builder (this, this, this)
				.AddApi (LocationServices.API)
				.Build ();
			// If a request is not already underway
			if (!mInProgress) {
				// Indicate that a request is underway
				mInProgress = true;
				// Request a connection from the client to Location Services
				apiClient.Connect ();
			} else {
				// A request is already underway, so disconnect the client and retry the request
				apiClient.Disconnect ();
				apiClient.Connect ();
			}
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			mInProgress = false;
			// If the error has a resolution, start a Google Play services activity to resolve it
			if (result.HasResolution) {
				try {
					result.StartResolutionForResult (this, Constants.CONNECTION_FAILURE_RESOLUTION_REQUEST);
				} catch (Exception ex) {
					Log.Error (Constants.TAG, "Exception while resolving connection error.", ex);
				}
			} else {
				int errorCode = result.ErrorCode;
				Log.Error (Constants.TAG, "Connection to Google Play services failed with error code " + errorCode);
			}
		}

		public void OnConnected (Bundle connectionHint)
		{
			// Use mRequestType to determine what action to take. Only Add used in this sample
			if (mRequestType == RequestType.Add) {
				// Get the PendingIntent for the geofence monitoring request
				mGeofenceRequestIntent = GeofenceTransitionPendingIntent;
				// Send a request to add the current geofences
				LocationServices.GeofencingApi.AddGeofences (apiClient, mGeofenceList, mGeofenceRequestIntent);
			}
		}

		public void OnConnectionSuspended (int i)
		{
		}

		public void OnDisconnected ()
		{
			// Turn off the request flag
			mInProgress = false;
			// Destroy the current location client
			apiClient = null;
		}

		public void OnAddGeofencesResult (int statusCode, string[] geofenceRequestIds)
		{
			// Log if adding the geofences was successful
			if (LocationStatusCodes.Success == statusCode) {
				if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
					Log.Debug (Constants.TAG, "Added geofences successfully.");
				}
			} else {
				Log.Error (Constants.TAG, "Failed to add geofences. Status code: " + statusCode);
			}
			// turn off the in progress flag and disconnect the client
			mInProgress = false;
			apiClient.Disconnect ();
		}
	}
}


