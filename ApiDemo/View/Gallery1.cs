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
using Android.Content.Res;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Gallery/1. Photos")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Gallery1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.gallery_1);

			// Reference the Gallery view
			Gallery g = FindViewById <Gallery> (Resource.Id.gallery);

			// Set the adapter to our custom adapter (below)
			g.Adapter = new ImageAdapter (this);

			// Set a item click listener, and just Toast the clicked position
			g.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				Toast.MakeText (this, "" + e.Position, ToastLength.Short).Show ();
			};

			// We also want to show context menu for longpressed items in the gallery
			RegisterForContextMenu (g);
		}

		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			menu.Add (Resource.String.gallery_2_text);
		}

		public override bool OnContextItemSelected (IMenuItem item)
		{
			var info = (AdapterView.AdapterContextMenuInfo) item.MenuInfo;
			Toast.MakeText (this, "Longpress: " + info.Position, ToastLength.Short).Show ();
			return true;
		}
	}

	public class ImageAdapter : BaseAdapter
	{
		const int ITEM_WIDTH = 136;
		const int ITEM_HEIGHT = 88;

		int mGalleryItemBackground;
		Context mContext;

		int[] mImageIds = {
			Resource.Drawable.gallery_photo_1,
			Resource.Drawable.gallery_photo_2,
			Resource.Drawable.gallery_photo_3,
			Resource.Drawable.gallery_photo_4,
			Resource.Drawable.gallery_photo_5,
			Resource.Drawable.gallery_photo_6,
			Resource.Drawable.gallery_photo_7,
			Resource.Drawable.gallery_photo_8
		};

		float mDensity;

		public ImageAdapter (Context c)
		{
			mContext = c;
			// See res/values/attrs.xml for the <declare-styleable> that defines
			// Gallery1.
			TypedArray a = mContext.ObtainStyledAttributes (Resource.Styleable.Gallery1);
			mGalleryItemBackground = a.GetResourceId (Resource.Styleable.Gallery1_android_galleryItemBackground, 0);
			a.Recycle ();

			mDensity = c.Resources.DisplayMetrics.Density;
		}

		public override int Count {
			get {
				return mImageIds.Length;
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
				convertView = new ImageView (mContext);

				imageView = (ImageView) convertView;
				imageView.SetScaleType (ImageView.ScaleType.FitXy);
				imageView.LayoutParameters = (new Gallery.LayoutParams (
					(int) (ITEM_WIDTH * mDensity + 0.5f),
					(int) (ITEM_HEIGHT * mDensity + 0.5f)));

				// The preferred Gallery item background
				imageView.SetBackgroundResource (mGalleryItemBackground);
			} else {
				imageView = (ImageView) convertView;
			}

			imageView.SetImageResource (mImageIds[position]);

			return imageView;
		}
	}
}

