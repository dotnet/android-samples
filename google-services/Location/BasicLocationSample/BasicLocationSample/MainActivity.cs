using Android;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Util;
using Android.Locations;
using Android.Net;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Views;
using static Android.Views.View;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using BasicLocationSample;
using Resource = BasicLocationSample.Resource;

namespace BasicLocationSample
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public string Tag = typeof(MainActivity).Name;

        protected const int RequestPermissionsRequestCode = 34;

        /**
         * Provides the entry point to the Fused Location Provider API.
         */
        private FusedLocationProviderClient mFusedLocationClient;

        /**
         * Represents a geographical location.
         */
        public Location mLastLocation;

        public string mLatitudeLabel;
        public string mLongitudeLabel;
        public TextView mLatitudeText;
        public TextView mLongitudeText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mLatitudeLabel = GetString(Resource.String.latitude_label);
            mLongitudeLabel = GetString(Resource.String.longitude_label);
            mLatitudeText = FindViewById<TextView>(Resource.Id.latitude_text);
            mLongitudeText = FindViewById<TextView>(Resource.Id.longitude_text);

            mFusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (!CheckPermissions())
            {
                RequestPermissions();
            }
            else
            {
                GetLastLocation();
            }
        }

        private void GetLastLocation()
        {
            mFusedLocationClient.LastLocation.AddOnCompleteListener(new OnCompleteListener { Activity = this });
        }

        public void ShowSnackbar(string text)
        {
            View container = FindViewById<TextView>(Resource.Id.main_activity_container);
            if (container != null)
            {
                Snackbar.Make(container, text, Snackbar.LengthLong).Show();
            }
        }

        public void ShowSnackbar(int mainTextStringId, int actionStringId, IOnClickListener listener)
        {
            Snackbar.Make(FindViewById(Resource.Id.wrap_content), GetString(mainTextStringId), Snackbar.LengthIndefinite)
                .SetAction(GetString(actionStringId), listener).Show();
        }

        private bool CheckPermissions()
        {
            var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            return permissionState == (int)Permission.Granted;
        }

        public void StartLocationPermissionRequest()
        {
            ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessCoarseLocation }, RequestPermissionsRequestCode);
        }

        private void RequestPermissions()
        {
            bool shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessCoarseLocation);

            if (shouldProvideRationale)
            {
                Log.Info(Tag, "Displaying permission rationale to provide additional context.");
                ShowSnackbar(Resource.String.permission_rationale, Android.Resource.String.Ok, new RequestPermissionsClickListener { Activity = this });
            }
            else
            {
                Log.Info(Tag, "Requesting permission");
                StartLocationPermissionRequest();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Log.Info(Tag, "OnRequestPermissionResult");
            if (requestCode == RequestPermissionsRequestCode)
            {
                if (grantResults.Length <= 0)
                {
                    Log.Info(Tag, "User interaction was cancelled.");
                }
                else if (grantResults[0] == (int)Permission.Granted)
                {
                    GetLastLocation();
                }
                else
                {
                    ShowSnackbar(Resource.String.permission_denied_explanation, Resource.String.settings, new OnRequestPermissionsResultClickListener { Activity = this });
                }
            }
        }
    }
}

public class RequestPermissionsClickListener : Java.Lang.Object, IOnClickListener
{
    public MainActivity Activity { get; set; }
    public void OnClick(View v)
    {
        Activity.StartLocationPermissionRequest();
    }
}

public class OnRequestPermissionsResultClickListener : Java.Lang.Object, IOnClickListener
{
    public MainActivity Activity { get; set; }
    public void OnClick(View v)
    {
        var intent = new Intent();
        intent.SetAction(Settings.ActionApplicationDetailsSettings);
        var uri = Uri.FromParts("package", BuildConfig.ApplicationId, null);
        intent.SetData(uri);
        intent.SetFlags(ActivityFlags.NewTask);
        Activity.StartActivity(intent);
    }
}

public class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
{
    public MainActivity Activity { get; set; }
    public void OnComplete(Task task)
    {
        if (task.IsSuccessful && task.Result != null)
        {
            Activity.mLastLocation = (Location)task.Result;

            Activity.mLatitudeText.Text = $"{Activity.mLatitudeLabel}: {Activity.mLastLocation.Latitude}";
            Activity.mLongitudeText.Text = $"{Activity.mLongitudeLabel}: {Activity.mLastLocation.Longitude}";
        }
        else
        {
            Log.Warn(Activity.Tag, "getLastLocation:exception", task.Exception);
            Activity.ShowSnackbar(Activity.GetString(Resource.String.no_location_detected));
        }
    }
}