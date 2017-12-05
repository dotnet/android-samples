using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Location;
using Android.Util;
using Android.Locations;
using Android.Support.Design.Widget;
using Android.Views;
using Java.Util;
using static Android.Views.View;
using Android.Support.V4.App;
using Android.Support.V4.Content;

namespace BasicLocationSample
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected string Tag = typeof(MainActivity).Name;

        protected const int RequestPermissionsRequestCode = 34;

        /**
         * Provides the entry point to the Fused Location Provider API.
         */
        private FusedLocationProviderClient mFusedLocationClient;

        /**
         * Represents a geographical location.
         */
        protected Location mLastLocation;

        private string mLatitudeLabel;
        private string mLongitudeLabel;
        private TextView mLatitudeText;
        private TextView mLongitudeText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mLatitudeLabel = GetString(Resource.Id.latitude_text);
            mLongitudeLabel = GetString(Resource.Id.longitude_label);
            mLatitudeText = FindViewById<TextView>(Resource.Id.latitude_text);
            mLongitudeText = FindViewById<TextView>(Resource.Id.longitude_label);

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
            //mFusedLocationClient.LastLocation.AddOnCompleteListener(new OnCompleteListener(...));
            var task = mFusedLocationClient.LastLocation;
            if (task.IsSuccessful && task.Result != null)
            {
                mLastLocation = (Location)task.Result;

                mLatitudeText.Text = String.Format(Locale.English.ToString(), "%s: %f", mLatitudeLabel, mLastLocation.Latitude);
                mLongitudeText.Text = String.Format(Locale.English.ToString(), "%s: %f", mLongitudeLabel, mLastLocation.Longitude);
            }
            else
            {
                Log.Warn(Tag, "GetLastLocation:exception", task.Exception);
                ShowSnackbar(GetString(Resource.String.no_location_detected));
            }
        }

        private void ShowSnackbar(String text)
        {
            View container = FindViewById<TextView>(Resource.Id.main_activity_container);
            if (container != null)
            {
                Snackbar.Make(container, text, Snackbar.LengthLong).Show();
            }
        }

        private void ShowSnackbar(int mainTextStringId, int actionStringId, IOnClickListener listener)
        {
            Snackbar.Make(FindViewById(Resource.Id.wrap_content), GetString(mainTextStringId), Snackbar.LengthIndefinite)
                .SetAction(GetString(actionStringId), listener).Show();
        }

        private bool CheckPermissions()
        {
            var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            return permissionState == (int)Permission.Granted;
        }

        private void StartLocationPermissionRequest()
        {
            ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessCoarseLocation }, RequestPermissionsRequestCode);
        }

        private void RequestPermissions()
        {
            bool shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessCoarseLocation);

            if (shouldProvideRationale)
            {
                Log.Info(Tag, "Displaying permission rationale to provide additional context.");

                //ShowSnackbar(Resource.String.permission_rationale, android.R.string.ok, new View.OnClickListener() {
                //    public void onClick(View view)
                //    {
                //    StartLocationPermissionRequest();
                //}
                //});

            }
            else
            {
                Log.Info(Tag, "Requesting permission");
                StartLocationPermissionRequest();
            }
        }

        public void OnRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
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
                    //    ShowSnackbar(Resource.String.permission_denied_explanation, R.string.settings,
                    //            new View.OnClickListener() {
                    //            @Override
                    //                public void onClick(View view)
                    //    {
                    //        // Build intent that displays the App settings screen.
                    //        Intent intent = new Intent();
                    //        intent.setAction(
                    //                Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
                    //        Uri uri = Uri.fromParts("package",
                    //                BuildConfig.APPLICATION_ID, null);
                    //        intent.setData(uri);
                    //        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    //        startActivity(intent);
                    //    }
                    //});
                }
            }
        }
    }
}

//public class OnCompleteListener : IOnCompleteListener {
//    protected Location mLastLocation;
//    private string mLatitudeLabel;
//    private string mLongitudeLabel;
//    private TextView mLatitudeText;
//    private TextView mLongitudeText;

//    public OnCompleteListener(Location mLastLocation, string mLatitudeLabel, string mLongitudeLabel, TextView mLatitudeText, TextView mLongitudeText)
//    {
//        this.mLastLocation = mLastLocation;
//        this.mLatitudeLabel = mLatitudeLabel;
//        this.mLongitudeLabel = mLongitudeLabel;
//        this.mLatitudeText = mLatitudeText;
//        this.mLongitudeText = mLongitudeText;
//    }

//    protected string TAG = typeof(MainActivity).Name;
//    public IntPtr Handle { get; set; }
//    public void Dispose() {}

//    public void OnComplete(Task task)
//    {
//        if (task.IsSuccessful && task.Result != null)
//        {
//            mLastLocation = (Location)task.Result;

//            mLatitudeText.Text = String.Format(Locale.English.ToString(), "%s: %f", mLatitudeLabel, mLastLocation.Latitude);
//            mLongitudeText.Text = String.Format(Locale.English.ToString(), "%s: %f", mLongitudeLabel, mLastLocation.Longitude);
//        }
//        else
//        {
//            Log.Warn(TAG, "getLastLocation:exception", task.Exception);
//            //ShowSnackbar(Context.GetString(Resource.Id.no_location_detected));
//        }
//    }
//}