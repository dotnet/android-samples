using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App.Admin;

namespace BasicManagedProfile
{
	[Activity (Label = "EnableProfileActivity")]			
	public class EnableProfileActivity : Activity, View.IOnClickListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle); //FIXME never reaches here which is not good
			if (null == bundle) {
				// Important: After the profile has been created, the MDM must enable it for corporate
				// apps to become visible in the launcher.
				EnableProfile ();
			}
			// This is just a friendly shortcut to the main screen.
			SetContentView (Resource.Layout.activity_setup);
			FindViewById (Resource.Id.icon).SetOnClickListener (this);
		}

		private void EnableProfile ()
		{
			DevicePolicyManager manager =
				(DevicePolicyManager)GetSystemService (Context.DevicePolicyService);
			// We enable the profile here.
			manager.SetProfileEnabled (BasicDeviceAdminReceiver.GetComponentName (this));
		}

		public void OnClick (View view)
		{
			switch (view.Id) {
			case Resource.Id.icon:
				{
					// Opens up the main screen
					StartActivity (new Intent (this, typeof(MainActivity)));
					Finish ();
					break;
				}
			}
		}
	}
}

