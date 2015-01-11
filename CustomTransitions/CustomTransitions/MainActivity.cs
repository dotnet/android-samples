using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

using CommonSampleLibrary;

namespace CustomTransitions
{
	[Activity (Label = "CustomTransition", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light")]
	public class MainActivity : SampleActivityBase
	{
		private bool log_shown;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var transaction = this.SupportFragmentManager.BeginTransaction ();
			CustomTransitionFragment fragment = new CustomTransitionFragment ();
			transaction.Replace (Resource.Id.sample_content_fragment, fragment);
			transaction.Commit ();

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			this.MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			var logToggle = menu.FindItem (Resource.Id.menu_toggle_log);
			logToggle.SetVisible (true);
			logToggle.SetTitle (log_shown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);
			return base.OnPrepareOptionsMenu (menu);

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				{
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
			}
			return base.OnOptionsItemSelected (item);
		}

		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework.
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView.
			var logFragment = (LogFragment)SupportFragmentManager.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");

		}


	}
}


