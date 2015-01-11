using System;
using Android.Content;
using Android.OS;
using Android.Preferences;

namespace JumpingJack
{
	public class Utils
	{
		const int DEFAULT_VIBRATUON_DURATION_MS = 200;
		const string PREF_KEY_COUNTER = "counter";

		//causes the device to vibrate for the given duration
		public static void Vibrate(Context context, int duration) 
		{
			if (duration == 0)
				duration = DEFAULT_VIBRATUON_DURATION_MS;

			Vibrator v = (Vibrator)context.GetSystemService (Context.VibratorService);
			v.Vibrate (duration);
		}

		public static void SaveCounterToPreference(Context context, int value)
		{
			ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences (context);
			if (value < 0) {
				//we want to remove
				pref.Edit ().Remove (PREF_KEY_COUNTER).Apply ();
			} else {
				pref.Edit ().PutInt (PREF_KEY_COUNTER, value).Apply ();
			}
		}

		public static int GetCounterFromPreference(Context context)
		{
			ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences (context);
			return pref.GetInt (PREF_KEY_COUNTER, 0);
		}
	}
}

