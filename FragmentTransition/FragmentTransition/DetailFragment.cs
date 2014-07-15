/*
 * Copyright 2014 The Android Open Source Project
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using Android.Transitions;
using Android.Support.V4.App;

using CommonSampleLibrary;

namespace FragmentTransition
{
	public class DetailFragment : Android.Support.V4.App.Fragment, Animation.IAnimationListener
	{
		public const string TAG = "DetailFragment";
		public const string ARG_RESOURCE_ID = "resource_id";
		public const string ARG_TITLE = "title";
		public const string ARG_X = "x";
		public const string ARG_Y = "y";
		public const string ARG_WIDTH = "width";
		public const string ARG_HEIGHT = "height";


		/**
	     * Create a new instance of DetailFragment.
	     * @param resourceId The resource ID of the Drawable image to show
	     * @param title The title of the image
	     * @param x The horizontal position of the grid item in pixel
	     * @param y The vertical position of the grid item in pixel
	     * @param width The width of the grid item in pixel
	     * @param height The height of the grid item in pixel
	     * @return a new instance of DetailFragment
	     */
		public static DetailFragment NewInstance (int resourceId, string title, int x, int y, int width, int height)
		{
			var fragment = new DetailFragment ();
			var args = new Bundle ();
			args.PutInt (ARG_RESOURCE_ID, resourceId);
			args.PutString (ARG_TITLE, title);
			args.PutInt (ARG_X, x);
			args.PutInt (ARG_Y, y);
			args.PutInt (ARG_WIDTH, width);
			args.PutInt (ARG_HEIGHT, height);
			fragment.Arguments = args;
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_detail, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			var root = (FrameLayout)view;
			var context = view.Context;
			System.Diagnostics.Debug.Assert (context != null);
			// This is how the fragment looks at first. Since the transition is one-way, we don't need to make
			// this a Scene.
			var item = LayoutInflater.From (context).Inflate (Resource.Layout.item_meat_grid, root, false);
			System.Diagnostics.Debug.Assert (item != null);
			Bind (item);
			// We adjust the position of the initial image with LayoutParams using the values supplied
			// as the fragment arguments.
			Bundle args = this.Arguments;
			FrameLayout.LayoutParams param = null;
			if (args != null) {
				param = new FrameLayout.LayoutParams (
					args.GetInt (ARG_WIDTH), args.GetInt (ARG_HEIGHT));
				param.TopMargin = args.GetInt (ARG_Y);
				param.LeftMargin = args.GetInt (ARG_X);
			}
			root.AddView (item, param);
		}
		public override void OnResume()
		{
			base.OnResume();
		}
		/**
		 * Bind the views inside of parent with the fragment arguments.
	     *
	     * @param parent The parent of views to bind.
	     */
		private void Bind (View parent)
		{
			Bundle args = Arguments;
			if (args == null) {
				return;
			}
			var image = parent.FindViewById<ImageView> (Resource.Id.image);
			image.SetImageResource (args.GetInt (ARG_RESOURCE_ID));
			var title = parent.FindViewById<TextView> (Resource.Id.title);
			title.Text = args.GetString (ARG_TITLE);
		}

		public override Animation OnCreateAnimation (int transit, bool enter, int nextAnim)
		{
			Animation animation = AnimationUtils.LoadAnimation (this.Activity,
				                      enter ? Android.Resource.Animation.FadeIn : Android.Resource.Animation.FadeOut);
			// We bind a listener for the fragment transaction. We only bind it when
			// this fragment is entering.
			if (animation != null && enter) {
				animation.SetAnimationListener (this);
			}
			return animation;
		}

		public void OnAnimationStart (Animation animation)
		{
			// This method is called at the end of the animation for the fragment transaction.
			// There is nothing we need to do in this sample.
		}

		public void OnAnimationEnd (Animation animation)
		{
			// This method is called at the end of the animation for the fragment transaction,
			// which is perfect time to start our Transition.
			Log.Info (TAG, "Fragment animation ended. Starting a Transition.");
			Scene scene = Scene.GetSceneForLayout ((ViewGroup)this.View,
				              Resource.Layout.fragment_detail_content, this.Activity);
			TransitionManager.Go (scene);
			// Note that we need to bind views with data after we call TransitionManager.go().
			Bind (scene.SceneRoot);
		}

		public void OnAnimationRepeat (Animation animation)
		{
			// This method is never called in this sample because the animation doesn't repeat.
		}
	}
}