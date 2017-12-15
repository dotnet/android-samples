using System;
using Android;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Locations;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;
using String = System.String;
using Uri = Android.Net.Uri;

namespace LocationAddress
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public readonly string TAG = typeof(MainActivity).Name;
        public readonly int RequestPermissionsRequestCode = 34;
        public const string AddressRequestedKey = "address-request-pending";
        public const string LocationAddressKey = "location-address";
        public FusedLocationProviderClient mFusedLocationClient;
        public Location mLastLocation;
        public bool mAddressRequested;
        public string mAddressOutput;
        public AddressResultReceiver mResultReceiver;
        public TextView mLocationAddressTextView;
        public ProgressBar mProgressBar;
        public Button mFetchAddressButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mResultReceiver = new AddressResultReceiver(new Handler()) { Activity = this };

            mLocationAddressTextView = FindViewById<TextView>(Resource.Id.location_address_view);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            mFetchAddressButton = FindViewById<Button>(Resource.Id.fetch_address_button);
            mFetchAddressButton.Click += FetchAddressButtonHandler;

            mAddressRequested = false;
            mAddressOutput = string.Empty;
            UpdateValuesFromBundle(savedInstanceState);

            mFusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);

            UpdateUiWidgets();
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
                GetAddress();
            }
        }

        private void UpdateValuesFromBundle(Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                if (savedInstanceState.KeySet().Contains(AddressRequestedKey))
                {
                    mAddressRequested = savedInstanceState.GetBoolean(AddressRequestedKey);
                }
                if (savedInstanceState.KeySet().Contains(LocationAddressKey))
                {
                    mAddressOutput = savedInstanceState.GetString(LocationAddressKey);
                    DisplayAddressOutput();
                }
            }
        }

        public void FetchAddressButtonHandler(object sender, EventArgs eventArgs)
        {
            if (mLastLocation != null)
            {
                StartIntentService();
                return;
            }
            mAddressRequested = true;
            UpdateUiWidgets();
        }

        public void StartIntentService()
        {
            Intent intent = new Intent(this, typeof(FetchAddressIntentService));
            intent.PutExtra(Constants.Receiver, mResultReceiver);
            intent.PutExtra(Constants.LocationDataExtra, mLastLocation);
            StartService(intent);
        }

        public void GetAddress()
        {
            var lastLocation = mFusedLocationClient.LastLocation;
            lastLocation.AddOnSuccessListener(this, new GetAddressOnSuccessListener { Activity = this });
            lastLocation.AddOnFailureListener(this, new GetAddressOnFailureListener { Activity = this });
        }

        public void DisplayAddressOutput()
        {
            mLocationAddressTextView.Text = mAddressOutput;
        }

        public void UpdateUiWidgets()
        {
            if (mAddressRequested)
            {
                mProgressBar.Visibility = ViewStates.Visible;
                mFetchAddressButton.Enabled = false;
            }
            else
            {
                mProgressBar.Visibility = ViewStates.Gone;
                mFetchAddressButton.Enabled = true;
            }
        }

        public void ShowToast(String text)
        {
            Toast.MakeText(this, text, ToastLength.Short).Show();
        }

	    protected override void OnSaveInstanceState(Bundle savedInstanceState)
        {
            savedInstanceState.PutBoolean(AddressRequestedKey, mAddressRequested);
            savedInstanceState.PutString(LocationAddressKey, mAddressOutput);
            base.OnSaveInstanceState(savedInstanceState);
        }

	    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
	    {
			Log.Info(TAG, "onRequestPermissionResult");
		    if (requestCode == RequestPermissionsRequestCode)
		    {
			    if (grantResults.Length <= 0)
			    {
				    Log.Info(TAG, "User interaction was cancelled.");
			    }
			    else if (grantResults[0] == (int)Permission.Granted)
			    {
				    GetAddress();
			    }
			    else
			    {
				    ShowSnackbar(Resource.String.permission_denied_explanation, Resource.String.settings, new AddressResultReceiver.OnRequestPermissionsResultClickListener { Activity = this });
			    }
		    }
		}

	    public void ShowSnackbar(string text)
        {
            View container = FindViewById(Android.Resource.Id.Content);
            if (container != null)
            {
                Snackbar.Make(container, text, Snackbar.LengthLong).Show();
            }
        }

        public void ShowSnackbar(int mainTextStringId, int actionStringId, View.IOnClickListener listener)
        {
            Snackbar.Make(FindViewById(Android.Resource.Id.Content), GetString(mainTextStringId), Snackbar.LengthIndefinite)
                .SetAction(GetString(actionStringId), listener).Show();
        }

        public bool CheckPermissions()
        {
            var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
            return permissionState == (int)Permission.Granted;
        }

        public void RequestPermissions()
        {
            bool shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation);

            if (shouldProvideRationale)
            {
                Log.Info(TAG, "Displaying permission rationale to provide additional context.");
                ShowSnackbar(Resource.String.permission_rationale, Android.Resource.String.Ok, new AddressResultReceiver.RequestPermissionsOnClickListener { Activity = this });
            }
            else
            {
                Log.Info(TAG, "Requesting permission");
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, RequestPermissionsRequestCode);
            }
        }
    }

    public class GetAddressOnSuccessListener : Object, IOnSuccessListener
    {

        public LocationAddress.MainActivity Activity { get; set; }

        public void OnSuccess(Object location)
        {
            if (location == null)
            {
                Log.Warn(Activity.TAG, "onSuccess:null");
                return;
            }

            Activity.mLastLocation = (Location)location;

            if (!Geocoder.IsPresent)
            {
                Activity.ShowSnackbar(Activity.GetString(Resource.String.no_geocoder_available));
                return;
            }

            if (Activity.mAddressRequested)
            {
                Activity.StartIntentService();
            }
        }
    }

    public class GetAddressOnFailureListener : Java.Lang.Object, IOnFailureListener
    {

        public LocationAddress.MainActivity Activity { get; set; }


        public void OnFailure(Exception e)
        {
            Log.Warn(Activity.TAG, "getLastLocation:onFailure", e);
        }
    }

    public class AddressResultReceiver : ResultReceiver
    {
        public MainActivity Activity { get; set; }
        public AddressResultReceiver(Handler handler) : base(handler) { }

        protected override void OnReceiveResult(int resultCode, Bundle resultData)
        {
            Activity.mAddressOutput = resultData.GetString(Constants.ResultDataKey);
            Activity.DisplayAddressOutput();

            if (resultCode == Constants.SuccessResult)
            {
                Activity.ShowToast(Activity.GetString(Resource.String.address_found));
            }

            Activity.mAddressRequested = false;
            Activity.UpdateUiWidgets();
        }

        public class RequestPermissionsOnClickListener : Object, View.IOnClickListener
        {
            public MainActivity Activity { get; set; }

            public void OnClick(View v)
            {
                ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.AccessFineLocation }, Activity.RequestPermissionsRequestCode);
            }
        }

        public class OnRequestPermissionsResultClickListener : Object, View.IOnClickListener
        {
            public MainActivity Activity { get; set; }
            public void OnClick(View v)
            {
                Intent intent = new Intent();
                intent.SetAction(Settings.ActionApplicationDetailsSettings);
                Uri uri = Uri.FromParts("package", BuildConfig.ApplicationId, null);
                intent.SetData(uri);
                intent.SetFlags(ActivityFlags.NewTask);
                Activity.StartActivity(intent);
            }
        }
    }
}

