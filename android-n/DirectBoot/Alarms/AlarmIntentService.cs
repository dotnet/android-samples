using System.Linq;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;

namespace DirectBoot
{
	/**
 	* IntentService to set off an alarm.
	*/
	[Service (Name = "com.xamarin.directboot.AlarmIntentService")]
	public class AlarmIntentService : IntentService
	{
		public static readonly string ALARM_WENT_OFF_ACTION = (typeof(AlarmIntentService)).Name + ".ALARM_WENT_OFF";
		public static readonly string ALARM_KEY = "alarm_instance";

		protected override void OnHandleIntent (Android.Content.Intent intent)
		{
			Context context = ApplicationContext;
			var alarm = (Alarm)intent.GetParcelableExtra (ALARM_KEY);
			var alarmStorage = new AlarmStorage (context);

			// HACK - workaround https://github.com/googlesamples/android-DirectBoot/issues/4
			if (alarm == null)
				alarm = alarmStorage.GetAlarms ().First (a => a.Minute == System.DateTime.Now.Minute);

			var manager = context.GetSystemService (Context.NotificationService).JavaCast<NotificationManager> ();
			var builder = new NotificationCompat.Builder (context)
												.SetSmallIcon (Resource.Drawable.ic_fbe_notification)
												.SetCategory (Notification.CategoryAlarm)
												.SetSound (Settings.System.DefaultAlarmAlertUri)
												.SetContentTitle (context.GetString (Resource.String.alarm_went_off, alarm.Hour, alarm.Minute));

			manager.Notify (alarm.Id, builder.Build ());
			alarmStorage.DeleteAlarm (alarm);

			var wentoffIntent = new Intent (ALARM_WENT_OFF_ACTION);
			wentoffIntent.PutExtra (ALARM_KEY, alarm);
			LocalBroadcastManager.GetInstance (context).SendBroadcast (wentoffIntent);
		}
	}
}

