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

using Android.Content;
using Java.Lang;
using Java.Text;
using Java.Util;

namespace MessagingService
{
	/// <summary>
	/// A simple logger that uses shared preferences to log messages, their reads
	/// and replies. Don't use this in a real world application. This logger is only
	/// used for displaying the messages in the text view.
	/// </summary>
	public static class MessageLogger
	{
		static readonly string PREFERENCE_MESSAGE = "MESSAGE_LOGGER";
		static readonly string LINE_BREAKS = "\n\n";
		static readonly DateFormat DATE_FORMAT =
			DateFormat.GetDateTimeInstance (DateFormat.Default, DateFormat.Default);
		
		public const string LOG_KEY = "message_data";

		public static void LogMessage (Context context, string message)
		{
			ISharedPreferences prefs = GetPrefs (context);
			message = DATE_FORMAT.Format (new Date (JavaSystem.CurrentTimeMillis ())) + ": " + message;
			prefs.Edit ()
				.PutString (LOG_KEY, prefs.GetString (LOG_KEY, "") + LINE_BREAKS + message)
				.Apply ();
		}

		public static ISharedPreferences GetPrefs (Context context)
		{
			return context.GetSharedPreferences (PREFERENCE_MESSAGE, FileCreationMode.Private);
		}

		public static string GetAllMessages (Context context)
		{
			return GetPrefs (context).GetString (LOG_KEY, "");
		}

		public static void Clear (Android.Content.Context context)
		{
			GetPrefs (context).Edit ().Remove (LOG_KEY).Apply ();
		}
	}
}

