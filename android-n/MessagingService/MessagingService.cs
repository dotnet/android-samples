/*
 * Copyright (C) 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

namespace MessagingService
{
	[Service]
	[IntentFilter (new String[]{ "com.xamarin.MessagingService" })]
	public class MessagingService : Service
	{
		static readonly string TAG = typeof(MessagingService).Name;
		const string EOL = "\n";
		public const string READ_ACTION =
			"com.xamarin.messagingservice.ACTION_MESSAGE_READ";
		
		public const string REPLY_ACTION =
			"com.xamarin.messagingservice.ACTION_MESSAGE_REPLY";
		public const string CONVERSATION_ID = "conversation_id";
		public const string EXTRA_REMOTE_REPLY = "extra_remote_reply";
		public const int MSG_SEND_NOTIFICATION = 1;

		NotificationManagerCompat mNotificationManager;

		readonly Messenger mMessenger;

		public MessagingService ()
		{
			mMessenger = new Messenger (new IncomingHandler (this));
		}

		public override void OnCreate ()
		{
			Log.Debug (TAG, "onCreate");
			mNotificationManager = NotificationManagerCompat.From (ApplicationContext);
		}

		public override IBinder OnBind (Intent intent)
		{
			Log.Debug (TAG, "onBind");
			return mMessenger.Binder;
		}

		// Creates an intent that will be triggered when a message is marked as read.
		static Intent GetMessageReadIntent (int id)
		{
			return new Intent (READ_ACTION)
				.AddFlags (ActivityFlags.IncludeStoppedPackages)
				.SetAction (READ_ACTION)
				.PutExtra (CONVERSATION_ID, id);
		}

		// Creates an Intent that will be triggered when a voice reply is received.
		static Intent GetMessageReplyIntent (int conversationId)
		{
			return new Intent (REPLY_ACTION)
				.AddFlags (ActivityFlags.IncludeStoppedPackages)
				.SetAction (REPLY_ACTION)
				.PutExtra (CONVERSATION_ID, conversationId);
		}

		void SendNotification (int howManyConversations, int messagesPerConversation)
		{
			Conversations.Conversation[] conversations = Conversations.GetUnreadConversations (
				                                             howManyConversations, messagesPerConversation);
			foreach (Conversations.Conversation conv in conversations) {
				SendNotificationForConversation (conv);
			}
		}

		void SendNotificationForConversation (Conversations.Conversation conversation)
		{
			// A pending Intent for reads
			PendingIntent readPendingIntent = PendingIntent.GetBroadcast (ApplicationContext,
				                                  conversation.ConversationId,
				                                  GetMessageReadIntent (conversation.ConversationId),
				                                  PendingIntentFlags.UpdateCurrent);

			// Build a RemoteInput for receiving voice input in a Car Notification or text input on
			// devices that support text input (like devices on Android N and above).
			var remoteInput = new Android.Support.V4.App.RemoteInput.Builder (EXTRA_REMOTE_REPLY)
				.SetLabel (ApplicationContext.GetString (Resource.String.reply))
				.Build ();

			// Building a Pending Intent for the reply action to trigger
			PendingIntent replyIntent = PendingIntent.GetBroadcast (ApplicationContext,
				                            conversation.ConversationId,
				                            GetMessageReplyIntent (conversation.ConversationId),
				                            PendingIntentFlags.UpdateCurrent);

			// Build an Android N compatible Remote Input enabled action.
			NotificationCompat.Action actionReplyByRemoteInput = new NotificationCompat.Action.Builder (
				Resource.Drawable.notification_icon,
				GetString (Resource.String.reply),
				replyIntent).AddRemoteInput (remoteInput).Build ();

			// Create the UnreadConversation and populate it with the participant name,
			// read and reply intents.
			var unreadConvBuilder =
				new NotificationCompat.CarExtender.UnreadConversation.Builder (conversation.ParticipantName)
					.SetLatestTimestamp (conversation.Timestamp)
					.SetReadPendingIntent (readPendingIntent)
					.SetReplyAction (replyIntent, remoteInput);

			// Note: Add messages from oldest to newest to the UnreadConversation.Builder
			StringBuilder messageForNotification = new StringBuilder ();
			for (int i = 0; i < conversation.Messages.Count; i++) {
				unreadConvBuilder.AddMessage (conversation.Messages [i]);
				messageForNotification.Append (conversation.Messages [i]);
				if (i != conversation.Messages.Count - 1)
					messageForNotification.Append (EOL);
			}

			NotificationCompat.Builder builder = new NotificationCompat.Builder (ApplicationContext)
				.SetSmallIcon (Resource.Drawable.notification_icon)
				.SetLargeIcon (BitmapFactory.DecodeResource (
				                                     ApplicationContext.Resources, Resource.Drawable.android_contact))
				.SetContentText (messageForNotification.ToString ())
				.SetWhen (conversation.Timestamp)
				.SetContentTitle (conversation.ParticipantName)
				.SetContentIntent (readPendingIntent)
				.Extend (new NotificationCompat.CarExtender ()
					.SetUnreadConversation (unreadConvBuilder.Build ())
					.SetColor (ApplicationContext
						.Resources.GetColor (Resource.Color.default_color_light)))
				.AddAction (actionReplyByRemoteInput);

			MessageLogger.LogMessage (ApplicationContext, "Sending notification "
			+ conversation.ConversationId + " conversation: " + conversation);

			mNotificationManager.Notify (conversation.ConversationId, builder.Build ());
		}

		class IncomingHandler : Handler
		{
			readonly WeakReference<MessagingService> mReference;

			public IncomingHandler (MessagingService service)
			{
				mReference = new WeakReference<MessagingService> (service);
			}

			public override void HandleMessage (Message msg)
			{
				MessagingService service;
				mReference.TryGetTarget (out service);
				switch (msg.What) {
				case MSG_SEND_NOTIFICATION:
					int howManyConversations = msg.Arg1 <= 0 ? 1 : msg.Arg1;
					int messagesPerConversation = msg.Arg2 <= 0 ? 1 : msg.Arg2;
					if (service != null) {
						service.SendNotification (howManyConversations, messagesPerConversation);
					}
					break;
				default:
					HandleMessage (msg);
					break;
				}
			}
		}
	}
}

