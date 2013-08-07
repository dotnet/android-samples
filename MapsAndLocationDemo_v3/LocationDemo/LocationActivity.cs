namespace MapsAndLocationDemo
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Android.App;
	using Android.Locations;
	using Android.OS;
	using Android.Util;
	using Android.Widget;
	using LocationDemo;

	[Activity(Label = "@string/activity_label_location", MainLauncher = true)]
	public class LocationActivity : Activity, ILocationListener
	{
		private LocationManager _locMgr;

		public void OnLocationChanged (Location location)
		{
			TextView locationText = FindViewById<TextView> (Resource.Id.locationTextView);

			locationText.Text = String.Format ("Latitude = {0}, Longitude = {1}", location.Latitude, location.Longitude);

			Task.Factory.StartNew (() => {
				// Do the reverse geocoding on a background thread.
				Geocoder geocdr = new Geocoder (this);
				IList<Address> addresses = geocdr.GetFromLocation (location.Latitude, location.Longitude, 5);
				return addresses;
			})
				.ContinueWith (task => {
				TextView addrText = FindViewById<TextView> (Resource.Id.addressTextView);
				task.Result.ToList ().ForEach ((addr) => addrText.Append (addr.ToString () + "\r\n\r\n"));

			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LocationView);

			// use location service directly       
			_locMgr = GetSystemService (LocationService) as LocationManager;
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			_locMgr.RemoveUpdates (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			Criteria locationCriteria = new Criteria ();
			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.NoRequirement;

			string locationProvider = _locMgr.GetBestProvider (locationCriteria, true);

			if (!String.IsNullOrEmpty (locationProvider)) {
				_locMgr.RequestLocationUpdates (locationProvider, 2000, 1, this);
			} else {
				Log.Warn ("LocationDemo", "Could not determine a location provider.");
			}
		}
	}
}
