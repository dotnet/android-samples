using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.App.Admin;

namespace BasicManagedProfile
{
	[Activity (Label = "BasicManagedProfile", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main_real);
			if (bundle == null) {
				var manager = (DevicePolicyManager)GetSystemService (Context.DevicePolicyService);
				if (manager.IsProfileOwnerApp (this.PackageName)) {
					ShowMainFragment ();
				} else {
					// If not, we show the set up screen.
					ShowSetupProfile ();
				}
			}
		}

		private void ShowSetupProfile ()
		{
			FragmentManager.BeginTransaction ()
				.Replace (Resource.Id.container, SetupProfileFragment.NewInstance ())
				.Commit ();
		}

		private void ShowMainFragment ()
		{
			FragmentManager.BeginTransaction ().Add (Resource.Id.container, BasicManagedProfileFragment.NewInstance ()).Commit ();
		}
	}
}