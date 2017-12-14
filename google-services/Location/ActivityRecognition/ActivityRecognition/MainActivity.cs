using System;
using System.Collections.Generic;
using ActivityRecognition.Listeners;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content.PM;
using Android.Preferences;

namespace ActivityRecognition
{
    [Activity(
        Label = "ActivityRecognition",
        MainLauncher = true,
        Icon = "@drawable/icon",
        ScreenOrientation = ScreenOrientation.SensorPortrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
    )]
    public class MainActivity : AppCompatActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        public string TAG = "MainActivity";
        public Context mContext;
        private ActivityRecognitionClient mActivityRecognitionClient;
        protected GoogleApiClient mGoogleApiClient;
        Button mRequestActivityUpdatesButton;
        Button mRemoveActivityUpdatesButton;
        public DetectedActivitiesAdapter mAdapter;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mContext = this;

            mRequestActivityUpdatesButton = FindViewById<Button>(Resource.Id.request_activity_updates_button);
            mRemoveActivityUpdatesButton = FindViewById<Button>(Resource.Id.remove_activity_updates_button);
            var detectedActivitiesListView = FindViewById<ListView>(Resource.Id.detected_activities_listview);

            mRequestActivityUpdatesButton.Click += RequestActivityUpdatesButtonHandler;
            mRemoveActivityUpdatesButton.Click += RemoveActivityUpdatesButtonHandler;

            SetButtonsEnabledState();

            var detectedActivities = Utils.DetectedActivitiesFromJson(
                PreferenceManager.GetDefaultSharedPreferences(this).GetString(
                    Constants.KeyDetectedActivities, string.Empty));

            mAdapter = new DetectedActivitiesAdapter(this, detectedActivities);
            detectedActivitiesListView.Adapter = mAdapter;

            mActivityRecognitionClient = new ActivityRecognitionClient(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(this)
                .RegisterOnSharedPreferenceChangeListener(this);
            UpdateDetectedActivitiesList();
        }

        protected override void OnPause()
        {
            PreferenceManager.GetDefaultSharedPreferences(this)
                .UnregisterOnSharedPreferenceChangeListener(this);
            base.OnPause();
        }

        public void RequestActivityUpdatesButtonHandler(object sender, EventArgs e)
        {
            var task = mActivityRecognitionClient.RequestActivityUpdates(Constants.DetectionIntervalInMilliseconds, ActivityDetectionPendingIntent);
            task.AddOnSuccessListener(new RequestOnSuccessListener { Activity = this });
            task.AddOnFailureListener(new RequestOnFailureListener { Activity = this });
        }

        public void RemoveActivityUpdatesButtonHandler(object sender, EventArgs e)
        {
            var task = mActivityRecognitionClient.RemoveActivityUpdates(ActivityDetectionPendingIntent);
            task.AddOnSuccessListener(new RemoveOnSuccessListener { Activity = this });
            task.AddOnFailureListener(new RemoveOnFailureListener { Activity = this });
        }

        PendingIntent ActivityDetectionPendingIntent
        {
            get
            {
                var intent = new Intent(this, typeof(DetectedActivitiesIntentService));

                return PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            }
        }

        void SetButtonsEnabledState()
        {
            if (GetUpdatesRequestedState())
            {
                mRequestActivityUpdatesButton.Enabled = false;
                mRemoveActivityUpdatesButton.Enabled = true;
            }
            else
            {
                mRequestActivityUpdatesButton.Enabled = true;
                mRemoveActivityUpdatesButton.Enabled = false;
            }
        }

        bool GetUpdatesRequestedState()
        {
            return PreferenceManager.GetDefaultSharedPreferences(this)
                .GetBoolean(Constants.KeyActivityUpdatesRequested, false);
        }

        public void SetUpdatesRequestedState(bool value)
        {
            PreferenceManager.GetDefaultSharedPreferences(this)
                .Edit()
                .PutBoolean(Constants.KeyActivityUpdatesRequested, value)
                .Apply();
            SetButtonsEnabledState();
        }

        public void UpdateDetectedActivitiesList()
        {
            var detectedActivities = Utils.DetectedActivitiesFromJson(
                PreferenceManager.GetDefaultSharedPreferences(mContext)
                    .GetString(Constants.KeyDetectedActivities, string.Empty));

            mAdapter.UpdateActivities(detectedActivities);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key == Constants.KeyDetectedActivities)
            {
                UpdateDetectedActivitiesList();
            }
        }

        public class ActivityDetectionBroadcastReceiver : BroadcastReceiver
        {
            public Action<Context, Intent> OnReceiveImpl { get; set; }

            public override void OnReceive(Context context, Intent intent)
            {
                OnReceiveImpl(context, intent);
            }
        }
    }
}