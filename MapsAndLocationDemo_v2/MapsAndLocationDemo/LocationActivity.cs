namespace MapsAndLocationDemo
{
    using System;
    using System.Linq;
    using System.Threading;

    using Android.App;
    using Android.Locations;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "@string/activity_label_location")]
    public class LocationActivity : Activity, ILocationListener
    {
        private LocationManager _locMgr;

        public void OnLocationChanged(Location location)
        {
            var locationText = FindViewById<TextView>(Resource.Id.locationTextView);

            locationText.Text = String.Format("Latitude = {0}, Longitude = {1}", location.Latitude, location.Longitude);

            // demo geocoder

            new Thread(() =>
                           {
                               var geocdr = new Geocoder(this);

                               var addresses = geocdr.GetFromLocation(location.Latitude, location.Longitude, 5);

                               RunOnUiThread(() =>
                                                 {
                                                     var addrText = FindViewById<TextView>(Resource.Id.addressTextView);

                                                     addresses.ToList().ForEach((addr) => addrText.Append(addr.ToString() + "\r\n\r\n"));
                                                 });
                           }).Start();
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

            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            var locationProvider = _locMgr.GetBestProvider(locationCriteria, true);

            _locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
        }
    }
}
