using System;
using Android.Gms.Common.Apis;
using Android.App;
using Android.Gms.Wearable;
using Android.Gms.Location;
using Android.Util;
using Java.Util.Concurrent;

namespace Geofencing
{
	[Service(Exported = false)]
	public class GeofenceTransitionsIntentService : IntentService, 
	    GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		private GoogleApiClient mGoogleApiClient;
		public GeofenceTransitionsIntentService ()
			:base(typeof(GeofenceTransitionsIntentService).Name)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		/// <summary>
		/// Handles incoming intents
		/// </summary>
		/// <param name="intent">The intent sent by Location Services. This Intent is provided to Location Services (inside a PendingIntent)
		/// when AddGeofences() is called</param>
		protected override void OnHandleIntent (Android.Content.Intent intent)
		{
			// First check for errors
			var geofencingEvent = GeofencingEvent.FromIntent (intent);
			if (geofencingEvent.HasError) {
				int errorCode = geofencingEvent.ErrorCode;
				Log.Error (Constants.TAG, "Location Services error: " + errorCode);
			} else {
				// Get the type of Geofence transition (i.e. enter or exit in this sample).
				int transitionType = geofencingEvent.GeofenceTransition;
				// Create a DataItem when a user enters one of the geofences. The wearable app will receie this and create a
				// notification to prompt him/her to check in
				if (transitionType == Geofence.GeofenceTransitionEnter) {
					// Connect to the Google Api service in preparation for sending a DataItem
					mGoogleApiClient.BlockingConnect (Constants.CONNECTION_TIME_OUT_MS, TimeUnit.Milliseconds);
					// Get the geofence ID triggered. Note that only one geofence can be triggered at a time in this example, but in some cases
					// you might want to consider the full list of geofences triggered
					string triggeredGeofenceId = geofencingEvent.TriggeringGeofences[0].RequestId;
					// Create a DataItem with this geofence's id. The wearable can use this to create a notification
					PutDataMapRequest putDataMapRequest = PutDataMapRequest.Create (Constants.GEOFENCE_DATA_ITEM_PATH);
					putDataMapRequest.DataMap.PutString (Constants.KEY_GEOFENCE_ID, triggeredGeofenceId);
					if (mGoogleApiClient.IsConnected) {
						WearableClass.DataApi.PutDataItem (
							mGoogleApiClient, putDataMapRequest.AsPutDataRequest ()).Await ();
					} else {
						Log.Error (Constants.TAG, "Failed to send data item: " + putDataMapRequest +
						" - disconnected from Google Play Services");
					}
					mGoogleApiClient.Disconnect ();
				} else if (Geofence.GeofenceTransitionExit == transitionType) {
					// Delete the data item when leaving a geofence region
					mGoogleApiClient.BlockingConnect (Constants.CONNECTION_TIME_OUT_MS, TimeUnit.Milliseconds);
					WearableClass.DataApi.DeleteDataItems (mGoogleApiClient, Constants.GEOFENCE_DATA_ITEM_URI).Await ();
					mGoogleApiClient.Disconnect ();
				}
			}
		}

		public void OnConnected (Android.OS.Bundle connectionHint)
		{

		}

		public void OnConnectionSuspended (int cause)
		{

		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{

		}
	}
}

