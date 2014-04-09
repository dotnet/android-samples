using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Util;

namespace Location
{
	[Activity (Label = "Location", MainLauncher = true)]

	//Implement ILocationListener interface to get location updates
	public class MainActivity : Activity, ILocationListener
	{
		LocationManager locMgr;
		string tag = "MainActivity";
		Button button;
		TextView latitude;
		TextView longitude;
		TextView provider;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug (tag, "OnCreate called");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			button = FindViewById<Button> (Resource.Id.myButton);
			latitude = FindViewById<TextView> (Resource.Id.latitude);
			longitude = FindViewById<TextView> (Resource.Id.longitude);
			provider = FindViewById<TextView> (Resource.Id.provider);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			Log.Debug (tag, "OnStart called");
		}

		// OnResume gets called every time the activity starts, so we'll put our RequestLocationUpdates
		// code here, so that 
		protected override void OnResume ()
		{
			base.OnResume (); 
			Log.Debug (tag, "OnResume called");

			// initialize location manager
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			button.Click += delegate {
				button.Text = "Location Service Running";

				// pass in the provider (GPS), 
				// the minimum time between updates (in seconds), 
				// the minimum distance the user needs to move to generate an update (in meters),
				// and an ILocationListener (recall that this class impletents the ILocationListener interface)
				if (locMgr.AllProviders.Contains (LocationManager.NetworkProvider)
					&& locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {
					locMgr.RequestLocationUpdates (LocationManager.NetworkProvider, 2000, 1, this);
				} else {
					Toast.MakeText (this, "The Network Provider does not exist or is not enabled!", ToastLength.Long).Show ();
				}


				// Comment the line above, and uncomment the following, to test 
				// the GetBestProvider option. This will determine the best provider
				// at application launch. Note that once the provide has been set
				// it will stay the same until the next time this method is called

				/*var locationCriteria = new Criteria();

				locationCriteria.Accuracy = Accuracy.Coarse;
				locationCriteria.PowerRequirement = Power.Medium;

				string locationProvider = locMgr.GetBestProvider(locationCriteria, true);

				Log.Debug(tag, "Starting location updates with " + locationProvider.ToString());
				locMgr.RequestLocationUpdates (locationProvider, 2000, 1, this);*/
			};
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// stop sending location updates when the application goes into the background
			// to learn about updating location in the background, refer to the Backgrounding guide
			// http://docs.xamarin.com/guides/cross-platform/application_fundamentals/backgrounding/


			// RemoveUpdates takes a pending intent - here, we pass the current Activity
			locMgr.RemoveUpdates (this);
			Log.Debug (tag, "Location updates paused because application is entering the background");
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			Log.Debug (tag, "OnStop called");
		}

		public void OnLocationChanged (Android.Locations.Location location)
		{
			Log.Debug (tag, "Location changed");
			latitude.Text = "Latitude: " + location.Latitude.ToString();
			longitude.Text = "Longitude: " + location.Longitude.ToString();
			provider.Text = "Provider: " + location.Provider.ToString();
		}
		public void OnProviderDisabled (string provider)
		{
			Log.Debug (tag, provider + " disabled by user");
		}
		public void OnProviderEnabled (string provider)
		{
			Log.Debug (tag, provider + " enabled by user");
		}
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			Log.Debug (tag, provider + " availability has changed to " + status.ToString());
		}
	}
}


