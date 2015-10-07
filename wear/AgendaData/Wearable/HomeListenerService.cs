using System;
using Java.Util;
using Android.Gms.Common.Apis;
using Android.App;
using Android.Gms.Wearable;
using Android.Util;
using Android.Content;
using System.Collections.Generic;
using Android.Text;
using Android.Text.Format;
using Android.Graphics;
using Android.Runtime;
using System.Threading.Tasks;

namespace Wearable
{
	[Service()]
	[IntentFilter(new []{ "com.google.android.gms.wearable.BIND_LISTENER"})]
	public class HomeListenerService : WearableListenerService
	{
		private static readonly Dictionary<Android.Net.Uri, int> sNotificationIdByDataItemUri = new Dictionary<Android.Net.Uri, int>();
		private static int sNotificationId = 1;
		private GoogleApiClient mGoogleApiClient;

		public override async void OnDataChanged (DataEventBuffer dataEvents)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Debug))
				Log.Debug (Constants.TAG, "OnDataChanged: " + dataEvents + " for " + PackageName);
			try 
			{
				for (int i = 0; i < dataEvents.Count; i++) {
					var et = dataEvents.Get (i).JavaCast <IDataEvent> ();
					if (et.Type == DataEvent.TypeDeleted)
						DeleteDataItem (et.DataItem);
					else if (et.Type == DataEvent.TypeChanged)
						await UpdateNotificationForDataItem (et.DataItem);
				}
			}
			catch (Exception ex) {
				if (Log.IsLoggable (Constants.TAG, LogPriority.Error))
					Log.Error (Constants.TAG, "OnDataChanged error: " + ex);
			}
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			if (Log.IsLoggable (Constants.TAG, LogPriority.Info))
				Log.Info (Constants.TAG, "HomeListenerService created");
			mGoogleApiClient = new GoogleApiClient.Builder (this.ApplicationContext)
				.AddApi (WearableClass.API)
				.Build ();
			mGoogleApiClient.Connect ();
		}
		public HomeListenerService ()
		{

		}

		/// <summary>
		/// Puts a local notification to show calendar card
		/// </summary>
		/// <param name="dataItem">Data item.</param>
		private async Task UpdateNotificationForDataItem(IDataItem dataItem)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Verbose))
				Log.Verbose (Constants.TAG, "Updating notification for IDataItem");

			DataMapItem mapDataItem = DataMapItem.FromDataItem (dataItem);
			DataMap data = mapDataItem.DataMap;

			String description = data.GetString (Constants.DESCRIPTION);
			if (TextUtils.IsEmpty (description)) {
				description = "";
			} else {
				// Add a space between the description and the time of the event
				description += " ";
			}

			String contentText;
			if (data.GetBoolean(Constants.ALL_DAY)) {
				contentText = GetString (Resource.String.desc_all_day, description);
			}
			else 
			{
				String startTime = DateFormat.GetTimeFormat (this).Format (new Date (data.GetLong (Constants.BEGIN)));
				String endTime = DateFormat.GetTimeFormat (this).Format (new Date (data.GetLong (Constants.END)));
				contentText = GetString (Resource.String.desc_time_period, description, startTime, endTime);
			}

			Intent deleteOperation = new Intent (this, typeof(DeleteService));
			// Use a unique identifier for the delete action.

			String deleteAction = "action_delete" + dataItem.Uri.ToString () + sNotificationId;
			deleteOperation.SetAction (deleteAction);
			deleteOperation.SetData (dataItem.Uri);
			PendingIntent deleteIntent = PendingIntent.GetService (this, 0, deleteOperation, PendingIntentFlags.OneShot);
			PendingIntent silentDeleteIntent = PendingIntent.GetService (this, 1, deleteOperation.PutExtra (Constants.EXTRA_SILENT, true), PendingIntentFlags.OneShot);

			Notification.Builder notificationBuilder = new Notification.Builder (this)
				.SetContentTitle (data.GetString (Constants.TITLE))
				.SetContentText (contentText)
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.AddAction (Resource.Drawable.ic_menu_delete, GetText (Resource.String.delete), deleteIntent)
				.SetDeleteIntent (silentDeleteIntent)
				.SetLocalOnly (true)
				.SetPriority ((int)NotificationPriority.Min);

			// Set the event owner's profile picture as the notification background
			Asset asset = data.GetAsset (Constants.PROFILE_PIC);
			if (asset != null) {
				if (mGoogleApiClient.IsConnected) {
                    var assetFdResult = await WearableClass.DataApi.GetFdForAssetAsync (mGoogleApiClient, asset);
					if (assetFdResult.Status.IsSuccess) {
						Bitmap profilePic = BitmapFactory.DecodeStream (assetFdResult.InputStream);
						notificationBuilder.Extend (new Notification.WearableExtender ().SetBackground (profilePic));
					} else if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
						Log.Debug (Constants.TAG, "asset fetch failed with StatusCode: " + assetFdResult.Status.StatusCode);
					}
				} else {
					Log.Error (Constants.TAG, "Failed to set notification background - Client disconnected from Google Play Services");
				}

				Notification card = notificationBuilder.Build ();

				(GetSystemService (Context.NotificationService).JavaCast<NotificationManager>()).Notify (sNotificationId, card);

				sNotificationIdByDataItemUri.Add (dataItem.Uri, sNotificationId++);
			}
		}
		private void DeleteDataItem(IDataItem dataITem)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Verbose)) {
				Log.Verbose (Constants.TAG, "OnDataItemDeleted:DataITem=" + dataITem.Uri);
			}
			if (sNotificationIdByDataItemUri.ContainsKey (dataITem.Uri)) {
				int notificaionId = sNotificationIdByDataItemUri [dataITem.Uri];
				sNotificationIdByDataItemUri.Remove (dataITem.Uri);
				if (notificaionId != -1) {
					(GetSystemService (Context.NotificationService).JavaCast<NotificationManager> ()).Cancel (notificaionId);
				}
			}
		}

		public override void OnMessageReceived (IMessageEvent messageEvent)
		{

			if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
				Log.Debug (Constants.TAG, "OnMessageReceived: " + messageEvent.Path
				+ " " + messageEvent.GetData () + " for " + PackageName);
			}

		}
	}
}

