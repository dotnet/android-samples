using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Util;

namespace DirectBoot
{
	/**
 	* Class responsible for saving/retrieving alarms. This class uses SharedPreferences as storage.
 	*/
	public class AlarmStorage
	{
		static readonly string TAG = typeof(AlarmStorage).Name;
		static readonly string ALARM_PREFERENCES_NAME = "alarm_preferences";
		Random rand = new Random ();

		ISharedPreferences sharedPreferences;

		public AlarmStorage (Context context)
		{
			Context storageContext;
			// TODO Use Support BuildCompat or proper N Version code.
			if (Build.VERSION.SdkInt > BuildVersionCodes.M) {
				// All N devices have split storage areas, but we may need to
				// migrate existing preferences into the new device encrypted
				// storage area, which is where the data lives from now on.
				Context deviceContext = context.CreateDeviceEncryptedStorageContext ();
				if (!deviceContext.MigrateSharedPreferencesFrom (context, ALARM_PREFERENCES_NAME))
					Log.Warn (TAG, "Failed to migrate shared preferences.");

				storageContext = deviceContext;
			} else {
				storageContext = context;
			}
			sharedPreferences = storageContext.GetSharedPreferences (ALARM_PREFERENCES_NAME, FileCreationMode.Private);
		}

		/// <summary>
		/// Stores an alarm in the SharedPreferences.
		/// </summary>
		/// <returns>The saved alarm instance.</returns>
		/// <param name="year">Year.</param>
		/// <param name="month">Month.</param>
		/// <param name="day">Day.</param>
		/// <param name="hour">Hour.</param>
		/// <param name="minute">Minute.</param>
		public Alarm SaveAlarm (int year, int month, int day, int hour, int minute)
		{
			var alarm = new Alarm {
				// Ignore the Id duplication if that happens
				Id = rand.Next (),
				Year = year,
				Month = month,
				Day = day,
				Hour = hour,
				Minute = minute
			};

			ISharedPreferencesEditor editor = sharedPreferences.Edit ();
			editor.PutString (alarm.Id.ToString (),  alarm.ToJson ());
			editor.Apply ();
			return alarm;
		}

		/// <summary>
		/// Retrieves the alarms stored in the SharedPreferences.
		/// Timing corresponds linearly to the alarms count.
		/// </summary>
		/// <returns>A List of Alarms.</returns>
		public List<Alarm> GetAlarms ()
		{
			var alarms = new List<Alarm> ();
			foreach (string alarmJson in sharedPreferences.All.Values) {
				alarms.Add (Alarm.FromJson (alarmJson));
			}
			return alarms;
		}

		/// <summary>
		/// Delete the specified alarm instance from the SharedPreferences.
		/// This method iterates through the alarms stored in the SharedPreferences,
		/// timing corresponds linearly to the alarms count.
		/// </summary>
		/// <param name="toBeDeleted">To be deleted.</param>
		public void DeleteAlarm (Alarm toBeDeleted)
		{
			foreach (string alarmJson in sharedPreferences.All.Values) {
				var alarm = Alarm.FromJson (alarmJson);
				if (alarm.Id == toBeDeleted.Id) {
					ISharedPreferencesEditor editor = sharedPreferences.Edit ();
					editor.Remove (alarm.Id.ToString ());
					editor.Apply ();
					return;
				}
			}
		}
	}
}

