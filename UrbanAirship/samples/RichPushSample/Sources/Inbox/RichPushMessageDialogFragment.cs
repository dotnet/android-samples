/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
//using Android.App;
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
using Xamarin.UrbanAirship.PreferencesAPI;
using Xamarin.UrbanAirship.Widget;
using Android.Support.V4.App;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * Dialog Fragment that displays a rich push message
 *
 */
	public class RichPushMessageDialogFragment : DialogFragment
	{
		const string MESSAGE_ID_KEY = "com.xamarin.samples.urbanairship.richpush.FIRST_MESSAGE_ID";

		/**
     * Creates a new instance of RichPushMessageDialogFragment
     * @param messageId The id of the message to display
     * @return RichPushMessageDialogFragment
     */
		public static RichPushMessageDialogFragment NewInstance (string messageId)
		{
			RichPushMessageDialogFragment fragment = new RichPushMessageDialogFragment ();

			Bundle args = new Bundle ();
			args.PutString (MESSAGE_ID_KEY, messageId);
			fragment.Arguments = args;

			return fragment;
		}

		override
			public void OnAttach (Android.App.Activity activity)
		{
			base.OnAttach (activity);
		}

		override
			public View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			string messageId = Arguments.GetString (MESSAGE_ID_KEY);
			RichPushMessage message = RichPushManager.Shared ().RichPushUser.Inbox.GetMessage (messageId);

			if (message == null) {
				return null;
			}


			View view = inflater.Inflate (Resource.Layout.message_dialog, container, true);

			RichPushMessageView messageView = (RichPushMessageView)view.FindViewById (Resource.Id.message_browser);
			messageView.LoadRichPushMessage (message);
			message.MarkRead ();

			// Update the widget, this dialog can show a message on any activity
			RichPushWidgetUtils.RefreshWidget (this.Activity);


			Dialog.SetTitle (Resource.String.rich_push_message_dialog_title);

			return view;
		}
	}
}
