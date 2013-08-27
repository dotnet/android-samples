/*
 * Copyright (C) 2010 The Android Open Source Project
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

// This sample only works on Android API 11+
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	// This demonstrates the basics of the Action Bar and how it interoperates with the
	// standard options menu. This demo is for informative purposes only; see ActionBarUsage for
	// an example of using the Action Bar in a more idiomatic manner.
	[Activity (Label = "@string/action_bar_mechanics")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ActionBarMechanics : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// The Action Bar is a window feature. The feature must be requested
			// before setting a content view. Normally this is set automatically
			// by your Activity's theme in your manifest. The provided system
			// theme Theme.WithActionBar enables this for you. Use it as you would
			// use Theme.NoTitleBar. You can add an Action Bar to your own themes
			// by adding the element <item name="android:windowActionBar">true</item>
			// to your style definition.
			Window.RequestFeature (WindowFeatures.ActionBar);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Menu items default to never show in the action bar. On most devices this means
			// they will show in the standard options menu panel when the menu button is pressed.
			// On xlarge-screen devices a "More" button will appear in the far right of the
			// Action Bar that will display remaining items in a cascading menu.
			menu.Add (new Java.Lang.String ("Normal item"));

			var action_item = menu.Add (new Java.Lang.String ("Action Button"));

			// Items that show as actions should favor the "if room" setting, which will
			// prevent too many buttons from crowding the bar. Extra items will show in the
			// overflow area.
			action_item.SetShowAsAction (ShowAsAction.IfRoom);

			// Items that show as actions are strongly encouraged to use an icon.
			// These icons are shown without a text description, and therefore should
			// be sufficiently descriptive on their own.
			action_item.SetIcon (Android.Resource.Drawable.IcMenuShare);

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			Toast.MakeText (this, "Selected Item: " + item.TitleFormatted, ToastLength.Short).Show ();
			
			return true;
		}
	}
}