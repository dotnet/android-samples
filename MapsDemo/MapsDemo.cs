/*
 * Copyright (C) 2009 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoDroid.Samples.MapsDemo
{
	[Activity (Label = "M4A MapsDemo", MainLauncher = true)]
	public class MapsDemo : ListActivity 
	{
		private List<string> menu = new List<string> ();

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);

			menu.Add ("MapView");
			menu.Add ("MapView and Compass");

			ListAdapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, menu);
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			var selected = l.GetItemAtPosition (position).ToString ();

			switch (selected) {
				case "MapView":
					StartActivity (typeof (MapViewDemo));
					break;
				case "MapView and Compass":
					StartActivity (typeof (MapViewCompassDemo));
					break;
			}
		}
	}
}
