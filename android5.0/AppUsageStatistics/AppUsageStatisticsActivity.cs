/*
* Copyright 2014 The Android Open Source Project
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

using Android.App;
using Android.OS;

namespace AppUsageStatistics
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
	/// <summary>
	/// Launcher Activity for the App Usage Statistics sample app.
	/// </summary>
	public class AppUsageStatisticsActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_app_usage_statistics);
			if (savedInstanceState == null) {
				FragmentManager.BeginTransaction ()
					.Add (Resource.Id.container, AppUsageStatisticsFragment.NewInstance ())
					.Commit ();
			}
		}
	}
}

