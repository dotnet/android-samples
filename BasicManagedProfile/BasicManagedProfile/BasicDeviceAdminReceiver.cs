using System;

using Android.App.Admin;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;

namespace BasicManagedProfile
{
	[BroadcastReceiver (Label = "@string/app_name", Permission = "android.permission.BIND_DEVICE_ADMIN")]
	[IntentFilter (new[]{ "android.app.action.DEVICE_ADMIN_ENABLED" })]
	[MetaData ("android.app.device_admin", Resource = "@xml/basic_device_admin_receiver")]
	[Android.Runtime.Preserve]
	public class BasicDeviceAdminReceiver : DeviceAdminReceiver
	{
		/* TODO WTF loosing runtime here-ish is my guess
		 * How does it lose the runtime partway through the app...
		 */

		/**
	     * Called on the new profile when managed profile provisioning has completed. Managed profile
	     * provisioning is the process of setting up the device so that it has a separate profile which
	     * is managed by the mobile device management(mdm) application that triggered the provisioning.
	     * Note that the managed profile is not fully visible until it is enabled.
	     */
		public override void OnProfileProvisioningComplete (Context context, Intent intent)
		{
			// EnableProfileActivity is launched with the newly set up profile.
			Intent launch = new Intent (context, typeof(EnableProfileActivity)); //FIXME never reaches here
			launch.AddFlags (ActivityFlags.NewTask); 
			context.StartActivity (launch);
		}

		/**
	     * Generates a {@link ComponentName} that is used throughout the app.
	     * @return a {@link ComponentName}
	     */
		public static ComponentName GetComponentName (Context context)
		{
			return new ComponentName (context.ApplicationContext, 
				Java.Lang.Class.FromType (typeof(BasicDeviceAdminReceiver))); //Equivalent to BasicDeviceAdminReceiver.class
		}
	}
}