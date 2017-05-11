using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

namespace Timer
{
	[Service(Exported = true)]
	public class TimerNotificationService : IntentService
	{
		public static string Tag = "TimerNotificationSvc";

		public TimerNotificationService () : base (Tag)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
		}

		protected override void OnHandleIntent (Intent intent)
		{
			if (Log.IsLoggable (Tag, LogPriority.Debug))
				Log.Debug (Tag, "OnHandleIntent called with intent: " + intent);

			switch (intent.Action) {
			case Constants.ACTION_SHOW_ALARM:
				ShowTimerDoneNotification ();
				break;
			case Constants.ACTION_DELETE_ALARM:
				DeleteTimer ();
				break;
			case Constants.ACTION_RESTART_ALARM:
				RestartAlarm ();
				break;
			default:
				throw new InvalidOperationException ("Undefined constant used: " + intent.Action);
			}
		}

		private void RestartAlarm ()
		{
			var dialogIntent = new Intent (this, typeof(SetTimerActivity));
			dialogIntent.AddFlags (ActivityFlags.NewTask);
			StartActivity (dialogIntent);
			if (Log.IsLoggable (Tag, LogPriority.Debug))
				Log.Debug (Tag, "Timer restarted.");
		}

		private void DeleteTimer ()
		{
			CancelCountdownNotification ();
			var alarm = (AlarmManager)GetSystemService (AlarmService);
			var intent = new Intent (Constants.ACTION_SHOW_ALARM, null, this,
				                typeof(TimerNotificationService));
			var pendingIntent = PendingIntent.GetService (this, 0, intent,
				                              PendingIntentFlags.UpdateCurrent);
			alarm.Cancel (pendingIntent);

			if (Log.IsLoggable (Tag, LogPriority.Debug)) {
				Log.Debug (Tag, "Timer deleted.");
			}
		}

		private void CancelCountdownNotification ()
		{
			var notifyMgr = (NotificationManager)GetSystemService (NotificationService);
			notifyMgr.Cancel (Constants.NOTIFICATION_TIMER_COUNTDOWN);
		}

		private void ShowTimerDoneNotification ()
		{
			// Cancel the countdown notification to show the "timer done" notification.
			CancelCountdownNotification ();

			// Create an intent to restart a timer.
			var restartIntent = new Intent (Constants.ACTION_RESTART_ALARM, null, this,
				                       typeof(TimerNotificationService));
			var pendingIntentRestart = PendingIntent
				.GetService (this, 0, restartIntent, PendingIntentFlags.UpdateCurrent);

			// Create notification that timer has expired.
			var notifyMgr = (NotificationManager)GetSystemService(NotificationService);
			Notification notif = new Notification.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_cc_alarm)
				.SetContentTitle (GetString (Resource.String.timer_done))
				.SetContentText (GetString (Resource.String.timer_done))
				.SetUsesChronometer (true)
				.SetWhen (Java.Lang.JavaSystem.CurrentTimeMillis ())
				.AddAction (Resource.Drawable.ic_cc_alarm, GetString (Resource.String.timer_restart),
				                     pendingIntentRestart)
				.SetLocalOnly (true)
				.Build ();
			notifyMgr.Notify (Constants.NOTIFICATION_TIMER_EXPIRED, notif);
		}
	}
}

