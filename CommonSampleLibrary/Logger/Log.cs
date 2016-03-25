/*
 * Copyright (C) 2013 The Android Open Source Project
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

using Android.Util;
using Java.Lang;

namespace CommonSampleLibrary
{
	/**
	* Helper class for a list (or tree) of LoggerNodes.
	*
	* When this is set as the head of the list,
	* an instance of it can function as a drop-in replacement for android.util.Log.
	* Most of the methods in this class server only to map a method call in Log to its equivalent
	* in LogNode.
	*/
	public static class Log
	{
		// Stores the beginning of the LogNode topology.

		// Returns the next LogNode in the linked list.
		public static ILogNode LogNode { get; set; }

		/**
		* Instructs the LogNode to print the log data provided. Other LogNodes can
		* be chained to the end of the LogNode as desired.
		*/
		public static void WriteLine (LogPriority priority, string tag, string msg, Throwable tr)
		{
			if (LogNode != null)
				LogNode.WriteLine (priority, tag, msg, tr);
		}

		/**
		* Instructs the LogNode to print the log data provided. Other LogNodes can
		* be chained to the end of the LogNode as desired.
		*/
		public static void WriteLine (LogPriority priority, string tag, string msg)
		{
			WriteLine (priority, tag, msg, null);
		}

		/**
		* Prints a message at VERBOSE priority.
		*/
		public static void Verbose (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Verbose, tag, msg, tr);
		}

		/**
		* Prints a message at VERBOSE priority.
		*/
		public static void Verbose (string tag, string msg)
		{
			Verbose (tag, msg, null);
		}


		/**
		* Prints a message at DEBUG priority.
		*/
		public static void Debug (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Debug, tag, msg, tr);
		}

		/**
		* Prints a message at DEBUG priority.
		*/
		public static void Debug (string tag, string msg)
		{
			Debug (tag, msg, null);
		}

		/**
		* Prints a message at INFO priority.
		*/
		public static void Info (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Info, tag, msg, tr);
		}

		/**
		* Prints a message at INFO priority.
		*/
		public static void Info (string tag, string msg)
		{
			Info (tag, msg, null);
		}

		/**
		* Prints a message at WARN priority.
		*/
		public static void Warn (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Warn, tag, msg, tr);
		}

		/**
		* Prints a message at WARN priority.
		*/
		public static void Warn (string tag, string msg)
		{
			Warn (tag, msg, null);
		}

		/**
		* Prints a message at WARN priority.
		*/
		public static void Warn (string tag, Throwable tr)
		{
			Warn (tag, null, tr);
		}

		/**
		* Prints a message at ERROR priority.
		*/
		public static void Error (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Error, tag, msg, tr);
		}

		/**
		* Prints a message at ERROR priority.
		*/
		public static void Error (string tag, string msg)
		{
			Error (tag, msg, null);
		}

		/**
		* Prints a message at ASSERT priority.
		*/
		public static void Wtf (string tag, string msg, Throwable tr)
		{
			WriteLine (LogPriority.Assert, tag, msg, tr);
		}

		/**
		* Prints a message at ASSERT priority.
		*/
		public static void Wtf (string tag, string msg)
		{
			Wtf (tag, msg, null);
		}

		/**
		* Prints a message at ASSERT priority.
		*/
		public static void Wtf (string tag, Throwable tr)
		{
			Wtf (tag, null, tr);
		}
	}
}

