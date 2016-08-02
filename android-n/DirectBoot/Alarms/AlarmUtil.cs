using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;

namespace DirectBoot
{
	public class AlarmUtil
	{
		static readonly string TAG = "AlarmUtil";
		Context context;
		AlarmManager alarmManager;

		public AlarmUtil (Context context)
		{
			this.context = context;
			alarmManager = context.GetSystemService (Context.AlarmService).JavaCast<AlarmManager> ();
		}

		/// <summary>
		/// Schedules an alarm.
		/// </summary>
		/// <param name="alarm">The alarm to be scheduled.</param>
		public void ScheduleAlarm (Alarm alarm)
		{
			var intent = new Intent (context, typeof (AlarmIntentService));
			// intent.PutExtra (AlarmIntentService.ALARM_KEY, alarm);
			// TODO - workaround https://github.com/googlesamples/android-DirectBoot/issues/4
			intent.PutExtra ("id", alarm.Id);
			intent.PutExtra ("year", alarm.Year);
			intent.PutExtra ("month", alarm.Month);
			intent.PutExtra ("day", alarm.Day);
			intent.PutExtra ("hour", alarm.Hour);
			intent.PutExtra ("minute", alarm.Minute);
			var pendingIntent = PendingIntent.GetService (context, alarm.Id, intent, PendingIntentFlags.UpdateCurrent);
			var triggerOffset = new DateTimeOffset (alarm.GetTriggerTime ());
			var alarmClockInfo = new AlarmManager.AlarmClockInfo (triggerOffset.ToUnixTimeMilliseconds (), pendingIntent);
			alarmManager.SetAlarmClock (alarmClockInfo, pendingIntent);
			Log.Info (TAG, $"Alarm scheduled at {alarm.Hour}:{alarm.Minute} {alarm.Year}-{alarm.Month}-{alarm.Day}");
		}

		/// <summary>
		/// Cancels the scheduled alarm.
		/// </summary>
		/// <returns>The alarm to be canceled.</returns>
		/// <param name="alarm">Alarm.</param>
		public void CancelAlarm (Alarm alarm)
		{
			var intent = new Intent (context, typeof (AlarmIntentService));
			intent.PutExtra (AlarmIntentService.ALARM_KEY, alarm);
			var pendingIntent = PendingIntent.GetService (context, alarm.Id, intent, PendingIntentFlags.UpdateCurrent);
			alarmManager.Cancel (pendingIntent);
		}

		/// <summary>
		/// Returns the alarm time closest to the hour and minute specified.
		/// </summary>
		/// <returns>The time the alarm should trigger.</returns>
		/// <param name="hour">The hour the alarm should trigger.</param>
		/// <param name="minute">the minute the alarm should trigger.</param>
		public DateTime GetNextAlarmTime (int hour, int minute)
		{
			var now = DateTime.Now;
			var alarmTime = new DateTime (now.Year, now.Month, now.Day, hour, minute, 0);
			if (alarmTime < now)
				alarmTime = alarmTime.AddDays (1);

			return alarmTime;
		}
	}
}

