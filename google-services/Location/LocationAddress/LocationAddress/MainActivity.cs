using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.Gms.Location;
using Android.Util;

namespace LocationAddress
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity, 
        GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener
	{
		protected const string TAG = "main-activity";
		protected const string ADDRESS_REQUESTED_KEY = "address-request-pending";
		protected const string LOCATION_ADDRESS_KEY = "location-address";
		protected GoogleApiClient mGoogleApiClient;
		protected Location mLastLocation;
		protected bool mAddressRequested;
		protected string mAddressOutput;
		private AddressResultReceiver mResultReceiver;
		protected TextView mLocationAddressTextView;
		ProgressBar mProgressBar;
		Button mFetchAddressButton;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			mResultReceiver = new AddressResultReceiver (new Handler ());
			mResultReceiver.OnReceiveResultImpl = (resultCode, resultData) => {
				mAddressOutput = resultData.GetString (Constants.ResultDataKey);
				DisplayAddressOutput ();

				if (resultCode == 0) {
					ShowToast (GetString (Resource.String.address_found));
				}
				mAddressRequested = false;
				UpdateUIWidgets ();
			};
			mLocationAddressTextView = FindViewById<TextView> (Resource.Id.location_address_view);
			mProgressBar = FindViewById<ProgressBar> (Resource.Id.progress_bar);
			mFetchAddressButton = FindViewById<Button> (Resource.Id.fetch_address_button);

			mFetchAddressButton.Click += FetchAddressButtonHandler;

			mAddressOutput = string.Empty;
			UpdateValuesFromBundle (savedInstanceState);

			UpdateUIWidgets ();
			BuildGoogleApiClient ();
		}

		void UpdateValuesFromBundle (Bundle savedInstanceState)
		{
			if (savedInstanceState != null) {
				if (savedInstanceState.KeySet ().Contains (ADDRESS_REQUESTED_KEY)) {
					mAddressRequested = savedInstanceState.GetBoolean (ADDRESS_REQUESTED_KEY);
				}
				if (savedInstanceState.KeySet ().Contains (LOCATION_ADDRESS_KEY)) {
					mAddressOutput = savedInstanceState.GetString (LOCATION_ADDRESS_KEY);
					DisplayAddressOutput ();
				}
			}
		}

		protected void BuildGoogleApiClient ()
		{
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.AddApi (LocationServices.API)
				.Build ();
		}

		public void FetchAddressButtonHandler (object sender, EventArgs e)
		{
			if (mGoogleApiClient.IsConnected && mLastLocation != null) {
				StartIntentService ();
			}
			mAddressRequested = true;
			UpdateUIWidgets ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			mGoogleApiClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			if (mGoogleApiClient.IsConnected) {
				mGoogleApiClient.Disconnect ();
			}
		}

		public void OnConnected (Bundle connectionHint)
		{
			mLastLocation = LocationServices.FusedLocationApi.GetLastLocation (mGoogleApiClient);
			if (mLastLocation != null) {
				if (!Geocoder.IsPresent) {
					Toast.MakeText (this, Resource.String.no_geocoder_available, ToastLength.Long).Show ();
					return;
				}
				if (mAddressRequested) {
					StartIntentService ();
				}
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

		protected void StartIntentService ()
		{
			var intent = new Intent (this, typeof(FetchAddressIntentService));
			intent.PutExtra (Constants.Receiver, mResultReceiver);
			intent.PutExtra (Constants.LocationDataExtra, mLastLocation);

			StartService (intent);
		}

		protected void DisplayAddressOutput ()
		{
			mLocationAddressTextView.Text = mAddressOutput;
		}

		void UpdateUIWidgets ()
		{
			if (mAddressRequested) {
				mProgressBar.Visibility = ViewStates.Visible;
				mFetchAddressButton.Enabled = false;
			} else {
				mProgressBar.Visibility = ViewStates.Gone;
				mFetchAddressButton.Enabled = true;
			}
		}

		protected void ShowToast (string text)
		{
			Toast.MakeText (this, text, ToastLength.Short).Show ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean (ADDRESS_REQUESTED_KEY, mAddressRequested);

			outState.PutString (LOCATION_ADDRESS_KEY, mAddressOutput);
			base.OnSaveInstanceState (outState);
		}

		class AddressResultReceiver : ResultReceiver
		{
			public Action<int, Bundle> OnReceiveResultImpl { get; set; }
			public AddressResultReceiver (Handler handler) 
				: base (handler)
			{
			}

			protected override void OnReceiveResult (int resultCode, Bundle resultData)
			{
				OnReceiveResultImpl (resultCode, resultData);
			}
		}
	}
}

