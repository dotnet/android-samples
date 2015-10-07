using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Content.PM;
using System.Threading.Tasks;

namespace ActivityRecognition
{
	[Activity (
		Label = "ActivityRecognition", 
		MainLauncher = true, 
		Icon = "@drawable/icon", 
		ScreenOrientation = ScreenOrientation.SensorPortrait, 
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
	)]
	public class MainActivity : ActionBarActivity,
		GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener
	{
		protected const string TAG = "activity-recognition";
		protected ActivityDetectionBroadcastReceiver mBroadcastReceiver;
		protected GoogleApiClient mGoogleApiClient;
		PendingIntent mActivityDetectionPendingIntent;
		Button mRequestActivityUpdatesButton;
		Button mRemoveActivityUpdatesButton;
		ListView mDetectedActivitiesListView;
		DetectedActivitiesAdapter mAdapter;
		List<DetectedActivity> mDetectedActivities;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			mRequestActivityUpdatesButton = FindViewById<Button> (Resource.Id.request_activity_updates_button);
			mRemoveActivityUpdatesButton = FindViewById<Button> (Resource.Id.remove_activity_updates_button);
			mDetectedActivitiesListView = FindViewById<ListView> (Resource.Id.detected_activities_listview);

			mRequestActivityUpdatesButton.Click += RequestActivityUpdatesButtonHandler;
			mRemoveActivityUpdatesButton.Click += RemoveActivityUpdatesButtonHandler;

			mBroadcastReceiver = new ActivityDetectionBroadcastReceiver ();
			mBroadcastReceiver.OnReceiveImpl = (context, intent) => {
				var updatedActivities = intent.GetParcelableArrayExtra (Constants.ActivityExtra).Cast<DetectedActivity>().ToList ();
				UpdateDetectedActivitiesList (updatedActivities);
			};

			SetButtonsEnabledState ();

			if (savedInstanceState != null && savedInstanceState.ContainsKey (Constants.DetectedActivities)) {
				mDetectedActivities = ((SerializableDetectedActivities)savedInstanceState.GetSerializable (
					Constants.DetectedActivities)).DetectedActivities;
			} else {
				mDetectedActivities = new List<DetectedActivity> ();

				for (int i = 0; i < Constants.MonitoredActivities.Length; i++) {
					mDetectedActivities.Add (new DetectedActivity (Constants.MonitoredActivities [i], 0));
				}
			}

			mAdapter = new DetectedActivitiesAdapter (this, mDetectedActivities);
			mDetectedActivitiesListView.Adapter = mAdapter;

			buildGoogleApiClient ();
		}

		protected void buildGoogleApiClient ()
		{
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.AddApi (Android.Gms.Location.ActivityRecognition.API)
				.Build ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			mGoogleApiClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			mGoogleApiClient.Disconnect ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			LocalBroadcastManager.GetInstance (this).RegisterReceiver (mBroadcastReceiver,
				new IntentFilter (Constants.BroadcastAction));
		}

		protected override void OnPause ()
		{
			LocalBroadcastManager.GetInstance (this).UnregisterReceiver (mBroadcastReceiver);
			base.OnPause ();
		}

		public void OnConnected (Bundle connectionHint)
		{
			Log.Info (TAG, "Connected to GoogleApiClient");
		}

		public void OnConnectionSuspended (int cause)
		{
			Log.Info (TAG, "Connection suspended");
			mGoogleApiClient.Connect ();
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Info (TAG, "Connection failed: ConnectionResult.ErrorCode = " + result.ErrorCode);
		}

        public async void RequestActivityUpdatesButtonHandler (object sender, EventArgs e)
		{
			if (!mGoogleApiClient.IsConnected) {
				Toast.MakeText (this, GetString (Resource.String.not_connected),
					ToastLength.Short).Show ();
				return;
			}
            var status = await Android.Gms.Location.ActivityRecognition.ActivityRecognitionApi.RequestActivityUpdatesAsync (
                    mGoogleApiClient,
                    Constants.DetectionIntervalInMilliseconds,
                    ActivityDetectionPendingIntent
                );
            HandleResult (status);
		}

        public async void RemoveActivityUpdatesButtonHandler (object sender, EventArgs e)
		{
			if (!mGoogleApiClient.IsConnected) {
				Toast.MakeText (this, GetString (Resource.String.not_connected), ToastLength.Short).Show ();
				return;
			}
            var status = await Android.Gms.Location.ActivityRecognition.ActivityRecognitionApi.RemoveActivityUpdatesAsync (
                    mGoogleApiClient,
                    ActivityDetectionPendingIntent
                );
            HandleResult (status);
		}

		public void HandleResult (Statuses status)
		{
			if (status.IsSuccess) {
				bool requestingUpdates = !UpdatesRequestedState;
				UpdatesRequestedState = requestingUpdates;

				SetButtonsEnabledState ();

				Toast.MakeText (
					this,
					GetString (requestingUpdates ? Resource.String.activity_updates_added : Resource.String.activity_updates_removed), 
					ToastLength.Short
				).Show ();
			} else {
				Log.Error (TAG, "Error adding or removing activity detection: " + status.StatusMessage);
			}
		}

		PendingIntent ActivityDetectionPendingIntent {
			get {
				if (mActivityDetectionPendingIntent != null) {
					return mActivityDetectionPendingIntent;
				}
				var intent = new Intent (this, typeof(DetectedActivitiesIntentService));

				return PendingIntent.GetService (this, 0, intent, PendingIntentFlags.UpdateCurrent);
			}
		}

		void SetButtonsEnabledState ()
		{
			if (UpdatesRequestedState) {
				mRequestActivityUpdatesButton.Enabled = false;
				mRemoveActivityUpdatesButton.Enabled = true;
			} else {
				mRequestActivityUpdatesButton.Enabled = true;
				mRemoveActivityUpdatesButton.Enabled = false;
			}
		}

		ISharedPreferences SharedPreferencesInstance {
			get {
				return GetSharedPreferences (Constants.SharedPreferencesName, FileCreationMode.Private);
			}
		}

		bool UpdatesRequestedState {
			get {
				return SharedPreferencesInstance.GetBoolean (Constants.ActivityUpdatesRequestedKey, false);
			}
			set {
				SharedPreferencesInstance.Edit ().PutBoolean (Constants.ActivityUpdatesRequestedKey, value).Commit ();
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutSerializable (Constants.DetectedActivities, new SerializableDetectedActivities (mDetectedActivities));
			base.OnSaveInstanceState (outState);
		}

		protected void UpdateDetectedActivitiesList (IList<DetectedActivity> detectedActivities)
		{
			mAdapter.UpdateActivities (detectedActivities);
		}

		public class ActivityDetectionBroadcastReceiver : BroadcastReceiver
		{
			public Action<Context, Intent> OnReceiveImpl { get; set;}

			public override void OnReceive (Context context, Intent intent)
			{
				OnReceiveImpl (context, intent);
			}
		}
	}
}


