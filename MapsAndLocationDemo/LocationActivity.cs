using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading;

namespace MapsAndLocationDemo
{
    [Activity (Label = "LocationActivity")]          
    public class LocationActivity : Activity, ILocationListener
    {
        LocationManager _locMgr;
     
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.LocationView);
            
            // use location service directly       
            _locMgr = GetSystemService (Context.LocationService) as LocationManager;
        }
     
        protected override void OnResume ()
        {
            base.OnResume ();
         
            var locationCriteria = new Criteria ();
         
            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;
         
            string locationProvider = _locMgr.GetBestProvider (locationCriteria, true);
            
            _locMgr.RequestLocationUpdates (locationProvider, 2000, 1, this);
            //_locMgr.RequestLocationUpdates (LocationManager.GpsProvider, 2000, 1, this);
        }
        
        protected override void OnPause ()
        {
            base.OnPause ();
            
            _locMgr.RemoveUpdates (this);
        }

     #region ILocationListener implementation
        public void OnLocationChanged (Location location)
        {
            var locationText = FindViewById<TextView> (Resource.Id.locationTextView);
         
            locationText.Text = String.Format ("Latitude = {0}, Longitude = {1}", location.Latitude, location.Longitude);
         
            // demo geocoder
         
            new Thread (new ThreadStart (() => {
                var geocdr = new Geocoder (this);
             
                var addresses = geocdr.GetFromLocation (location.Latitude, location.Longitude, 5);
             
                //var addresses = geocdr.GetFromLocationName("Harvard University", 5);
             
                RunOnUiThread (() => {
                    var addrText = FindViewById<TextView> (Resource.Id.addressTextView);
         
                    addresses.ToList ().ForEach ((addr) => {
                        addrText.Append (addr.ToString () + "\r\n\r\n");
                    });
                });
             
            })).Start ();
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
     #endregion


    }
}

