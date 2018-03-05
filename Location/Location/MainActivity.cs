using System;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.xamarin.samples.location.locationmanager
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    //Implement ILocationListener interface to get location updates
    public class MainActivity : AppCompatActivity, ILocationListener
    {
        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        Button getLastLocationButton;
        bool isRequestingLocationUpdates;
        TextView latitude;
        internal TextView latitude2;
        LocationManager locationManager;
        TextView longitude;
        internal TextView longitude2;
        TextView provider;
        internal TextView provider2;
        internal Button requestLocationUpdatesButton;
        View rootLayout;

        public void OnLocationChanged(Location location)
        {
            latitude2.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            longitude2.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            provider2.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
        }

        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
            requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            latitude2.Text = string.Empty;
            longitude2.Text = string.Empty;
            provider2.Text = string.Empty;
        }

        public void OnProviderEnabled(string provider)
        {
            // Nothing to do in this example.
            Log.Debug("LocationExample", "The provider " + provider + " is enabled.");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        GetLastLocationFromDevice();
                    }
                    else
                    {
                        isRequestingLocationUpdates = true;
                        StartRequestingLocationUpdates();
                    }
                }
                else
                {
                    Snackbar.Make(rootLayout, Resource.String.permission_not_granted_termininating_app, Snackbar.LengthIndefinite)
                            .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                            .Show();
                    return;
                }
            }
            else
            {
                Log.Debug("LocationSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            locationManager = (LocationManager) GetSystemService(LocationService);

            if (bundle != null)
            {
                isRequestingLocationUpdates = bundle.KeySet().Contains(KEY_REQUESTING_LOCATION_UPDATES) &&
                                              bundle.GetBoolean(KEY_REQUESTING_LOCATION_UPDATES);
            }
            else
            {
                isRequestingLocationUpdates = false;
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            rootLayout = FindViewById(Resource.Id.root_layout);

			getLastLocationButton = FindViewById<Button>(Resource.Id.get_last_location_button);
			latitude = FindViewById<TextView>(Resource.Id.latitude);
			longitude = FindViewById<TextView>(Resource.Id.longitude);
			provider = FindViewById<TextView>(Resource.Id.provider);

			requestLocationUpdatesButton = FindViewById<Button>(Resource.Id.request_location_updates_button);
			latitude2 = FindViewById<TextView>(Resource.Id.latitude2);
			longitude2 = FindViewById<TextView>(Resource.Id.longitude2);
			provider2 = FindViewById<TextView>(Resource.Id.provider2);
			
            if (locationManager.AllProviders.Contains(LocationManager.NetworkProvider)
                && locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                getLastLocationButton.Click += GetLastLocationButtonOnClick;
                requestLocationUpdatesButton.Click += RequestLocationUpdatesButtonOnClick;
            }
            else
            {
                Snackbar.Make(rootLayout, Resource.String.missing_gps_location_provider, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                        .Show();
            }
        }

        void RequestLocationUpdatesButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (isRequestingLocationUpdates)
            {
                isRequestingLocationUpdates = false;
                StopRequestingLocationUpdates();
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    StartRequestingLocationUpdates();
                    isRequestingLocationUpdates = true;
                }
                else
                {
                    RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
                }
            }            
        }

        void GetLastLocationButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                GetLastLocationFromDevice();
            }
            else
            {
                RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
            }
        }

        void GetLastLocationFromDevice()
        {
            getLastLocationButton.SetText(Resource.String.getting_last_location);

            var criteria = new Criteria {PowerRequirement = Power.Medium};

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
            {
                latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
                longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
                provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
                getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
            else
            {
                latitude.SetText(Resource.String.location_unavailable);
                longitude.SetText(Resource.String.location_unavailable);
                provider.Text = Resources.GetString(Resource.String.provider_string, bestProvider);
                getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            locationManager = GetSystemService(LocationService) as LocationManager;
        }

        protected override void OnPause()
        {
            locationManager.RemoveUpdates(this);
            base.OnPause();
        }

        void RequestLocationPermission(int requestCode)
        {
            isRequestingLocationUpdates = false;
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                Snackbar.Make(rootLayout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                                   delegate
                                   {
                                       ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.AccessFineLocation}, requestCode);
                                   })
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.AccessFineLocation}, requestCode);
            }
        }

        void StartRequestingLocationUpdates()
        {
            requestLocationUpdatesButton.SetText(Resource.String.request_location_in_progress_button_text);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, ONE_MINUTE, 1, this);
        }

        void StopRequestingLocationUpdates()
        {
            latitude2.Text = string.Empty;
            longitude2.Text = string.Empty;
            provider2.Text = string.Empty;

            requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            locationManager.RemoveUpdates(this);
        }
    }
}
