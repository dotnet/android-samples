using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Common.Apis;
using Android;
using Android.Gms.Fitness;
using Android.Gms.Common;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Result;
using CommonSampleLibrary;
using Android.Graphics;
using System.Threading.Tasks;

namespace BasicRecordingApi
{
	[Activity (MainLauncher = true)]
	public class MainActivity : ActionBarActivity
	{
		public const string TAG = "BasicRecordingApi";
		const int REQUEST_OAUTH = 1;

		const string AUTH_PENDING = "auth_state_pending";
		bool authInProgress;

		GoogleApiClient mClient;

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
            clientConnectionCallback.OnConnectedImpl = () => Subscribe ();
			// Create the Google API Client
			mClient = new GoogleApiClient.Builder (this)
				.AddApi (FitnessClass.RECORDING_API)
				.AddScope (new Scope (Scopes.FitnessActivityRead))
				.AddConnectionCallbacks (clientConnectionCallback)
				.AddOnConnectionFailedListener ((ConnectionResult result) => {
					Log.Info (TAG, "Connection failed. Cause: " + result.ToString ());
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
			// Connect to the Fitness API
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

        public async Task Subscribe ()
		{
            var status = await FitnessClass.RecordingApi.SubscribeAsync (mClient, DataType.TypeActivitySample);
			if (status.IsSuccess) {
				if (status.StatusCode == FitnessStatusCodes.SuccessAlreadySubscribed) {
					Log.Info (TAG, "Existing subscription for activity detected.");
				} else {
					Log.Info (TAG, "Successfully subscribed!");
				}
			} else {
				Log.Info (TAG, "There was a problem subscribing.");
			}
		}

        async Task DumpSubscriptionsList ()
		{
            var listSubscriptionsResult = await FitnessClass.RecordingApi.ListSubscriptionsAsync (mClient, DataType.TypeActivitySample);

			foreach (Subscription sc in listSubscriptionsResult.Subscriptions) {
				DataType dt = sc.DataType;
				Log.Info (TAG, "Active subscription for data type: " + dt.Name);
			}
		}

        async Task CancelSubscription ()
		{
			string dataTypeStr = DataType.TypeActivitySample.ToString ();
			Log.Info (TAG, "Unsubscribing from data type: " + dataTypeStr);

            var status = await FitnessClass.RecordingApi.UnsubscribeAsync (mClient, DataType.TypeActivitySample);
			if (status.IsSuccess) {
				Log.Info(TAG, "Successfully unsubscribed for data type: " + dataTypeStr);
			} else {
				// Subscription not removed
				Log.Info(TAG, "Failed to unsubscribe for data type: " + dataTypeStr);
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
			if (id == Resource.Id.action_cancel_subs) {
				CancelSubscription ();
				return true;
			} else if (id == Resource.Id.action_dump_subs) {
				DumpSubscriptionsList ();
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

