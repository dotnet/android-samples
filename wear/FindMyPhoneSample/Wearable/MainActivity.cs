using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using Android.Text.Style;

namespace FindMyPhoneSample
{
	[Activity (Label = "FindMyPhoneSample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		const int FIND_PHONE_NOTIFICATION_ID = 2;
		private static Notification.Builder notification;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var toggle_alarm_operation = new Intent (this, typeof(FindPhoneService));

			toggle_alarm_operation.SetAction(FindPhoneService.ACTION_TOGGLE_ALARM);
			var toggle_alarm_intent = PendingIntent.GetService (this, 0, toggle_alarm_operation, PendingIntentFlags.CancelCurrent);
			Android.App.Notification.Action alarm_action = new Android.App.Notification.Action (Resource.Drawable.alarm_action_icon, "", toggle_alarm_intent);
			var cancel_alarm_operation = new Intent (this, typeof(FindPhoneService));
			cancel_alarm_operation.SetAction (FindPhoneService.ACTION_CANCEL_ALARM);
			var cancel_alarm_intent = PendingIntent.GetService (this, 0, cancel_alarm_operation, PendingIntentFlags.CancelCurrent);
			var title = new SpannableString ("Find My Phone");
			title.SetSpan (new RelativeSizeSpan (0.85f), 0, title.Length(), SpanTypes.PointMark);
			notification = new Notification.Builder (this)
				.SetContentTitle (title)
				.SetContentText ("Tap to sound an alarm on phone")
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetVibrate (new long[]{ 0, 50 })
				.SetDeleteIntent (cancel_alarm_intent)
				.Extend (new Notification.WearableExtender ()
					.AddAction (alarm_action)
					.SetContentAction (0)
					.SetHintHideIcon (true))
				.SetLocalOnly (true)
				.SetPriority (NotificationPriority.Max);
			((NotificationManager)GetSystemService (NotificationService))
				.Notify (FIND_PHONE_NOTIFICATION_ID, notification.Build ());

			Finish ();
		}

		public static void UpdateNotification(Context context, string notificationText)
		{
			notification.SetContentText (notificationText);
			((NotificationManager)context.GetSystemService (NotificationService))
				.Notify (FIND_PHONE_NOTIFICATION_ID, notification.Build ());
		}
	}
}


