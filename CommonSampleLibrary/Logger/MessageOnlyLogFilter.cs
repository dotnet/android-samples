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

namespace CommonSampleLibrary
{
	/**
	* Simple ILogNode filter, removes everything except the message.
	* Useful for situations like on-screen log output where you don't want a lot of metadata displayed,
	* just easy-to-read message updates as they're happening.
	*/
	public class MessageOnlyLogFilter : ILogNode
	{

		/**
		* Gets the next LogNode in the chain.
		* Sets the LogNode data will be sent to..
		*/
		public ILogNode NextNode { get; set; }

		/**
		* Takes the "next" LogNode as a parameter, to simplify chaining.
		*/
		public MessageOnlyLogFilter (ILogNode next)
		{
			NextNode = next;
		}

		public MessageOnlyLogFilter ()
		{
		}

		public void WriteLine (LogPriority priority, string tag, string msg, Java.Lang.Throwable tr)
		{
			if (NextNode != null) {
				NextNode.WriteLine (LogPriority.Info, null, msg, null);
			}
		}
	}
}

