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
using System.Threading.Tasks;

namespace BasicSensorsApi
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity
	{
		public const string TAG = "BasicSensorsApi";

		const int REQUEST_OAUTH = 1;

		const string AUTH_PENDING = "auth_state_pending";
		bool authInProgress;

		GoogleApiClient mClient;

		IOnDataPointListener mListener;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);
			InitializeLogging();

			if (savedInstanceState != null) {
				authInProgress = savedInstanceState.GetBoolean(AUTH_PENDING);
			}

			BuildFitnessClient();
		}


		void BuildFitnessClient ()
		{
			var clientConnectionCallback = new ClientConnectionCallback ();
            clientConnectionCallback.OnConnectedImpl = () => FindFitnessDataSources ();
			mClient = new GoogleApiClient.Builder (this)
				.AddApi (FitnessClass.SENSORS_API)
				.AddScope (new Scope (Scopes.FitnessLocationRead))
				.AddConnectionCallbacks (clientConnectionCallback)
				.AddOnConnectionFailedListener ((ConnectionResult result) => {
					Log.Info(TAG, "Connection failed. Cause: " + result);
					if (!result.HasResolution) {
						// Show the localized error dialog
						GooglePlayServicesUtil.GetErrorDialog (result.ErrorCode, this, 0).Show ();
								return;
					}
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

				OnConnectedImpl();
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

        async Task FindFitnessDataSources ()
		{
            var dataSourcesResult = await FitnessClass.SensorsApi.FindDataSourcesAsync (mClient, new DataSourcesRequest.Builder ()
				.SetDataTypes (DataType.TypeLocationSample)
				.SetDataSourceTypes (DataSource.TypeRaw)
                .Build ());
            
			Log.Info (TAG, "Result: " + dataSourcesResult.Status);
			foreach (DataSource dataSource in dataSourcesResult.DataSources) {
				Log.Info (TAG, "Data source found: " + dataSource);
				Log.Info (TAG, "Data Source type: " + dataSource.DataType.Name);

				//Let's register a listener to receive Activity data!
				if (dataSource.DataType == DataType.TypeLocationSample && mListener == null) {
					Log.Info (TAG, "Data source for LOCATION_SAMPLE found!  Registering.");
					await RegisterFitnessDataListener (dataSource, DataType.TypeLocationSample);
				}
			}
		}
			
        async Task RegisterFitnessDataListener (DataSource dataSource, DataType dataType)
		{
			// [START register_data_listener]
			mListener = new OnDataPointListener ();
			var status = await FitnessClass.SensorsApi.AddAsync (mClient, new SensorRequest.Builder ()
				.SetDataSource (dataSource) // Optional but recommended for custom data sets.
				.SetDataType (dataType) // Can't be omitted.
				.SetSamplingRate (10, TimeUnit.Seconds)
				.Build (),
                mListener);
			if (status.IsSuccess) {
				Log.Info (TAG, "Listener registered!");
			} else {
				Log.Info (TAG, "Listener not registered.");
			}
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

        async Task UnregisterFitnessDataListener ()
		{
			if (mListener == null) {
				return;
			}

            var status = await FitnessClass.SensorsApi.RemoveAsync (mClient, mListener);
				
			if (status.IsSuccess) {
				Log.Info (TAG, "Listener was removed!");
			} else {
				Log.Info (TAG, "Listener was not removed.");
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
			if (id == Resource.Id.action_unregister_listener) {
				UnregisterFitnessDataListener ();
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


