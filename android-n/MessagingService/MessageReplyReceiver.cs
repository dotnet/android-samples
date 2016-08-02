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

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

namespace MessagingService
{
	[BroadcastReceiver (Enabled = true)]
	[Android.App.IntentFilter (new [] { MessagingService.REPLY_ACTION })]
	/// <summary>
	/// A receiver that gets called when a reply is sent to a given conversationId
	/// </summary>
	public class MessageReplyReceiver : BroadcastReceiver
	{
		readonly string TAG = typeof(MessageReplyReceiver).Name;

		public override void OnReceive (Context context, Intent intent)
		{
			if (MessagingService.REPLY_ACTION.Equals (intent.Action)) {
				int conversationId = intent.GetIntExtra (MessagingService.CONVERSATION_ID, -1);
				var reply = GetMessageText (intent);
				if (conversationId != -1) {
					Log.Debug (TAG, "Got reply (" + reply + ") for ConversationId " + conversationId);
					MessageLogger.LogMessage (context, "ConversationId: " + conversationId +
					" received a reply: [" + reply + "]");

					using (var notificationManager = NotificationManagerCompat.From (context)) {
						var notificationBuilder = new NotificationCompat.Builder (context);
						notificationBuilder.SetSmallIcon (Resource.Drawable.notification_icon);
						notificationBuilder.SetLargeIcon (BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.android_contact));
						notificationBuilder.SetContentText (context.GetString (Resource.String.replied));
						Notification repliedNotification = notificationBuilder.Build ();
						notificationManager.Notify (conversationId, repliedNotification);
					}
				}
			}
		}

		/// <summary>
		/// Get the message text from the intent.
		/// Note that you should call <see cref="Android.Support.V4.App.RemoteInput.GetResultsFromIntent(intent)"/> 
		/// to process the RemoteInput.
		/// </summary>
		/// <returns>The message text.</returns>
		/// <param name="intent">Intent.</param>
		static string GetMessageText (Intent intent)
		{
			Bundle remoteInput = Android.App.RemoteInput.GetResultsFromIntent (intent);
			return remoteInput != null ? remoteInput.GetCharSequence (MessagingService.EXTRA_REMOTE_REPLY) : null;
		}
	}
}

