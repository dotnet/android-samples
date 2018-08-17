using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System.Collections.Generic;
using Android;
using Android.Arch.Lifecycle;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Preferences;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using static Android.Support.V4.App.ActivityCompat;

namespace Geofencing
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnCompleteListener
    {
        protected string TAG = typeof(MainActivity).Name;
        public int REQUEST_PERMISSIONS_REQUEST_CODE = 34;

        private enum PendingGeofenceTask
        {
            ADD, REMOVE, NONE
        }

        private GeofencingClient mGeofencingClient;
        private IList<IGeofence> mGeofenceList;
        private PendingIntent mGeofencePendingIntent;
        private Button mAddGeofencesButton;
        private Button mRemoveGeofencesButton;
        private PendingGeofenceTask mPendingGeofenceTask = PendingGeofenceTask.NONE;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mAddGeofencesButton = FindViewById<Button>(Resource.Id.add_geofences_button);
            mRemoveGeofencesButton = FindViewById<Button>(Resource.Id.remove_geofences_button);
            mGeofenceList = new List<IGeofence>();
            mGeofencePendingIntent = null;
            SetButtonsEnabledState();
            PopulateGeofenceList();
            mGeofencingClient = LocationServices.GetGeofencingClient(this);

            mAddGeofencesButton.Click += AddGeofencesButtonHandler;
            mRemoveGeofencesButton.Click += RemoveGeofencesButtonHandler;
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
                PerformPendingGeofenceTask();
            }
        }

        private GeofencingRequest GetGeofencingRequest()
        {
            GeofencingRequest.Builder builder = new GeofencingRequest.Builder();
            builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
            builder.AddGeofences(mGeofenceList);
            return builder.Build();
        }

        public void AddGeofencesButtonHandler(object sender, EventArgs ea)
        {
            if (!CheckPermissions())
            {
                mPendingGeofenceTask = PendingGeofenceTask.ADD;
                RequestPermissions();
                return;
            }
            AddGeofences();
        }

        private void AddGeofences()
        {
            if (!CheckPermissions())
            {
                ShowSnackbar(GetString(Resource.String.insufficient_permissions));
                return;
            }

            mGeofencingClient.AddGeofences(GetGeofencingRequest(), GetGeofencePendingIntent()).AddOnCompleteListener(this);
        }

        public void RemoveGeofencesButtonHandler(object sender, EventArgs ea)
        {
            if (!CheckPermissions())
            {
                mPendingGeofenceTask = PendingGeofenceTask.REMOVE;
                RequestPermissions();
                return;
            }
            RemoveGeofences();
        }

        private void RemoveGeofences()
        {
            if (!CheckPermissions())
            {
                ShowSnackbar(GetString(Resource.String.insufficient_permissions));
                return;
            }

            mGeofencingClient.RemoveGeofences(GetGeofencePendingIntent()).AddOnCompleteListener(this);
        }


        public void OnComplete(Task task)
        {
            mPendingGeofenceTask = PendingGeofenceTask.NONE;
            if (task.IsSuccessful)
            {
                UpdateGeofencesAdded(!GetGeofencesAdded());
                SetButtonsEnabledState();

                var messageId = GetGeofencesAdded() ? Resource.String.geofences_added : Resource.String.geofences_removed;
                Toast.MakeText(this, GetString(messageId), ToastLength.Short).Show();
            }
            else
            {
                var errorMessage = GeofenceErrorMessages.GetErrorString(this, task.Exception);
                Log.Warn(TAG, errorMessage);
            }
        }

        private PendingIntent GetGeofencePendingIntent()
        {
            if (mGeofencePendingIntent != null)
            {
                return mGeofencePendingIntent;
            }
            var intent = new Intent(this, typeof(GeofenceTransitionsIntentService));
            return PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private void PopulateGeofenceList()
        {
            foreach (var entry in Constants.BAY_AREA_LANDMARKS)
            {

                mGeofenceList.Add(new GeofenceBuilder()
                    .SetRequestId(entry.Key)
                    .SetCircularRegion(
                        entry.Value.Latitude,
                        entry.Value.Longitude,
                        Constants.GEOFENCE_RADIUS_IN_METERS
                    )
                    .SetExpirationDuration(Constants.GEOFENCE_EXPIRATION_IN_MILLISECONDS)
                    .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                    .Build());
            }
        }

        private void SetButtonsEnabledState()
        {
            if (GetGeofencesAdded())
            {
                mAddGeofencesButton.Enabled = false;
                mRemoveGeofencesButton.Enabled = true;
            }
            else
            {
                mAddGeofencesButton.Enabled = true;
                mRemoveGeofencesButton.Enabled = false;
            }
        }

        private void ShowSnackbar(string text)
        {
            var container = FindViewById<View>(Android.Resource.Id.Content);
            if (container != null)
            {
                Snackbar.Make(container, text, Snackbar.LengthLong).Show();
            }
        }

        private void ShowSnackbar(int mainTextStringId, int actionStringId, View.IOnClickListener listener)
        {
            Snackbar.Make(FindViewById(Android.Resource.Id.Content),
                    GetString(mainTextStringId),
                    Snackbar.LengthIndefinite)
                .SetAction(GetString(actionStringId), listener).Show();
        }
        private bool GetGeofencesAdded()
        {
            return PreferenceManager.GetDefaultSharedPreferences(this).GetBoolean(Constants.GEOFENCES_ADDED_KEY, false);
        }

        private void UpdateGeofencesAdded(bool added)
        {
            PreferenceManager.GetDefaultSharedPreferences(this)
                .Edit()
                .PutBoolean(Constants.GEOFENCES_ADDED_KEY, added)
                .Apply();
        }

        private void PerformPendingGeofenceTask()
        {
            if (mPendingGeofenceTask == PendingGeofenceTask.ADD)
            {
                AddGeofences();
            }
            else if (mPendingGeofenceTask == PendingGeofenceTask.REMOVE)
            {
                RemoveGeofences();
            }
        }

        private bool CheckPermissions()
        {
            var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
            return permissionState == (int)Permission.Granted;
        }
        private void RequestPermissions()
        {
            var shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation);

            if (shouldProvideRationale)
            {
                Log.Info(TAG, "Displaying permission rationale to provide additional context.");
                var listener = (View.IOnClickListener)new RequestPermissionsClickListener { Activity = this };
                ShowSnackbar(Resource.String.permission_rationale, Android.Resource.String.Ok, listener);
            }
            else
            {
                Log.Info(TAG, "Requesting permission");
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, REQUEST_PERMISSIONS_REQUEST_CODE);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Log.Info(TAG, "onRequestPermissionResult");
            if (requestCode == REQUEST_PERMISSIONS_REQUEST_CODE)
            {
                if (grantResults.Length <= 0)
                {
                    Log.Info(TAG, "User interaction was cancelled.");
                }
                else if (grantResults[0] == (int)Permission.Granted)
                {
                    Log.Info(TAG, "Permission granted.");
                    PerformPendingGeofenceTask();
                }
                else
                {
                    var listener = (View.IOnClickListener)new OnRequestPermissionsResultClickListener { Activity = this };
                    ShowSnackbar(Resource.String.permission_denied_explanation, Resource.String.settings, listener);
                    mPendingGeofenceTask = PendingGeofenceTask.NONE;
                }
            }
        }
    }

    public class RequestPermissionsClickListener : Java.Lang.Object, View.IOnClickListener
    {
        public MainActivity Activity { get; set; }

        public void OnClick(View v)
        {
            RequestPermissions(Activity, new[] { Manifest.Permission.AccessFineLocation }, Activity.REQUEST_PERMISSIONS_REQUEST_CODE);
        }
    }

    public class OnRequestPermissionsResultClickListener : Java.Lang.Object, View.IOnClickListener
    {

        public MainActivity Activity { get; set; }
        public void OnClick(View v)
        {
            Intent intent = new Intent();
            intent.SetAction(Settings.ActionApplicationDetailsSettings);
            var uri = Android.Net.Uri.FromParts("package", Activity.PackageName, null);
            intent.SetData(uri);
            intent.SetFlags(ActivityFlags.NewTask);
            Activity.StartActivity(intent);
        }
    }
}


