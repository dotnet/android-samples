using System;
using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Common;
using Android.Support.V4.App;
using Android.Util;
using Java.Interop;
using Android.App;

namespace SynchronizedNotifications
{
	[Service(), IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" }),
		IntentFilter(new string[] { "com.example.android.wearable.synchronizednotifications.DISMISS" })]
	public class DismissListener : WearableListenerService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener,
	IResultCallback
	{
		const string Tag = "DismissListener";
		GoogleApiClient googleApiClient;

		public override void OnCreate ()
		{
			base.OnCreate ();
			googleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		public override void OnDataChanged (DataEventBuffer dataEvents)
		{
			for (int i = 0; i < dataEvents.Count; i++) {
				IDataEvent dataEvent = Java.Interop.JavaObjectExtensions.JavaCast<IDataEvent>(dataEvents.Get (i));
				if (dataEvent.Type == DataEvent.TypeDeleted) {
					if (Constants.BothPath.Equals (dataEvent.DataItem.Uri.Path)) {
						// Notification on the phone should be dismissed
						NotificationManagerCompat.From (this).Cancel (Constants.BothId);
					}
				}
			}
		}

		public override Android.App.StartCommandResult OnStartCommand (Android.Content.Intent intent, Android.App.StartCommandFlags flags, int startId)
		{
			if (intent != null) {
				string action = intent.Action;
				if (Constants.ActionDismiss.Equals (action)) {
					// We need to dismiss the wearable notification. We delete the DataItem that created the notification to inform the wearable.
					int notificationId = intent.GetIntExtra (Constants.KeyNotificationId, -1);
					if (notificationId == Constants.BothId) {
						DismissWearableNotification (notificationId);
					}
				}
			}
			return base.OnStartCommand (intent, flags, startId);
		}

		void DismissWearableNotification(int id) {
			googleApiClient.Connect ();
		}

		public void OnConnected (Android.OS.Bundle connectionHint)
		{
			var dataItemUri = new Android.Net.Uri.Builder ().Scheme (PutDataRequest.WearUriScheme).Path (Constants.BothPath).Build ();
			if (Log.IsLoggable(Tag, LogPriority.Debug)) {
				Log.Debug(Tag, "Deleting Uri: " + dataItemUri.ToString());
			}
			WearableClass.DataApi.DeleteDataItems (googleApiClient, dataItemUri).SetResultCallback (this);
		}

		public void OnConnectionSuspended (int cause) { }

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Error (Tag, "Failed to connect to the Google API client");
		}

		public void OnResult (Java.Lang.Object result)
		{
			googleApiClient.Disconnect ();
			IDataApiDeleteDataItemsResult deleteDataItemsResult;
			try 
			{
				deleteDataItemsResult = result.JavaCast<IDataApiDeleteDataItemsResult>();
			}
			catch {
				return;
			}
			if (!deleteDataItemsResult.Status.IsSuccess)
				Log.Error (Tag, "DismissWearableNotification(): Failed to delete IDataItem");
		}
	}
}

