/*
 * Copyright (C) 2014 The Android Open Source Project
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;

using ActivitySceneTransitionBasic.ImageLoader;
using Java.Interop;

namespace ActivitySceneTransitionBasic
{
	/**
 	* Our main Activity in this sample. Displays a grid of items which an image and title. When the
 	* user clicks on an item, DetailActivity is launched, using the Activity Scene Transitions
 	* framework to animatedly do so.
 	*/
	[Activity (Label="@string/app_name", MainLauncher=true)]
	public class MainActivity : Activity, AdapterView.IOnItemClickListener
	{

		private GridView mGridView;
		private GridAdapter mAdapter;

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.grid);

			FileCache.SaveLocation = CacheDir.AbsolutePath;

			// Setup the GridView and set the adapter
			mGridView = (GridView) FindViewById (Resource.Id.grid);
			mGridView.OnItemClickListener = this;
			mAdapter = new GridAdapter(this);
			mGridView.Adapter = mAdapter;
		}

		public void OnItemClick (AdapterView parent, View view, int position, long id)
		{
			Item item = parent.GetItemAtPosition (position).JavaCast<Item>();

			// Construct an Intent as normal
			Intent intent = new Intent (this, typeof(DetailActivity));
			intent.PutExtra (DetailActivity.EXTRA_PARAM_ID, item.id);

			ActivityOptions activityOptions = ActivityOptions.MakeSceneTransitionAnimation (
				                                  this,
				// Now we provide a list of Pair items which contain the view we can transition from, and the name of the view it is 
				// transitioning to, in the launched activity
				                                  new Pair (view.FindViewById (Resource.Id.imageview_item), DetailActivity.VIEW_NAME_HEADER_IMAGE),
				                                  new Pair (view.FindViewById (Resource.Id.textview_name), DetailActivity.VIEW_NAME_HEADER_TITLE)
			                                  );
			// Now we can start the Activity, providing the activity options as a bundle
			StartActivity (intent, activityOptions.ToBundle ());
		}
	} 

	/**
     * BaseAdapter which displays items.
     */
	public class GridAdapter : BaseAdapter<Item>
	{
	
		Activity context;

		public GridAdapter(Activity context ) : base ()
		{
			this.context = context;
		}

		#region implemented abstract members of BaseAdapter

		public override int Count {
			get { return Item.ITEMS.Length; }

		}
	
		public override Item this [int position] {
			get { return Item.ITEMS [position]; }
		}



		public override long GetItemId (int position) {
			return this [position].id;
		}

		public override View GetView(int position, View view, ViewGroup viewGroup){
			if (view == null) {
				view = context.LayoutInflater.Inflate(Resource.Layout.grid_item, viewGroup, false);
			}
			Item item = this[position];

			// Load the thumbnail image
			ImageView image = view.FindViewById<ImageView>(Resource.Id.imageview_item);
			#pragma warning disable 4014
			Images.SetImageFromUrlAsync (image, item.thumbnailUrl);
			#pragma warning restore 4014

			// Set the TextView's contents
			TextView name = (TextView) view.FindViewById(Resource.Id.textview_name);
			name.SetText (item.name, TextView.BufferType.Normal);

			/**
             * As we're in an adapter we need to set each view's name dynamically, using the
             * item's ID so that the names are unique.
             */
			image.TransitionName = "grid:image:" + item.id;
			name.TransitionName = "grid:name:" + item.id;

			return view;
		}
		#endregion

	}
}

