/*
 * Copyright (C) 2013 The Android Open Source Project
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
using Android.Transitions;
using Java.Interop;

namespace MonoDroid.ApiDemo
{
	/**
 	* This application demonstrates some of the capabilities and uses of the
 	* {@link android.transition transitions} APIs. Scenes and a TransitionManager
 	* are loaded from resource files and transitions are run between those scenes
 	* as well as a dynamically-configured scene.
 	*/
	[Activity (Label = "Animation/Simple Transitions", Name = "monodroid.apidemo.Transitions")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class Transitions : Activity
	{
		Scene mScene1, mScene2, mScene3;
		ViewGroup mSceneRoot;
		TransitionManager mTransitionManager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.transition);

			mSceneRoot = FindViewById <ViewGroup> (Resource.Id.sceneRoot);

			var inflater = TransitionInflater.From (this);

			// Note that this is not the only way to create a Scene object, but that
			// loading them from layout resources cooperates with the
			// TransitionManager that we are also loading from resources, and which
			// uses the same layout resource files to determine the scenes to transition
			// from/to.
			mScene1 = Scene.GetSceneForLayout (mSceneRoot, Resource.Layout.transition_scene1, this);
			mScene2 = Scene.GetSceneForLayout (mSceneRoot, Resource.Layout.transition_scene2, this);
			mScene3 = Scene.GetSceneForLayout (mSceneRoot, Resource.Layout.transition_scene3, this);
			mTransitionManager = inflater.InflateTransitionManager (Resource.Transition.transitions_mgr, mSceneRoot);
		}

		[Export]
		public void selectScene (View view)
		{
			switch (view.Id) {
			case Resource.Id.scene1:
				mTransitionManager.TransitionTo (mScene1);
				break;
			case Resource.Id.scene2:
				mTransitionManager.TransitionTo (mScene2);
				break;
			case Resource.Id.scene3:
				mTransitionManager.TransitionTo (mScene3);
				break;
			case Resource.Id.scene4:
				// scene4 is not an actual 'Scene', but rather a dynamic change in the UI,
				// transitioned to using beginDelayedTransition() to tell the TransitionManager
				// to get ready to run a transition at the next frame
				TransitionManager.BeginDelayedTransition (mSceneRoot);
				SetNewSize (Resource.Id.view1, 150, 25);
				SetNewSize (Resource.Id.view2, 150, 25);
				SetNewSize (Resource.Id.view3, 150, 25);
				SetNewSize (Resource.Id.view4, 150, 25);
				break;
			}
		}

		void SetNewSize (int id, int width, int height)
		{
			var view = FindViewById (id);
			ViewGroup.LayoutParams parms = view.LayoutParameters;
			parms.Width = width;
			parms.Height = height;
			view.LayoutParameters = parms;
		}
	}
}

