/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Push;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.UrbanAirship.Utils;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
	 * An empty activity used for the home.
	 *
	 * If activity is started with an intent that has a message id under the key
	 * <code>RichPushApplication.MESSAGE_ID_RECEIVED_KEY</code> it will display
	 * the message in a dialog fragment.
	 *
	 */
	[Android.App.Activity (Label = "RichPushSample", MainLauncher = true)]
	public class MainActivity : SherlockFragmentActivity, ActionBar.IOnNavigationListener
	{
		protected const string TAG = "MainActivity";
		const string ALIAS_KEY = "com.xamarin.samples.urbanairship.richpush.ALIAS";
		const int aliasType = 1;
		ArrayAdapter<String> navAdapter;
		RichPushUser user;
		string pendingMessageId = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.main);
			this.ConfigureActionBar ();

			this.user = RichPushManager.Shared ().RichPushUser;

			// If we have a message id and its the first create, set the pending message id if available
			if (savedInstanceState == null) {
				pendingMessageId = Intent.GetStringExtra (RichPushApplication.MESSAGE_ID_RECEIVED_KEY);
			}
		}

		protected override void OnNewIntent (Intent intent)
		{
			pendingMessageId = intent.GetStringExtra (RichPushApplication.MESSAGE_ID_RECEIVED_KEY);
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStarted (this);
		}

		protected override void OnStop ()
		{
			base.OnStop ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStopped (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			SetNavigationToMainActivity ();

			// Show a message dialog if the pending message id is not null
			if (!UAStringUtil.IsEmpty (pendingMessageId)) {
				ShowRichPushMessage (pendingMessageId);
				pendingMessageId = null;
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			this.SupportMenuInflater.Inflate (Resource.Menu.main_menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.preferences:
				this.StartActivity (new Intent (this, typeof(PushPreferencesActivity)));
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		public bool OnNavigationItemSelected (int itemPosition, long itemId)
		{
			string navName = (string) this.navAdapter.GetItem (itemPosition);
			if (RichPushApplication.HOME_ACTIVITY == navName) {
				// do nothing, we're here
			} else if (RichPushApplication.INBOX_ACTIVITY == navName) {
				Intent intent = new Intent (this, typeof(InboxActivity));
				intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.SingleTop);
				this.StartActivity (intent);
			}

			return true;
		}

		/**
	     * Displays the rich push message in a RichPushMessageDialogFragment
	     * @param messageId The specified message id
	     */
		private void ShowRichPushMessage (string messageId)
		{
			RichPushMessageDialogFragment message = RichPushMessageDialogFragment.NewInstance (messageId);
			message.Show (this.SupportFragmentManager, "message");
		}

		/**
	     * Configures the action bar to have a navigation list of
	     * 'Home' and 'Inbox'
	     */
		private void ConfigureActionBar ()
		{
			ActionBar actionBar = this.SupportActionBar;
			actionBar.SetDisplayUseLogoEnabled (true);
			actionBar.SetDisplayShowTitleEnabled (false);
			// FIXME: compiler bug: it resolves unqualified "ActionBar" as Android.App.ActionBar.
			actionBar.NavigationMode = Xamarin.ActionbarSherlockBinding.App.ActionBar.NavigationModeList;

			this.navAdapter = new ArrayAdapter<String> (this, Resource.Layout.sherlock_spinner_dropdown_item,
			                                           RichPushApplication.navList);
			actionBar.SetListNavigationCallbacks (this.navAdapter, this);
		}

		/**
	     * Sets the action bar navigation to show 'Home'
	     */
		private void SetNavigationToMainActivity ()
		{
			int position = this.navAdapter.GetPosition ("Home");
			SupportActionBar.SetSelectedNavigationItem (position);
		}
	}
}