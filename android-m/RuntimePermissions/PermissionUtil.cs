using System;
using Android.Content.PM;
using Android.OS;
using Android.App;

namespace RuntimePermissions
{
	/**
 	* Utility class that wraps access to the runtime permissions API in M and provides basic helper
 	* methods.
 	*/
	public abstract class PermissionUtil
	{
		/**
		* Check that all given permissions have been granted by verifying that each entry in the
		* given array is of the value Permission.Granted.
		*
		* See Activity#onRequestPermissionsResult (int, String[], int[])
		*/
		public static bool VerifyPermissions (int[] grantResults)
		{
			// Verify that each required permission has been granted, otherwise return false.
			foreach (Permission result in grantResults) {
				if (result != Permission.Granted) {
					return false;
				}
			}
			return true;
		}

		/**
     	* Returns true if the Activity has access to all given permissions.
     	* Always returns true on platforms below M.
     	*
     	* See Activity#checkSelfPermission (String)
     	*/
		public static bool HasSelfPermission (Activity activity, string[] permissions)
		{
			// Below Android M all permissions are granted at install time and are already available.
			if (!IsMNC)
				return true;
			
			// Verify that all required permissions have been granted
			foreach (string permission in permissions) 
				if (activity.CheckSelfPermission (permission) != (int)Permission.Granted) 
					return false;
			
			return true;
		}

		/**
     	* Returns true if the Activity has access to a given permission.
     	* Always returns true on platforms below M.
     	*
     	* @see Activity#checkSelfPermission(String)
     	*/
		public static bool HasSelfPermission (Activity activity, string permission)
		{
			// Below Android M all permissions are granted at install time and are already available.
			if (!IsMNC)
				return true;

			return activity.CheckSelfPermission (permission) == (int)Permission.Granted;
		}

		public static bool IsMNC {
			get {
				/*
         		TODO: In the Android M Preview release, checking if the platform is M is done through
         		the codename, not the version code. Once the API has been finalised, the following check
         		should be used: */

				// return Build.VERSION.SDK_INT == Build.VERSION_CODES.MNC
				return "MNC" == Build.VERSION.Codename;
			}
		}
	}
}

