/*
 * Copyright 2013 Chris Banes
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
// Under the same license as above.
//

using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.ActionBarPullToRefresh.Library;

namespace ActionBarPullToRefreshSample
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : ListActivity {

		// We don't want to display these activities for a couple of different reasons:
		// MainActivity - this is the current activity
		// ignore generated splash screen activity for trial users
		private string[] _activitiesToExclude = new string[] {
			"actionbarpulltorefreshsample.MainActivity",
			"actionbarpulltorefreshsample.actionbarpulltorefreshsample.TrialSplashScreen"
		};

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			ListAdapter = GetSampleAdapter ();
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id) {
			ActivityInfo info = (ActivityInfo) l.GetItemAtPosition(position);
			Intent intent = new Intent();
			intent.SetComponent(new ComponentName(this, info.Name));
			StartActivity(intent);
		}

		IListAdapter GetSampleAdapter ()
		{
			List<ActivityInfo> items = new List<ActivityInfo>();

			try {
				PackageInfo pInfo = PackageManager.GetPackageInfo (PackageName, PackageInfoFlags.Activities);
				var aInfos = pInfo.Activities.Where (ai => !_activitiesToExclude.Contains(ai.Name));

				foreach (ActivityInfo aInfo in aInfos) {
					Console.WriteLine (aInfo.Name);

					items.Add(aInfo);

				}
			} catch (PackageManager.NameNotFoundException e) {
				Console.WriteLine (e.StackTrace);
			}

			return new SampleAdapter (this, items);
		}

		class SampleAdapter : BaseAdapter {

			private List<ActivityInfo> mItems;

			readonly LayoutInflater mInflater;

			public SampleAdapter(Context context, List<ActivityInfo> activities) {
				mItems = activities;
				mInflater = LayoutInflater.From(context);
			}

			public override int Count {
				get { return mItems.Count; }
			}

			public override Java.Lang.Object GetItem (int position) {
				return mItems [position];
			}

			public override long GetItemId (int position) {
				return position;
			}

			public override View GetView (int position, View convertView, ViewGroup parent) {
				TextView tv = (TextView) convertView;
				if (tv == null) {
					tv = (TextView) mInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
				}
				ActivityInfo item = (ActivityInfo) GetItem(position);
				if (!TextUtils.IsEmpty(item.NonLocalizedLabel)) {
					tv.SetText (item.NonLocalizedLabel, TextView.BufferType.Normal);
				} else {
					tv.SetText(item.LabelRes);
				}
				return tv;
			}
		}
	}
}


