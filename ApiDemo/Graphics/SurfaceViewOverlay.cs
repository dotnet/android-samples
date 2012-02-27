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
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/SurfaceView Overlay")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class SurfaceViewOverlay : Activity
	{
		View mVictimContainer;
		View mVictim1;
		View mVictim2;

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
			
			SetContentView (Resource.Layout.surface_view_overlay);
			
			var glSurfaceView = (GLSurfaceView) FindViewById (Resource.Id.glsurfaceview);
			glSurfaceView.SetRenderer (new CubeRenderer (false));
			
			// Find the views whose visibility will change
			mVictimContainer = FindViewById (Resource.Id.hidecontainer);
			mVictim1 = FindViewById (Resource.Id.hideme1);
			mVictim1.Click += delegate { mVictim1.Visibility = ViewStates.Invisible; };
			mVictim2 = FindViewById (Resource.Id.hideme2);
			mVictim2.Click += delegate { mVictim2.Visibility = ViewStates.Invisible; };
			
			// Find our buttons
			Button visibleButton = (Button) FindViewById (Resource.Id.vis);
			Button invisibleButton = (Button) FindViewById (Resource.Id.invis);
			Button goneButton = (Button) FindViewById (Resource.Id.gone);
			
			// Wire each button to a click listener
			visibleButton.Click += delegate {
				mVictim1.Visibility = ViewStates.Visible;
				mVictim2.Visibility = ViewStates.Visible;
				mVictimContainer.Visibility = ViewStates.Visible;
			};
			invisibleButton.Click += delegate {
				mVictim1.Visibility = ViewStates.Invisible;
				mVictim2.Visibility = ViewStates.Invisible;
				mVictimContainer.Visibility = ViewStates.Invisible;
			};
			goneButton.Click += delegate {
				mVictim1.Visibility = ViewStates.Gone;
				mVictim2.Visibility = ViewStates.Gone;
				mVictimContainer.Visibility = ViewStates.Gone;
			};
		}
		
		protected override void OnResume ()
		{
			// Ideally a game should implement onResume() and onPause()
			// to take appropriate action when the activity looses focus
			base.OnResume ();
		}
		
		protected override void OnPause () 
		{
			// Ideally a game should implement onResume() and onPause()
			// to take appropriate action when the activity looses focus
			base.OnPause ();
		}
	}
}
