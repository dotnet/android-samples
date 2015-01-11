/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
//using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using System.Collections.Generic;
using Android.Graphics;
using Xamarin.ActionbarSherlockBinding.App;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;
using Xamarin.UrbanAirship.Utils;
using Android.Support.V4.Widget;
using Android.Support.V4.App;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
	 * Activity that manages the inbox.
	 * On a tablet it also manages the message view pager.
	 */
	[Android.App.Activity (Label = "InboxActivity")]			
	public class InboxActivity : SherlockFragmentActivity,
		InboxFragment.IOnMessageListener,
		ActionBar.IOnNavigationListener,
		ActionMode.ICallback,
		RichPushManager.IListener,
		RichPushInbox.IListener,
		SlidingPaneLayout.IPanelSlideListener
	{

		const string CHECKED_IDS_KEY = "com.xamarin.samples.urbanairship.richpush.CHECKED_IDS";
		const string MESSAGE_ID_KEY = "com.xamarin.samples.urbanairship.richpush.FIRST_MESSAGE_ID";
		private ActionMode actionMode;
		private ArrayAdapter<String> navAdapter;
		private CustomViewPager messagePager;
		private InboxFragment inbox;
		private RichPushInbox richPushInbox;
		private ActionBar actionBar;
		private string pendingMessageId;
		private IList<RichPushMessage> messages;
		private CustomSlidingPaneLayout slidingPaneLayout;
		private Button actionSelectionButton;

		class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			public GlobalLayoutListener (System.Action onGlobalLayout)
			{
				this.onGlobalLayout = onGlobalLayout;
			}

			System.Action onGlobalLayout;

			public void OnGlobalLayout ()
			{
				onGlobalLayout ();
			}
		}

		override
			protected void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.inbox);

			actionBar = SupportActionBar;
			ConfigureActionBar ();

			this.richPushInbox = RichPushManager.Shared ().RichPushUser.Inbox;

			// Set up the inbox fragment
			this.inbox = (InboxFragment)this.SupportFragmentManager.FindFragmentById (Resource.Id.inbox);
			this.inbox.ListView.ChoiceMode = ChoiceMode.Single;
			this.inbox.ListView.SetBackgroundColor (Color.Black);

			// Set up the message view pager if it exists
			this.messagePager = (CustomViewPager)this.FindViewById (Resource.Id.message_pager);
			if (messagePager != null) {
				messagePager.Adapter = new MessageFragmentAdapter (this.SupportFragmentManager);
				this.messagePager.PageSelected += (sender, e) => {
					int position = e.P0;
					messages [position].MarkRead ();
					// Highlight the current item you are viewing in the inbox
					inbox.ListView.SetItemChecked (position, true);

					// If we are in actionMode, update the menu items
					if (actionMode != null) {
						actionMode.Invalidate ();
					}
				};
			}

			slidingPaneLayout = (CustomSlidingPaneLayout)FindViewById (Resource.Id.sliding_pane_layout);
			if (slidingPaneLayout != null) {
				slidingPaneLayout.SetPanelSlideListener (this);
				slidingPaneLayout.OpenPane ();

				GlobalLayoutListener l = null;
				l = new GlobalLayoutListener (() => {
					// If sliding pane layout is slidable, set the actionbar to have an up action
					actionBar.SetDisplayHomeAsUpEnabled (slidingPaneLayout.IsSlideable);
					slidingPaneLayout.ViewTreeObserver.RemoveGlobalOnLayoutListener (l);
				});
				slidingPaneLayout.ViewTreeObserver.AddOnGlobalLayoutListener (l);
			}

			// First create, try to show any messages from the intent
			if (savedInstanceState == null) {
				this.SetPendingMessageIdFromIntent (Intent);
			}
		}

		override
			protected void OnNewIntent (Intent intent)
		{
			this.SetPendingMessageIdFromIntent (intent);
		}

		override
			protected void OnStart ()
		{
			base.OnStart ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStarted (this);
		}

		override
			protected void OnResume ()
		{
			base.OnResume ();

			// Set the navigation to show Inbox
			SetNavigationToInboxActivity ();

			// Listen for any rich push message changes
			RichPushManager.Shared ().AddListener (this);
			RichPushManager.Shared ().RichPushUser.Inbox.AddListener (this);

			// Update the rich push messages to the latest
			UpdateRichPushMessages ();

			// Show any pending message ids from the intent
			ShowPendingMessageId ();

			StartActionModeIfNecessary ();

			// Dismiss any notifications if available
			RichNotificationBuilder.DismissInboxNotification ();
		}

		override
			protected void OnPause ()
		{
			base.OnPause ();

			// Remove listener for message changes
			RichPushManager.Shared ().RemoveListener (this);
			richPushInbox.RemoveListener (this);

			RichPushWidgetUtils.RefreshWidget (this);

		}

		override
			protected void OnStop ()
		{
			base.OnStop ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStopped (this);
		}

		public void OnMessageOpen (RichPushMessage message)
		{
			message.MarkRead ();
			ShowMessage (message.MessageId);

			// If we are in actionMode, update the menu items
			if (actionMode != null) {
				actionMode.Invalidate ();
			}
		}

		public void OnSelectionChanged ()
		{
			StartActionModeIfNecessary ();

			// If we are in actionMode, update the menu items
			if (actionMode != null) {
				actionMode.Invalidate ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			SupportMenuInflater.Inflate (Resource.Menu.inbox_menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				if (slidingPaneLayout != null) {
					if (slidingPaneLayout.IsOpen) {
						slidingPaneLayout.ClosePane ();
					} else {
						slidingPaneLayout.OpenPane ();
					}
				}
				break;
			case Resource.Id.refresh:
				inbox.SetListShownNoAnimation (false);
				RichPushManager.Shared ().RefreshMessages ();
				break;
			case Resource.Id.preferences:
				this.StartActivity (new Intent (this, typeof(PushPreferencesActivity)));
				break;

			}
			return true;
		}

		public bool OnNavigationItemSelected (int itemPosition, long itemId)
		{
			string navName = (string)this.navAdapter.GetItem (itemPosition);
			if (RichPushApplication.HOME_ACTIVITY == navName) {
				NavigateToMain ();
			}
			return true;
		}
		//SuppressLint("NewApi")
		public bool OnCreateActionMode (ActionMode mode, Android.Views.IMenu menu)
		{
			mode.MenuInflater.Inflate (Resource.Menu.inbox_actions_menu, menu);

			// Add a pop up menu to the action bar to select/deselect all
			// Pop up menu requires api >= 11
			if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.Honeycomb) {
				View customView = LayoutInflater.From (this).Inflate (Resource.Layout.cab_selection_dropdown, null);
				actionSelectionButton = (Button)customView.FindViewById (Resource.Id.selection_button);

				PopupMenu popupMenu = new PopupMenu (this, customView);
				popupMenu.MenuInflater.Inflate (Resource.Menu.selection, popupMenu.Menu);
				popupMenu.MenuItemClick += (sender, e) => {
					var item = e.Item;
					if (item.ItemId == Resource.Id.menu_deselect_all) {
						inbox.ClearSelection ();
					} else {
						inbox.SelectAll ();
					}
					e.Handled = true;
				};

				actionSelectionButton.Click += (sender, e) => {
					var _menu = popupMenu.Menu;
					_menu.FindItem (Resource.Id.menu_deselect_all).SetVisible (true);
					_menu.FindItem (Resource.Id.menu_select_all).SetVisible (inbox.SelectedMessages.Count != messages.Count);
					popupMenu.Show ();
				};

				mode.CustomView = customView;
			}

			return true;
		}

		public bool OnPrepareActionMode (ActionMode mode, Android.Views.IMenu menu)
		{
			Logger.Debug ("onPrepareActionMode");

			bool selectionContainsRead = false;
			bool selectionContainsUnread = false;

			foreach (string id in inbox.SelectedMessages) {
				RichPushMessage message = richPushInbox.GetMessage (id);
				if (message.IsRead) {
					selectionContainsRead = true;
				} else {
					selectionContainsUnread = true;
				}

				if (selectionContainsRead && selectionContainsUnread) {
					break;
				}
			}

			// Show them both
			menu.FindItem (Resource.Id.mark_read).SetVisible (selectionContainsUnread);
			menu.FindItem (Resource.Id.mark_unread).SetVisible (selectionContainsRead);

			// If we have an action selection button update the text
			if (actionSelectionButton != null) {
				string selectionText = this.GetString (Resource.String.cab_selection, inbox.SelectedMessages.Count);
				actionSelectionButton.Text = selectionText;
			}

			return true;
		}

		public bool OnActionItemClicked (ActionMode mode, Android.Views.IMenuItem item)
		{
			Logger.Debug ("onActionItemClicked");
			switch (item.ItemId) {
			case Resource.Id.mark_read:
				richPushInbox.MarkMessagesRead (new HashSet<string> (inbox.SelectedMessages));
				break;
			case Resource.Id.mark_unread:
				richPushInbox.MarkMessagesUnread (new HashSet<string> (inbox.SelectedMessages));
				break;
			case Resource.Id.delete:
				richPushInbox.DeleteMessages (new HashSet<string> (inbox.SelectedMessages));
				break;
			case Resource.Id.abs__action_mode_close_button:
				break;
			default:
				return false;
			}

			actionMode.Finish ();
			return true;
		}

		public void OnDestroyActionMode (ActionMode mode)
		{
			Logger.Debug ("onDestroyActionMode");
			actionMode = null;
			inbox.ClearSelection ();
		}

		public override void OnBackPressed ()
		{
			NavigateToMain ();
		}

		/**
	     * Navigates to the main activity and finishes the current one
	     */
		private void NavigateToMain ()
		{
			Intent intent = new Intent (this, typeof(MainActivity));
			intent.AddFlags (ActivityFlags.ClearTop);
			this.StartActivity (intent);

			this.Finish ();
		}

		/**
	     * Configures the action bar to have a navigation list of
	     * 'Home' and 'Inbox'
	     */
		private void ConfigureActionBar ()
		{
			actionBar.SetHomeButtonEnabled (false);
			actionBar.SetDisplayHomeAsUpEnabled (false);
			actionBar.SetDisplayUseLogoEnabled (true);
			actionBar.SetDisplayShowTitleEnabled (false);
			// FIXME: this ActionBar is fully qualified only because the compiler fails to resolve it (bug)
			actionBar.NavigationMode = Xamarin.ActionbarSherlockBinding.App.ActionBar.NavigationModeList;

			this.navAdapter = new ArrayAdapter<String> (this, Resource.Layout.sherlock_spinner_dropdown_item,
			                                            RichPushApplication.navList);
			actionBar.SetListNavigationCallbacks (this.navAdapter, this);
		}

		/**
	     * Sets the action bar navigation to show 'Inbox'
	     */
		private void SetNavigationToInboxActivity ()
		{
			int position = this.navAdapter.GetPosition (RichPushApplication.INBOX_ACTIVITY);
			SupportActionBar.SetSelectedNavigationItem (position);
		}

		/**
     * Sets the pending message by looking for an id in the intent's extra
     * with key <code>RichPushApplication.MESSAGE_ID_RECEIVED_KEY</code>
     * 
     * @param intent Intent to look for a rich push message id
     */
		private void SetPendingMessageIdFromIntent (Intent intent)
		{
			pendingMessageId = intent.GetStringExtra (RichPushApplication.MESSAGE_ID_RECEIVED_KEY);

			if (!UAStringUtil.IsEmpty (pendingMessageId)) {
				Logger.Debug ("Received message id " + pendingMessageId);
			}
		}

		/**
     * Tries to show a message if the pendingMessageId is set.
     * Clears the pendingMessageId after.
     */
		private void ShowPendingMessageId ()
		{
			if (!UAStringUtil.IsEmpty (pendingMessageId)) {
				ShowMessage (pendingMessageId);
				pendingMessageId = null;
			}
		}

		/**
     * Shows a message either in the message view pager, or by launching
     * a new MessageActivity
     * @param messageId the specified message id
     */
		private void ShowMessage (string messageId)
		{
			RichPushMessage message = richPushInbox.GetMessage (messageId);

			// Message is already deleted, skip
			if (message == null) {
				return;
			}

			if (slidingPaneLayout != null && slidingPaneLayout.IsOpen) {
				slidingPaneLayout.ClosePane ();
			}

			message.MarkRead ();

			if (messagePager != null) {
				this.messagePager.CurrentItem = messages.IndexOf (message);
			} else {
				Intent intent = new Intent (this, typeof(MessageActivity));
				intent.PutExtra (MessageActivity.EXTRA_MESSAGE_ID_KEY, messageId);
				this.StartActivity (intent);
			}
		}

		/**
     * Starts the action mode if there are any selected
     * messages in the inbox fragment
     */
		private void StartActionModeIfNecessary ()
		{
			var checkedIds = inbox.SelectedMessages;
			if (actionMode != null && checkedIds.Count == 0) {
				actionMode.Finish ();
				return;
			} else if (actionMode == null && checkedIds.Count > 0) {
				actionMode = this.StartActionMode (this);
			}
		}

		public void OnUpdateMessages (bool success)
		{
			// Stop the progress spinner and display the list
			inbox.SetListShownNoAnimation (true);

			// If the message update failed
			if (!success) {
				// Show an error dialog
				DialogFragment fragment = new InboxRefreshFailedDialog ();
				fragment.Show (SupportFragmentManager, "dialog");
			}
		}

		public void OnUpdateUser (bool success)
		{
			// no-op
		}

		public void OnRetrieveMessage (bool success, string messageId)
		{
			// no-op
		}

		public void OnUpdateInbox ()
		{
			UpdateRichPushMessages ();
		}

		/**
     * Grabs the latest messages from the rich push inbox, and syncs them
     * with the inbox fragment and message view pager if available
     */
		private void UpdateRichPushMessages ()
		{
			messages = RichPushManager.Shared ().RichPushUser.Inbox.Messages;
			this.inbox.SetMessages (messages);
			if (messagePager != null) {
				((MessageFragmentAdapter)messagePager.Adapter).SetRichPushMessages (messages);
			}
		}

		/**
     * Alert dialog for when messages fail to refresh
     */
		public class InboxRefreshFailedDialog : DialogFragment
		{
			override
				public Android.App.Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				return new Android.App.AlertDialog.Builder (Activity)
					.SetIcon (Resource.Drawable.ua_launcher)
						.SetTitle (Resource.String.inbox_refresh_failed_dialog_title)
						.SetMessage (Resource.String.inbox_refresh_failed_dialog_message)
						.SetNeutralButton (Resource.String.ok, (o, e) => {
					((Android.App.Dialog)o).Dismiss ();
				})
						.Create ();
			}
		}

		public void OnPanelClosed (View panel)
		{
			if (messagePager != null) {
				messagePager.EnableTouchEvents (true);
			}
		}

		public void OnPanelOpened (View panel)
		{
			if (messagePager != null) {
				messagePager.EnableTouchEvents (false);
			}
		}

		public void OnPanelSlide (View arg0, float arg1)
		{
			//do nothing
		}
	}
}
