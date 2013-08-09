/*
 * Copyright 2013 The Android Open Source Project
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

using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

/**
 * This sample demonstrates how to create custom single- or multi-choice
 * {@link android.widget.ListView} UIs. The most interesting bits are in
 * the <code>res/layout/</code> directory of this sample.
 */

namespace CustomChoiceList
{
	[Activity (Label = "CustomChoiceList", MainLauncher = true, Theme = "@style/Theme.Sample")]
	public class MainActivity : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			ListAdapter = new MyAdapter (this);
		}
	}

	/**
     * A simple array adapter that creates a list of cheeses.
     */
	class MyAdapter : BaseAdapter 
	{
		Context context;

		public MyAdapter (Context c)
		{
			context = c;
		}

		public override int Count {
			get {
				return Cheeses.CHEESES.Length;
			}
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return Cheeses.CHEESES [position];
		}

		public override long GetItemId (int position)
		{
			return Cheeses.CHEESES [position].GetHashCode ();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
				convertView = LayoutInflater.From (context).Inflate (Resource.Layout.list_item, parent, false);

			convertView.FindViewById <TextView> (Android.Resource.Id.Text1).Text = ((string) GetItem (position));
			return convertView;
		}
	}
}


