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
 	* Demonstrates how a list can avoid expensive operations during scrolls or flings. In this
 	* case, we pretend that binding a view to its data is slow (even though it really isn't). When
 	* a scroll/fling is happening, the adapter binds the view to temporary data. After the scroll/fling
 	* has finished, the temporary data is replace with the actual data.
 	*/
	[Activity (Label = "Views/Lists/13. Slow Adapter")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]				
	public class List13 : ListActivity
	{
		TextView mStatus;
		public bool mBusy = false;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list_13);

			mStatus = FindViewById <TextView> (Resource.Id.status);
			mStatus.Text = "Idle";

			// Use an existing ListAdapter that will map an array
			// of strings to TextViews
			ListAdapter = new SlowAdapter (this);

			ListView.ScrollStateChanged += OnScrollStateChanged;
		}

		void OnScrollStateChanged (object sender, AbsListView.ScrollStateChangedEventArgs e)
		{
			var view = (AbsListView)sender;

			switch (e.ScrollState) {
			case ScrollState.Idle:
				mBusy = false;

				int first = view.FirstVisiblePosition;
				int count = view.ChildCount;
				for (int i = 0; i < count; i++) {
					TextView t = (TextView)view.GetChildAt (i);
					if (t.Tag != null) {
						t.Text = (Cheeses.CheeseStrings[first + i]);
						t.Tag = null;
					}
				}

				mStatus.Text = "Idle";
				break;

			case ScrollState.TouchScroll:
				mBusy = true;
				mStatus.Text = "Touch scroll";
				break;

			case ScrollState.Fling:
				mBusy = true;
				mStatus.Text = "Fling";
				break;
			}
		}
	}

	/**
     * Will not bind views while the list is scrolling
     * 
     */
	class SlowAdapter : BaseAdapter
	{
		Context self;

		public SlowAdapter (Context context)
		{
			self = context;
		}

		public override int Count {
			get {
				return Cheeses.CheeseStrings.Length;
			}
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TextView text;

			if (convertView == null) {
				text = (TextView)LayoutInflater.From (self).Inflate (Android.Resource.Layout.SimpleListItem1, parent, false);
			} else {
				text = (TextView)convertView;
			}

			if (!((List13)self).mBusy) {
				text.Text = Cheeses.CheeseStrings[position];
				// Null tag means the view has the correct data
				text.Tag = null;
			} else {
				text.Text = "Loading...";
				// Non-null tag means the view still needs to load it's data
				text.Tag = this;
			}

			return text;
		}
	}
}

