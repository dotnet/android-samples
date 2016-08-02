/*
 * Copyright (C) 2012 The Android Open Source Project
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
using Android.Util;

namespace CommonSampleLibrary
{
	/**
	* Helper class which wraps Android's native Log utility in the Logger interface.  This way
	* normal DDMS output can be one of the many targets receiving and outputting logs simultaneously.
	*/
	public class LogWrapper : ILogNode
	{
		// For piping:  The next node to receive Log data after this one has done its work.

		/**
		* Gets the next LogNode in the linked list.
		* Sets the LogNode data will be sent to..
		*/public ILogNode NextNode { get; set; }

		/**
		* Prints data out to the console using Android's native log mechanism.
		*/
		public void WriteLine (LogPriority priority, string tag, string msg, Java.Lang.Throwable tr)
		{
			// There actually are log methods that don't take a msg parameter.  For now,
			// if that's the case, just convert null to the empty string and move on.
			String useMsg = msg ?? string.Empty;

			// If an exeption was provided, convert that exception to a usable string and attach
			// it to the end of the msg method.
			if (tr != null) {
				msg += "\n" + Android.Util.Log.GetStackTraceString (tr);
			}

			// This is functionally identical to Log.x(tag, useMsg);
			// For instance, if priority were Log.VERBOSE, this would be the same as Log.v(tag, useMsg)
			Android.Util.Log.WriteLine (priority, tag, useMsg);

			// If this isn't the last node in the chain, move things along.
			if (NextNode != null) {
				NextNode.WriteLine (priority, tag, msg, tr);
			}
		}
	}
}

