/*
 * Copyright (C) 2011 Jake Wharton
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

// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/iprogress")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class IndeterminateProgress : SherlockActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);

			//This has to be called before setContentView and you must use the
			//class in com.actionbarsherlock.view and NOT android.view
			RequestWindowFeature (Android.Views.WindowFeatures.IndeterminateProgress);

			SetContentView (Resource.Layout.iprogress);

			FindViewById (Resource.Id.enable).Click += (object sender, EventArgs e) => {
				SetSupportProgressBarIndeterminateVisibility (true);
			};
			FindViewById (Resource.Id.disable).Click += (object sender, EventArgs e) => {
				SetSupportProgressBarIndeterminateVisibility (false);
			};
		}
	}
}

