/*
 * Copyright (C) 2009 The Android Open Source Project
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
 	 * Example of using a custom animation when transitioning between activities.
 	 */
	[Activity (Label = "@string/activity_animation")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class AnimationDemo : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_animation);

			// Watch for button clicks.
			var button = FindViewById <Button> (Resource.Id.fade_animation);
			button.Click += OnFadeClicked;

			button = FindViewById <Button> (Resource.Id.zoom_animation);
			button.Click += OnZoomClicked;

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean) {
				button = FindViewById <Button> (Resource.Id.modern_fade_animation);
				button.Click += OnModernFadeClicked;

				button = FindViewById <Button> (Resource.Id.modern_zoom_animation);
				button.Click += OnModernZoomClicked;;

				button = FindViewById <Button> (Resource.Id.scale_up_animation);
				button.Click += OnScaleUpClicked;

				button = FindViewById <Button> (Resource.Id.zoom_thumbnail_animation);
				button.Click += OnZoomThumbnailClicked;
			} else {
				FindViewById (Resource.Id.modern_fade_animation).Enabled = false;
				FindViewById (Resource.Id.modern_zoom_animation).Enabled = false;;
				FindViewById (Resource.Id.scale_up_animation).Enabled = false;;
				FindViewById (Resource.Id.zoom_thumbnail_animation).Enabled = false;
			}
		}

		void OnFadeClicked (object sender, EventArgs e)
		{
			// Request the next activity transition (here starting a new one).
			StartActivity (new Intent (this, typeof (Controls1)));

			// Supply a custom animation.  This one will just fade the new
			// activity on top.  Note that we need to also supply an animation
			// (here just doing nothing for the same amount of time) for the
			// old activity to prevent it from going away too soon.
			OverridePendingTransition(Resource.Animation.fade, Resource.Animation.hold);
		}

		void OnZoomClicked (object sender, EventArgs e)
		{
			// Request the next activity transition (here starting a new one).
			StartActivity (new Intent (this, typeof (Controls1)));
			// This is a more complicated animation, involving transformations
			// on both this (exit) and the new (enter) activity.  Note how for
			// the duration of the animation we force the exiting activity
			// to be Z-ordered on top (even though it really isn't) to achieve
			// the effect we want.
			OverridePendingTransition(Resource.Animation.zoom_enter, Resource.Animation.zoom_exit);
		}

		void OnModernFadeClicked (object sender, EventArgs e)
		{
			// Create the desired custom animation, involving transformations
			// on both this (exit) and the new (enter) activity.  Note how for
			// the duration of the animation we force the exiting activity
			// to be Z-ordered on top (even though it really isn't) to achieve
			// the effect we want.
			ActivityOptions opts = ActivityOptions.MakeCustomAnimation (this, Resource.Animation.fade, 
			                                                            Resource.Animation.hold);
			// Request the activity be started, using the custom animation options.
			StartActivity (new Intent (this, typeof (AlertDialogSamples)), opts.ToBundle ());
		}

		void OnModernZoomClicked (object sender, EventArgs e)
		{
			// Create a more complicated animation, involving transformations
			// on both this (exit) and the new (enter) activity.  Note how for
			// the duration of the animation we force the exiting activity
			// to be Z-ordered on top (even though it really isn't) to achieve
			// the effect we want.
			ActivityOptions opts = ActivityOptions.MakeCustomAnimation(this, Resource.Animation.zoom_enter,
			                                                           Resource.Animation.zoom_enter);
			// Request the activity be started, using the custom animation options.
			StartActivity (new Intent (this, typeof (AlertDialogSamples)), opts.ToBundle ());
		}

		void OnScaleUpClicked (object sender, EventArgs e)
		{
			View v = (View)sender;
			// Create a scale-up animation that originates at the button
			// being pressed.

			ActivityOptions opts = ActivityOptions.MakeScaleUpAnimation (v, 0, 0, v.Width, v.Height);

			// Request the activity be started, using the custom animation options.
			StartActivity (new Intent (this, typeof (AlertDialogSamples)), opts.ToBundle ());
		}

		void OnZoomThumbnailClicked (object sender, EventArgs e)
		{
			View v = (View)sender;
			// Create a thumbnail animation.  We are going to build our thumbnail
			// just from the view that was pressed.  We make sure the view is
			// not selected, because by the time the animation starts we will
			// have finished with the selection of the tap.
			v.DrawingCacheEnabled = true;
			v.Pressed = false;
			v.RefreshDrawableState ();
			Bitmap bm = v.DrawingCache;
			Canvas c = new Canvas (bm);
			c.DrawARGB (255, 255, 0, 0);
			ActivityOptions opts = ActivityOptions.MakeThumbnailScaleUpAnimation (v, bm, 0, 0);

			// Request the activity be started, using the custom animation options.
			StartActivity (new Intent (this, typeof (AlertDialogSamples)), opts.ToBundle ());
			v.DrawingCacheEnabled = false;
		}
	}
}

