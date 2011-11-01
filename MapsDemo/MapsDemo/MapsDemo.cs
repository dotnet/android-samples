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
	[Activity (Name = "com.example.monodroid.googleapis.maps.MapsDemo", Label = "M4A MapsDemo", MainLauncher = true)]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { Intent.CategoryLauncher })]
	public class MapsDemo : ListActivity 
	{
		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
				
			ListAdapter = new SimpleAdapter (this, GetData (),
				Android.Resource.Layout.SimpleListItem1, new String [] { "title" },
				new int[] { Android.Resource.Id.Text1 });

			ListView.TextFilterEnabled = true;
		}

		protected List<IDictionary<String,Object>> GetData () 
		{
			var myData = new List<IDictionary<String,Object>> ();
			
			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.CategorySampleCode);
			
			PackageManager pm = PackageManager;
			IList<ResolveInfo> list = pm.QueryIntentActivities (mainIntent, 0);
			
			if (null == list)
				return myData;
				
			int len = list.Count;
			
			IDictionary<String, Boolean> entries = new Dictionary<String, Boolean> ();
			
			for (int i = 0; i < len; i++) {
				ResolveInfo info = list [i];
				
				var labelSeq = info.LoadLabel (pm);
				
				if ("com.example.monodroid.googleapis.maps" == info.ActivityInfo.ApplicationInfo.PackageName) {
					AddItem (myData, labelSeq.ToString (), ActivityIntent (
						info.ActivityInfo.ApplicationInfo.PackageName,
						info.ActivityInfo.Name));
				}
			}
			
			myData.Sort (delegate (IDictionary<string,object> map1, IDictionary<string,object> map2) {
				return String.Compare ((string) map1 ["title"], (string) map2 ["title"], StringComparison.CurrentCulture);
				});
			
			return myData;
		}

		protected Intent ActivityIntent (String pkg, String componentName) 
		{
			Intent result = new Intent ();
			result.SetClassName (pkg, componentName);
			return result;
		}
    
		protected void AddItem (List<IDictionary<String, Object>> data, String name, Intent intent) {
			IDictionary<String, Object> temp = new Dictionary<String, Object> ();
			temp ["title"] = name;
			temp ["intent"] = intent;
			data.Add (temp);
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id) {
			var map = (IDictionary<String, Object>) l.GetItemAtPosition (position);
			
			Intent intent = (Intent) map ["intent"];
			StartActivity (intent);
		}
	}
}
