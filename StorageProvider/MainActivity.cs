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
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CommonSampleLibrary;

namespace StorageProvider
{

	/**
 	* A simple launcher activity containing a summary sample description
 	* and a few action bar buttons.
 	*/
	[Activity (Label = "StorageProvider", MainLauncher = true, Theme = "@style/MyAppTheme",
		UiOptions = Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
	public class MainActivity : SampleActivityBase
	{
		public override string TAG {
			get { return "MainActivity"; }
		}

		public const string FRAGTAG = "MyCloudFragment";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			if (SupportFragmentManager.FindFragmentByTag (FRAGTAG) == null ) {
				Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
				MyCloudFragment fragment = new MyCloudFragment ();
				transaction.Add (fragment, FRAGTAG);
				transaction.Commit ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_menu, menu);
			return true;
		}

		/** Create a chain of targets that will receive log data */
		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework.
			LogWrapper logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			MessageOnlyLogFilter msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView.
			LogFragment logFragment = (LogFragment) SupportFragmentManager
				.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;

			Log.Info (TAG, "Ready");
		}
	}
}


