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

using System.Collections.Generic;
using Java.Lang;
using Java.Util.Concurrent;

namespace MessagingService
{
	/// <summary>
	/// A simple class that denotes unread conversations and messages. In a real world application,
	/// this would be replaced by a content provider that actually gets the unread messages to be
	/// shown to the user.
	/// </summary>
	public class Conversations
	{
		/// <summary>
		/// Set of strings used as messages by the sample.
		/// </summary>
		static readonly string[] DefinedMessages = {
			"Are you at home?",
			"Can you give me a call?",
			"Hey yt?",
			"Don't forget to get some milk on your way back home",
			"Is that project done?",
			"Did you finish the Messaging app yet?"
		};

		/// <summary>
		/// Senders of the said messages.
		/// </summary>
		static readonly string[] Participants = {
			"John Smith",
			"Robert Lawrence",
			"James Smith",
			"Jane Doe"

		};

		public class Conversation
		{
			/// <summary>
			/// A given conversation can have a single or multiple messages.
			/// Note that the messages are sorted from *newest* to *oldest*
			/// </summary>
			readonly List<string> messages;
			readonly int conversationId;
			readonly string participantName;
			readonly long timestamp;

			public Conversation (int conversationId, string participantName,
			                     List<string> messages)
			{
				this.conversationId = conversationId;
				this.participantName = participantName;
				this.messages = messages ?? new List<string> ();
				timestamp = JavaSystem.CurrentTimeMillis ();
			}

			public int ConversationId {
				get { return conversationId; }
			}

			public List<string> Messages {
				get { return messages; }
			}

			public string ParticipantName {
				get { return participantName; }
			}

			public long Timestamp {
				get { return timestamp; }
			}

			public override string ToString ()
			{
				return "[Conversation: conversationId=" + conversationId +
				", participantName=" + participantName +
				", messages=[" + string.Join (",", messages) +
				"], timestamp=" + timestamp + "]";
			}
		}

		public static Conversation[] GetUnreadConversations (int howManyConversations,
		                                                     int messagesPerConversation)
		{
			var conversations = new Conversation[howManyConversations];
			for (int i = 0; i < howManyConversations; i++) {
				conversations [i] = new Conversation (
					ThreadLocalRandom.Current ().NextInt (),
					Name, makeMessages (messagesPerConversation));
			}
			return conversations;
		}

		static List<string> makeMessages (int messagesPerConversation)
		{
			int maxLen = DefinedMessages.Length;
			List<string> messages = new List<string> (messagesPerConversation);
			for (int i = 0; i < messagesPerConversation; i++) {
				messages.Add (DefinedMessages [ThreadLocalRandom.Current ().NextInt (0, maxLen)]);
			}
			return messages;
		}

		static string Name {
			get{ return Participants [ThreadLocalRandom.Current ().NextInt (0, Participants.Length)]; }
		}
	}
}

