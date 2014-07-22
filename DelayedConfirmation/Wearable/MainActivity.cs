using System;

using Android.App;
using Android.Content;
using Android.Gms;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Support.Wearable.View;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Util;
using Android.Widget;

namespace DelayedConfirmation
{
	[Activity (Label = "Wearable", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity, DelayedConfirmationView.IDelayedConfirmationListener, IGoogleApiClientOnConnectionFailedListener, IResultCallback
	{

		private static  string TAG = "DelayedConfirmation";
		private static  int NUM_SECONDS = 5;

		private static string TIMER_SELECTED_PATH = "/timer_selected";
		private static  string TIMER_FINISHED_PATH = "/timer_finished";

		private DelayedConfirmationView delayedConfirmationView;
		private IGoogleApiClient mGoogleApiClient;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_activity);
			delayedConfirmationView = FindViewById<DelayedConfirmationView> (Resource.Id.delayed_confirmation); //Present need refactor the package name
			delayedConfirmationView.SetTotalTimeMs (NUM_SECONDS * 1000);
			mGoogleApiClient = new GoogleApiClientBuilder (this)
				.AddApi (WearableClass.Api)
				.AddOnConnectionFailedListener (this)
				.Build ();
			FindViewById(Resource.Id.strart_timer).Click += delegate {
				OnStartTimer(null);
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (!mGoogleApiClient.IsConnected) {
				mGoogleApiClient.Connect ();
			}
		}

		protected override void OnDestroy ()
		{
			if (mGoogleApiClient.IsConnected) {
				mGoogleApiClient.Disconnect ();
			}
			base.OnDestroy ();
		}

		/**
	     * Starts the DelayedConfirmationView when user presses "Start Timer" button.
	     */
		public void OnStartTimer (View view)
		{
			delayedConfirmationView.Start ();
			delayedConfirmationView.SetListener (this);
		}

		public void OnTimerSelected (View v)
		{
			v.Pressed = true;
			Notification notification = new NotificationCompat.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetContentTitle (GetString (Resource.String.notification_title))
				.SetContentText (GetString (Resource.String.notification_timer_selected))
				.Build ();
			NotificationManagerCompat.From (this).Notify (0, notification);
			SendMessageToCompanion (TIMER_SELECTED_PATH);
			// Prevent onTimerFinished from being heard.
			((DelayedConfirmationView)v).SetListener (null);
			Finish ();
		}

		public void OnTimerFinished (View v)
		{
			Notification notification = new NotificationCompat.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetContentTitle (GetString (Resource.String.notification_title))
				.SetContentText (GetString (Resource.String.notification_timer_finished))
				.Build ();
			NotificationManagerCompat.From (this).Notify (0, notification);
			SendMessageToCompanion (TIMER_FINISHED_PATH);
			Finish ();
		}

		public void OnConnectionFailed (ConnectionResult connectionResult)
		{
			Log.Error (TAG, "Failed to connect to Google Api Client");
		}

		string path;

		private void SendMessageToCompanion (string path)
		{
			this.path = path;
			WearableClass.NodeApi.GetConnectedNodes (mGoogleApiClient).SetResultCallback (this);
		}

		public void OnResult (Java.Lang.Object raw)
		{
			Exception nodeException, messageException;
			try {
				var nodeResult = raw.JavaCast<INodeApiGetConnectedNodesResult> ();
				foreach (var node in nodeResult.Nodes)
					WearableClass.MessageApi.SendMessage (mGoogleApiClient, node.Id, path, new byte[0]).SetResultCallback (this);
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
			Log.Warn (TAG, "Unexpected type for OnResult");
			Log.Error (TAG, "Node Exception", nodeException);
			Log.Error (TAG, "Message Exception", messageException);
		}
	}
}


