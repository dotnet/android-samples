using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace TvLeanback
{
	[BroadcastReceiver (Label = "BootupActivity", Enabled = true, Exported = false)]
	[IntentFilter (new[]{ "android.intent.action.BOOT_COMPLETED" })]
	public class BootupActivity : BroadcastReceiver
	{
		private const string TAG = "BootupActivity";
		private const long INITIAL_DELAY = 5000;

		public override void OnReceive (Context context, Intent intent)
		{
			Log.Debug (TAG, "BootupActivity initiated");
			if (intent.Action.EndsWith (Intent.ActionBootCompleted)) {
				scheduleRecommendationUpdate (context);
			}
		}

		private void scheduleRecommendationUpdate (Context context)
		{
			Log.Debug (TAG, "Scheduling recommendations update");

			var alarmManager = (AlarmManager)context.GetSystemService (Context.AlarmService);
			var recommendationIntent = new Intent (context, Java.Lang.Class.FromType (typeof(UpdateRecommendationsService)));
			var alarmIntent = PendingIntent.GetService (context, 0, recommendationIntent, 0);

			alarmManager.SetInexactRepeating (AlarmType.ElapsedRealtimeWakeup,
				INITIAL_DELAY,
				AlarmManager.IntervalHalfHour,
				alarmIntent);
		}

	}
}


