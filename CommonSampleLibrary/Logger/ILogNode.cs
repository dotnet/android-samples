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

using Java.Lang;
using Android.Util;

namespace CommonSampleLibrary
{
	/**
	* Basic interface for a logging system that can output to one or more targets.
	* Note that in addition to classes that will output these logs in some format,
	* one can also implement this interface over a filter and insert that in the chain,
	* such that no targets further down see certain data, or see manipulated forms of the data.
	* You could, for instance, write a "ToHtmlLoggerNode" that just converted all the log data
	* it received to HTML and sent it along to the next node in the chain, without printing it
	* anywhere.
	*/
	public interface ILogNode
	{
		/**
		* Instructs first LogNode in the list to print the log data provided.
		*/
		void WriteLine (LogPriority priority, string tag, string msg, Throwable tr);
	}
}

