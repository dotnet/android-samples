using System;
using Android;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Java.Text;
using Java.Util;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LocationUpdates
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        public readonly string Tag = typeof(MainActivity).Name;
        public readonly int RequestPermissionsRequestCode = 34;

        public int RequestCheckSettings = 0x1;
        public static long UpdateIntervalInMilliseconds = 10000;
        public readonly long FastestUpdateIntervalInMilliseconds = UpdateIntervalInMilliseconds / 2;

        public readonly string KeyRequestingLocationUpdates = "requesting-location-updates";
        public readonly string KeyLocation = "location";
        public readonly string KeyLastUpdatedTimeString = "last-updated-time-string";

        public FusedLocationProviderClient mFusedLocationClient;

        public SettingsClient mSettingsClient;

        public LocationRequest mLocationRequest;

        public LocationSettingsRequest mLocationSettingsRequest;

        public LocationCallback mLocationCallback;

        public Location mCurrentLocation;

        private Button mStartUpdatesButton;
        private Button mStopUpdatesButton;
        private TextView mLastUpdateTimeTextView;
        private TextView mLatitudeTextView;
        private TextView mLongitudeTextView;

        private string mLatitudeLabel;
        private string mLongitudeLabel;
        private string mLastUpdateTimeLabel;

        public bool mRequestingLocationUpdates;

        public string mLastUpdateTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mStartUpdatesButton = FindViewById<Button>(Resource.Id.start_updates_button);
            mStopUpdatesButton = FindViewById<Button>(Resource.Id.stop_updates_button);
            mLatitudeTextView = FindViewById<TextView>(Resource.Id.latitude_text);
            mLongitudeTextView = FindViewById<TextView>(Resource.Id.longitude_text);
            mLastUpdateTimeTextView = FindViewById<TextView>(Resource.Id.last_update_time_text);

            mStartUpdatesButton.Click += StartUpdatesButtonHandler;
            mStopUpdatesButton.Click += StopUpdatesButtonHandler;

            mLatitudeLabel = GetString(Resource.String.latitude_label);
            mLongitudeLabel = GetString(Resource.String.longitude_label);
            mLastUpdateTimeLabel = GetString(Resource.String.last_update_time_label);

            mRequestingLocationUpdates = false;
            mLastUpdateTime = string.Empty;

            UpdateValuesFromBundle(savedInstanceState);

            mFusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);
            mSettingsClient = LocationServices.GetSettingsClient(this);

            CreateLocationCallback();
            CreateLocationRequest();
            BuildLocationSettingsRequest();
        }

        public void UpdateValuesFromBundle(Bundle savedInstanceState)
        {
            if (savedInstanceState == null) return;
            if (savedInstanceState.KeySet().Contains(KeyRequestingLocationUpdates))
            {
                mRequestingLocationUpdates = savedInstanceState.GetBoolean(KeyRequestingLocationUpdates);
            }

            if (savedInstanceState.KeySet().Contains(KeyLocation))
            {
                mCurrentLocation = (Location)savedInstanceState.GetParcelable(KeyLocation);
            }

            if (savedInstanceState.KeySet().Contains(KeyLastUpdatedTimeString))
            {
                mLastUpdateTime = savedInstanceState.GetString(KeyLastUpdatedTimeString);
            }
            UpdateUi();
        }

        public void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();

            mLocationRequest.SetInterval(UpdateIntervalInMilliseconds);

            mLocationRequest.SetFastestInterval(FastestUpdateIntervalInMilliseconds);

            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
        }

        public void CreateLocationCallback()
        {
            mLocationCallback = new LocationCallbackClass { Activity = this };
        }

        public void BuildLocationSettingsRequest()
        {
            LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
            builder.AddLocationRequest(mLocationRequest);
            mLocationSettingsRequest = builder.Build();
        }

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
            const int requestCheckSettings = 0x1;

            switch (requestCode)
            {
                case requestCheckSettings:
                    switch (resultCode)
                    {
                        case Result.Ok:
                            Log.Info(Tag, "User agreed to make required location settings changes.");
                            break;
                        case Result.Canceled:
                            Log.Info(Tag, "User chose not to make required location settings changes.");
                            mRequestingLocationUpdates = false;
                            UpdateUi();
                            break;
                    }
                    break;
            }
        }


        public void StartUpdatesButtonHandler(object sender, EventArgs eventArgs)
        {
            if (mRequestingLocationUpdates) return;
            mRequestingLocationUpdates = true;
            SetButtonsEnabledState();
            StartLocationUpdates();
        }

        public void StopUpdatesButtonHandler(object sender, EventArgs eventArgs)
        {
            StopLocationUpdates();
        }

        public void StartLocationUpdates()
        {
            var task = mSettingsClient.CheckLocationSettings(mLocationSettingsRequest);
            task.AddOnSuccessListener(this, new StartLocationUpdatesOnSuccessListener { Activity = this });
            task.AddOnFailureListener(this, new StartLocationUpdatesOnFailureListener { Activity = this });
        }

        public void UpdateUi()
        {
            SetButtonsEnabledState();
            UpdateLocationUi();
        }

        public void SetButtonsEnabledState()
        {
            if (mRequestingLocationUpdates)
            {
                mStartUpdatesButton.Enabled = false;
                mStopUpdatesButton.Enabled = true;
            }
            else
            {
                mStartUpdatesButton.Enabled = true;
                mStopUpdatesButton.Enabled = false;
            }
        }

        public void UpdateLocationUi()
        {
            if (mCurrentLocation == null) return;
            mLatitudeTextView.Text = $"{mLatitudeLabel}: {mCurrentLocation.Latitude}";
            mLongitudeTextView.Text = $"{mLongitudeLabel}: {mCurrentLocation.Longitude}";
            mLastUpdateTimeTextView.Text = $"{mLastUpdateTimeLabel}: {mLastUpdateTime}";
        }

        public void StopLocationUpdates()
        {
            if (!mRequestingLocationUpdates)
            {
                Log.Debug(Tag, "stopLocationUpdates: updates never requested, no-op.");
                return;
            }
            mFusedLocationClient.RemoveLocationUpdates(mLocationCallback).AddOnCompleteListener(this, new StopLocationUpdatesOnCompleteListener { Activity = this });
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (mRequestingLocationUpdates && CheckPermissions())
            {
                StartLocationUpdates();
            }
            else if (!CheckPermissions())
            {
                RequestPermissions();
            }

            UpdateUi();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopLocationUpdates();
        }

        protected override void OnSaveInstanceState(Bundle savedInstanceState)
        {
            savedInstanceState.PutBoolean(KeyRequestingLocationUpdates, mRequestingLocationUpdates);
            savedInstanceState.PutParcelable(KeyLocation, mCurrentLocation);
            savedInstanceState.PutString(KeyLastUpdatedTimeString, mLastUpdateTime);
            base.OnSaveInstanceState(savedInstanceState);
        }

        public void ShowSnackbar(int mainTextStringId, int actionStringId, View.IOnClickListener listener)
        {
            Snackbar.Make(FindViewById(Android.Resource.Id.Content), GetString(mainTextStringId), Snackbar.LengthIndefinite)
                .SetAction(GetString(actionStringId), listener).Show();
        }

        public bool CheckPermissions()
        {
            var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
            return permissionState == Permission.Granted;
        }

        public void RequestPermissions()
        {
            bool shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation);

            if (shouldProvideRationale)
            {
                Log.Info(Tag, "Displaying permission rationale to provide additional context.");
                ShowSnackbar(Resource.String.permission_rationale, Android.Resource.String.Ok, new RequestPermissionsOnClickListener { Activity = this });
            }
            else
            {
                Log.Info(Tag, "Requesting permission");
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, RequestPermissionsRequestCode);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Log.Info(Tag, "onRequestPermissionResult");
            if (requestCode == RequestPermissionsRequestCode)
            {
                if (grantResults.Length <= 0)
                {
                    Log.Info(Tag, "User interaction was cancelled.");
                }
                else if (grantResults[0] == Permission.Granted)
                {
                    if (mRequestingLocationUpdates)
                    {
                        Log.Info(Tag, "Permission granted, updates requested, starting location updates");
                        StartLocationUpdates();
                    }
                }
                else
                {
                    ShowSnackbar(Resource.String.permission_denied_explanation, Resource.String.settings, new OnRequestPermissionsResultOnClickListener { Activity = this });
                }
            }
        }
    }

    public class OnRequestPermissionsResultOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        public MainActivity Activity { get; set; }
        public void OnClick(View v)
        {
            Intent intent = new Intent();
            intent.SetAction(Settings.ActionApplicationDetailsSettings);
            var uri = Android.Net.Uri.FromParts("package", BuildConfig.ApplicationId, null);
            intent.SetData(uri);
            intent.SetFlags(ActivityFlags.NewTask);
            Activity.StartActivity(intent);
        }
    }


    public class LocationCallbackClass : LocationCallback
    {
        public MainActivity Activity { get; set; }

        public override void OnLocationResult(LocationResult locationResult)
        {
            base.OnLocationResult(locationResult);

            Activity.mCurrentLocation = locationResult.LastLocation;
            Activity.mLastUpdateTime = DateFormat.TimeInstance.Format(new Date());
            Activity.UpdateLocationUi();
        }
    }

    public class StartLocationUpdatesOnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public MainActivity Activity { get; set; }

        public void OnSuccess(Java.Lang.Object result)
        {
            Log.Info(Activity.Tag, "All location settings are satisfied.");
            Activity.mFusedLocationClient.RequestLocationUpdates(Activity.mLocationRequest, Activity.mLocationCallback, Looper.MyLooper());
            Activity.UpdateUi();
        }
    }

    public class StartLocationUpdatesOnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public MainActivity Activity { get; set; }

        public void OnFailure(Java.Lang.Exception e)
        {
            int statusCode = ((ApiException)e).StatusCode;
            switch (statusCode)
            {
                case CommonStatusCodes.ResolutionRequired:
                    Log.Info(Activity.Tag, "Location settings are not satisfied. Attempting to upgrade location settings ");
                    try
                    {
                        ResolvableApiException rae = (ResolvableApiException)e;
                        rae.StartResolutionForResult(Activity, Activity.RequestCheckSettings);
                    }
                    catch (IntentSender.SendIntentException)
                    {
                        Log.Info(Activity.Tag, "PendingIntent unable to execute request.");
                    }
                    break;
                case LocationSettingsStatusCodes.SettingsChangeUnavailable:
                    string errorMessage = "Location settings are inadequate, and cannot be fixed here. Fix in Settings.";
                    Log.Error(Activity.Tag, errorMessage);
                    Toast.MakeText(Activity, errorMessage, ToastLength.Long).Show();
                    Activity.mRequestingLocationUpdates = false;
                    break;
            }

            Activity.UpdateUi();
        }
    }

    public class StopLocationUpdatesOnCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        public MainActivity Activity { get; set; }

        public void OnComplete(Task task)
        {
            Activity.mRequestingLocationUpdates = false;
            Activity.SetButtonsEnabledState();
        }
    }

    public class RequestPermissionsOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        public MainActivity Activity { get; set; }
        public void OnClick(View v)
        {
            ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.AccessFineLocation }, Activity.RequestPermissionsRequestCode);
        }
    }
}