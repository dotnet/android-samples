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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Date Widgets/Inline")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class DateWidgets2 : Activity
	{
		// where we display the selected date and time
		private TextView mTimeDisplay;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.date_widgets_example_2);

			TimePicker timePicker = FindViewById <TimePicker> (Resource.Id.timePicker);
			timePicker.CurrentHour = (Java.Lang.Integer) 12;
			timePicker.CurrentMinute = (Java.Lang.Integer) 15;

			mTimeDisplay = FindViewById <TextView> (Resource.Id.dateDisplay);

			UpdateDisplay (12, 15);

			timePicker.TimeChanged += delegate(object sender, TimePicker.TimeChangedEventArgs e) {
				UpdateDisplay (e.HourOfDay, e.Minute);
			};
		}

		private void UpdateDisplay (int hourOfDay, int minute)
		{
			mTimeDisplay.Text = Pad (hourOfDay) + ":" + Pad (minute);
		}

		private static String Pad (int c)
		{
			if (c >= 10)
				return c.ToString ();
			else
				return "0" + c;
		}
	}
}

