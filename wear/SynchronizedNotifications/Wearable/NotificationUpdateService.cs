using System;
using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Common;
using Android.Runtime;
using Android.Util;
using Android.App;
using Android.Content;

namespace Wearable
{
	[Service(), IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" }), 
		IntentFilter(new string [] {"com.example.android.wearable.synchronizednotifications.DISMISS"})]
	public class NotificationUpdateService : WearableListenerService, GoogleApiClient.IConnectionCallbacks, 
	    GoogleApiClient.IOnConnectionFailedListener, IResultCallback
	{
		const string Tag = "NotificationUpdate";
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

		public override void OnDataChanged (DataEventBuffer dataEvents)
		{
			for (int i = 0; i < dataEvents.Count; i++) {
				var dataEvent = dataEvents.Get (i).JavaCast<IDataEvent> ();
				if (dataEvent.Type == DataEvent.TypeChanged) {
					DataMap dataMap = DataMapItem.FromDataItem (dataEvent.DataItem).DataMap;
					var content = dataMap.GetString (Constants.KeyContent);
					var title = dataMap.GetString (Constants.KeyTitle);
					if (Constants.WatchOnlyPath.Equals (dataEvent.DataItem.Uri.Path))
						BuildWearableOnlyNotification (title, content, false);
					else if (Constants.BothPath.Equals (dataEvent.DataItem.Uri.Path))
						BuildWearableOnlyNotification (title, content, true);
				} else if (dataEvent.Type == DataEvent.TypeDeleted) {
					if (Log.IsLoggable (Tag, LogPriority.Debug))
						Log.Debug (Tag, "DataItem deleted: " + dataEvent.DataItem.Uri.Path);
					if (Constants.BothPath.Equals (dataEvent.DataItem.Uri.Path))
						((NotificationManager)GetSystemService (NotificationService)).Cancel (Constants.BothId);
				}
			}
		}

		void BuildWearableOnlyNotification (string title, string content, bool withDismissal)
		{
			var builder = new Notification.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetContentTitle (title)
				.SetContentText (content);

			if (withDismissal) {
				var dismissIntent = new Intent (Constants.ActionDismiss);
				dismissIntent.PutExtra (Constants.KeyNotificationId, Constants.BothId);
				PendingIntent pendingIntent = PendingIntent.GetService (this, 0, dismissIntent, PendingIntentFlags.UpdateCurrent);
				builder.SetDeleteIntent (pendingIntent);
			}

			((NotificationManager)GetSystemService (NotificationService)).Notify (Constants.WatchOnlyId, builder.Build ());
		}

		public void OnConnected (Android.OS.Bundle p0)
		{
			var dataItemUri = new Android.Net.Uri.Builder ().Scheme (PutDataRequest.WearUriScheme).Path (Constants.BothPath).Build ();
			if (Log.IsLoggable(Tag, LogPriority.Debug)) {
				Log.Debug(Tag, "Deleting Uri: " + dataItemUri.ToString());
			}
			WearableClass.DataApi.DeleteDataItems (googleApiClient, dataItemUri).SetResultCallback (this);
		}

		public void OnConnectionSuspended (int p0) { }

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult p0) { }

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

