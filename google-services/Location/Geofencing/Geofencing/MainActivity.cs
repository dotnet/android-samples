using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Common.Apis;
using System.Collections.Generic;
using Android.Gms.Location;
using Android.Media;
using Android.Util;
using Java.Lang;

namespace Geofencing
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity, 
        GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener
	{
		protected const string TAG = "creating-and-monitoring-geofences";
		protected GoogleApiClient mGoogleApiClient;
		protected IList<IGeofence> mGeofenceList;
		bool mGeofencesAdded;
		PendingIntent mGeofencePendingIntent;
		ISharedPreferences mSharedPreferences;
		Button mAddGeofencesButton;
		Button mRemoveGeofencesButton;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			mAddGeofencesButton = FindViewById<Button> (Resource.Id.add_geofences_button);
			mRemoveGeofencesButton = FindViewById<Button> (Resource.Id.remove_geofences_button);

			mAddGeofencesButton.Click += AddGeofencesButtonHandler;
			mRemoveGeofencesButton.Click += RemoveGeofencesButtonHandler;

			mGeofenceList = new List<IGeofence> ();
			mGeofencePendingIntent = null;

			mSharedPreferences = GetSharedPreferences (Constants.SHARED_PREFERENCES_NAME,
				FileCreationMode.Private);

			mGeofencesAdded = mSharedPreferences.GetBoolean (Constants.GEOFENCES_ADDED_KEY, false);

			SetButtonsEnabledState ();
			PopulateGeofenceList ();
			BuildGoogleApiClient ();
		}

		protected void BuildGoogleApiClient ()
		{
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.AddApi (LocationServices.API)
				.Build ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			mGoogleApiClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			mGoogleApiClient.Disconnect ();
		}

		public void OnConnected (Bundle connectionHint)
		{
			Log.Info (TAG, "Connected to GoogleApiClient");
		}

		public void OnConnectionSuspended (int cause)
		{
			Log.Info (TAG, "Connection suspended");
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Info (TAG, "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
		}

		GeofencingRequest GetGeofencingRequest ()
		{
			var builder = new GeofencingRequest.Builder ();
			builder.SetInitialTrigger (GeofencingRequest.InitialTriggerEnter);
			builder.AddGeofences (mGeofenceList);

			return builder.Build ();
		}

		public async void AddGeofencesButtonHandler (object sender, EventArgs e)
		{
			if (!mGoogleApiClient.IsConnected) {
				Toast.MakeText (this, GetString (Resource.String.not_connected), ToastLength.Short).Show ();
				return;
			}

			try {
				var status = await LocationServices.GeofencingApi.AddGeofencesAsync (mGoogleApiClient, GetGeofencingRequest (),
					GetGeofencePendingIntent ());
                HandleResult (status);
			} catch (SecurityException securityException) {
				LogSecurityException(securityException);
			}
		}

        public async void RemoveGeofencesButtonHandler (object sender, EventArgs e)
		{
			if (!mGoogleApiClient.IsConnected) {
				Toast.MakeText (this, GetString(Resource.String.not_connected), ToastLength.Short).Show ();
				return;
			}
			try {
				var status = await LocationServices.GeofencingApi.RemoveGeofencesAsync (mGoogleApiClient, 
                    GetGeofencePendingIntent ());
                HandleResult (status);
			} catch (SecurityException securityException) {
				LogSecurityException (securityException);
			}
		}

		void LogSecurityException (SecurityException securityException)
		{
			Log.Error (TAG, "Invalid location permission. " +
				"You need to use ACCESS_FINE_LOCATION with geofences", securityException);
		}

        public void HandleResult (Statuses status)
		{
			if (status.IsSuccess) {
				mGeofencesAdded = !mGeofencesAdded;
				var editor = mSharedPreferences.Edit ();
				editor.PutBoolean (Constants.GEOFENCES_ADDED_KEY, mGeofencesAdded);
				editor.Commit ();

				SetButtonsEnabledState ();

				Toast.MakeText (
					this,
					GetString (mGeofencesAdded ? Resource.String.geofences_added :
						Resource.String.geofences_removed),
					ToastLength.Short
				).Show ();
			} else {
				var errorMessage = GeofenceErrorMessages.GetErrorString (this,
					status.StatusCode);
				Log.Error (TAG, errorMessage);
			}
		}

		PendingIntent GetGeofencePendingIntent ()
		{
			if (mGeofencePendingIntent != null) {
				return mGeofencePendingIntent;
			}
			var intent = new Intent (this, typeof(GeofenceTransitionsIntentService));
			return PendingIntent.GetService (this, 0, intent, PendingIntentFlags.UpdateCurrent);
		}

		public void PopulateGeofenceList ()
		{
			foreach (var entry in Constants.BAY_AREA_LANDMARKS) {
				mGeofenceList.Add (new GeofenceBuilder ()
					.SetRequestId (entry.Key)
					.SetCircularRegion (
						entry.Value.Latitude,
						entry.Value.Longitude,
						Constants.GEOFENCE_RADIUS_IN_METERS
					)
					.SetExpirationDuration (Constants.GEOFENCE_EXPIRATION_IN_MILLISECONDS)
					.SetTransitionTypes (Geofence.GeofenceTransitionEnter |
						Geofence.GeofenceTransitionExit)
					.Build ());
			}
		}

		void SetButtonsEnabledState ()
		{
			if (mGeofencesAdded) {
				mAddGeofencesButton.Enabled = false;
				mRemoveGeofencesButton.Enabled = true;
			} else {
				mAddGeofencesButton.Enabled = true;
				mRemoveGeofencesButton.Enabled = false;
			}
		}
	}
}


