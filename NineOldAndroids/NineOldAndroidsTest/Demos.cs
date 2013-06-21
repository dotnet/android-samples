/*
 * Copyright (C) 2011 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Text;

namespace NineOldAndroidsTest
{
	[Activity (Name = PackageName + ".Demos", Label = "Nine Old Androids")]
	[IntentFilter (new string [] {Intent.ActionMain},
		Categories = new string [] { Intent.CategoryLauncher, Intent.CategoryDefault })]
	public class Demos : ListActivity
	{
		public const string PackageName = "com.xamarin.nineoldandroids.samples";
		public const string SampleCategory = "com.xamarin.nineoldandroids.sample.SAMPLE";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Intent intent = Intent;
			String path = intent.GetStringExtra ("com.example.android.apis.Path");

			if (path == null) {
				path = "";
			}

			ListAdapter = new SimpleAdapter (this, GetData (path),
			                                 Android.Resource.Layout.SimpleListItem1, new string[] { "title" },
			                                 new int[] { Android.Resource.Id.Text1 });
			ListView.TextFilterEnabled = true;
		}

		protected JavaList<IDictionary<String, Object>> GetData (String prefix)
		{
			var myData = new List<IDictionary<String, Object>> ();

			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (SampleCategory);

			PackageManager pm = PackageManager;
			IList<ResolveInfo> list = (IList<ResolveInfo>) pm.QueryIntentActivities (mainIntent, PackageInfoFlags.Activities);

			if (null == list)
				return new JavaList<IDictionary<String, Object>> (myData);

			String[] prefixPath;
			String prefixWithSlash = prefix;

			if (prefix.Equals ("")) {
				prefixPath = null;
			} else {
				prefixPath = prefix.Split ('/');
				prefixWithSlash = prefix + "/";
			}

			int len = list.Count;

			IDictionary<String, bool?> entries = new JavaDictionary<String, bool?> ();

			for (int i = 0; i < len; i++) {
				ResolveInfo info = list [i];
				var labelSeq = info.LoadLabel (pm);
				String label = labelSeq != null
					? labelSeq.ToString ()
						: info.ActivityInfo.Name;

				if (prefixWithSlash.Length  == 0 || label.StartsWith (prefixWithSlash)) {

					String[] labelPath = label.Split ('/');

					String nextLabel = prefixPath == null ? labelPath [0] : labelPath [prefixPath.Length];

					if ((prefixPath != null ? prefixPath.Length : 0) == labelPath.Length - 1) {
						AddItem (myData, nextLabel, ActivityIntent (
							info.ActivityInfo.ApplicationInfo.PackageName,
							info.ActivityInfo.Name));
					} else {
						if (entries [nextLabel] == null) {
							AddItem (myData, nextLabel, BrowseIntent (prefix.Equals ("") ? nextLabel : prefix + "/" + nextLabel));
							entries [nextLabel] = true;
						}
					}
				}
			}

			myData.Sort (sDisplayNameComparator);

			return new JavaList<IDictionary<String, Object>> (myData);
		}

		static readonly Comparison<IDictionary<String, Object>> sDisplayNameComparator =
			new Comparison<IDictionary<String, Object>> (delegate (IDictionary<String, Object> map1, IDictionary<String, Object> map2) {
				return String.Compare(map1 ["title"].ToString (), map2 ["title"].ToString ());
		});

		protected Intent ActivityIntent (String pkg, String componentName)
		{
			Intent result = new Intent ();
			result.SetClassName(pkg, componentName);
			return result;
		}

		protected Intent BrowseIntent (String path)
		{
			Intent result = new Intent ();
			result.SetClass (this, Java.Lang.Class.FromType (typeof (Demos)));
			result.PutExtra ("com.example.android.apis.Path", path);
			return result;
		}

		protected void AddItem (List<IDictionary<String, Object>> data, String name, Intent intent)
		{
			IDictionary<String, Object> temp = new JavaDictionary<String, Object> ();
			temp.Add ("title", name);
			temp.Add ("intent", intent);
			data.Add (temp);
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			JavaDictionary<String, Object> map = (JavaDictionary<String, Object>)l.GetItemAtPosition (position);

			Intent intent = (Intent)map["intent"];
			StartActivity (intent);
		}
	}
}


