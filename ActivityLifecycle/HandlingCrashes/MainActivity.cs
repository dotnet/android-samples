using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using AppLifecycle.AppLayer;

namespace AppLifecycle
{
	[Activity (Label = "Crash!", MainLauncher = true)]
	public class MainActivity : ActivityBase
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// initialize our app (will also start our service).
			Log.Debug ("!!!!!! MainActivity", "MainActivity.OnCreate Called");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// get our button references
			Button launchNonGracefulIntermediateScreenButton = FindViewById<Button> (Resource.Id.LaunchNonGracefulIntermediateScreenButton);
			launchNonGracefulIntermediateScreenButton.Click += (object sender, EventArgs e) => {
				StartActivity ( typeof (NonGracefulIntermediateScreen) );
			};
			Button launchGracefulIntermediateScreenButton = FindViewById<Button> (Resource.Id.LaunchGracefulIntermediateScreenButton);
			launchGracefulIntermediateScreenButton.Click += (object sender, EventArgs e) => {
				StartActivity ( typeof (GracefulIntermediateScreen) );
			};
		}

		protected override void FinishedInitializing ()
		{
			Log.Debug ("!!!!!!! MainActivity", "MainActivity.FinishedInitializing Called");
		}
	}
}


