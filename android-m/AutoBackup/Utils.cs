using System;
using Android.Util;

namespace AutoBackup
{
	public class Utils
	{
		static readonly String TAG = "AutoBackupSample";

		public static bool IsExternalStorageAvailable () 
		{
			string state = Android.OS.Environment.ExternalStorageState;
			if (Android.OS.Environment.MediaMounted != state) {
				Log.Debug (TAG, "The external storage is not available.");
				return false;
			}
			return true;
		}
	}
}

