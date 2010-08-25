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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MonoDroid.ApiDemo
{
	public class ApiDemo : ListActivity
	{
		public ApiDemo (IntPtr handle)
			: base (handle)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			String path = Intent.GetStringExtra ("com.example.android.apis.Path");
			
			if (path == null)
				path = "";
				
			ListAdapter = new SimpleAdapter (this, GetData (path),
				Android.R.Layout.SimpleListItem1, new String[] { "title" },
				new int[] { Android.R.Id.Text1 });
			ListView.TextFilterEnabled = true;
		}

		protected Java.Util.IList<Java.Util.IMap<string, object>> GetData (String prefix)
		{
			var myData = new Java.Util.ArrayList<Java.Util.IMap<string, object>> ();

			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.CategorySampleCode);

			PackageManager pm = PackageManager;
			var list = pm.QueryIntentActivities (mainIntent, 0);

			if (null == list)
				return myData;

			String[] prefixPath;

			if (prefix == string.Empty)
				prefixPath = null;
			else
				prefixPath = prefix.Split ('/');

			int len = list.Size ();

			Java.Util.HashMap<string, bool> entries = new Java.Util.HashMap<string, bool> ();

			for (int i = 0; i < len; i++) {
				ResolveInfo info = list.Get (i);
				CharSequence labelSeq = info.LoadLabel (pm);

				String label = labelSeq != null ? labelSeq.ToString () : info.ActivityInfo.Name;

				if (prefix.Length == 0 || label.StartsWith (prefix)) {

					String[] labelPath = label.Split ('/');
					String nextLabel = prefixPath == null ? labelPath[0] : labelPath[prefixPath.Length];

					if ((prefixPath != null ? prefixPath.Length : 0) == labelPath.Length - 1) {
						AddItem (myData, nextLabel, ActivityIntent (
							info.ActivityInfo.ApplicationInfo.PackageName,
							info.ActivityInfo.Name));
					} else {
						if (entries.Get (nextLabel) == false) {
							AddItem (myData, nextLabel, BrowseIntent (prefix == "" ? nextLabel : prefix + "/" + nextLabel));
							entries.Put (nextLabel, true);
						}
					}
				}
			}

			return myData;
		}

		protected Intent ActivityIntent (String pkg, String componentName)
		{
			Intent result = new Intent ();
			result.SetClassName (pkg, componentName);

			return result;
		}

		protected Intent BrowseIntent (String path)
		{
			Intent result = new Intent ();
			result.SetClassName (this, "monoDroid.apiDemo.ApiDemo");
			result.PutExtra ("com.example.android.apis.Path", path);

			return result;
		}

		protected void AddItem (Java.Util.ArrayList<Java.Util.IMap<string, object>> data, String name, Intent intent)
		{
			Java.Util.IMap<string, object> temp = new Java.Util.HashMap<string, object> ();

			temp.Put ("title", name);
			temp.Put ("intent", intent);

			data.Add (temp);
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			Java.Util.IMap<string, object> map = (Java.Util.IMap<string, object>)l.GetItemAtPosition (position);

			Intent intent = (Intent)map.Get ("intent");
			intent.SetFlags (Intent.FlagActivityNewTask);

			StartActivity (intent);
		}
	}
}