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
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "MonoDroid API Demo", MainLauncher = true)]
	public class ApiDemo : ListActivity
	{
		public ApiDemo ()
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetDefaultKeyMode (DefaultKey.SearchLocal);

			String path = Intent.GetStringExtra ("com.example.android.apis.Path");
			
			path = path ?? "";
				
			ListAdapter = new SimpleAdapter (this, GetData (path),
				Android.R.Layout.SimpleListItem1, new String[] { "title" },
				new int[] { Android.R.Id.Text1 });
			ListView.TextFilterEnabled = true;

			ListView.ItemClick += delegate (object sender, ItemEventArgs args) {
				IDictionary<string, object> map = (IDictionary<string, object>) (sender as ListView).GetItemAtPosition (args.Position);
				Intent intent = (Intent)map ["intent"];
				intent.SetFlags (ActivityFlags.NewTask);
				StartActivity (intent);
			};
		}

		protected IList<IDictionary<string, object>> GetData (String prefix)
		{
			var myData = new JavaList<IDictionary<string, object>> ();

			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.CategorySampleCode);

			PackageManager pm = PackageManager;
			var list = pm.QueryIntentActivities (mainIntent, 0);

			if (list == null)
				return myData;

			String[] prefixPath;

			if (prefix == string.Empty)
				prefixPath = null;
			else
				prefixPath = prefix.Split ('/');

			int len = list.Count;

			JavaDictionary<string, bool> entries = new JavaDictionary<string, bool> ();

			list = list.OrderBy (p => (p.ActivityInfo.NonLocalizedLabel ?? "").ToString ()).ToList ();

			for (int i = 0; i < len; i++) {
				ResolveInfo info = list [i];
				IEnumerable<char> labelSeq = info.LoadLabel (pm);

				String label = labelSeq != null ? labelSeq.ToString () : info.ActivityInfo.Name;

				if (prefix.Length == 0 || label.StartsWith (prefix)) {

					String[] labelPath = label.Split ('/');
					String nextLabel = prefixPath == null ? labelPath[0] : labelPath[prefixPath.Length];

					if ((prefixPath != null ? prefixPath.Length : 0) == labelPath.Length - 1) {
						AddItem (myData, nextLabel, ActivityIntent (
							info.ActivityInfo.ApplicationInfo.PackageName,
							info.ActivityInfo.Name));
					} else {
						if (!entries.ContainsKey (nextLabel) || entries [nextLabel] == false) {
							AddItem (myData, nextLabel, BrowseIntent (prefix == "" ? nextLabel : prefix + "/" + nextLabel));
							entries [nextLabel] = true;
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
			result.SetClass (this, typeof (ApiDemo));
			result.PutExtra ("com.example.android.apis.Path", path);

			return result;
		}

		protected void AddItem (JavaList<IDictionary<string, object>> data, String name, Intent intent)
		{
			JavaDictionary<string, object> temp = new JavaDictionary<string, object> ();

			temp ["title"] = name;
			temp ["intent"] = intent;

			data.Add (temp);
		}
	}
}
