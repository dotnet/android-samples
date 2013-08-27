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
 	* An example of tabs that uses labels ({@link TabSpec#setIndicator(CharSequence)})
 	* for its indicators and views by id from a layout file ({@link TabSpec#setContent(int)}).
 	*/
	[Activity (Label = "Views/Tabs/01. Content By Id")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Tabs01 : TabActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			LayoutInflater.From (this).Inflate (Resource.Layout.tabs1, TabHost.TabContentView, true);

			TabHost.AddTab (TabHost.NewTabSpec ("tab1").SetIndicator ("tab1").SetContent (Resource.Id.view1));
			TabHost.AddTab (TabHost.NewTabSpec ("tab3").SetIndicator ("tab2").SetContent (Resource.Id.view2));
			TabHost.AddTab (TabHost.NewTabSpec ("tab3").SetIndicator ("tab3").SetContent (Resource.Id.view3));
		}
	}
}