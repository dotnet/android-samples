using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

using CommonSampleLibrary;

namespace ActiveNotifications
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
	public class MainActivity : SampleActivityBase
	{
		public ActiveNotificationsFragment fragment;
		public static readonly string ACTION_NOTIFICATION_DELETE = "com.xamarin.activenotifications.delete";

		bool logShown;
		ANBroadcastReceiver deleteReceiver;

		public override string TAG {
			get {
				return "MainActivity";
			}
		} 

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
			deleteReceiver = new ANBroadcastReceiver (this);

			if (bundle == null) {
				FragmentTransaction transaction = FragmentManager.BeginTransaction ();
				transaction.Replace (Resource.Id.sample_content_fragment, new ActiveNotificationsFragment ());
				transaction.Commit ();
			}
		}

		public override void OnCreate (Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate (savedInstanceState, persistentState);

			FindFragment ();
			fragment.UpdateNumberOfNotifications ();
		}

		public void FindFragment ()
		{
			fragment = (ActiveNotificationsFragment)FragmentManager
				.FindFragmentById (Resource.Id.sample_content_fragment);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			RegisterReceiver (deleteReceiver, new IntentFilter (ACTION_NOTIFICATION_DELETE));
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			UnregisterReceiver (deleteReceiver);
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
			logToggle.SetTitle (logShown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);

			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				logShown = !logShown;
				var output = FindViewById <ViewAnimator> (Resource.Id.sample_output);

				output.DisplayedChild = logShown ? 1 : 0;

				InvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		/** Create a chain of targets that will receive log data */
		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework.
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates Android.Util.Log method signatures.
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView
			var logFragment = (LogFragment)FragmentManager.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");
		}
	}

	public class ANBroadcastReceiver : BroadcastReceiver
	{
		readonly MainActivity self;


		public ANBroadcastReceiver (MainActivity self)
		{
			this.self = self;
		}

		public override void OnReceive (Context context, Intent intent)
		{
			if (self.fragment == null)
				self.FindFragment ();

			self.fragment.UpdateNumberOfNotifications ();
		}
	}

}


