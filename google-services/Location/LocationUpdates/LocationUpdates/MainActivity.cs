using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Locations;
using Android.Util;
using System.Threading.Tasks;

namespace LocationUpdates
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity, GoogleApiClient.IConnectionCallbacks,
	    GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener {

		protected const string TAG = "location-updates-sample";
		public const long UPDATE_INTERVAL_IN_MILLISECONDS = 10000;
		public const long FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS = UPDATE_INTERVAL_IN_MILLISECONDS / 2;
		protected const string REQUESTING_LOCATION_UPDATES_KEY = "requesting-location-updates-key";
		protected const string LOCATION_KEY = "location-key";
		protected const string LAST_UPDATED_TIME_STRING_KEY = "last-updated-time-string-key";

		protected GoogleApiClient mGoogleApiClient;
		protected LocationRequest mLocationRequest;
		protected Location mCurrentLocation;

		// UI Widgets.
		protected Button mStartUpdatesButton;
		protected Button mStopUpdatesButton;
		protected TextView mLastUpdateTimeTextView;
		protected TextView mLatitudeTextView;
		protected TextView mLongitudeTextView;

		protected bool mRequestingLocationUpdates;
		protected string mLastUpdateTime;

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
		}

		void UpdateValuesFromBundle (Bundle savedInstanceState)
		{
			Log.Info (TAG, "Updating values from bundle");
			if (savedInstanceState != null) {
				if (savedInstanceState.KeySet ().Contains (REQUESTING_LOCATION_UPDATES_KEY)) {
					mRequestingLocationUpdates = savedInstanceState.GetBoolean (REQUESTING_LOCATION_UPDATES_KEY);
					SetButtonsEnabledState();
				}

				if (savedInstanceState.KeySet ().Contains (LOCATION_KEY)) {
					mCurrentLocation = (Location)savedInstanceState.GetParcelable (LOCATION_KEY);
				}

				if (savedInstanceState.KeySet ().Contains (LAST_UPDATED_TIME_STRING_KEY)) {
					mLastUpdateTime = savedInstanceState.GetString (LAST_UPDATED_TIME_STRING_KEY);
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
			CreateLocationRequest ();
		}

		protected void CreateLocationRequest ()
		{
			mLocationRequest = new LocationRequest ();
			mLocationRequest.SetInterval (UPDATE_INTERVAL_IN_MILLISECONDS);
			mLocationRequest.SetFastestInterval (FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS);
			mLocationRequest.SetPriority (LocationRequest.PriorityHighAccuracy);
		}

		public async void StartUpdatesButtonHandler (object sender, EventArgs e)
		{
			if (!mRequestingLocationUpdates) {
				mRequestingLocationUpdates = true;
				SetButtonsEnabledState ();
				await StartLocationUpdates ();
			}
		}

		public async void StopUpdatesButtonHandler (object sender, EventArgs e)
		{
			if (mRequestingLocationUpdates) {
				mRequestingLocationUpdates = false;
				SetButtonsEnabledState ();
				await StopLocationUpdates ();
			}
		}

		protected async Task StartLocationUpdates ()
		{
			await LocationServices.FusedLocationApi.RequestLocationUpdates (mGoogleApiClient, mLocationRequest, this);
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

		void UpdateUI ()
		{
			if (mCurrentLocation != null) {
				mLatitudeTextView.Text = mCurrentLocation.Latitude.ToString ();
				mLongitudeTextView.Text = mCurrentLocation.Longitude.ToString ();
				mLastUpdateTimeTextView.Text = mLastUpdateTime;
			}
		}

        protected async Task StopLocationUpdates ()
		{
			await LocationServices.FusedLocationApi.RemoveLocationUpdates (mGoogleApiClient, this);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			mGoogleApiClient.Connect();
		}

		protected override async void OnResume ()
		{
			base.OnResume ();

			if (mGoogleApiClient.IsConnected && mRequestingLocationUpdates) {
				await StartLocationUpdates();
			}
		}

		protected override async void OnPause ()
		{
			base.OnPause ();
			if (mGoogleApiClient.IsConnected) {
				await StopLocationUpdates();
			}
		}

		protected override void OnStop ()
		{
			mGoogleApiClient.Disconnect();

			base.OnStop ();
		}

		public async void OnConnected (Bundle connectionHint)
		{
			Log.Info(TAG, "Connected to GoogleApiClient");

			if (mCurrentLocation == null) {
				mCurrentLocation = LocationServices.FusedLocationApi.GetLastLocation (mGoogleApiClient);
				mLastUpdateTime = DateTime.Now.TimeOfDay.ToString();
				UpdateUI();
			}

			if (mRequestingLocationUpdates) {
				await StartLocationUpdates ();
			}
		}

		public void OnConnectionSuspended (int cause)
		{
			Log.Info (TAG, "Connection suspended");
			mGoogleApiClient.Connect ();
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Info (TAG, "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
		}

		public void OnLocationChanged (Location location)
		{
			mCurrentLocation = location;
			mLastUpdateTime = DateTime.Now.TimeOfDay.ToString ();
			UpdateUI ();
			Toast.MakeText (this, Resources.GetString (Resource.String.location_updated_message),
				ToastLength.Short).Show ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean (REQUESTING_LOCATION_UPDATES_KEY, mRequestingLocationUpdates);
			outState.PutParcelable (LOCATION_KEY, mCurrentLocation);
			outState.PutString (LAST_UPDATED_TIME_STRING_KEY, mLastUpdateTime);
			base.OnSaveInstanceState (outState);
		}
	}
}


