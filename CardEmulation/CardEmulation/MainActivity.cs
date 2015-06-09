using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CommonSampleLibrary;

namespace CardEmulation
{
	[Activity (Label = "CardEmulation", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : SampleActivityBase
	{
		private bool mLogShown;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);

			if (bundle == null) {
				FragmentTransaction transaction = FragmentManager.BeginTransaction ();
				CardEmulationFragment fragment = new CardEmulationFragment ();
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
			IMenuItem logToggle = menu.FindItem (Resource.Id.menu_toggle_log);
			logToggle.SetVisible (FindViewById (Resource.Id.sample_output) is ViewAnimator);
			logToggle.SetTitle (mLogShown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);

			return base.OnPrepareOptionsMenu (menu);
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {

			case Resource.Id.menu_toggle_log:
				mLogShown = !mLogShown;
				ViewAnimator output = (ViewAnimator)FindViewById (Resource.Id.sample_output);
				if (mLogShown) {
					output.DisplayedChild = 1;
				} else {
					output.DisplayedChild = 0;
				}
				InvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework
			var logWrapper = new LogWrapper ();

			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView
			var logFragment = (LogFragment)FragmentManager.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");
		}
	}
}


