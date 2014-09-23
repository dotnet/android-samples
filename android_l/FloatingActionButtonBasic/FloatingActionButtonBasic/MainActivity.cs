using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;

using CommonSampleLibrary;


namespace FloatingActionButtonBasic
{
	[Activity (Label = "FloatingActionButtonBasic", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : SampleActivityBase
	{
		public const string TAG = "MainActivity";

		// Whether the Log Fragment is currently shown
		private bool logShown;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

			Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
			var fragment = new FloatingActionButtonBasicFragment ();
			transaction.Replace (Resource.Id.sample_content_fragment, fragment);
			transaction.Commit ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			IMenuItem logToggle = menu.FindItem (Resource.Id.menu_toggle_log);
			logToggle.SetVisible(FindViewById(Resource.Id.sample_output) is ViewAnimator);
			logToggle.SetTitle (logShown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);

			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				logShown = !logShown;
				ViewAnimator output = (ViewAnimator)FindViewById (Resource.Id.sample_output);
				if (logShown)
					output.DisplayedChild = 1;
				else
					output.DisplayedChild = 0;
				SupportInvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		// Create a chain of targets that will recieve the log data
		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView
			var logFragment = (LogFragment)SupportFragmentManager
				.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "READY");
		}
	}
}


