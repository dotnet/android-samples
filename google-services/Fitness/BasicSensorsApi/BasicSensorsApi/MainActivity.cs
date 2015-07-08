using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using CommonSampleLibrary;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness.Request;
using Android.Gms.Common;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Result;
using Java.Util.Concurrent;
using Android.Graphics;

namespace BasicSensorsApi
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity
	{
		public const string TAG = "BasicSensorsApi";

		const int REQUEST_OAUTH = 1;

		const string AUTH_PENDING = "auth_state_pending";
		bool authInProgress;

		IGoogleApiClient mClient;

		IOnDataPointListener mListener;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			InitializeLogging();

			if (savedInstanceState != null) {
				authInProgress = savedInstanceState.GetBoolean(AUTH_PENDING);
			}

			BuildFitnessClient();
		}


		void BuildFitnessClient() {
			var clientConnectionCallback = new ClientConnectionCallback ();
			clientConnectionCallback.OnConnectedImpl = FindFitnessDataSources;
			mClient = new GoogleApiClientBuilder(this)
				.AddApi(FitnessClass.SENSORS_API)
				.AddScope(new Scope(Scopes.FitnessLocationRead))
				.AddConnectionCallbacks(clientConnectionCallback)
				.AddOnConnectionFailedListener((ConnectionResult result) => {
							Log.Info(TAG, "Connection failed. Cause: " + result);
							if (!result.HasResolution) {
								// Show the localized error dialog
						GooglePlayServicesUtil.GetErrorDialog(result.ErrorCode,
									this, 0).Show();
								return;
							}
							if (!authInProgress) {
								try {
							Log.Info(TAG, "Attempting to resolve failed connection");
									authInProgress = true;
									result.StartResolutionForResult(this,
										REQUEST_OAUTH);
								} catch (IntentSender.SendIntentException e) {
							Log.Error(TAG,
										"Exception while starting resolution activity", e);
								}
							}
						})
				.Build();
		}

		class ClientConnectionCallback : Java.Lang.Object, IGoogleApiClientConnectionCallbacks
		{
			public Action OnConnectedImpl { get; set; }

			public void OnConnected (Bundle connectionHint)
			{
				Log.Info (TAG, "Connected!!!");

				OnConnectedImpl();
			}

			public void OnConnectionSuspended (int cause)
			{
				if (cause == GoogleApiClientConnectionCallbacksConsts.CauseNetworkLost) {
					Log.Info (TAG, "Connection lost.  Cause: Network Lost.");
				} else if (cause == GoogleApiClientConnectionCallbacksConsts.CauseServiceDisconnected) {
					Log.Info (TAG, "Connection lost.  Reason: Service Disconnected");
				}
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			Log.Info(TAG, "Connecting...");
			mClient.Connect();
		}


		protected override void OnStop ()
		{
			base.OnStop ();
			if (mClient.IsConnected) {
				mClient.Disconnect();
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == REQUEST_OAUTH) {
				authInProgress = false;
				if (resultCode == Result.Ok) {
					if (!mClient.IsConnecting && !mClient.IsConnected) {
						mClient.Connect();
					}
				}
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutBoolean(AUTH_PENDING, authInProgress);
		}

		void FindFitnessDataSources() {
			FitnessClass.SensorsApi.FindDataSources(mClient, new DataSourcesRequest.Builder()
				.SetDataTypes(DataType.TypeLocationSample)
				.SetDataSourceTypes(DataSource.TypeRaw)
				.Build())
				.SetResultCallback((DataSourcesResult dataSourcesResult) => {
						Log.Info(TAG, "Result: " + dataSourcesResult.Status);
					foreach (var dataSource in dataSourcesResult.DataSources) {
						Log.Info(TAG, "Data source found: " + dataSource);
						Log.Info(TAG, "Data Source type: " + dataSource.DataType.Name);

							//Let's register a listener to receive Activity data!
							if (dataSource.DataType == DataType.TypeLocationSample && mListener == null) {
								Log.Info(TAG, "Data source for LOCATION_SAMPLE found!  Registering.");
								RegisterFitnessDataListener(dataSource,
								DataType.TypeLocationSample);
							}
					}
				});
		}


		void RegisterFitnessDataListener(DataSource dataSource, DataType dataType) {
			// [START register_data_listener]
			mListener = new OnDataPointListener();
			FitnessClass.SensorsApi.Add(mClient, 
				new SensorRequest.Builder()
				.SetDataSource(dataSource) // Optional but recommended for custom data sets.
				.SetDataType(dataType) // Can't be omitted.
				.SetSamplingRate(10, TimeUnit.Seconds)
				.Build(),
				mListener)
				.SetResultCallback((Statuses status) => {
						if (status.IsSuccess) {
						Log.Info(TAG, "Listener registered!");
						} else {
						Log.Info(TAG, "Listener not registered.");
						}
				});
		}

		class OnDataPointListener : Java.Lang.Object, IOnDataPointListener
		{
			public void OnDataPoint (DataPoint dataPoint)
			{
				foreach (var field in dataPoint.DataType.Fields) {
					Value val = dataPoint.GetValue(field);
					Log.Info(TAG, "Detected DataPoint field: " + field.Name);
					Log.Info(TAG, "Detected DataPoint value: " + val);
				}
			}
			
		}
		void UnregisterFitnessDataListener() {
			if (mListener == null) {
				return;
			}

			FitnessClass.SensorsApi.Remove(
				mClient,
				mListener)
				.SetResultCallback((Statuses status) => {
						if (status.IsSuccess) {
							Log.Info(TAG, "Listener was removed!");
						} else {
							Log.Info(TAG, "Listener was not removed.");
						}
				});
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			if (id == Resource.Id.action_unregister_listener) {
				UnregisterFitnessDataListener();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void InitializeLogging() {
			// Wraps Android's native log framework.
			LogWrapper logWrapper = new LogWrapper();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			Log.LogNode = logWrapper;
			// Filter strips out everything except the message text.
			MessageOnlyLogFilter msgFilter = new MessageOnlyLogFilter();
			logWrapper.NextNode = msgFilter;
			// On screen logging via a customized TextView.
			LogView logView = FindViewById<LogView>(Resource.Id.sample_logview);
			logView.SetTextAppearance(this, Resource.Style.Log);
			logView.SetBackgroundColor(Color.White);
			msgFilter.NextNode = logView;
			Log.Info(TAG, "Ready");
		}
	}
}


