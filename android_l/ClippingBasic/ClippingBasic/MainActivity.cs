using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using CommonSampleLibrary;

namespace ClippingBasic
{
	[Activity (Label = "ClippingBasic", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme = "@android:style/Theme.Material.Light")]
	public class MainActivity : SampleActivityBase
	{
		const string TAG = "MainActivity";

		// Whether the log fragment is shown or not
		private bool log_shown;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.activity_main);

			Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
			ClippingBasicFragment fragment = new ClippingBasicFragment ();
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
			logToggle.SetVisible (FindViewById (Resource.Id.sample_output) is ViewAnimator);
			logToggle.SetTitle (log_shown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);
			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				log_shown = !log_shown;
				ViewAnimator output = (ViewAnimator)FindViewById (Resource.Id.sample_output);
				if (log_shown) {
					output.DisplayedChild = 1;
				} else {
					output.DisplayedChild = 0;
				}
				SupportInvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		// Create a chain of targets that will receive log data
		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework
			LogWrapper logWrapper = new LogWrapper ();

			// Using Log, front-end to the logging chain, emulates android.util.log method signatures
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			MessageOnlyLogFilter msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView
			LogFragment logFragment = (LogFragment)SupportFragmentManager
				.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;
			Log.Info (TAG, "Ready");
		}
	}
}


