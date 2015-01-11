/*
 * Copyright (C) 2007 The Android Open Source Project
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

namespace MonoDroid.ApiDemo
{
	/**
 	* Demonstrates how the layout_weight attribute can shrink an element too big
 	* to fit on screen.
 	*/
	[Activity (Label = "Views/Layouts/LinearLayout/09. Layout Weight")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class LinearLayout9 : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.linear_layout_9);
			ListView list = FindViewById <ListView> (Resource.Id.list);
			list.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, AutoComplete1.COUNTRIES);
		}
	}
}