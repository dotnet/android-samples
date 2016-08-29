﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace DirectBoot
{
	[BroadcastReceiver (Exported = false, DirectBootAware = true)]
	[IntentFilter (new [] { "android.intent.action.BOOT_COMPLETED", "android.intent.action.LOCKED_BOOT_COMPLETED" })]
	public class BootBroadcastReceiver : BroadcastReceiver
	{
		static readonly string TAG = "BootBroadcastReceiver";

		public override void OnReceive (Context context, Intent intent)
		{
			bool bootCompleted;
			string action = intent.Action;
			//TODO Switch to UserManagerCompat, BuildCompat
			Log.Info (TAG, $"Recieved action {action}, user unlocked: "); //{UserManagerCompat.IsUserUnlocked (context))}");

			if (Build.VERSION.SdkInt > BuildVersionCodes.M)
				bootCompleted = Intent.ActionLockedBootCompleted == action;
			else
				bootCompleted = Intent.ActionBootCompleted == action;

			if (!bootCompleted)
				return;

			var util = new AlarmUtil (context);
			var alarmStorage = new AlarmStorage (context);
			foreach (Alarm alarm in alarmStorage.GetAlarms ())
				util.ScheduleAlarm (alarm);
		}
	}
}

