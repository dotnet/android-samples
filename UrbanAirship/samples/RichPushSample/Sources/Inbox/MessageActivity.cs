/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using System.Collections.Generic;
using Xamarin.ActionbarSherlockBinding.App;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;
using Android.Support.V4.View;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	[Activity (Label = "MessageActivity")]			
	/**
 * Manages the message view pager and display messages
 *
 */
	public class MessageActivity : SherlockFragmentActivity
	{

		public const string EXTRA_MESSAGE_ID_KEY = "com.xamarin.samples.urbanairship.richpush.EXTRA_MESSAGE_ID_KEY";
		private ViewPager messagePager;
		private IList<RichPushMessage> messages;

		override
			protected void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.message);

			string messageId = savedInstanceState == null ? this.Intent.GetStringExtra (EXTRA_MESSAGE_ID_KEY) :
				savedInstanceState.GetString (EXTRA_MESSAGE_ID_KEY);

			// Get the list of rich push messages
			this.messages = RichPushManager.Shared ().RichPushUser.Inbox.Messages;

			// Sets up the MessageViewPager
			this.messagePager = (ViewPager)this.FindViewById (Resource.Id.message_pager);
			MessageFragmentAdapter messageAdapter = new MessageFragmentAdapter (this.SupportFragmentManager);
			this.messagePager.PageSelected += (sender, e) => {
				int _position = e.P0;
				messages [_position].MarkRead ();
			};
			messageAdapter.SetRichPushMessages (messages);
			this.messagePager.Adapter = messageAdapter;

			// Get the first item to show
			int position = 0;
			RichPushMessage firstMessage = RichPushManager.Shared ().RichPushUser.Inbox.GetMessage (messageId);
			if (firstMessage != null) {
				position = messages.IndexOf (firstMessage);
				if (position == -1) {
					position = 0;
				}
			}

			// Mark it as read
			messages [position].MarkRead ();

			// Sets the current item to the position of the current message
			this.messagePager.CurrentItem = position;

			this.SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			this.SupportActionBar.SetHomeButtonEnabled (true);
		}

		override
			protected void OnStart ()
		{
			base.OnStart ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStarted (this);
		}

		override
			protected void OnPause ()
		{
			base.OnPause ();

			// Refresh any widgets
			RichPushWidgetUtils.RefreshWidget (this);
		}

		override
			protected void OnStop ()
		{
			base.OnStop ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStopped (this);
		}

		override
			protected void OnSaveInstanceState (Bundle savedInstanceState)
		{
			string messageId = messages [messagePager.CurrentItem].MessageId;
			savedInstanceState.PutString (EXTRA_MESSAGE_ID_KEY, messageId);
		}

		override
			public bool OnOptionsItemSelected (IMenuItem menuItem)
		{

			Intent intent = new Intent (this, typeof(InboxActivity));
			intent.AddFlags (ActivityFlags.ClearTop);
			this.StartActivity (intent);

			this.Finish ();
			return true;
		}
	}
}