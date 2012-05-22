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
#if __ANDROID_11__

using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace MonoDroid.ApiDemo.App
{
	// This demo shows how various action bar display option flags can be combined and their effects.
	[Activity (Label = "App/Action Bar Display Options")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ActionBarDisplayOptionsActivity : Activity, Android.App.ActionBar.ITabListener
	{
		private View custom_view;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ActionBarDisplayOptions);

			// Set up handlers for each of our buttons
			var home_as_up = FindViewById (Resource.Id.toggle_home_as_up);
			home_as_up.Click += delegate { AddActionBarFlag (ActionBarDisplayOptions.HomeAsUp); };

			var home_show_home = FindViewById (Resource.Id.toggle_show_home);
			home_show_home.Click += delegate { AddActionBarFlag (ActionBarDisplayOptions.ShowHome); };

			var home_use_logo = FindViewById (Resource.Id.toggle_use_logo);
			home_use_logo.Click += delegate { AddActionBarFlag (ActionBarDisplayOptions.UseLogo); };

			var home_show_title = FindViewById (Resource.Id.toggle_show_title);
			home_show_title.Click += delegate { AddActionBarFlag (ActionBarDisplayOptions.ShowTitle); };

			var home_show_custom = FindViewById (Resource.Id.toggle_show_custom);
			home_show_custom.Click += delegate { AddActionBarFlag (ActionBarDisplayOptions.ShowCustom); };

			var home_toggle_navigation = FindViewById (Resource.Id.toggle_navigation);
			home_toggle_navigation.Click += delegate {
				var mode = ActionBar.NavigationMode == ActionBarNavigationMode.Standard ? ActionBarNavigationMode.Tabs : ActionBarNavigationMode.Standard;
				ActionBar.NavigationMode = mode;
			};

			var cycle_gravity = FindViewById (Resource.Id.cycle_custom_gravity);
			cycle_gravity.Click += CycleGravity;

			custom_view = LayoutInflater.Inflate (Resource.Layout.ActionBarDisplayOptionsCustom, null);

			// Configure several action bar elements that will be toggled by display options.
			ActionBar.SetCustomView (custom_view, new ActionBar.LayoutParams (WindowManagerLayoutParams.WrapContent, WindowManagerLayoutParams.WrapContent));

			var tab1 = ActionBar.NewTab ();
			tab1.SetText ("Tab 1");
			tab1.SetTabListener (this);
			ActionBar.AddTab (tab1);

			var tab2 = ActionBar.NewTab ();
			tab2.SetText ("Tab 2");
			tab2.SetTabListener (this);
			ActionBar.AddTab (tab2);

			var tab3 = ActionBar.NewTab ();
			tab3.SetText ("Tab 3");
			tab3.SetTabListener (this);
			ActionBar.AddTab (tab3);
		}

		private void AddActionBarFlag (ActionBarDisplayOptions flag)
		{
			var bar = ActionBar;

			var change = bar.DisplayOptions ^ flag;
			ActionBar.SetDisplayOptions (change, flag);
		}

		private void CycleGravity (object sender, EventArgs e)
		{
			var lp = (ActionBar.LayoutParams)custom_view.LayoutParameters;

			GravityFlags new_gravity = GravityFlags.NoGravity;

			switch ((GravityFlags)lp.Gravity & GravityFlags.HorizontalGravityMask) {
				case GravityFlags.Left:
					new_gravity = GravityFlags.CenterHorizontal;
					break;
				case GravityFlags.CenterHorizontal:
					new_gravity = GravityFlags.Right;
					break;
				case GravityFlags.Right:
					new_gravity = GravityFlags.Left;
					break;
			}

			lp.Gravity = ((GravityFlags)lp.Gravity & ~GravityFlags.HorizontalGravityMask | new_gravity);
			ActionBar.SetCustomView (custom_view, lp);
		}

		#region ITabListener Members
		public void OnTabReselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public void OnTabSelected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public void OnTabUnselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}
		#endregion
	}
}
#endif