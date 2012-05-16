//
// Copyright (C) 2007 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Mono API Demos", MainLauncher = true, Icon = "@drawable/icon")]
	public class ApiDemo : ListActivity
	{
		public const string SAMPLE_CATEGORY = "mono.apidemo.sample";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// If this is a submenu list, this will have the prefix to get here
			var prefix = Intent.GetStringExtra ("com.example.android.apis.Path");
			
			// This must be the top-level menu list
			prefix = prefix ?? string.Empty;

			// Get the activities for this prefix
			var activities = GetDemoActivities (prefix);

			// Get the menu items we need to show
			var items = GetMenuItems (activities, prefix);

			// Add the menu items to the list
			ListAdapter = new ArrayAdapter<ActivityListItem> (this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, items);

			// Launch the new activity when the list is clicked
			ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
				var item = (ActivityListItem) (sender as ListView).GetItemAtPosition (args.Position);
				LaunchActivityItem (item);
			};
		}

		private List<ActivityListItem> GetDemoActivities (string prefix)
		{
			var results = new List<ActivityListItem> ();

			// Create an intent to query the package manager with,
			// we are looking for ActionMain with our custom category
			Intent query = new Intent (Intent.ActionMain, null);
			query.AddCategory (ApiDemo.SAMPLE_CATEGORY);

			var list = PackageManager.QueryIntentActivities (query, 0);

			// If there were no results, bail
			if (list == null)
				return results;

			// Process the results
			foreach (var resolve in list) {
				// Get the menu category from the activity label
				var category = resolve.LoadLabel (PackageManager);

				// Get the data we'll need to launch the activity
				string type = string.Format ("{0}:{1}", resolve.ActivityInfo.ApplicationInfo.PackageName, resolve.ActivityInfo.Name);

				if (string.IsNullOrWhiteSpace (prefix) || category.StartsWith (prefix, StringComparison.InvariantCultureIgnoreCase))
					results.Add (new ActivityListItem (prefix, category, type));
			}

			return results;
		}

		private List<ActivityListItem> GetMenuItems (List<ActivityListItem> activities, string prefix)
		{
			// Get menu items at this level
			var items = activities.Where (a => a.IsMenuItem);

			// Get Submenus at this level, but we only need 1 of each
			var submenus = activities.Where (a => a.IsSubMenu).Distinct (new ActivityListItem.NameComparer ());

			// Combine, sort, return
			return items.Union (submenus).OrderBy (a => a.Name).ToList ();
		}

		private void LaunchActivityItem (ActivityListItem item)
		{
			if (item.IsSubMenu) {
				// Launch this menu activity again with an updated prefix
				Intent result = new Intent ();

				result.SetClass (this, typeof (ApiDemo));
				result.PutExtra ("com.example.android.apis.Path", string.Format ("{0}/{1}", item.Prefix, item.Name).Trim ('/'));

				StartActivity (result);
			} else {
				// Launch the item activity
				Intent result = new Intent ();
				result.SetClassName (item.Package, item.Component);

				StartActivity (result);
			}
		}
	}
}
