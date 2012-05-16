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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Animation/3D Transition")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Transition3d : Activity
	{
		private ListView mPhotosList;
		private ViewGroup mContainer;
		private ImageView mImageView;

		// Names of the photos we show in the list
		private static string[] PHOTOS_NAMES = new String[] {
			"Lyon",
			"Livermore",
			"Tahoe Pier",
			"Lake Tahoe",
			"Grand Canyon",
			"Bodie"
		};

		// Resource identifiers for the photos we want to display
		private static int[] PHOTOS_RESOURCES = new int[] {
			Resource.Drawable.photo1,
			Resource.Drawable.photo2,
			Resource.Drawable.photo3,
			Resource.Drawable.photo4,
			Resource.Drawable.photo5,
			Resource.Drawable.photo6
		};

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.animations_main_screen);

			mPhotosList = (ListView)FindViewById (Android.Resource.Id.List);
			mImageView = (ImageView)FindViewById (Resource.Id.picture);
			mContainer = (ViewGroup)FindViewById (Resource.Id.container);

			// Prepare the ListView
			ArrayAdapter<String> adapter = new ArrayAdapter<String> (this,
				Android.Resource.Layout.SimpleListItem1, PHOTOS_NAMES);

			mPhotosList.Adapter = adapter;
			mPhotosList.ItemClick += OnItemClick;

			// Prepare the ImageView
			mImageView.Clickable = true;
			mImageView.Focusable = true;
			mImageView.Click += OnClick;
			//mImageView.SetOnClickListener (this);

			// Since we are caching large views, we want to keep their cache
			// between each animation
			mContainer.PersistentDrawingCache = PersistentDrawingCaches.AnimationCache;
		}

		/**
 * Setup a new 3D rotation on the container view.
 *
 * @param position the item that was clicked to show a picture, or -1 to show the list
 * @param start the start angle at which the rotation must begin
 * @param end the end angle of the rotation
 */
		private void ApplyRotation (int position, float start, float end)
		{
			// Find the center of the container
			float centerX = mContainer.Width / 2.0f;
			float centerY = mContainer.Height / 2.0f;

			// Create a new 3D rotation with the supplied parameter
			// The animation listener is used to trigger the next animation
			Rotate3dAnimation rotation =
				new Rotate3dAnimation (start, end, centerX, centerY, 310.0f, true);
			rotation.Duration = 500;
			rotation.FillAfter = true;
			rotation.Interpolator = new AccelerateInterpolator ();
			rotation.SetAnimationListener (new DisplayNextView (position, mContainer, mPhotosList, mImageView));

			mContainer.StartAnimation (rotation);
		}


		/**
	     * This class listens for the end of the first half of the animation.
	     * It then posts a new action that effectively swaps the views when the container
	     * is rotated 90 degrees and thus invisible.
	     */
		private class DisplayNextView : Java.Lang.Object, Animation.IAnimationListener
		{
			private int position;
			private ViewGroup container;
			private ListView photos_list;
			private ImageView image_view;

			public DisplayNextView (int position, ViewGroup container, ListView photosList, ImageView imageView)
			{
				this.position = position;
				this.container = container;
				photos_list = photosList;
				image_view = imageView;
			}


			#region IAnimationListener Members
			public void OnAnimationEnd (Animation animation)
			{
				container.Post (new SwapViews (position, container, photos_list, image_view));
			}

			public void OnAnimationRepeat (Animation animation)
			{
			}

			public void OnAnimationStart (Animation animation)
			{
			}
			#endregion
		}

		/**
	     * This class is responsible for swapping the views and start the second
	     * half of the animation.
	     */
		private class SwapViews : Java.Lang.Object, Java.Lang.IRunnable
		{
			private int position;
			private ViewGroup container;
			private ListView photos_list;
			private ImageView image_view;

			public SwapViews (int position, ViewGroup container, ListView photosList, ImageView imageView)
			{
				this.position = position;
				this.container = container;
				photos_list = photosList;
				image_view = imageView;
			}

			public void Run ()
			{
				float centerX = container.Width / 2.0f;
				float centerY = container.Height / 2.0f;
				Rotate3dAnimation rotation;

				if (position > -1) {
					photos_list.Visibility = ViewStates.Gone;
					image_view.Visibility = ViewStates.Visible;
					image_view.RequestFocus ();

					rotation = new Rotate3dAnimation (90, 180, centerX, centerY, 310.0f, false);
				} else {
					image_view.Visibility = ViewStates.Gone;
					photos_list.Visibility = ViewStates.Visible;
					photos_list.RequestFocus ();

					rotation = new Rotate3dAnimation (90, 0, centerX, centerY, 310.0f, false);
				}

				rotation.Duration = 500;
				rotation.FillAfter = true;
				rotation.Interpolator = new DecelerateInterpolator ();

				container.StartAnimation (rotation);
			}
		}


		#region IOnItemClickListener Members

		public void OnItemClick (object parent, AdapterView.ItemClickEventArgs args)
		{
			// Pre-load the image then start the animation
			mImageView.SetImageResource (PHOTOS_RESOURCES[args.Position]);
			ApplyRotation (args.Position, 0, 90);
		}

		#endregion

		#region IOnClickListener Members

		public void OnClick (object sender, EventArgs e)
		{
			ApplyRotation (-1, 180, 90);
		}

		#endregion
	}
}
