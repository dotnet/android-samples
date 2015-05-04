/*
 * Copyright (C) 2011 The Android Open Source Project
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

// This sample only works on Android API 14+
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
using Android.Provider;

namespace MonoDroid.ApiDemo
{
	/**
 	* This activity demonstrates how to implement an {@link android.view.ActionProvider}
	* for adding functionality to the Action Bar. In particular this demo creates an
 	* ActionProvider for launching the system settings and adds a menu item with that
 	* provider.
 	*/
	[Activity (Label = "@string/action_bar_settings_action_provider", 
		Name = "monodroid.apidemo.ActionBarSettingsActionProviderActivity")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class ActionBarSettingsActionProviderActivity : Activity
	{
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.action_bar_settings_action_provider, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// If this callback does not handle the item click, onPerformDefaultAction
			// of the ActionProvider is invoked. Hence, the provider encapsulates the
			// complete functionality of the menu item.
			Toast.MakeText (this, Resource.String.action_bar_settings_action_provider_no_handling,
			                ToastLength.Short).Show ();
			return false;
		}
	}

	[Register ("monodroid.apidemo.SettingsActionProvider")]
	public class SettingsActionProvider : ActionProvider
	{
		/** An intent for launching the system settings. */
		static Intent sSettingsIntent = new Intent (Settings.ActionSettings);
		/** Context for accessing resources. */
		Context mContext;

		/**
        * Creates a new instance.
        *
        * @param context Context for accessing resources.
        */
		public SettingsActionProvider (Context context) : base (context)
		{
			mContext = context;
		}

		public override View OnCreateActionView ()
		{
			// Inflate the action view to be shown on the action bar.
			LayoutInflater layoutInflater = LayoutInflater.From (mContext);
			View view = layoutInflater.Inflate (Resource.Layout.action_bar_settings_action_provider, null);

			ImageButton button = view.FindViewById <ImageButton> (Resource.Id.button);

			// Attach a click listener for launching the system settings.
			button.Click += delegate {
				mContext.StartActivity (sSettingsIntent);
			};

			return view;
		}

		public override bool OnPerformDefaultAction ()
		{
			// This is called if the host menu item placed in the overflow menu of the
			// action bar is clicked and the host activity did not handle the click.
			mContext.StartActivity (sSettingsIntent);
			return true;
		}
	}
}