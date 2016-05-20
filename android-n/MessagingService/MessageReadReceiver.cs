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
using Android.Content;
using Android.Support.V4.App;
using Android.Util;

namespace MessagingService
{
	[BroadcastReceiver (Enabled = true)]
	[Android.App.IntentFilter (new [] { MessagingService.READ_ACTION })]
	public class MessageReadReceiver : BroadcastReceiver
	{
		readonly string TAG = typeof(MessageReadReceiver).Name;

		const string CONVERSATION_ID = "conversation_id";

		public override void OnReceive (Context context, Intent intent)
		{
			Log.Debug (TAG, "onReceive");
			int conversationId = intent.GetIntExtra (CONVERSATION_ID, -1);
			if (conversationId != -1) {
				Log.Debug (TAG, "Conversation " + conversationId + " was read");
				MessageLogger.LogMessage (context, "Conversation " + conversationId + " was read.");
				NotificationManagerCompat.From (context)
					.Cancel (conversationId);
			}
		}
	}
}

