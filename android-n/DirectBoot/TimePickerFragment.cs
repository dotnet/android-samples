using System;

using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace DirectBoot
{
	/**
 	* DialogFragment for showing a TimePicker.
 	*/
	public class TimePickerFragment : DialogFragment
	{
		TimePicker timePicker;
		AlarmStorage alarmStorage;
		IAlarmAddListener alarmAddListener;
		AlarmUtil alarmUtil;

		public static TimePickerFragment NewInstance ()
		{
			return new TimePickerFragment ();
		}

		public void SetAlarmAddListener (IAlarmAddListener listener)
		{
			alarmAddListener = listener;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetStyle (DialogFragment.StyleNormal, Android.Resource.Style.ThemeMaterialLightDialog);
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			alarmStorage = new AlarmStorage (Activity);
			alarmUtil = new AlarmUtil (Activity);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate (Resource.Layout.fragment_time_picker, container, false);
			timePicker = (TimePicker)view.FindViewById (Resource.Id.time_picker_alarm);

			var buttonOk = (Button)view.FindViewById (Resource.Id.button_ok_time_picker);
			buttonOk.Click += delegate {
				DateTime alarmTime = alarmUtil.GetNextAlarmTime (timePicker.Hour, timePicker.Minute);
				Alarm alarm = alarmStorage.SaveAlarm (alarmTime.Year, alarmTime.Month, alarmTime.Day,
													  alarmTime.Hour, alarmTime.Minute);
				string alarmSavedString = Activity.GetString (Resource.String.alarm_saved, alarm.Hour, alarm.Minute);
				Toast.MakeText (Activity, alarmSavedString, ToastLength.Short).Show ();

				if (alarmAddListener != null)
					alarmAddListener.OnAlarmAdded (alarm);

				Dismiss ();
			};

			var buttonCancel = (Button)view.FindViewById (Resource.Id.button_cancel_time_picker);
			buttonCancel.Click += delegate {
				Dismiss ();
			};

			return view;
		}
	}

	public interface IAlarmAddListener
	{
		void OnAlarmAdded (Alarm alarm);
	}
}

