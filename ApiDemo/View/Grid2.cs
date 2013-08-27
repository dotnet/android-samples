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

namespace MonoDroid.ApiDemo
{
	
	/**
 	* A grid that displays a set of framed photos.
 	*/
	[Activity (Label = "Views/Grid/2. Photo Grid")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]				
	public class Grid2 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.grid_2);

			GridView g = FindViewById <GridView> (Resource.Id.myGrid);
			g.Adapter = new ImageAdapter (this);
		}

		class ImageAdapter : BaseAdapter
		{
			Context self;

			public ImageAdapter (Context s)
			{
				self = s;
			}

			public override int Count {
				get {
					return mThumbIds.Length;
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
				ImageView imageView;
				if (convertView == null) {
					imageView = new ImageView (self);
					imageView.LayoutParameters = new GridView.LayoutParams (45, 45);
					imageView.SetAdjustViewBounds (false);
					imageView.SetScaleType (ImageView.ScaleType.CenterCrop);
					imageView.SetPadding (8, 8, 8, 8);
				} else {
					imageView = (ImageView) convertView;
				}

				imageView.SetImageResource (mThumbIds[position]);

				return imageView;
			}

			int[] mThumbIds = {
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
				Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1,
				Resource.Drawable.sample_thumb_2, Resource.Drawable.sample_thumb_3,
				Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
				Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7,
			};
		}
	}
}

