using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Service.Notification;
using Android.Support.V4.App;
using Android.Util;

namespace ActiveNotifications
{
	// A fragment that allows notifications to be enqueued.
	public class ActiveNotificationsFragment : Android.App.Fragment
	{
		// The request code can be any number as long as it doesn't match another request code used in the same app.
		static readonly int REQUEST_CODE = 2323;

		static readonly string TAG = "ActiveNotificationsFragment";

		static readonly string NOTIFICATION_GROUP = "com.example.android.activenotifications.notification_type";

		static readonly int NOTIFICATION_GROUP_SUMMARY_ID = 1;

		NotificationManager notificationManager;
		TextView numberOfNotifications;

		// Every notification needs a unique ID otherwise the previous one would be overwritten. This
		// variable is incremented when used.
		int notificationId = NOTIFICATION_GROUP_SUMMARY_ID + 1;

		PendingIntent deletePendingIntent;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_notification_builder, container, false);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			UpdateNumberOfNotifications ();
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			notificationManager = (NotificationManager)Activity.GetSystemService (Context.NotificationService);
			numberOfNotifications = view.FindViewById <TextView> (Resource.Id.number_of_notifications);
			var addNotification = view.FindViewById <Button> (Resource.Id.add_notification);

			addNotification.Click += (sender, e) => AddNotificationAndUpdateSummaries ();

			// Create a PendingIntent to be fired upon deletion of a Notification.
			var deleteIntent = new Intent (MainActivity.ACTION_NOTIFICATION_DELETE);
			deletePendingIntent = PendingIntent.GetBroadcast (Activity, REQUEST_CODE, deleteIntent, 0);
		}

		/**
		* Adds a new {@link Notification} with sample data and sends it to the system.
		* Then updates the current number of displayed notifications for this application and
		* creates a notification summary if more than one notification exists.
		*/
		void AddNotificationAndUpdateSummaries ()
		{
			// Create a Notification and notify the system.
			var builder = new NotificationCompat.Builder (Activity)
				.SetSmallIcon (Resource.Mipmap.ic_notification)
				.SetContentTitle (GetString (Resource.String.app_name))
				.SetContentText (GetString (Resource.String.sample_notification_content))
				.SetAutoCancel (true)
				.SetDeleteIntent (deletePendingIntent)
				.SetGroup (NOTIFICATION_GROUP);

			Notification notification = builder.Build ();
			notificationManager.Notify (GetNewNotificationId (), notification);

			CommonSampleLibrary.Log.Info (TAG, "Add a notification");
			UpdateNotificationSummary ();
			UpdateNumberOfNotifications ();
		}

		// Adds/updates/removes the notification summary as necessary.
		void UpdateNotificationSummary ()
		{
			StatusBarNotification[] activeNotifications = notificationManager.GetActiveNotifications ();
			int numberOfNotifications = activeNotifications.Length;

			// Since the notifications might include a summary notification remove it from the count if
			// it is present.
			foreach (StatusBarNotification notification in activeNotifications) {
				if (notification.Id == NOTIFICATION_GROUP_SUMMARY_ID) {
					numberOfNotifications--;
					break;
				}
			}

			if (numberOfNotifications > 1) {
				// Add/update the notification summary.
				string notificationContent = GetString (Resource.String.sample_notification_content, "" + numberOfNotifications);
				var builder = new NotificationCompat.Builder (Activity);
				builder.SetSmallIcon (Resource.Mipmap.ic_notification);
				builder.SetStyle (new NotificationCompat.BigTextStyle ().SetSummaryText (notificationContent));
				builder.SetGroup (NOTIFICATION_GROUP);
				builder.SetGroupSummary (true);

				Notification notification = builder.Build ();
				notificationManager.Notify (NOTIFICATION_GROUP_SUMMARY_ID, notification);
			} else {
				// Remove the notification summary.
				notificationManager.Cancel (NOTIFICATION_GROUP_SUMMARY_ID);
			}
		}

		/**
		 * Request the current number of notifications from the NotificationManager
		 * and display them to the user
		 */
		public void UpdateNumberOfNotifications () 
		{
			int notificationsCount = GetNumberOfNotifications();
			numberOfNotifications.Text = GetString(Resource.String.active_notifications, notificationsCount);
			Log.Info(TAG, GetString(Resource.String.active_notifications, notificationsCount));
		}

		/**
		* Retrieves a unique notification ID.
		*/
		public int GetNewNotificationId ()
		{
			int updatedNotificationId = notificationId++;
			// Unlikely in the sample, but the int will overflow if used enough so we skip the summary
			// ID. Most apps will prefer a more deterministic way of identifying an ID such as hashing
			// the content of the notification.
			if (updatedNotificationId == NOTIFICATION_GROUP_SUMMARY_ID)
				updatedNotificationId = notificationId++;

			return updatedNotificationId;
		}

		private int GetNumberOfNotifications()
		{
			StatusBarNotification[] activeNotifications = notificationManager.GetActiveNotifications();

			foreach (StatusBarNotification notification in activeNotifications)
			{
				if (notification.Id == NOTIFICATION_GROUP_SUMMARY_ID)
				{
					return activeNotifications.Length - 1;
				}
			}
			return activeNotifications.Length;
		}
	}
}

