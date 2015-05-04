/*
* Copyright (C) 2012 The Android Open Source Project
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

namespace AdvancedImmersiveMode
{
	/**
 	* Demonstrates how to update the app's UI by toggling immersive mode.
 	* Checkboxes are also made available for toggling other UI flags which can
 	* alter the behavior of immersive mode.
 	*/
	public class AdvancedImmersiveModeFragment : Android.Support.V4.App.Fragment
	{
		public static readonly String TAG = "AdvancedImmersiveModeFragment";
		public CheckBox mHideNavCheckbox;
		public CheckBox mHideStatusBarCheckBox;
		public CheckBox mImmersiveModeCheckBox;
		public CheckBox mImmersiveModeStickyCheckBox;
		public CheckBox mLowProfileCheckBox;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			View decorView = Activity.Window.DecorView;
			var parentView = (ViewGroup)Activity.Window.DecorView
				.FindViewById (Resource.Id.sample_main_layout);

			mLowProfileCheckBox = new CheckBox (Activity);
			mLowProfileCheckBox.Text = "Enable Low Profile mode.";
			parentView.AddView (mLowProfileCheckBox);

			mHideNavCheckbox = new CheckBox (Activity);
			mHideNavCheckbox.Checked = true;
			mHideNavCheckbox.Text = "Hide Navigation bar";
			parentView.AddView (mHideNavCheckbox);

			mHideStatusBarCheckBox = new CheckBox (Activity);
			mHideStatusBarCheckBox.Checked = true;
			mHideStatusBarCheckBox.Text = "Hide Status Bar";
			parentView.AddView (mHideStatusBarCheckBox);

			mImmersiveModeCheckBox = new CheckBox (Activity);
			mImmersiveModeCheckBox.Text = "Enable Immersive Mode.";
			parentView.AddView (mImmersiveModeCheckBox);

			mImmersiveModeStickyCheckBox = new CheckBox (Activity);
			mImmersiveModeStickyCheckBox.Text = "Enable Immersive Mode (Sticky)";
			parentView.AddView (mImmersiveModeStickyCheckBox);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.sample_action) {
				ToggleImmersiveMode ();
			}
			return true;
		}

		/**
     	* Detects and toggles immersive mode (also known as "hidey bar" mode).
     	*/
		public void ToggleImmersiveMode ()
		{
			// BEGIN_INCLUDE (get_current_ui_flags)
			// The "Decor View" is the parent view of the Activity.  It's also conveniently the easiest
			// one to find from within a fragment, since there's a handy helper method to pull it, and
			// we don't have to bother with picking a view somewhere deeper in the hierarchy and calling
			// "findViewById" on it.
			View decorView = Activity.Window.DecorView;
			var uiOptions = (int)decorView.SystemUiVisibility;
			var newUiOptions = (int)uiOptions;
			// END_INCLUDE (get_current_ui_flags)

			// BEGIN_INCLUDE (toggle_lowprofile_mode)
			// Low profile mode doesn't resize the screen at all, but it covers the nav & status bar
			// icons with black so they're less distracting.  Unlike "full screen" and "hide nav bar,"
			// this mode doesn't interact with immersive mode at all, but it's instructive when running
			// this sample to observe the differences in behavior.
			if (mLowProfileCheckBox.Checked) {
				newUiOptions |= (int)SystemUiFlags.LowProfile;
			} else {
				newUiOptions &= ~(int)SystemUiFlags.LowProfile;
			}
			// END_INCLUDE (toggle_lowprofile_mode)

			//  BEGIN_INCLUDE (toggle_fullscreen_mode)
			// When enabled, this flag hides non-critical UI, such as the status bar,
			// which usually shows notification icons, battery life, etc
			// on phone-sized devices.  The bar reappears when the user swipes it down.  When immersive
			// mode is also enabled, the app-drawable area expands, and when the status bar is swiped
			// down, it appears semi-transparently and slides in over the app, instead of pushing it
			// down.
			if (mHideStatusBarCheckBox.Checked) {
				newUiOptions |= (int)SystemUiFlags.Fullscreen;
			} else {
				newUiOptions &= ~(int)SystemUiFlags.Fullscreen;
			}
			// END_INCLUDE (toggle_fullscreen_mode)

			// BEGIN_INCLUDE (toggle_hidenav_mode)
			// When enabled, this flag hides the black nav bar along the bottom,
			// where the home/back buttons are.  The nav bar normally instantly reappears
			// when the user touches the screen.  When immersive mode is also enabled, the nav bar
			// stays hidden until the user swipes it back.
			if (mHideNavCheckbox.Checked) {
				newUiOptions |= (int)SystemUiFlags.HideNavigation;
			} else {
				newUiOptions &= ~(int)SystemUiFlags.HideNavigation;
			}
			// END_INCLUDE (toggle_hidenav_mode)

			//BEGIN_INCLUDE (toggle_immersive_mode)
			// Immersive mode doesn't do anything without at least one of the previous flags
			// enabled.  When enabled, it allows the user to swipe the status and/or nav bars
			// off-screen.  When the user swipes the bars back onto the screen, the flags are cleared
			// and immersive mode is automatically disabled.
			if (mImmersiveModeCheckBox.Checked) {
				newUiOptions |= (int)SystemUiFlags.Immersive;
			} else {
				newUiOptions &= ~(int)SystemUiFlags.Immersive;
			}
			// END_INCLUDE (toggle_immersive_mode)

			// BEGIN_INCLUDE (toggle_immersive_mode_sticky)
			// There's actually two forms of immersive mode, normal and "sticky".  Sticky immersive mode
			// is different in 2 key ways:
			//
			// * Uses semi-transparent bars for the nav and status bars
			// * This UI flag will *not* be cleared when the user interacts with the UI.
			//   When the user swipes, the bars will temporarily appear for a few seconds and then
			//   disappear again.
			if (mImmersiveModeStickyCheckBox.Checked) {
				newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;
			} else {
				newUiOptions &= ~(int)SystemUiFlags.ImmersiveSticky;
			}
			// END_INCLUDE (toggle_immersive_mode_sticky)

			// BEGIN_INCLUDE (set_ui_flags)
			//Set the new UI flags.
			decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
			Log.Info (TAG, "Current height: " + decorView.Height + ", width: " + decorView.Width);
			// END_INCLUDE (set_ui_flags)
		}
	}
}

