using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Google.Ads;

namespace AdMobTest
{
	[Activity (Label = "AdMobTest", MainLauncher = true)]
	public class MainActivity : Activity
	{
		AdView adView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			// Create an ad.
			adView = FindViewById<AdView> (Resource.Id.ad);
			
			// Create an ad request.
			AdRequest adRequest = new AdRequest ();
			adRequest.SetTesting (true);
			
			adRequest.AddTestDevice (AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);
			
			// Start loading the ad in the background.
			adView.LoadAd (adRequest);
		}
		
		protected override void OnDestroy ()
		{
			// Destroy the AdView.
			adView.Destroy();
			
			base.OnDestroy ();
		}
	}
}


