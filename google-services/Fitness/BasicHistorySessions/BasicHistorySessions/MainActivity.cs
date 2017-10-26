using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android;
using Android.Gms.Fitness.Request;
using CommonSampleLibrary;
using Android.Gms.Fitness.Data;
using Java.Util.Concurrent;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Result;
using Android.Gms.Common;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Support.V7.App;
using System.Collections.Generic;

namespace BasicHistorySessions
{
	[Activity (MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		public const string TAG = "BasicSessions";
		public const string SAMPLE_SESSION_NAME = "Afternoon run";
		const int REQUEST_OAUTH = 1;
		const string DATE_FORMAT = "yyyy.MM.dd HH:mm:ss";

		const string AUTH_PENDING = "auth_state_pending";
		bool authInProgress;

		GoogleApiClient mClient;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			// This method sets up our custom logger, which will print all log messages to the device
			// screen, as well as to adb logcat.
			InitializeLogging();

			if (savedInstanceState != null) {
				authInProgress = savedInstanceState.GetBoolean(AUTH_PENDING);
			}

			BuildFitnessClient();
		}

		void BuildFitnessClient ()
		{
			var clientConnectionCallback = new ClientConnectionCallback ();
			clientConnectionCallback.OnConnectedImpl = async () => {
				SessionInsertRequest insertRequest = InsertFitnessSession();

				// [START insert_session]
				// Then, invoke the Sessions API to insert the session and await the result,
				// which is possible here because of the AsyncTask. Always include a timeout when
				// calling await() to avoid hanging that can occur from the service being shutdown
				// because of low memory or other conditions.
				Log.Info(TAG, "Inserting the session in the History API");
                var insertStatus = await FitnessClass.SessionsApi.InsertSessionAsync (mClient, insertRequest);

				// Before querying the session, check to see if the insertion succeeded.
				if (!insertStatus.IsSuccess) {
					Log.Info(TAG, "There was a problem inserting the session: " +
						insertStatus.StatusMessage);
					return;
				}

				// At this point, the session has been inserted and can be read.
				Log.Info (TAG, "Session insert was successful!");
				// [END insert_session]

				// Begin by creating the query.
				var readRequest = ReadFitnessSession ();

				// [START read_session]
				// Invoke the Sessions API to fetch the session with the query and wait for the result
				// of the read request.
				var sessionReadResult = await FitnessClass.SessionsApi.ReadSessionAsync (mClient, readRequest);

				// Get a list of the sessions that match the criteria to check the result.
				Log.Info (TAG, "Session read was successful. Number of returned sessions is: "
					+ sessionReadResult.Sessions.Count);
				foreach (Session session in sessionReadResult.Sessions) {
					// Process the session
					DumpSession (session);

					// Process the data sets for this session
					IList<DataSet> dataSets = sessionReadResult.GetDataSet(session);
					foreach (DataSet dataSet in dataSets) {
						DumpDataSet(dataSet);
					}
				}
			};

			// Create the Google API Client
			mClient = new GoogleApiClient.Builder (this)
				.AddApi (FitnessClass.HISTORY_API)
				.AddApi (FitnessClass.SESSIONS_API)
				.AddScope (new Scope (Scopes.FitnessActivityReadWrite))
				.AddConnectionCallbacks (clientConnectionCallback)
				.AddOnConnectionFailedListener (result => {
					Log.Info (TAG, "Connection failed. Cause: " + result);
					if (!result.HasResolution) {
						// Show the localized error dialog
						GooglePlayServicesUtil.GetErrorDialog (result.ErrorCode, this, 0).Show ();
						return;
					}
					// The failure has a resolution. Resolve it.
					// Called typically when the app is not yet authorized, and an
					// authorization dialog is displayed to the user.
					if (!authInProgress) {
						try {
							Log.Info (TAG, "Attempting to resolve failed connection");
							authInProgress = true;
							result.StartResolutionForResult (this,
								REQUEST_OAUTH);
						} catch (IntentSender.SendIntentException e) {
							Log.Error (TAG,
								"Exception while starting resolution activity", e);
						}
					}
				}).Build ();
		}

		class ClientConnectionCallback : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks
		{
			public Action OnConnectedImpl { get; set; }

			public void OnConnected (Bundle connectionHint)
			{
				Log.Info (TAG, "Connected!!!");

				Task.Run (OnConnectedImpl);
			}

			public void OnConnectionSuspended (int cause)
			{
				if (cause == GoogleApiClient.ConnectionCallbacksConsts.CauseNetworkLost) {
					Log.Info (TAG, "Connection lost.  Cause: Network Lost.");
				} else if (cause == GoogleApiClient.ConnectionCallbacksConsts.CauseServiceDisconnected) {
					Log.Info (TAG, "Connection lost.  Reason: Service Disconnected");
				}
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			Log.Info (TAG, "Connecting...");
			mClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			if (mClient.IsConnected) {
				mClient.Disconnect ();
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == REQUEST_OAUTH) {
				authInProgress = false;
				if (resultCode == Result.Ok) {
					// Make sure the app is not already connected or attempting to connect
					if (!mClient.IsConnecting && !mClient.IsConnected) {
						mClient.Connect ();
					}
				}
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutBoolean (AUTH_PENDING, authInProgress);
		}

		SessionInsertRequest InsertFitnessSession()
		{
			Log.Info(TAG, "Creating a new session for an afternoon run");

			var epoch = new DateTime (1970, 1, 1);
			var now = DateTime.UtcNow;
			long endTime = (long)(now - epoch).TotalMilliseconds;
			long endWalkTime = (long)(now.Subtract (TimeSpan.FromMinutes (10)) - epoch).TotalMilliseconds;
			long startWalkTime = (long)(now.Subtract (TimeSpan.FromMinutes (20)) - epoch).TotalMilliseconds;
			long startTime = (long)(now.Subtract (TimeSpan.FromMinutes (30)) - epoch).TotalMilliseconds;

			var speedDataSource = new DataSource.Builder()
				.SetAppPackageName (PackageName)
				.SetDataType (DataType.TypeSpeed)
				.SetName (SAMPLE_SESSION_NAME + "- speed")
				.SetType (DataSource.TypeRaw)
				.Build ();

			const float runSpeedMps = 10;
			const float walkSpeedMps = 3;
			// Create a data set of the run speeds to include in the session.
			DataSet speedDataSet = DataSet.Create (speedDataSource);

			DataPoint firstRunSpeed = speedDataSet.CreateDataPoint ()
				.SetTimeInterval (startTime, startWalkTime, TimeUnit.Milliseconds);
			firstRunSpeed.GetValue (Field.FieldSpeed).SetFloat (runSpeedMps);
			speedDataSet.Add (firstRunSpeed);

			DataPoint walkSpeed = speedDataSet.CreateDataPoint ()
				.SetTimeInterval (startWalkTime, endWalkTime, TimeUnit.Milliseconds);
			walkSpeed.GetValue (Field.FieldSpeed).SetFloat (walkSpeedMps);
			speedDataSet.Add (walkSpeed);

			DataPoint secondRunSpeed = speedDataSet.CreateDataPoint ()
				.SetTimeInterval (endWalkTime, endTime, TimeUnit.Milliseconds);
			secondRunSpeed.GetValue (Field.FieldSpeed).SetFloat(runSpeedMps);
			speedDataSet.Add (secondRunSpeed);

			// [START build_insert_session_request_with_activity_segments]
			// Create a second DataSet of ActivitySegments to indicate the runner took a 10-minute walk
			// in the middle of the run.
			var activitySegmentDataSource = new DataSource.Builder()
				.SetAppPackageName (PackageName)
				.SetDataType (DataType.TypeActivitySegment)
				.SetName (SAMPLE_SESSION_NAME + "-activity segments")
				.SetType (DataSource.TypeRaw)
				.Build ();
			DataSet activitySegments = DataSet.Create (activitySegmentDataSource);

			DataPoint firstRunningDp = activitySegments.CreateDataPoint ()
				.SetTimeInterval (startTime, startWalkTime, TimeUnit.Milliseconds);
			firstRunningDp.GetValue (Field.FieldActivity).SetActivity (FitnessActivities.Running);
			activitySegments.Add (firstRunningDp);

			DataPoint walkingDp = activitySegments.CreateDataPoint ()
				.SetTimeInterval (startWalkTime, endWalkTime, TimeUnit.Milliseconds);
			walkingDp.GetValue (Field.FieldActivity).SetActivity (FitnessActivities.Walking);
			activitySegments.Add (walkingDp);

			DataPoint secondRunningDp = activitySegments.CreateDataPoint ()
				.SetTimeInterval (endWalkTime, endTime, TimeUnit.Milliseconds);
			secondRunningDp.GetValue (Field.FieldActivity).SetActivity (FitnessActivities.Running);
			activitySegments.Add (secondRunningDp);

			var session = new Session.Builder ()
				.SetName (SAMPLE_SESSION_NAME)
				.SetDescription ("Long run around Shoreline Park")
				.SetIdentifier ("UniqueIdentifierHere")
				.SetActivity (FitnessActivities.Running)
				.SetStartTime (startTime, TimeUnit.Milliseconds)
				.SetEndTime (endTime, TimeUnit.Milliseconds)
				.Build ();

			var insertRequest = new SessionInsertRequest.Builder()
				.SetSession (session)
				.AddDataSet (speedDataSet)
				.AddDataSet (activitySegments)
				.Build ();

			return insertRequest;
		}

		SessionReadRequest ReadFitnessSession ()
		{
			Log.Info(TAG, "Reading History API results for session: " + SAMPLE_SESSION_NAME);

			var epoch = new DateTime (1970, 1, 1);
			var now = DateTime.UtcNow;
			long endTime = (long)(now - epoch).TotalMilliseconds;
			long startTime = (long)(now.Subtract (TimeSpan.FromDays (7)) - epoch).TotalMilliseconds;

			// Build a session read request
			SessionReadRequest readRequest = new SessionReadRequest.Builder( )
				.SetTimeInterval (startTime, endTime, TimeUnit.Milliseconds)
				.Read (DataType.TypeSpeed)
				.SetSessionName (SAMPLE_SESSION_NAME)
				.Build ();
			// [END build_read_session_request]

			return readRequest;
		}

		void DumpDataSet (DataSet dataSet)
		{
			Log.Info (TAG, "Data returned for Data type: " + dataSet.DataType.Name);
			foreach (DataPoint dp in dataSet.DataPoints) {
				Log.Info (TAG, "Data point:");
				Log.Info (TAG, "\tType: " + dp.DataType.Name);
				Log.Info (TAG, "\tStart: " + new DateTime (1970, 1, 1).AddMilliseconds (
					dp.GetStartTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT));
				Log.Info (TAG, "\tEnd: " + new DateTime (1970, 1, 1).AddMilliseconds (
					dp.GetEndTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT));
				foreach (Field field in dp.DataType.Fields) {
					Log.Info (TAG, "\tField: " + field.Name + " Value: " + dp.GetValue (field));
				}
			}
		}

		void DumpSession (Session session)
		{
			Log.Info (TAG, "Data returned for Session: " + session.Name
				+ "\n\tDescription: " + session.Description
				+ "\n\tStart: " + new DateTime (1970, 1, 1).AddMilliseconds (
					session.GetStartTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT)
				+ "\n\tEnd: " + new DateTime (1970, 1, 1).AddMilliseconds (
					session.GetEndTime (TimeUnit.Milliseconds)).ToString (DATE_FORMAT));
		}

        async Task DeleteSession ()
		{
			Log.Info (TAG, "Deleting today's session data for speed");

			var epoch = new DateTime (1970, 1, 1);
			var now = DateTime.UtcNow;
			var endTime = (now - epoch).TotalMilliseconds;
			var startTime = (now.Subtract (TimeSpan.FromDays (1)) - epoch).TotalMilliseconds;

			var request = new DataDeleteRequest.Builder ()
				.SetTimeInterval ((long)startTime, (long)endTime, TimeUnit.Milliseconds)
				.AddDataType (DataType.TypeSpeed)
				.DeleteAllSessions()
				.Build ();

			// Invoke the History API with the Google API client object and the delete request and
			// specify a callback that will check the result.
            var status = await FitnessClass.HistoryApi.DeleteDataAsync (mClient, request);
			if (status.IsSuccess) {
				Log.Info(TAG, "Successfully deleted today's sessions");
			} else {
				// The deletion will fail if the requesting app tries to delete data
				// that it did not insert.
				Log.Info(TAG, "Failed to delete today's sessions");
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}
			
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_delete_session) {
				DeleteSession();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void InitializeLogging ()
		{
			// Wraps Android's native log framework.
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			Log.LogNode = logWrapper;
			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;
			// On screen logging via a customized TextView.
			var logView = FindViewById<LogView> (Resource.Id.sample_logview);
			logView.SetTextAppearance (this, Resource.Style.Log);
			logView.SetBackgroundColor (Color.White);
			msgFilter.NextNode = logView;
			Log.Info (TAG, "Ready");
		}
	}
}


