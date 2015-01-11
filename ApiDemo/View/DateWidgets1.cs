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
using Android.Widget;
using Android.Views;
using Java.Util;
using System.Text;

namespace MonoDroid.ApiDemo
{
	/**
 	* Basic example of using date and time widgets, including
 	* android.app.TimePickerDialog and android.widget.DatePicker.
 	*
 	* Also provides a good example of using Activity#onCreateDialog,
 	* Activity#onPrepareDialog and Activity#showDialog to have the
 	* activity automatically save and restore the state of the dialogs.
 	*/
	[Activity (Label = "Views/Date Widgets/Dialog")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class DateWidgets1 : Activity
	{
		// where we display the selected date and time
		TextView mDateDisplay;

		// date and time
		int mYear;
		int mMonth;
		int mDay;
		int mHour;
		int mMinute;

		const int TIME_12_DIALOG_ID = 0;
		const int TIME_24_DIALOG_ID = 1;
		const int DATE_DIALOG_ID = 2;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.date_widgets_example_1);

			mDateDisplay = FindViewById <TextView> (Resource.Id.dateDisplay);

			SetDialogOnClickListener (Resource.Id.pickDate, DATE_DIALOG_ID);
			SetDialogOnClickListener (Resource.Id.pickTime12, TIME_12_DIALOG_ID);
			SetDialogOnClickListener (Resource.Id.pickTime24, TIME_24_DIALOG_ID);

			Calendar c = Calendar.Instance;
			mYear = c.Get (CalendarField.Year);
			mMonth = c.Get(CalendarField.Month);
			mDay = c.Get (CalendarField.DayOfMonth);
			mHour = c.Get (CalendarField.HourOfDay);
			mMinute = c.Get (CalendarField.Minute);

			UpdateDisplay ();
		}

		void SetDialogOnClickListener (int buttonId, int dialogId)
		{
			var b = FindViewById <Button> (buttonId);
			b.Click += delegate {
				ShowDialog (dialogId);
			};
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case TIME_12_DIALOG_ID:
			case TIME_24_DIALOG_ID:
				return new TimePickerDialog (this, mTimeSetListener, mHour, mMinute, id == TIME_24_DIALOG_ID);

			case DATE_DIALOG_ID:
				return new DatePickerDialog (this, mDateSetListener, mYear, mMonth, mDay);
			}
			return null;
		}

		protected override void OnPrepareDialog (int id, Dialog dialog)
		{
			switch (id) {
			case TIME_12_DIALOG_ID:
			case TIME_24_DIALOG_ID:
				((TimePickerDialog) dialog).UpdateTime (mHour, mMinute);
				break;

			case DATE_DIALOG_ID:
				((DatePickerDialog) dialog).UpdateDate (mYear, mMonth, mDay);
				break;
			}		
		}

		void mTimeSetListener (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			mHour = e.HourOfDay;
			mMinute = e.Minute;
			UpdateDisplay ();
		}

		void mDateSetListener (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			mYear = e.Year;
			mMonth = e.MonthOfYear;
			mDay = e.DayOfMonth;
			UpdateDisplay ();
		}


		void UpdateDisplay ()
		{
			var sb = new StringBuilder ();
			// Month is 0 based so add 1
			sb.Append (mMonth + 1).Append ("-");
			sb.Append (mDay).Append ("-");
			sb.Append (mYear).Append (" ");
			sb.Append (Pad (mHour)).Append (":");
			sb.Append (Pad (mMinute));

			mDateDisplay.Text = sb.ToString ();
		}

		static string Pad (int c)
		{
			if (c >= 10)
				return c.ToString ();
			else
				return "0" + c;
		}
	}
}