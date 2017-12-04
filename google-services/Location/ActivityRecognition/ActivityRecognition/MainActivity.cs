using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content.PM;
using Android.Preferences;
using Android.Util;

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

        protected const string TAG = "MainActivity";
        public Context mContext;
        private ActivityRecognitionClient mActivityRecognitionClient;
        protected GoogleApiClient mGoogleApiClient;
        Button mRequestActivityUpdatesButton;
        Button mRemoveActivityUpdatesButton;
        DetectedActivitiesAdapter mAdapter;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            mContext = this;

            mRequestActivityUpdatesButton = FindViewById<Button>(Resource.Id.request_activity_updates_button);
            mRemoveActivityUpdatesButton = FindViewById<Button>(Resource.Id.remove_activity_updates_button);
            ListView detectedActivitiesListView = FindViewById<ListView>(Resource.Id.detected_activities_listview);

            mRequestActivityUpdatesButton.Click += RequestActivityUpdatesButtonHandler;
            mRemoveActivityUpdatesButton.Click += RemoveActivityUpdatesButtonHandler;

            SetButtonsEnabledState();

            List<DetectedActivity> detectedActivities = Utils.DetectedActivitiesFromJson(
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

        public async void RequestActivityUpdatesButtonHandler(object sender, EventArgs e)
        {
            try
            {
                await mActivityRecognitionClient.RequestActivityUpdatesAsync(Constants.DetectionIntervalInMilliseconds, ActivityDetectionPendingIntent);

                Toast.MakeText(mContext, mContext.GetString(Resource.String.activity_updates_enabled), ToastLength.Short).Show();
                SetUpdatesRequestedState(true);
                UpdateDetectedActivitiesList();

            }
            catch
            {
                Log.Warn(TAG, mContext.GetString(Resource.String.activity_updates_not_enabled));
                Toast.MakeText(mContext, mContext.GetString(Resource.String.activity_updates_not_enabled), ToastLength.Short).Show();
                SetUpdatesRequestedState(false);
            }
        }

        public async void RemoveActivityUpdatesButtonHandler(object sender, EventArgs e)
        {
            try
            {
                await mActivityRecognitionClient.RemoveActivityUpdatesAsync(ActivityDetectionPendingIntent);

                Toast.MakeText(mContext, mContext.GetString(Resource.String.activity_updates_removed), ToastLength.Short).Show();
                SetUpdatesRequestedState(false);
                mAdapter.UpdateActivities(new List<DetectedActivity>());

            }
            catch
            {
                Log.Warn(TAG, "Failed to enable activity recognition.");
                Toast.MakeText(mContext, mContext.GetString(Resource.String.activity_updates_not_removed), ToastLength.Short).Show();
                SetUpdatesRequestedState(true);
            }
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

        void SetUpdatesRequestedState(bool value)
        {
            PreferenceManager.GetDefaultSharedPreferences(this)
                .Edit()
                .PutBoolean(Constants.KeyActivityUpdatesRequested, value)
                .Apply();
            SetButtonsEnabledState();
        }

        protected void UpdateDetectedActivitiesList()
        {
            List<DetectedActivity> detectedActivities = Utils.DetectedActivitiesFromJson(
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