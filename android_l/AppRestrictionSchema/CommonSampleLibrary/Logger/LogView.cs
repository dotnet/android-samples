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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace CommonSampleLibrary
{
	/** Simple TextView which is used to output log data received through the LogNode interface.
	*/
	public class LogView : TextView, ILogNode
	{
		public LogView (Context context) : base (context)
		{
		}

		public LogView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
		}

		public LogView (Context context, IAttributeSet attrs, int defStyle)
			: base (context, attrs, defStyle)
		{
		}

		/**
     	* Formats the log data and prints it out to the LogView.
     	*/
		public void WriteLine (LogPriority priority, string tag, string msg, Java.Lang.Throwable tr)
		{
			string priorityStr = null;

			// For the purposes of this View, we want to print the priority as readable text.
			switch (priority) {
			case LogPriority.Verbose:
				priorityStr = "VERBOSE";
				break;
			case LogPriority.Debug:
				priorityStr = "DEBUG";
				break;
			case LogPriority.Info:
				priorityStr = "INFO";
				break;
			case LogPriority.Warn:
				priorityStr = "WARN";
				break;
			case LogPriority.Error:
				priorityStr = "ERROR";
				break;
			case LogPriority.Assert:
				priorityStr = "ASSERT";
				break;
			default:
				break;
			}

			// Handily, the Log class has a facility for converting a stack trace into a usable string.
			string exceptionStr = null;
			if (tr != null) {
				exceptionStr = Android.Util.Log.GetStackTraceString (tr);
			}

			// Take the priority, tag, message, and exception, and concatenate as necessary
			// into one usable line of text.
			StringBuilder outputBuilder = new StringBuilder ();

			string delimiter = "\t";
			AppendIfNotNull (outputBuilder, priorityStr, delimiter);
			AppendIfNotNull (outputBuilder, tag, delimiter);
			AppendIfNotNull (outputBuilder, msg, delimiter);
			AppendIfNotNull (outputBuilder, exceptionStr, delimiter);

			// In case this was originally called from an AsyncTask or some other off-UI thread,
			// make sure the update occurs within the UI thread.

			((Activity)Context).RunOnUiThread (new Action (delegate {
				AppendToLog (outputBuilder.ToString ());
			}));

			if (NextNode != null) {
				NextNode.WriteLine (priority, tag, msg, tr);
			}
		}


		// The next LogNode in the chain.
		ILogNode mNext;

		public ILogNode NextNode {
			get { return mNext; }
			set { mNext = value; }
		}

		/** Takes a string and adds to it, with a separator, if the bit to be added isn't null. Since
     	* the logger takes so many arguments that might be null, this method helps cut out some of the
     	* agonizing tedium of writing the same 3 lines over and over.
     	*/
		StringBuilder AppendIfNotNull (StringBuilder source, string addStr, string delimiter)
		{
			if (addStr != null) {
				if (addStr.Length == 0) {
					delimiter = "";
				}
				return source.Append (addStr).Append (delimiter);
			}
			return source;
		}

		/** Outputs the string as a new line of log data in the LogView. */
		public void AppendToLog (string s)
		{
			Append ("\n" + s);
		}
	}
}

