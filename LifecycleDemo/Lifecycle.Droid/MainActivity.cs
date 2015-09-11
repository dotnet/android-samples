using System;

using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.OS;

namespace Lifecycle.Droid
{
	[Activity (Label = "Android Lifecycle Demo", MainLauncher = true)]
	public class MainActivity : Activity
	{
		string logTag = "MainActivity";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug (logTag, "OnCreate called, App is becoming Active");
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.memory);

			// Pressing this button will launch another activity that 
			// uses up a LOT of memory
			button.Click += delegate {
				StartActivity (typeof(MemoryEaterActivity));
			};
		}

		protected override void OnStart()
		{
			Log.Debug (logTag, "OnStart called, App is Active");
			base.OnStart();
		}

		protected override void OnResume()
		{
			Log.Debug (logTag, "OnResume called, app is ready to interact with the user");
			base.OnResume();
		}
		
		protected override void OnPause()
		{
			Log.Debug (logTag, "OnPause called, App is moving to background");
			base.OnPause();
		}

		protected override void OnStop()
		{
			Log.Debug (logTag, "OnStop called, App is in the background");
			base.OnStop();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Log.Debug (logTag, "OnDestroy called, App is Terminating");
		}
	}
}


