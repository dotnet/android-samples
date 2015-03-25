/*
 * Copyright (C) 2010 The Android Open Source Project
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
 	* A list view where the last item the user clicked is placed in
 	* the "activated" state, causing its background to highlight.
 	*/
	[Activity (Label = "Views/Lists/17. Activate items", Name = "monodroid.apidemo.List17")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]				
	public class List17 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Use the built-in layout for showing a list item with a single
			// line of text whose background is changes when activated.
			ListAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItemActivated1, Cheeses.CheeseStrings);
			ListView.TextFilterEnabled = true;

			// Tell the list view to show one checked/activated item at a time.
			ListView.ChoiceMode = ChoiceMode.Single;

			// Start with first item activated.
			// Make the newly clicked item the currently selected one.
			ListView.SetItemChecked (0, true);

			ListView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				// Make the newly clicked item the currently selected one.
				ListView.SetItemChecked (e.Position, true);
			};
		}
	}
}

