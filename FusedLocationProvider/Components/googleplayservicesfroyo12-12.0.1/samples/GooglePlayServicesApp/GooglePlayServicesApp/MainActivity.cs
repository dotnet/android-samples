using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Support.V4.App;

namespace GooglePlayServicesApp
{
	[Activity (Label = "GooglePlayServicesApp", MainLauncher = true)]
	public class Activity1 : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// add map fragment to frame layout
			var mapFragment = new SupportMapFragment ();
			var fragmentTx = this.SupportFragmentManager.BeginTransaction ();
			fragmentTx.Add (Resource.Id.linearLayout1, mapFragment);
			fragmentTx.Commit ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		}
	}
}


