using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BasicManagedProfile
{
	//TODO remove all random comments in all dah files
	//TODO update the manifest file
	public class SetupProfileFragment : Fragment, View.IOnClickListener
	{
		const string ExtraDeviceAdmin = Android.App.Admin.DevicePolicyManager.ExtraDeviceAdmin;
		const string ActionProvisionManagedProfile = Android.App.Admin.DevicePolicyManager.ActionProvisionManagedProfile;
		const string ExtraProvisioningDefaultManagedProfileName = Android.App.Admin.DevicePolicyManager.ExtraProvisioningDefaultManagedProfileName;
		const string ExtraProvisioningDeviceAdminPackageName = Android.App.Admin.DevicePolicyManager.ExtraProvisioningDeviceAdminPackageName;

		public static SetupProfileFragment NewInstance ()
		{
			return new SetupProfileFragment ();
		}

		public SetupProfileFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_setup_profile, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			view.FindViewById (Resource.Id.set_up_profile).SetOnClickListener (this);
		}

		public void OnClick (View v)
		{
			switch (v.Id) {
			case Resource.Id.set_up_profile:
				ProvisionManagedProfile ();
				break;
			}
		}

		/**
	     * Initiates the managed profile provisioning. If we already have a managed profile set up on
	     * this device, we will get an error dialog in the following provisioning phase.
	     */
		private void ProvisionManagedProfile ()
		{
			var owner = this.Activity;
			if (null == owner) {
				return;
			}
			Intent intent = new Intent (ActionProvisionManagedProfile);
			intent.PutExtra (ExtraProvisioningDeviceAdminPackageName,
				owner.ApplicationContext.PackageName);
			intent.PutExtra (ExtraProvisioningDefaultManagedProfileName,
				"Sample Managed Profile");
			intent.PutExtra (ExtraDeviceAdmin, BasicDeviceAdminReceiver.GetComponentName (owner));
			if (intent.ResolveActivity (owner.PackageManager) != null) {
				StartActivity (intent); 
				owner.Finish ();
			} else {
				Toast.MakeText (owner, "Device provisioning is not enabled. Stopping.", 
					ToastLength.Short).Show ();
			}
		}
	}
}

