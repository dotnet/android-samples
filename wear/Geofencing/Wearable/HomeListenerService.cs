using System;
using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Android.Util;
using Java.Interop;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using Android.Content;
using Android.App;

namespace Wearable
{
	[Service(), IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" })]
	public class HomeListenerService : WearableListenerService
	{
		private GoogleApiClient mGoogleApiClient;
		public override void OnCreate ()
		{
			base.OnCreate ();
			mGoogleApiClient = new GoogleApiClient.Builder (this.ApplicationContext)
				.AddApi (WearableClass.API)
				.Build ();
			mGoogleApiClient.Connect ();
		}

		/// <summary>
		/// Listens for DataItems added/deleted from the geofence service running on the companion
		/// </summary>
		public override void OnDataChanged (DataEventBuffer dataEvents)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Debug))
				Log.Debug (Constants.TAG, "OnDataChanged: " + dataEvents + " for " + PackageName);

			for (int i = 0; i < dataEvents.Count; i++) {
				var dEvent = dataEvents.Get (i).JavaCast<IDataEvent> ();
				if (dEvent.Type == DataEvent.TypeDeleted) {
					CancelNotificationForDataItem (dEvent.DataItem);
				} else if (dEvent.Type == DataEvent.TypeChanged) {
					// The user has entered a geofence - post a notification!
					String geofenceId = DataMap.FromByteArray (dEvent.DataItem.GetData ()).GetString (Constants.KEY_GEOFENCE_ID);
					PostNotificationForGeofenceId (geofenceId, dEvent.DataItem.Uri);
				}
			}
		}

		/// <summary>
		/// Deletes the check-in notification when the DataITem is deleted
		/// </summary>
		/// <param name="dataItem">Used only for logging in this sample, but could be used to identify which notifications to cancel
		/// (in this case there is at most 1 notification)</param>
		void CancelNotificationForDataItem(IDataItem dataItem)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Verbose)) {
				Log.Verbose(Constants.TAG, "OnDataItemDeleted: DataItem = " + dataItem.Uri);
			}
			GetSystemService(NotificationService).JavaCast<NotificationManager>().Cancel(Constants.NOTIFICATION_ID);
		}

		/// <summary>
		/// Posts a local notification for the given geofence ID, with an option to check in
		/// </summary>
		/// <param name="geofenceId">The geodence id that the user has triggered</param>
		/// <param name="dataItemUri">The Uri for the DataItem that triggered this notification. Used to delete this DataItem
		/// when the notification is dismissed</param>
		void PostNotificationForGeofenceId(String geofenceId, Android.Net.Uri dataItemUri) {
			// Use the geofenceId to determine the title and background of the check-in notification.
			// A SpannableString is used for the notification title for resizing capabilities
			SpannableString checkInTitle;
			Bitmap notificationBackground;
			if (Constants.ANDROID_BUILDING_ID.Equals (geofenceId)) {
				checkInTitle = new SpannableString (GetText (Resource.String.android_building_title));
				notificationBackground = BitmapFactory.DecodeResource (Resources, Resource.Drawable.android_building);
			} else if (Constants.YERBA_BUENA_ID.Equals (geofenceId)) {
				checkInTitle = new SpannableString (GetText (Resource.String.yerba_buena_title));
				notificationBackground = BitmapFactory.DecodeResource (Resources, Resource.Drawable.yerba_buena);
			} else {
				Log.Error (Constants.TAG, "Unrecognized geofence id: " + geofenceId);
				return;
			}
			// Resize the title to avoid truncation
			checkInTitle.SetSpan (new RelativeSizeSpan (0.8f), 0, checkInTitle.Length(), SpanTypes.PointMark);

			Intent checkInOperation = new Intent (this, typeof(CheckInAndDeleteDataItemsService)).SetData (dataItemUri);
			PendingIntent checkInIntent = PendingIntent.GetService (this, 0, checkInOperation.SetAction (Constants.ACTION_CHECK_IN),
				                              PendingIntentFlags.CancelCurrent);
			PendingIntent deleteDataItemIntent = PendingIntent.GetService (this, 0, checkInOperation.SetAction (Constants.ACTION_DELETE_DATA_ITEM),
				                                     PendingIntentFlags.CancelCurrent);
			// This action will be embedded into the notification
			var checkInAction = new Notification.Action (Resource.Drawable.ic_action_check_in,
				                       GetText (Resource.String.check_in_prompt), checkInIntent);

			Notification notification = new Notification.Builder (this)
				.SetContentTitle (checkInTitle)
				.SetContentText (GetText (Resource.String.check_in_prompt))
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetDeleteIntent (deleteDataItemIntent)
				.Extend (new Notification.WearableExtender ()
					.SetBackground (notificationBackground)
					.AddAction (checkInAction)
					.SetContentAction (0)
					.SetHintHideIcon (true))
				.SetLocalOnly (true)
				.Build ();

			GetSystemService (NotificationService).JavaCast<NotificationManager> ().Notify (Constants.NOTIFICATION_ID, notification);
		}
	}
}

