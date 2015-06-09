using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using CommonSampleLibrary;

namespace ScreenCapture
{
	/*
	 * A simple launcher activity containing a summary sample description, sample log, and a custom
	 * Fragment which can display a View.
	 * 
	 * For devices with displays with a width of 720dp or greater, the sample log is always visibile,
	 * on other devices its visibility is controlled by an item on the action bar.
	 */ 
	[Activity (Label = "ScreenCapture", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : SampleActivityBase
	{

		public override string TAG {
			get {
				return "MainActivity";
			}
		}

		// Wheter the Log Fragment is shown currently
		private bool logShown;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

			if (bundle == null) {
				var transaction = FragmentManager.BeginTransaction ();
				var fragment = new ScreenCaptureFragment ();
				transaction.Replace (Resource.Id.sample_content_fragment, fragment);
				transaction.Commit ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			var logToggle = menu.FindItem (Resource.Id.menu_toggle_log);
			logToggle.SetVisible (FindViewById (Resource.Id.sample_output) is ViewAnimator);
			logToggle.SetTitle (logShown ? GetString (Resource.String.sample_hide_log) : GetString (Resource.String.sample_show_log));

			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				logShown = !logShown;
				var output = FindViewById<ViewAnimator> (Resource.Id.sample_output);
				if (logShown)
					output.DisplayedChild = 1;
				else
					output.DisplayedChild = 0;
				InvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		// Create a chain of targets that will receive log data
		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screem logging via a fragment with a TextView
			var logFragment = (LogFragment)FragmentManager
				.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");
		}
	}
}


