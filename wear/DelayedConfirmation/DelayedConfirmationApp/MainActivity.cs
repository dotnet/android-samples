using System;

using Android.App;
using Android.Content;
using Android.Gms;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DelayedConfirmation
{
	[Activity (Label = "DelayedConfirmation", MainLauncher = true, Icon = "@drawable/ic_launcher", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
	public class MainActivity : Activity, GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener, IMessageApiMessageListener, IResultCallback
	{
		private static string TAG = "DelayedConfirmation";
		private static string START_ACTIVITY_PATH = "/start-activity";
		private static string TIMER_SELECTED_PATH = "/timer_selected";
		private static string TIMER_FINISHED_PATH = "/timer_finished";
		private GoogleApiClient mGoogleApiClient;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_activity);
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
			FindViewById (Resource.Id.start_wearable_activity).Click += delegate {
				OnStartWearableActivityClick (null);
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (!mGoogleApiClient.IsConnected)
				mGoogleApiClient.Connect ();
		}

		protected override void OnDestroy ()
		{
			if (mGoogleApiClient.IsConnected)
				mGoogleApiClient.Disconnect ();
			base.OnDestroy ();
		}

		public void OnConnected (Bundle bundle)
		{
			WearableClass.MessageApi.AddListener (mGoogleApiClient, this);
		}

		public void OnConnectionSuspended (int i)
		{
			WearableClass.MessageApi.RemoveListener (mGoogleApiClient, this);
		}

		public void OnMessageReceived (IMessageEvent messageEvent)
		{
			RunOnUiThread (() => {
				if (messageEvent.Path.Equals (TIMER_SELECTED_PATH)) {
					Toast.MakeText (ApplicationContext, Resource.String.toast_timer_selected,
						ToastLength.Short).Show ();
				} else if (messageEvent.Path.Equals (TIMER_FINISHED_PATH)) {
					Toast.MakeText (ApplicationContext, Resource.String.toast_timer_finished,
						ToastLength.Short).Show ();
				}
			});
		}

		public void OnConnectionFailed (ConnectionResult connectionResult)
		{
			Log.Error (TAG, "Failed to connect to Google Api Client with error code "
			+ connectionResult.ErrorCode);
		}

		public void OnStartWearableActivityClick (View v)
		{
			WearableClass.NodeApi.GetConnectedNodes (mGoogleApiClient).SetResultCallback (this);
		}

		//Follow the same flow as the MainActivity for the Wearable (Wearable/MainActivity has more details).
		public void OnResult (Java.Lang.Object raw)
		{
			Exception nodeException, messageException;
			try {
				var nodeResult = raw.JavaCast<INodeApiGetConnectedNodesResult> ();
				foreach (var node in nodeResult.Nodes)
					WearableClass.MessageApi.SendMessage (mGoogleApiClient, node.Id, START_ACTIVITY_PATH, new byte[0])
							.SetResultCallback (this);
				return;
			} catch (Exception e) {
				nodeException = e;
			}
			try {
				var messageResult = raw.JavaCast<IMessageApiSendMessageResult> ();
				if (!messageResult.Status.IsSuccess)
					Log.Error (TAG, "Failed to connect to Google Api Client with status "
					+ messageResult.Status);
				return;
			} catch (Exception e) {
				messageException = e;
			}
			//We should never get to this point
			Log.Wtf (TAG, "Unexpected type for OnResult");
			Log.Error (TAG, "Node Exception", nodeException);
			Log.Error (TAG, "Message Exception", messageException);
		}
	}
}