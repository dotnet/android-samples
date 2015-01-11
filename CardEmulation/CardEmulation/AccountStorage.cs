using System;
using Android.Content;
using CommonSampleLibrary;
using Android.Preferences;

namespace CardEmulation
{
	public static class AccountStorage
	{
		private const String PREF_ACCOUNT_NUMBER = "account_number";
		private const String DEFAULT_ACCOUNT_NUMBER = "00000000";
		private const String TAG = "AccountStorage";
		private static String sAccount = null;
		private static readonly Object sAccountLock = new Object();

		public static void SetAccount(Context c, String s)
		{
			lock (sAccountLock) {
				Log.Info (TAG, "Setting account number: " + s);
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (c);
				prefs.Edit ().PutString (PREF_ACCOUNT_NUMBER, s).Commit();
				sAccount = s;
			}
		}
		public static string GetAccount(Context c)
		{
			lock (sAccountLock) {
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
				String account = prefs.GetString(PREF_ACCOUNT_NUMBER, DEFAULT_ACCOUNT_NUMBER);
				sAccount = account;
			}
			return sAccount;
		}
	}
}

