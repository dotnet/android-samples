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
using AppLifecycle.AppLayer;

namespace AppLifecycle
{
	[Activity (Label = "Graceful IntermediateScreen")]			
	public class GracefulIntermediateScreen : ActivityBase
	{
		protected TextView appInitializationStatus;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug ("!!!!!! IntermediateScreen", "IntermediateScreen.OnCreate.");
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.IntermediateActivity);

			appInitializationStatus = FindViewById<TextView> (Resource.Id.AppInitializationStatus);
			this.UpdateAppInitStatus ();

			// get our button references
			Button launchCrashScreenButton = FindViewById<Button> (Resource.Id.LaunchCrashScreenButton);
			launchCrashScreenButton.Click += (object sender, EventArgs e) => {
				StartActivity ( typeof (CrashActivity) );
			};
		}

		protected void UpdateAppInitStatus()
		{
			appInitializationStatus.Text = "App Initialized: " + App.Current.IsInitialized;
		}

		protected override void FinishedInitializing () {
			Log.Debug ("!!!!!! IntermediateScreen", "IntermediateScreen.FinishedInitializing");
			this.UpdateAppInitStatus ();
		}
	}
}

