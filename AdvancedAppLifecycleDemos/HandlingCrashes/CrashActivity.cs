using System;
using Android.App;
using AppLifecycle.AppLayer;
using Android.Widget;
using Android.Util;
using Android.OS;

namespace AppLifecycle
{
	[Activity (Label="Graceful Crash")]
	public class CrashActivity : ActivityBase
	{
		public CrashActivity ()
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug ("!!!!!! GracefulCrashActivity", "OnCreate.");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.CrashActivity);

			Button killProcessButton = FindViewById<Button> (Resource.Id.KillAppProcessButton);
			killProcessButton.Click += delegate {
				Log.Debug ("!!!!!! CrashActivity", "Killing Process");
				Process.KillProcess( Process.MyPid() );
			};
			
			Button throwExceptionButton = FindViewById<Button> (Resource.Id.ThrowExceptionButton);
			throwExceptionButton.Click += delegate {
				Log.Debug ("!!!!!! CrashActivity", "Throwing Exception");
				throw new Exception ("An ERRRRRRRRR!");
			};
		}

		protected override void FinishedInitializing () { }
	}
}

