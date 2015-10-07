using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.Util;
using System.Threading.Tasks;

namespace LocationSettings
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity, GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener,	Android.Gms.Location.ILocationListener
	{
		protected const string TAG = "location-settings";
		protected const int REQUEST_CHECK_SETTINGS = 0x1;
		public const long UPDATE_INTERVAL_IN_MILLISECONDS = 10000;
		public const long FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS = UPDATE_INTERVAL_IN_MILLISECONDS / 2;
		protected const string KEY_REQUESTING_LOCATION_UPDATES = "requesting-location-updates";
		protected const string KEY_LOCATION = "location";
		protected const string KEY_LAST_UPDATED_TIME_STRING = "last-updated-time-string";

		protected GoogleApiClient mGoogleApiClient;
		protected LocationRequest mLocationRequest;
		protected LocationSettingsRequest mLocationSettingsRequest;
		protected Location mCurrentLocation;
		protected Button mStartUpdatesButton;
		protected Button mStopUpdatesButton;
		protected TextView mLastUpdateTimeTextView;
		protected TextView mLatitudeTextView;
		protected TextView mLongitudeTextView;
		protected Boolean mRequestingLocationUpdates;
		protected String mLastUpdateTime;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			mStartUpdatesButton = FindViewById<Button> (Resource.Id.start_updates_button);
			mStopUpdatesButton = FindViewById<Button> (Resource.Id.stop_updates_button);
			mLatitudeTextView = FindViewById<TextView> (Resource.Id.latitude_text);
			mLongitudeTextView = FindViewById<TextView> (Resource.Id.longitude_text);
			mLastUpdateTimeTextView = FindViewById<TextView> (Resource.Id.last_update_time_text);

			mStartUpdatesButton.Click += StartUpdatesButtonHandler;
			mStopUpdatesButton.Click += StopUpdatesButtonHandler; 

			mRequestingLocationUpdates = false;
			mLastUpdateTime = "";
			UpdateValuesFromBundle (savedInstanceState);

			BuildGoogleApiClient ();
			CreateLocationRequest ();
			BuildLocationSettingsRequest ();
		}

		void UpdateValuesFromBundle (Bundle savedInstanceState)
		{
			if (savedInstanceState != null) {
				if (savedInstanceState.KeySet ().Contains (KEY_REQUESTING_LOCATION_UPDATES)) {
					mRequestingLocationUpdates = savedInstanceState.GetBoolean (
						KEY_REQUESTING_LOCATION_UPDATES);
				}

				if (savedInstanceState.KeySet ().Contains (KEY_LOCATION)) {
					mCurrentLocation = (Location)savedInstanceState.GetParcelable (KEY_LOCATION);
				}

				if (savedInstanceState.KeySet ().Contains (KEY_LAST_UPDATED_TIME_STRING)) {
					mLastUpdateTime = savedInstanceState.GetString (KEY_LAST_UPDATED_TIME_STRING);
				}

				UpdateUI ();
			}
		}

		protected void BuildGoogleApiClient ()
		{
			Log.Info (TAG, "Building GoogleApiClient");
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.AddApi (LocationServices.API)
				.Build ();
		}

		protected void CreateLocationRequest ()
		{
			mLocationRequest = new LocationRequest ();
			mLocationRequest.SetInterval (UPDATE_INTERVAL_IN_MILLISECONDS);
			mLocationRequest.SetFastestInterval (FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS);
			mLocationRequest.SetPriority (LocationRequest.PriorityHighAccuracy);
		}

		protected void BuildLocationSettingsRequest ()
		{
			LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder ();
			builder.AddLocationRequest (mLocationRequest);
			mLocationSettingsRequest = builder.Build ();
		}

		protected async Task CheckLocationSettings ()
		{
			var result = await LocationServices.SettingsApi.CheckLocationSettingsAsync (
				mGoogleApiClient, mLocationSettingsRequest);
            await HandleResult (result);
		}

        public async Task HandleResult (LocationSettingsResult locationSettingsResult)
		{
			var status = locationSettingsResult.Status;
			switch (status.StatusCode) {
			case CommonStatusCodes.Success:
				Log.Info (TAG, "All location settings are satisfied.");
				await StartLocationUpdates ();
				break;
			case CommonStatusCodes.ResolutionRequired:
				Log.Info (TAG, "Location settings are not satisfied. Show the user a dialog to" +
				"upgrade location settings ");

				try {
					status.StartResolutionForResult (this, REQUEST_CHECK_SETTINGS);
				} catch (IntentSender.SendIntentException) {
					Log.Info (TAG, "PendingIntent unable to execute request.");
				}
				break;
			case LocationSettingsStatusCodes.SettingsChangeUnavailable:
				Log.Info (TAG, "Location settings are inadequate, and cannot be fixed here. Dialog " +
				"not created.");
				break;
			}
		}

		protected override async void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {
			case REQUEST_CHECK_SETTINGS:
				switch (resultCode) {
				case Result.Ok:
					Log.Info (TAG, "User agreed to make required location settings changes.");
					await StartLocationUpdates ();
					break;
				case Result.Canceled:
					Log.Info (TAG, "User chose not to make required location settings changes.");
					break;
				}
				break;
			}
		}

		public async void StartUpdatesButtonHandler (object sender, EventArgs e)
		{
			await CheckLocationSettings ();
		}

		public async void StopUpdatesButtonHandler (object sender, EventArgs e)
		{
			await StopLocationUpdates ();
		}

		protected async Task StartLocationUpdates ()
		{
			await LocationServices.FusedLocationApi.RequestLocationUpdates (
				mGoogleApiClient,
				mLocationRequest,
				this
            );
                
            mRequestingLocationUpdates = true;
			SetButtonsEnabledState ();		
		}

		void UpdateUI ()
		{
			SetButtonsEnabledState ();
			UpdateLocationUI ();
		}

		void SetButtonsEnabledState ()
		{
			if (mRequestingLocationUpdates) {
				mStartUpdatesButton.Enabled = false;
				mStopUpdatesButton.Enabled = true;
			} else {
				mStartUpdatesButton.Enabled = true;
				mStopUpdatesButton.Enabled = false;
			}
		}

		void UpdateLocationUI ()
		{
			if (mCurrentLocation != null) {
				mLatitudeTextView.Text = mCurrentLocation.Latitude.ToString ();
				mLongitudeTextView.Text = mCurrentLocation.Longitude.ToString ();
				mLastUpdateTimeTextView.Text = mLastUpdateTime;
			}
		}

        protected async Task StopLocationUpdates ()
		{
            await LocationServices.FusedLocationApi.RemoveLocationUpdates (
                    mGoogleApiClient,
                    this
                );

			mRequestingLocationUpdates = false;
			SetButtonsEnabledState ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			mGoogleApiClient.Connect ();
		}

		protected override async void OnResume ()
		{
			base.OnResume ();
			if (mGoogleApiClient.IsConnected && mRequestingLocationUpdates) {
				await StartLocationUpdates ();
			}
		}

		protected override async void OnPause ()
		{
			base.OnPause ();
			if (mGoogleApiClient.IsConnected) {
				await StopLocationUpdates ();
			}
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			mGoogleApiClient.Disconnect ();
		}

		public void OnConnected (Bundle connectionHint)
		{
			Log.Info (TAG, "Connected to GoogleApiClient");

			if (mCurrentLocation == null) {
				mCurrentLocation = LocationServices.FusedLocationApi.GetLastLocation (mGoogleApiClient);
				mLastUpdateTime = DateTime.Now.TimeOfDay.ToString ();
				UpdateLocationUI ();
			}
		}

		public void OnConnectionSuspended (int cause)
		{
			Log.Info (TAG, "Connection suspended");
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Info (TAG, "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
		}

		public void OnLocationChanged (Location location)
		{
			mCurrentLocation = location;
			mLastUpdateTime = DateTime.Now.TimeOfDay.ToString ();
			UpdateLocationUI ();
			Toast.MakeText (this, Resources.GetString (Resource.String.location_updated_message),
				ToastLength.Short).Show ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean (KEY_REQUESTING_LOCATION_UPDATES, mRequestingLocationUpdates);
			outState.PutParcelable (KEY_LOCATION, mCurrentLocation);
			outState.PutString (KEY_LAST_UPDATED_TIME_STRING, mLastUpdateTime);
			base.OnSaveInstanceState (outState);
		}
	}
}


