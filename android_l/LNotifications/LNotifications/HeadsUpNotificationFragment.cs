
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

namespace LNotifications
{
	// Fragment that demonstrates options for displaying Heads-Up Notifications.
	public class HeadsUpNotificationFragment : Fragment
	{
		// NotificationId used for the notifications in this Fragment.
		public const int NOTIFICATION_ID = 1;

		private NotificationManager notificationManager;

		// Button to show a notification
		private Button showNotificationButton;

		// If checked, notifications that this Fragment creates will be Heads-Up Notifications.
		private CheckBox useHeadsUpCheckbox;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			notificationManager = (NotificationManager)Activity.GetSystemService (Context.NotificationService);
		}

		public static HeadsUpNotificationFragment NewInstance()
		{
			var fragment = new HeadsUpNotificationFragment ();
			fragment.RetainInstance = true;
			return fragment;
		}

		public HeadsUpNotificationFragment()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_heads_up_notification, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			showNotificationButton = (Button)view.FindViewById (Resource.Id.show_notification_button);
			showNotificationButton.Click += delegate {
				notificationManager.Notify(NOTIFICATION_ID,CreateNotification(useHeadsUpCheckbox.Checked));
				Toast.MakeText(Activity,"Show Notification clicked",ToastLength.Short).Show();
			};
			useHeadsUpCheckbox = (CheckBox)view.FindViewById (Resource.Id.use_heads_up_checkbox);
		}

		// Creates a new notification depending on makeHeadsUpNotification.
		// If makeHeadsUpNotification is true, the notifications will be
		// Heads-Up Notifications.
		public Notification CreateNotification(bool makeHeadsUpNotification)
		{
			var builder = new Notification.Builder (Activity)
				.SetSmallIcon (Resource.Drawable.ic_launcher_notification)
				.SetPriority ((int) NotificationPriority.Default)
				.SetCategory (Notification.CategoryMessage)
				.SetContentTitle ("Sample Notification")
				.SetContentText ("This is a normal notification.");
			if (makeHeadsUpNotification) {
				var push = new Intent ();
				push.AddFlags (ActivityFlags.NewTask);
				push.SetClass (Activity, Java.Lang.Class.FromType (typeof(MainActivity)));
				var fullScreenPendingIntent = PendingIntent.GetActivity (Activity, 0,
					push, PendingIntentFlags.CancelCurrent);
				builder
					.SetContentText ("Heads-Up Notification on Android L or above.")
					.SetFullScreenIntent (fullScreenPendingIntent, true);
			}
			return builder.Build ();
		}


	}
}

