/*
 * Copyright (C) 2008 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	/**
 	* Demonstrates the Tab scrolling when too many tabs are displayed to fit in the screen.
 	*/
	[Activity (Label = "Views/Tabs/05. Scrollable")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Tabs05 : TabActivity, TabHost.ITabContentFactory
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.tabs_scroll);

			for (int i = 1; i <= 30; i++) {
				String name = "Tab " + i;
				TabHost.AddTab (TabHost.NewTabSpec (name).SetIndicator (name).SetContent (this));
			}
		}

		public View CreateTabContent (String tag)
		{
			TextView tv = new TextView (this);
			tv.Text = "Content for tab with tag " + tag;
			return tv;
		}
	}
}