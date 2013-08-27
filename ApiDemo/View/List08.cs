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
 	* A list view that demonstrates the use of setEmptyView. This example alos uses
 	* a custom layout file that adds some extra buttons to the screen.
	*/
	[Activity (Label = "Views/Lists/08. Photos")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List8 : ListActivity
	{
		PhotoAdapter mAdapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Use a custom layout file
			SetContentView (Resource.Layout.list_8);

			// Tell the list view which view to display when the list is empty
			ListView.EmptyView = FindViewById (Resource.Id.empty);

			// Set up our adapter
			mAdapter = new PhotoAdapter (this);
			ListAdapter = mAdapter;

			// Wire up the clear button to remove all photos
			Button clear = FindViewById <Button> (Resource.Id.clear);
			clear.Click += delegate {
				mAdapter.ClearPhotos ();
			};

			// Wire up the add button to add a new photo
			Button addPhotos = FindViewById <Button>(Resource.Id.add);
			addPhotos.Click += delegate {
				mAdapter.AddPhotos ();
			};
		}
	}

	/**
     * A simple adapter which maintains an ArrayList of photo resource Ids. 
     * Each photo is displayed as an image. This adapter supports clearing the
     * list of photos and adding a new photo.
     *
     */
	class PhotoAdapter : BaseAdapter
	{
		Context self;

		int[] mPhotoPool = {
			Resource.Drawable.sample_thumb_0, Resource.Drawable.sample_thumb_1, Resource.Drawable.sample_thumb_2,
			Resource.Drawable.sample_thumb_3, Resource.Drawable.sample_thumb_4, Resource.Drawable.sample_thumb_5,
			Resource.Drawable.sample_thumb_6, Resource.Drawable.sample_thumb_7};

		List<int> mPhotos = new List<int>();

		public PhotoAdapter (Context s)
		{
			self = s;
		}

		public override int Count {
			get {
				return mPhotos.Count;
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
			// Make an ImageView to show a photo
			ImageView i = new ImageView (self);

			i.SetImageResource (mPhotos[position]);
			i.SetAdjustViewBounds (true);
			i.LayoutParameters = new AbsListView.LayoutParams (AbsListView.LayoutParams.WrapContent,
			                                                   AbsListView.LayoutParams.WrapContent);
			// Give it a nice background
			i.SetBackgroundResource (Resource.Drawable.picture_frame);
			return i;
		}

		public void ClearPhotos()
		{
			mPhotos.Clear ();
			NotifyDataSetChanged ();
		}

		public void AddPhotos()
		{
			Random gen = new Random ();

			int whichPhoto = (int)Math.Round (gen.NextDouble() * (mPhotoPool.Length - 1));
			int newPhoto = mPhotoPool[whichPhoto];
			mPhotos.Add (newPhoto);
			NotifyDataSetChanged();
		}
	}
}

