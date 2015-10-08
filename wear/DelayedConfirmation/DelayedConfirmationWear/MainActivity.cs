using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Widget;

using Android.Support.V4.App;

using Android.Gms;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;

using Android.Support.Wearable.Views;

namespace DelayedConfirmation
{
	[Activity (Label = "Wearable", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity, DelayedConfirmationView.IDelayedConfirmationListener, 
        GoogleApiClient.IOnConnectionFailedListener, IResultCallback
	{
		private static string TAG = "DelayedConfirmation";
		private static int NUM_SECONDS = 5;

		private static string TIMER_SELECTED_PATH = "/timer_selected";
		private static string TIMER_FINISHED_PATH = "/timer_finished";

		string path;

		private DelayedConfirmationView delayedConfirmationView;
		private GoogleApiClient mGoogleApiClient;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_activity);
			delayedConfirmationView = FindViewById<DelayedConfirmationView> (Resource.Id.delayed_confirmation);
			delayedConfirmationView.SetTotalTimeMs (NUM_SECONDS * 1000);
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.Api)
				.AddOnConnectionFailedListener (this)
				.Build ();
			FindViewById (Resource.Id.start_timer).Click += delegate {
				OnStartTimer (null);
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

		private void SendMessageToCompanion (string path)
		{
			this.path = path;
			WearableClass.NodeApi.GetConnectedNodes (mGoogleApiClient).SetResultCallback (this);
		}

		//Since Java bytecode does not support generics, casting between types can be messy. Handle everything here.
		public void OnResult (Java.Lang.Object raw)
		{
			Exception nodeException, messageException;
			try {
				//send the message
				var nodeResult = raw.JavaCast<INodeApiGetConnectedNodesResult> ();
				foreach (var node in nodeResult.Nodes)
					WearableClass.MessageApi.SendMessage (mGoogleApiClient, node.Id, path, new byte[0]).SetResultCallback (this); //will go to second try/catch block
				return;
			} catch (Exception e) {
				nodeException = e;
			}
			try {
				//check that it worked correctly
				var messageResult = raw.JavaCast<IMessageApiSendMessageResult> ();
				if (!messageResult.Status.IsSuccess)
					Log.Error (TAG, "Failed to connect to Google Api Client with status "
					+ messageResult.Status);
				return;
			} catch (Exception e) {
				messageException = e;
			}
			//Will never get here
			Log.Warn (TAG, "Unexpected type for OnResult");
			Log.Error (TAG, "Node Exception", nodeException);
			Log.Error (TAG, "Message Exception", messageException);
		}
	}
}


