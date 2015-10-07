using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Location;
using Android.Util;
using Android.Locations;

namespace BasicLocationSample
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity, 
        GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener
	{
		protected const string TAG = "basic-location-sample";

		/**
     	* Provides the entry point to Google Play services.
     	*/
		protected GoogleApiClient mGoogleApiClient;

		/**
    	 * Represents a geographical location.
    	 */
		protected Location mLastLocation;

		protected TextView mLatitudeText;
		protected TextView mLongitudeText;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			mLatitudeText = FindViewById<TextView> (Resource.Id.latitude_text);
			mLongitudeText = FindViewById<TextView> (Resource.Id.longitude_text);

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
			if (mGoogleApiClient.IsConnected) {
				mGoogleApiClient.Disconnect ();
			}
		}

		public void OnConnected (Bundle connectionHint)
		{
			mLastLocation = LocationServices.FusedLocationApi.GetLastLocation (mGoogleApiClient);
			if (mLastLocation != null) {
				mLatitudeText.Text = mLastLocation.Latitude.ToString ();
				mLongitudeText.Text = mLastLocation.Longitude.ToString ();
			} else {
				Toast.MakeText (this, Resource.String.no_location_detected, ToastLength.Long).Show ();
			}
		}

		public void OnConnectionSuspended (int cause)
		{
			Log.Info (TAG, "Connection suspended");
			mGoogleApiClient.Connect ();
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Info(TAG, "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
		}
	}
}


