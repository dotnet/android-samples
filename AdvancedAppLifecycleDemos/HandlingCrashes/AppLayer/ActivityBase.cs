using System;
using Android.App;
using AppLifecycle;
using Android.Util;

namespace AppLifecycle.AppLayer
{
	public abstract class ActivityBase : Activity
	{
		private readonly string logTag = "!!!!!! ActivityBase";
		ProgressDialog progress;
		protected EventHandler<EventArgs> initializedEventHandler;

		protected override void OnResume ()
		{
			base.OnResume ();

			Log.Debug (logTag, "ActivityBase.OnCreate.");

			if (!App.Current.IsInitialized){
				Log.Debug(logTag, "ActivityBase.App is NOT initialized");

				// if we're to restart on the main activty, and this isn't it, do so
				if (App.Current.RestartMainActivityOnCrash && (this.GetType() != typeof(MainActivity))){
					//
					Log.Debug (logTag, "ActivityBase.Not MainActivity, heading home");
					this.RestartApp();
					return;
				} else {
					// show the loading overlay on the UI thread
					progress = ProgressDialog.Show(this, "Loading", "Please Wait...", true); 
					
					// when the app has initialized, hide the progress bar and call Finished Initialzing
					initializedEventHandler = (s, e) => {
						// call finished initializing so that any derived activities have a chance to do work
						RunOnUiThread( () => {
							this.FinishedInitializing();
							// hide the progress bar
							if (progress != null)
								progress.Dismiss();
						});
					};
					App.Current.Initialized += initializedEventHandler;
					
				}
			} else {
				Log.Debug(logTag, "ActivityBase.App is initialized");
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			
			// in the case of rotation before the app is fully intialized, we have
			// to remove our intialized event handler, and dismiss the progres. otherwise
			// we'll get multiple Initialized handler calls and a window leak.
			if (this.initializedEventHandler != null)
				App.Current.Initialized -= initializedEventHandler;
			if (progress != null)
				progress.Dismiss ();
		}

		protected void RestartApp ()
		{
			Log.Debug (logTag, "ActivityBase.RestartApp");
			// start our first activity
			StartActivity (typeof(MainActivity));
			// kill this activity
			base.Finish ();
		}

		/// <summary>
		/// Override this method to perform tasks after the app class has fully initialized
		/// </summary>
		protected abstract void FinishedInitializing ();
	}
}

