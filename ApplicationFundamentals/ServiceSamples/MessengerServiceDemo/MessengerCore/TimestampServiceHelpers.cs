using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.Content;

namespace MessengerCore
{
	/// <summary>
	/// Various extension methods to help client apps consume the Timestamp service.
	/// </summary>
	public static class TimestampServiceHelpers
	{
		/// <summary>
		/// This will check to see if the current Context has been granted the custom permission to use the 
		/// Timestamp service.
		/// </summary>
		/// <returns><c>true</c>, if the current app has been granted permission to use the Timestamp Service, <c>false</c> otherwise.</returns>
		/// <param name="context">Context.</param>
		public static bool HasPermissionToRunTimestampService(this Context context)
		{
			Permission permissionCheck = ContextCompat.CheckSelfPermission(context, Constants.TIMESTAMP_SERVICE_PERMISSION);
			return permissionCheck == Permission.Granted;
		}

		/// <summary>
		/// Creates an explicit intent that can be used to start the Timestamp service.
		/// </summary>
		/// <returns>The intent to bind service.</returns>
		public static Intent CreateIntentToBindService(Context context)
		{
			if (IsTimestampServicePackageInstalled(context))
			{
				return CreateIntentForServiceInternal();
			}
			return null;
		}

		/// <summary>
		/// This method will create an explicit intent for starting the Timestamp service (or checking to see if it
		/// is installed).
		/// </summary>
		/// <returns></returns>
		static Intent CreateIntentForServiceInternal()
		{
			ComponentName cn = new ComponentName(Constants.REMOTE_SERVICE_PACKAGE_NAME, Constants.REMOTE_SERVICE_COMPONENT_NAME);
			Intent serviceToStart = new Intent();
			serviceToStart.SetComponent(cn);
			return serviceToStart;
		}

		/// <summary>
		/// Checks to see if the Timestamp service has been installed on the device.
		/// </summary>
		/// <returns><c>true</c>, if timestamp service package is installed, <c>false</c> otherwise.</returns>
		/// <param name="context">Context.</param>
		public static bool IsTimestampServicePackageInstalled(this Context context)
		{
			if (context == null)
			{
				throw new NullReferenceException("Must provide a valid Android context.");
			}

			Intent intent = CreateIntentForServiceInternal();
			IList<ResolveInfo> list = context.PackageManager.QueryIntentServices(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
			return list.Count > 0;
		}


	}
}
