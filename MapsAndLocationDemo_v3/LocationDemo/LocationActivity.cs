namespace MapsAndLocationDemo
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Android.App;
	using Android.Locations;
	using Android.OS;
	using Android.Util;
	using Android.Widget;
	using LocationDemo;

	[Activity(Label = "@string/activity_label_location", MainLauncher = true)]
	public class LocationActivity : Activity, ILocationListener
	{
		private LocationManager _locationManager;

		public void OnLocationChanged (Location location)
		{
			TextView locationText = FindViewById<TextView> (Resource.Id.locationTextView);

			locationText.Text = String.Format ("Latitude = {0}, Longitude = {1}", location.Latitude, location.Longitude);

			// demo geocoder
			new Thread (() => {
				Geocoder geocdr = new Geocoder (this);

				IList<Address> addresses = geocdr.GetFromLocation (location.Latitude, location.Longitude, 5);

				RunOnUiThread (() => {
					TextView addrText = FindViewById<TextView> (Resource.Id.addressTextView);

					addresses.ToList ().ForEach ((addr) => addrText.Append (addr.ToString () + "\r\n\r\n"));
				});
			}).Start ();

			// remove the mock location provider
			if (_locationManager.GetProvider ("MockLocationProvider") != null) {
				_locationManager.RemoveTestProvider ("MockLocationProvider");
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			var deviceLocationButton = FindViewById<Button> (Resource.Id.deviceLocation);
			var mockLocationButton = FindViewById<Button> (Resource.Id.mockLocation); 

			Criteria locationCriteria = new Criteria ();
			locationCriteria.Accuracy = Accuracy.Fine;
			locationCriteria.PowerRequirement = Power.NoRequirement;

			TextView addrText = FindViewById<TextView> (Resource.Id.addressTextView);

			deviceLocationButton.Click += (sender, e) => {

				// get the location from the best device provider

				string locationProvider = _locationManager.GetBestProvider (locationCriteria, true);

				if (!String.IsNullOrEmpty (locationProvider)) {
					_locationManager.RequestLocationUpdates (locationProvider, 2000, 1, this);
				} else {
					Log.Warn ("LocationDemo", "Could not determine a location provider.");
				}
			};   

			mockLocationButton.Click += (sender, e) => {

				// get the location from a mock provider

				// remove the mock location provider
				if (_locationManager.GetProvider ("MockLocationProvider") != null) {
					_locationManager.RemoveTestProvider ("MockLocationProvider");
				}

				//locationCriteria.Accuracy = Accuracy.Coarse;
				// Does not work, must use Android.Hardware.SensorStatus
				_locationManager.AddTestProvider ("MockLocationProvider", true, false, false, false, true, true, true, locationCriteria.PowerRequirement, Android.Hardware.SensorStatus.AccuracyHigh);

				Location loc = new Location ("MockLocationProvider");
				loc.Latitude = 37.763319;
				loc.Longitude = -122.388255;

				_locationManager.SetTestProviderEnabled ("MockLocationProvider", true);

				_locationManager.RequestLocationUpdates ("MockLocationProvider", 2000, 0, this);

				_locationManager.SetTestProviderLocation ("MockLocationProvider", loc);
			};  
		}

		public void OnProviderDisabled (string provider)
		{
			var builder = new AlertDialog.Builder (this)
				.SetTitle ("Notice")
					.SetMessage (provider + "provider disabled.")
					.SetPositiveButton ("Ok", (innerSender, innere) => { });
			var dialog = builder.Create ();
			dialog.Show ();
		}

		public void OnProviderEnabled (string provider)
		{
			var builder = new AlertDialog.Builder (this)
				.SetTitle ("Notice")
					.SetMessage (provider + "provider enabled.")
					.SetPositiveButton ("Ok", (innerSender, innere) => { });
			var dialog = builder.Create ();
			dialog.Show ();
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			var builder = new AlertDialog.Builder (this)
				.SetTitle ("Notice")
					.SetMessage (provider + "provider has status " + status)
					.SetPositiveButton ("Ok", (innerSender, innere) => { });
			var dialog = builder.Create ();
			dialog.Show ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LocationView);

			_locationManager = GetSystemService (LocationService) as LocationManager;
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			_locationManager.RemoveUpdates (this);
		}
	}
}