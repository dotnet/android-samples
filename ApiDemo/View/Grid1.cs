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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Grid/1. Icon Grid")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]	
	public class Grid1 : Activity
	{
		GridView mGrid;
		public IList<ResolveInfo> mApps;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			LoadApps (); // do this in onresume?

			SetContentView (Resource.Layout.grid_1);
			mGrid = FindViewById <GridView> (Resource.Id.myGrid);
			mGrid.Adapter = new AppsAdapter(this);
		}

		void LoadApps ()
		{
			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.CategoryLauncher);

			mApps = PackageManager.QueryIntentActivities (mainIntent, 0);
		}

		class AppsAdapter : BaseAdapter
		{
			Context self;

			public AppsAdapter (Context s)
			{
				self = s;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ImageView i;

				if (convertView == null) {
					i = new ImageView (self);
					i.SetScaleType (ImageView.ScaleType.FitCenter);
					i.LayoutParameters = new GridView.LayoutParams (50, 50);
				} else {
					i = (ImageView) convertView;
				}

				ResolveInfo info = ((Grid1)self).mApps[position];
				i.SetImageDrawable (info.ActivityInfo.LoadIcon (self.PackageManager));

				return i;
			}

			public override int Count {
				get {
					return ((Grid1)self).mApps.Count;
				}
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return ((Grid1)self).mApps [position];
			}

			public override long GetItemId (int position)
			{
				return position;
			}
		}
	}
}

