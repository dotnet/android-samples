/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	/**
 	* Example of scheduling one-shot and repeating alarms.  See
 	* {@link OneShotAlarm} for the code run when the one-shot alarm goes off, and
 	* {@link RepeatingAlarm} for the code run when the repeating alarm goes off.
 	* <h4>Demo</h4>
 	*/
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
			// When the alarm goes off, we want to broadcast an Intent to our
			// BroadcastReceiver.  Here we make an Intent with an explicit class
			// name to have our own receiver (which has been published in
			// AndroidManifest.xml) instantiated and called, and then create an
			// IntentSender to have the intent executed as a broadcast.
			var intent = new Intent (this, typeof (OneShotAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			// Schedule the alarm for 30 seconds from now!
			var am = (AlarmManager) GetSystemService (AlarmService);
			am.Set (AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime () + 30*1000, source);

			// Tell the user about what we did.
			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.one_shot_scheduled, ToastLength.Long);
			repeating.Show ();
		}

		void StartRepeatingClick (object sender, EventArgs e)
		{
			// When the alarm goes off, we want to broadcast an Intent to our
			// BroadcastReceiver.  Here we make an Intent with an explicit class
			// name to have our own receiver (which has been published in
			// AndroidManifest.xml) instantiated and called, and then create an
			// IntentSender to have the intent executed as a broadcast.
			// Note that unlike above, this IntentSender is configured to
			// allow itself to be sent multiple times.
			var intent = new Intent (this, typeof (RepeatingAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			// Schedule the alarm!
			var am = (AlarmManager) GetSystemService (AlarmService);
			am.SetRepeating (AlarmType.ElapsedRealtimeWakeup, 
					SystemClock.ElapsedRealtime () + 15*1000, 
					15*1000, 
					source);

			// Tell the user about what we did.
			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.repeating_scheduled, ToastLength.Long);
			repeating.Show ();
		}

		void StopRepeatingClick (object sender, EventArgs e)
		{
			// Create the same intent, and thus a matching IntentSender, for
			// the one that was scheduled.
			var intent = new Intent (this, typeof (RepeatingAlarm));
			var source = PendingIntent.GetBroadcast (this, 0, intent, 0);

			// And cancel the alarm.
			var am = (AlarmManager) GetSystemService (AlarmService);
			am.Cancel (source);

			// Tell the user about what we did.
			if (repeating != null)
				repeating.Cancel ();
			repeating = Toast.MakeText (this, Resource.String.repeating_unscheduled, ToastLength.Long);
			repeating.Show ();
		}
	}
}

