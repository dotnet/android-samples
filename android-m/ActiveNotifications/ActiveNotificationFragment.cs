
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Service.Notification;

namespace ActiveNotifications
{
	public class ActiveNotificationFragment :Fragment
	{
		static readonly string TAG = "ActiveNotificationFragment";

		NotificationManager notificationManager;
		TextView numberOfNotifications;

		// Every notification needs a unique ID otherwise the previous one would be overwritten.
		int notificationId = 0;
		PendingIntent deletePendingIntent;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_notification_builder, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			notificationManager = (NotificationManager)Activity.GetSystemService (Context.NotificationService);
			numberOfNotifications = view.FindViewById <TextView> (Resource.Id.number_of_notifications);
			var addNotification = view.FindViewById <Button> (Resource.Id.add_notification);

			addNotification.Click += (object sender, EventArgs e) => AddNotificationAndReadNumber ();

			// Create a PendingIntent to be fired upon deletion of a Notification.
			var deleteIntent = new Intent (MainActivity.ACTION_NOTIFICATION_DELETE);
			deletePendingIntent = PendingIntent.GetBroadcast (Activity, 2323 /* requestCode */, deleteIntent, 0);
		}
			
		/**
		 *  Add a new Notification with sample data and send it to the system.
		 * Then read the current number of displayed notifications for this application.
		 */
		void AddNotificationAndReadNumber ()
		{
			// Create a Notification and notify the system.
			var builder = new Notification.Builder (Activity)
				.SetSmallIcon (Resource.Mipmap.ic_notification)
				.SetContentTitle (GetString (Resource.String.app_name))
				.SetContentText (GetString (Resource.String.sample_notification_content))
				.SetAutoCancel (true)
				.SetDeleteIntent (deletePendingIntent);

			Notification notification = builder.Build ();
			notificationManager.Notify (++notificationId, notification);

			CommonSampleLibrary.Log.Info (TAG, "Add a notification");
			UpdateNumberOfNotifications ();
		}

		/**
		 * Request the current number of notifications from the NotificationManager
		 * and display them to the user
		 */
		public void UpdateNumberOfNotifications () 
		{
			/** TODO Clearing large sets of notifications at once currently throws an exception. 
			 * See https://github.com/googlesamples/android-ActiveNotifications/issues/1
			 * for more information.
			*/
			try {
				// Query the currently displayed notifications.
				StatusBarNotification[] activeNotifications = notificationManager.GetActiveNotifications ();

				int totalNotifications = activeNotifications.Length;
				numberOfNotifications.Text = GetString (Resource.String.active_notifications, totalNotifications);

				CommonSampleLibrary.Log.Info (TAG, GetString (Resource.String.active_notifications, totalNotifications));
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}
		}
	}
}

