using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using CommonSampleLibrary;
using Java.Util.Concurrent;
using Android.Graphics;

namespace BasicHistoryApi
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity
	{
		public const string TAG = "BasicHistoryApi";
		const int REQUEST_OAUTH = 1;
		const string AUTH_PENDING = "auth_state_pending";
		const string DATE_FORMAT = "yyyy.MM.dd HH:mm:ss";
		bool authInProgress;
		GoogleApiClient mClient;

		protected override async void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			InitializeLogging ();

			if (savedInstanceState != null) {
				authInProgress = savedInstanceState.GetBoolean (AUTH_PENDING);
			}

			await BuildFitnessClient ();
		}

		async Task BuildFitnessClient ()
		{
			var clientConnectionCallback = new ClientConnectionCallback ();
			clientConnectionCallback.OnConnectedImpl = async () => {
				DataSet dataSet = InsertFitnessData ();

				Log.Info (TAG, "Inserting the dataset in the History API");
				var insertStatus = (Statuses)FitnessClass.HistoryApi.InsertData (mClient, dataSet).Await (1, TimeUnit.Minutes);

				if (!insertStatus.IsSuccess) {
					Log.Info (TAG, "There was a problem inserting the dataset.");
					return;
				}

				Log.Info (TAG, "Data insert was successful!");

				DataReadRequest readRequest = QueryFitnessData ();

                var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync (mClient, readRequest);

				PrintData (dataReadResult);
			};
			// Create the Google API Client
			mClient = new GoogleApiClient.Builder (this)
				.AddApi (FitnessClass.HISTORY_API)
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
							result.StartResolutionForResult (this, REQUEST_OAUTH);
						} catch (IntentSender.SendIntentException e) {
							Log.Error (TAG, "Exception while starting resolution activity", e);
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

		long GetMsSinceEpochAsLong (DateTime dateTime)
		{
			return (long)dateTime.ToUniversalTime ()
				.Subtract (new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
				.TotalMilliseconds;
		}

		DataSet InsertFitnessData ()
		{
			Log.Info (TAG, "Creating a new data insert request");

			// Set a start and end time for our data, using a start time of 1 hour before this moment.
			DateTime endTime = DateTime.Now;
			DateTime startTime = endTime.Subtract (TimeSpan.FromHours (1));
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);

			// Create a data source
			var dataSource = new DataSource.Builder ()
				.SetAppPackageName (this)
				.SetDataType (DataType.TypeStepCountDelta)
				.SetName (TAG + " - step count")
				.SetType (DataSource.TypeRaw)
				.Build ();

			// Create a data set
			const int stepCountDelta = 1000;
			var dataSet = DataSet.Create (dataSource);
			DataPoint dataPoint = dataSet.CreateDataPoint ()
				.SetTimeInterval (startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds);
			dataPoint.GetValue (Field.FieldSteps).SetInt (stepCountDelta);
			dataSet.Add (dataPoint);

			return dataSet;
		}

		DataReadRequest QueryFitnessData ()
		{
			// Setting a start and end date using a range of 1 week before this moment.
			DateTime endTime = DateTime.Now;
			DateTime startTime = endTime.Subtract (TimeSpan.FromDays (7));
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);

			Log.Info (TAG, "Range Start: " + startTime.ToString (DATE_FORMAT));
			Log.Info (TAG, "Range End: " + endTime.ToString (DATE_FORMAT));

			var readRequest = new DataReadRequest.Builder ()
				.Aggregate (DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
				.BucketByTime (1, TimeUnit.Days)
				.SetTimeRange (startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.Build ();

			return readRequest;
		}

		void PrintData (DataReadResult dataReadResult)
		{
			if (dataReadResult.Buckets.Count > 0) {
				Log.Info (TAG, "Number of returned buckets of DataSets is: "
				+ dataReadResult.Buckets.Count);
				
				foreach (Bucket bucket in dataReadResult.Buckets) {
					IList<DataSet> dataSets = bucket.DataSets;
					foreach (DataSet dataSet in dataSets) {
						DumpDataSet (dataSet);
					}
				}
			} else if (dataReadResult.DataSets.Count > 0) {
				Log.Info (TAG, "Number of returned DataSets is: "
				+ dataReadResult.DataSets.Count);
				
				foreach (DataSet dataSet in dataReadResult.DataSets) {
					DumpDataSet (dataSet);
				}
			}
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
					Log.Info (TAG, "\tField: " + field.Name +
					" Value: " + dp.GetValue (field));
				}
			}
		}

        async Task DeleteData ()
		{
			Log.Info (TAG, "Deleting today's step count data");

			// Set a start and end time for our data, using a start time of 1 day before this moment.
			DateTime endTime = DateTime.Now;
			DateTime startTime = endTime.Subtract (TimeSpan.FromDays (1));
			long endTimeElapsed = GetMsSinceEpochAsLong (endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong (startTime);

			//  Create a delete request object, providing a data type and a time interval
			var request = new DataDeleteRequest.Builder ()
				.SetTimeInterval (startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.AddDataType (DataType.TypeStepCountDelta)
				.Build ();

			// Invoke the History API with the Google API client object and delete request, and then
			// specify a callback that will check the result.
            var status = await FitnessClass.HistoryApi.DeleteDataAsync (mClient, request);
			if (status.IsSuccess) {
				Log.Info (TAG, "Successfully deleted today's step count data");
			} else {
				// The deletion will fail if the requesting app tries to delete data
				// that it did not insert.
				Log.Info (TAG, "Failed to delete today's step count data");
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
			if (id == Resource.Id.action_delete_data) {
				DeleteData ();
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


