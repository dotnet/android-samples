namespace MapsAndLocationDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public async void OnLocationChanged(Location location)
        {
            TextView locationText = FindViewById<TextView>(Resource.Id.locationTextView);

            locationText.Text = String.Format("Latitude = {0:N5}, Longitude = {1:N5}", location.Latitude, location.Longitude);

            Geocoder geocdr = new Geocoder(this);
            Task<IList<Address>> getAddressTask = geocdr.GetFromLocationAsync(location.Latitude, location.Longitude, 5);
            TextView addressTextView = FindViewById<TextView>(Resource.Id.addressTextView);
            addressTextView.Text = "Trying to reverse geo-code the latitude/longitude...";

            IList<Address> addresses = await getAddressTask;

            if (addresses.Any())
            {
                Address addr = addresses.First();
                addressTextView.Text = FormatAddress(addr);
            }
            else
            {
                Toast.MakeText(this, "Could not reverse geo-code the location", ToastLength.Short).Show();
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LocationView);

            // use location service directly       
            _locMgr = GetSystemService(LocationService) as LocationManager;
        }

        protected override void OnPause()
        {
            base.OnPause();

            _locMgr.RemoveUpdates(this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            string locationProvider = _locMgr.GetBestProvider(locationCriteria, true);

            if (!String.IsNullOrEmpty(locationProvider))
            {
                _locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
            }
            else
            {
                Log.Warn("LocationDemo", "Could not determine a location provider.");
            }
        }

        private string FormatAddress(Address addr)
        {
            StringBuilder addressText = new StringBuilder();
            addressText.Append(addr.SubThoroughfare);
            addressText.AppendFormat(" {0}", addr.Thoroughfare);
            addressText.AppendFormat(", {0}", addr.Locality);
            addressText.AppendFormat(", {0}", addr.CountryCode);
            addressText.AppendLine();
            addressText.AppendLine();
            return addressText.ToString();
        }
    }
}
