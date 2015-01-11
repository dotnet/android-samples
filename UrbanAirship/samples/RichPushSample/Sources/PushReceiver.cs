/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using System;
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

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
	 * Broadcast receiver to handle all push notifications
	 *
	 */
	[BroadcastReceiver]
	[IntentFilter (new string [] {"com.urbanairship.push.PushManager.ACTION_NOTIFICATION_OPENED"})]
	public class PushReceiver : BroadcastReceiver 
	{
		public const string ACTIVITY_NAME_KEY = "activity";
		public const string EXTRA_MESSAGE_ID_KEY = "_uamid";

		/**
	     * Delay to refresh widget to give time to fetch the rich push message
	     */
		private const long WIDGET_REFRESH_DELAY_MS = 5000; //5 Seconds


			public override void OnReceive(Context context, Intent intent) {

			// Refresh the widget after a push comes in
			if (PushManager.ActionPushReceived == intent.Action) {
				RichPushWidgetUtils.RefreshWidget(context, WIDGET_REFRESH_DELAY_MS);
			}

			// Only takes action when a notification is opened
			if (PushManager.ActionNotificationOpened != intent.Action) {
				return;
			}

			// Ignore any non rich push notifications
			if (!RichPushManager.IsRichPushMessage(intent.Extras)) {
				return;
			}

			string messageId = intent.GetStringExtra(EXTRA_MESSAGE_ID_KEY);
			Logger.Debug("Notified of a notification opened with id " + messageId);

			Intent messageIntent = null;

			// Set the activity to receive the intent
			if ("home" == intent.GetStringExtra(ACTIVITY_NAME_KEY)) {
				messageIntent = new Intent(context, typeof (MainActivity));
			} else {
				// default to the Inbox
				messageIntent =  new Intent(context, typeof (InboxActivity));
			}

			messageIntent.PutExtra(RichPushApplication.MESSAGE_ID_RECEIVED_KEY, messageId);
			messageIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
			context.StartActivity(messageIntent);
		}
	}
}