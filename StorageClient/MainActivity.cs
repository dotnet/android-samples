using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CommonSampleLibrary;

namespace StorageClient
{
	/**
 	* A simple launcher activity containing a summary sample description
 	* and a few action bar buttons.
 	*/
	[Activity (Label = "StorageClient", MainLauncher = true,
		UiOptions = Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
	public class MainActivity : SampleActivityBase
	{
		public override string TAG {
			get { return "MainActivity";}
		}

		public static readonly string FRAGTAG = "StorageClientFragment";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
 			SetContentView (Resource.Layout.Main);

			if (SupportFragmentManager.FindFragmentByTag (FRAGTAG) == null ) {
				Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
				var fragment = new StorageClientFragment ();
				transaction.Add (fragment, FRAGTAG);
				transaction.Commit ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_menu, menu);
			return true;
		}

		/** Create a chain of targets that will receive log data */
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
			LogFragment logFragment = (LogFragment) SupportFragmentManager
				.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");
		}
	}
}


