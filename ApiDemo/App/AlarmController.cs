using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo {

	[Activity (Label = "@string/activity_alarm_controller")]
	[IntentFilter (new[] { Intent.ActionMain },
			Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class AlarmController : Activity
	{
		Toast repeating;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.alarm_controller);

			FindViewById<Button>(Resource.Id.one_shot).Click += OneShotClick;

			FindViewById<Button>(Resource.Id.start_repeating).Click += StartRepeatingClick;

			FindViewById<Button>(Resource.Id.stop_repeating).Click += StopRepeatingClick;
		}

		void OneShotClick (object sender, EventArgs e)
		{
			var intent = new Intent (this, typeof (OneShotAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			var am = (AlarmManager) GetSystemService (AlarmService);
			am.Set (AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime () + 30*1000, source);

			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.one_shot_scheduled, ToastLength.Long);
			repeating.Show ();
		}

		void StartRepeatingClick (object sender, EventArgs e)
		{
			var intent = new Intent (this, typeof (RepeatingAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			var am = (AlarmManager) GetSystemService (AlarmService);
			am.SetRepeating (AlarmType.ElapsedRealtimeWakeup, 
					SystemClock.ElapsedRealtime () + 15*1000, 
					15*1000, 
					source);

			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.repeating_scheduled, ToastLength.Long);
			repeating.Show ();
		}

		void StopRepeatingClick (object sender, EventArgs e)
		{
			var intent = new Intent (this, typeof (RepeatingAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			var am = (AlarmManager) GetSystemService (AlarmService);
			am.Cancel (source);

			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.repeating_unscheduled, ToastLength.Long);
			repeating.Show ();
		}
	}
}

