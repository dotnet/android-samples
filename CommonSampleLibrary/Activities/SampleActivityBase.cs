/*
* Copyright 2013 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
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

namespace CommonSampleLibrary
{
	/**
 	* Base launcher activity, to handle most of the common plumbing for samples.
 	*/
	[Activity (Label = "SampleActivityBase")]			
	public class SampleActivityBase : Activity
	{
		public virtual string TAG {
			get { return "SampleActivityBase"; }
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			InitializeLogging ();
		}

		/** Set up targets to receive log data */
		public virtual void InitializeLogging ()
		{
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			// Wraps Android's native log framework
			LogWrapper logWrapper = new LogWrapper ();
			Log.LogNode = logWrapper;

			Log.Info (TAG, "Ready");
		}
	}
}

