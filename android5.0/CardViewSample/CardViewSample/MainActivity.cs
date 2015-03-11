using System;

using Android.App;
using Android.OS;

namespace CardViewSample
{
	[Activity (Label = "CardViewSample", MainLauncher = true, Icon = "@drawable/ic_launcher",Theme = "@style/AppTheme")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_card_view);
			if (bundle == null) {
				FragmentManager.BeginTransaction ()
					.Add (Resource.Id.container, CardViewFragment.NewInstance ())
					.Commit ();
			}
		}
	}
}


