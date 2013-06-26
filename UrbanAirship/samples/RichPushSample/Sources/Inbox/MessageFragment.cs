/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.Views;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.ActionbarSherlockBinding.App;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;
using Xamarin.UrbanAirship.Widget;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * Fragment that displays a rich push message in a RichPushMessageView
 */
	public class MessageFragment : SherlockFragment
	{
		private const string MESSAGE_ID_KEY = "com.urbanairship.richpush.URL_KEY";
		private RichPushMessageView browser;

		/**
     * Creates a new MessageFragment
     * @param messageId The message's id to display
     * @return messageFragment new MessageFragment
     */
		public static MessageFragment NewInstance (string messageId)
		{
			MessageFragment message = new MessageFragment ();
			Bundle arguments = new Bundle ();
			arguments.PutString (MESSAGE_ID_KEY, messageId);
			message.Arguments = arguments;
			return message;
		}

		override
			public View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			browser = new RichPushMessageView (container.Context);
			browser.LayoutParameters = container.LayoutParameters;
			return browser;
		}

		override
			public void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			string messageId = Arguments.GetString (MESSAGE_ID_KEY);
			RichPushMessage message = RichPushManager.Shared ().RichPushUser.Inbox.GetMessage (messageId);

			if (message != null) {
				browser.LoadRichPushMessage (message);
			} else {
				Logger.Info ("Couldn't retrieve message for ID: " + messageId);
			}
		}
	}
}
