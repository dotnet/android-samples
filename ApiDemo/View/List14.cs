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
using Android.Graphics;

namespace MonoDroid.ApiDemo
{
	/**
 	* Demonstrates how to write an efficient list adapter. The adapter used in this example binds
 	* to an ImageView and to a TextView for each row in the list.
 	*
 	* To work efficiently the adapter implemented here uses two techniques:
 	* - It reuses the convertView passed to getView() to avoid inflating View when it is not necessary
 	* - It uses the ViewHolder pattern to avoid calling findViewById() when it is not necessary
 	*
 	* The ViewHolder pattern consists in storing a data structure in the tag of the view returned by
 	* getView(). This data structures contains references to the views we want to bind data to, thus
 	* avoiding calls to findViewById() every time getView() is invoked.
 	*/
	[Activity (Label = "Views/Lists/14. Efficient Adapter")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List14 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ListAdapter = new EfficientAdapter (this);
		}
	}

	class EfficientAdapter : BaseAdapter
	{
		LayoutInflater mInflater;
		Bitmap mIcon1;
		Bitmap mIcon2;

		public EfficientAdapter (Context context)
		{
			// Cache the LayoutInflate to avoid asking for a new one each time.
			mInflater = LayoutInflater.From (context);

			// Icons bound to the rows.
			mIcon1 = BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.icon48x48_1);
			mIcon2 = BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.icon48x48_2);
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
			// A ViewHolder keeps references to children views to avoid unneccessary calls
			// to findViewById() on each row.
			ViewHolder holder;

			// When convertView is not null, we can reuse it directly, there is no need
			// to reinflate it. We only inflate a new View when the convertView supplied
			// by ListView is null.
			if (convertView == null) {
				convertView = mInflater.Inflate (Resource.Layout.list_item_icon_text, null);

				// Creates a ViewHolder and store references to the two children views
				// we want to bind data to.
				holder = new ViewHolder ();
				holder.View = convertView.FindViewById <TextView> (Resource.Id.text);
				holder.Icon = convertView.FindViewById <ImageView> (Resource.Id.icon);

				convertView.Tag = holder;
			} else {
				// Get the ViewHolder back to get fast access to the TextView
				// and the ImageView.
				holder = (ViewHolder) convertView.Tag;
			}

			// Bind the data efficiently with the holder.
			holder.View.Text = (Cheeses.CheeseStrings[position]);
			holder.Icon.SetImageBitmap ((position & 1) == 1 ? mIcon1 : mIcon2);

			return convertView;
		}

		class ViewHolder : Java.Lang.Object
		{
			public TextView View { get; set; }
			public ImageView Icon { get; set; }
		}
	}
}

