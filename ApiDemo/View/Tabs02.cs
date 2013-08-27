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
 	* Example of using a tab content factory for the content via {@link TabHost.TabSpec#setContent(android.widget.TabHost.TabContentFactory)}
 	*
 	* It also demonstrates using an icon on one of the tabs via {@link TabHost.TabSpec#setIndicator(CharSequence, android.graphics.drawable.Drawable)}
 	*
 	*/
	[Activity (Label = "Views/Tabs/02. Content By Factory")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Tabs02 : TabActivity, TabHost.ITabContentFactory
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			TabHost.AddTab (TabHost.NewTabSpec ("tab1")
			               .SetIndicator ("tab1", Resources.GetDrawable (Resource.Drawable.star_big_on))
			               .SetContent (this));

			TabHost.AddTab (TabHost.NewTabSpec ("tab2").SetIndicator ("tab2").SetContent (this));
			TabHost.AddTab (TabHost.NewTabSpec ("tab3").SetIndicator ("tab3").SetContent (this));
		}

		public View CreateTabContent (string tag)
		{
			TextView tv = new TextView (this);
			tv.Text = "Content for tab with tag " + tag;
			return tv;
		}

	}
}