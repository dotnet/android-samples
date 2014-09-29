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
using Android.Support.V4.App;
using CommonSampleLibrary;

namespace BasicImmersiveMode
{
	public class BasicImmersiveModeFragment : Android.Support.V4.App.Fragment
	{
		public static readonly string TAG = "BasicImmersiveModeFragment";

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			View decorView = Activity.Window.DecorView;

			decorView.SystemUiVisibilityChange += delegate {
				int height = decorView.Height;
				Log.Info (TAG, "Current height: " + height);
			};
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.sample_action) {
				ToggleHideyBar ();
			}
			return true;
		}

		/**
     	* Detects and toggles immersive mode.
     	*/
		public void ToggleHideyBar ()
		{
			// BEGIN_INCLUDE (get_current_ui_flags)
			// The UI options currently enabled are represented by a bitfield.
			// getSystemUiVisibility() gives us that bitfield.
			int uiOptions = (int)Activity.Window.DecorView.SystemUiVisibility;
			int newUiOptions = uiOptions;
			// END_INCLUDE (get_current_ui_flags)

			// BEGIN_INCLUDE (toggle_ui_flags)
			bool isImmersiveModeEnabled = ((uiOptions | (int)SystemUiFlags.ImmersiveSticky) == uiOptions);
			if (isImmersiveModeEnabled) {
				Log.Info (TAG, "Turning immersive mode mode off. ");
			} else {
				Log.Info (TAG, "Turning immersive mode mode on.");
			}

			// Immersive mode: Backward compatible to KitKat (API 19).
			// Note that this flag doesn't do anything by itself, it only augments the behavior
			// of HIDE_NAVIGATION and FLAG_FULLSCREEN.  For the purposes of this sample
			// all three flags are being toggled together.
			// This sample uses the "sticky" form of immersive mode, which will let the user swipe
			// the bars back in again, but will automatically make them disappear a few seconds later.
			newUiOptions ^= (int)SystemUiFlags.HideNavigation;
			newUiOptions ^= (int)SystemUiFlags.Fullscreen;
			newUiOptions ^= (int)SystemUiFlags.ImmersiveSticky;
			Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
			//END_INCLUDE (set_ui_flags)
		}
	}
}

