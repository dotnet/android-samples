using System;
using System.Threading.Tasks;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.xamarin.samples.location.fusedlocationprovider
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        const long TWO_MINUTES = 2 * ONE_MINUTE;

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        FusedLocationProviderClient fusedLocationProviderClient;
        Button getLastLocationButton;
        bool isGooglePlayServicesInstalled;
        bool isRequestingLocationUpdates;
        TextView latitude;
        internal TextView latitude2;
        LocationCallback locationCallback;
        LocationRequest locationRequest;
        TextView longitude;
        internal TextView longitude2;
        TextView provider;
        internal TextView provider2;

        internal Button requestLocationUpdatesButton;

        View rootLayout;

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        await GetLastLocationFromDevice();
                    }
                    else
                    {
                        await StartRequestingLocationUpdates();
                        isRequestingLocationUpdates = true;
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
                Log.Debug("FusedLocationProviderSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
            isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();
            rootLayout = FindViewById(Resource.Id.root_layout);

            // UI to display last location
            getLastLocationButton = FindViewById<Button>(Resource.Id.get_last_location_button);
            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            provider = FindViewById<TextView>(Resource.Id.provider);

            // UI to display location updates
            requestLocationUpdatesButton = FindViewById<Button>(Resource.Id.request_location_updates_button);
            latitude2 = FindViewById<TextView>(Resource.Id.latitude2);
            longitude2 = FindViewById<TextView>(Resource.Id.longitude2);
            provider2 = FindViewById<TextView>(Resource.Id.provider2);

            if (isGooglePlayServicesInstalled)
            {
                locationRequest = new LocationRequest()
                                  .SetPriority(LocationRequest.PriorityHighAccuracy)
                                  .SetInterval(FIVE_MINUTES)
                                  .SetFastestInterval(TWO_MINUTES);
                locationCallback = new FusedLocationProviderCallback(this);

                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
                getLastLocationButton.Click += GetLastLocationButtonOnClick;
                requestLocationUpdatesButton.Click += RequestLocationUpdatesButtonOnClick;
            }
            else
            {
                // If there is no Google Play Services installed, then this sample won't run.
                Snackbar.Make(rootLayout, Resource.String.missing_googleplayservices_terminating, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                        .Show();
            }
        }

        async void RequestLocationUpdatesButtonOnClick(object sender, EventArgs eventArgs)
        {
            // No need to request location updates if we're already doing so.
            if (isRequestingLocationUpdates)
            {
                StopRequestionLocationUpdates();
                isRequestingLocationUpdates = false;
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    await StartRequestingLocationUpdates();
                    isRequestingLocationUpdates = true;
                }
                else
                {
                    RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
                }
            }
        }

        async void GetLastLocationButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                await GetLastLocationFromDevice();
            }
            else
            {
                RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
            }
        }

        async Task GetLastLocationFromDevice()
        {
            getLastLocationButton.SetText(Resource.String.getting_last_location);
            var location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                latitude.SetText(Resource.String.location_unavailable);
                longitude.SetText(Resource.String.location_unavailable);
                provider.SetText(Resource.String.could_not_get_last_location);
            }
            else
            {
                latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
                longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
                provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
                getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
        }

        void RequestLocationPermission(int requestCode)
        {
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

        async Task StartRequestingLocationUpdates()
        {
            requestLocationUpdatesButton.SetText(Resource.String.request_location_in_progress_button_text);
            await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        }

        async void StopRequestionLocationUpdates()
        {
            latitude2.Text = string.Empty;
            longitude2.Text = string.Empty;
            provider2.Text = string.Empty;

            requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);

            if (isRequestingLocationUpdates)
            {
                await fusedLocationProviderClient.RemoveLocationUpdatesAsync(locationCallback);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(KEY_REQUESTING_LOCATION_UPDATES, isRequestingLocationUpdates);
            base.OnSaveInstanceState(outState);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                if (isRequestingLocationUpdates)
                {
                    await StartRequestingLocationUpdates();
                }
            }
            else
            {
                RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
            }
        }

        protected override void OnPause()
        {
            StopRequestionLocationUpdates();
            base.OnPause();
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);
            }

            return false;
        }
    }
}
