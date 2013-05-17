
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

namespace AppLifecycle
{
	[Activity (Label = "Non Graceful IntermediateScreen")]			
	public class NonGracefulIntermediateScreen : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug ("!!!!!! IntermediateScreen", "OnCreate.");
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.IntermediateActivity);
			
			TextView appInitializationStatus = FindViewById<TextView> (Resource.Id.AppInitializationStatus);
			appInitializationStatus.Text = "App Initialized: " + App.Current.IsInitialized;

			// get our button references
			Button launchCrashScreenButton = FindViewById<Button> (Resource.Id.LaunchCrashScreenButton);
			launchCrashScreenButton.Click += (object sender, EventArgs e) => {
				StartActivity ( typeof (CrashActivity) );
			};
		}
	}
}

