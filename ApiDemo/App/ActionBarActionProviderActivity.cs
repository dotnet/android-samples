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
#if __ANDROID_14__

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

namespace MonoDroid.ApiDemo.App
{
	// This activity demonstrates how to use an {@link android.view.ActionProvider}
	// for adding functionality to the Action Bar. In particular this demo is adding
	// a menu item with ShareActionProvider as its action provider. The
	// ShareActionProvider is responsible for managing the UI for sharing actions.
	[Activity (Label = "App/Action Bar Action Provider")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ActionBarActionProviderActivity : Activity
	{
		private string shared_file_name = "shared.png";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate the menu
			MenuInflater.Inflate (Resource.Menu.ActionBarActionProvider, menu);

			// Set file with share history to the provider and set the share intent.
			var action_item = menu.FindItem (Resource.Id.menu_item_share_action_provider_action_bar);
			var action_provider = (ShareActionProvider)action_item.ActionProvider;

			action_provider.SetShareHistoryFileName (ShareActionProvider.DefaultShareHistoryFileName);

			// Note that you can set/change the intent any time,
			// say when the user has selected an image.
			action_provider.SetShareIntent (CreateShareIntent ());

			// Set file with share history to the provider and set the share intent.
			var overflow_item = menu.FindItem (Resource.Id.menu_item_share_action_provider_overflow);
			var overflow_provider = (ShareActionProvider)overflow_item.ActionProvider;

			overflow_provider.SetShareHistoryFileName (ShareActionProvider.DefaultShareHistoryFileName);

			// Note that you can set/change the intent any time,
			// say when the user has selected an image.
			overflow_provider.SetShareIntent (CreateShareIntent ());

			return true;
		}

		private Intent CreateShareIntent ()
		{
			var shareIntent = new Intent (Intent.ActionSend);
			var uri = Android.Net.Uri.FromFile (GetFileStreamPath ("shared.png"));

			shareIntent.SetType ("image/*");
			shareIntent.PutExtra (Intent.ExtraStream, uri);

			return shareIntent;
		}
	}
}
#endif