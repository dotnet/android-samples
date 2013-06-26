/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Push;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Graphics;
using Android.App;
using Android.Media;
using Android.Text;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
	 * A custom push notification builder to create inbox style notifications
	 * for rich push messages.  In the case of standard push notifications, it will
	 * fall back to the default behavior.
	 * 
	 */
	public class RichNotificationBuilder : BasicPushNotificationBuilder
	{

		const int EXTRA_MESSAGES_TO_SHOW = 2;
		const int INBOX_NOTIFICATION_ID = 9000000;

		public override Android.App.Notification BuildNotification (string alert, IDictionary<string, string> extras)
		{
			if (extras != null && RichPushManager.IsRichPushMessage (extras)) {
				return CreateInboxNotification (alert);
			} else {
				return base.BuildNotification (alert, extras);
			}
		}

		public override int GetNextId (string alert, IDictionary<string, string> extras)
		{
			if (extras != null && extras.ContainsKey (PushReceiver.EXTRA_MESSAGE_ID_KEY)) {
				return INBOX_NOTIFICATION_ID;
			} else {
				return base.GetNextId (alert, extras);
			}
		}

		/**
	     * Creates an inbox style notification summarizing the unread messages
	     * in the inbox
	     * 
	     * @param incomingAlert The alert message from an Urban Airship push
	     * @return An inbox style notification
	     */
		private Android.App.Notification CreateInboxNotification (string incomingAlert)
		{
			Context context = UAirship.Shared ().ApplicationContext;

			IList<RichPushMessage> unreadMessages = RichPushInbox.Shared ().UnreadMessages;
			int inboxUnreadCount = unreadMessages.Count;

			// The incoming message is not immediately made available to the inbox because it needs
			// to first fetch its contents.
			int totalUnreadCount = inboxUnreadCount + 1;

			Resources res = UAirship.Shared ().ApplicationContext.Resources;
			string title = res.GetQuantityString (Resource.Plurals.inbox_notification_title, totalUnreadCount, totalUnreadCount);

			Bitmap largeIcon = BitmapFactory.DecodeResource (res, Resource.Drawable.ua_launcher);

			var style = new Notification.InboxStyle (
				new Notification.Builder (context)
					.SetContentTitle (title)
					.SetContentText (incomingAlert)
					.SetLargeIcon (largeIcon)
					.SetSmallIcon (Resource.Drawable.ua_notification_icon)
					.SetSound (RingtoneManager.GetDefaultUri (RingtoneType.Notification))
					.SetNumber (totalUnreadCount));

			// Add the incoming alert as the first line in bold
			style.AddLine (Html.FromHtml ("<b>" + incomingAlert + "</b>"));

			// Add any extra messages to the notification style
			int extraMessages = Math.Min (EXTRA_MESSAGES_TO_SHOW, inboxUnreadCount);
			for (int i = 0; i < extraMessages; i++) {
				style.AddLine (unreadMessages [i].Title);
			}

			// If we have more messages to show then the EXTRA_MESSAGES_TO_SHOW, add a summary
			if (inboxUnreadCount > EXTRA_MESSAGES_TO_SHOW) {
				style.SetSummaryText (context.GetString (Resource.String.inbox_summary, inboxUnreadCount - EXTRA_MESSAGES_TO_SHOW));
			}

			return style.Build ();
		}

		/**
	     * Dismisses the inbox style notification if it exists
	     */
		public static void DismissInboxNotification ()
		{
			NotificationManager manager = (NotificationManager)UAirship.Shared ().
					ApplicationContext.GetSystemService (Context.NotificationService);

			manager.Cancel (INBOX_NOTIFICATION_ID);
		}
	}
}
