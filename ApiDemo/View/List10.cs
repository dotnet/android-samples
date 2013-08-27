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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	/**
 	* This example shows how to use choice mode on a list. This list is 
 	* in CHOICE_MODE_SINGLE mode, which means the items behave like
 	* checkboxes.
 	*/
	[Activity (Label = "Views/Lists/10. Single choice list")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List10 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			ListAdapter = new ArrayAdapter<String>
				(this, Android.Resource.Layout.SimpleListItemSingleChoice, GENRES);

			ListView.ItemsCanFocus = false;
			ListView.ChoiceMode = ChoiceMode.Single;
		}

		static string[] GENRES = new string[] {
			"Action", "Adventure", "Animation", "Children", "Comedy", "Documentary", "Drama",
			"Foreign", "History", "Independent", "Romance", "Sci-Fi", "Television", "Thriller"
		};
	}
}

