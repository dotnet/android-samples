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
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	// This activity demonstrates how to use an {@link android.view.ActionProvider}
	// for adding functionality to the Action Bar. In particular this demo is adding
	// a menu item with ShareActionProvider as its action provider. The
	// ShareActionProvider is responsible for managing the UI for sharing actions.
	[Activity (Label = "@string/action_bar_share_action_provider",
		Name = "monodroid.apidemo.ActionBarShareActionProviderActivity")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ActionBarShareActionProviderActivity : Activity
	{
		private string shared_file_name = "shared.png";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			CopyPrivateRawResourceToPubliclyAccessibleFile ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate the menu
			MenuInflater.Inflate (Resource.Menu.action_bar_share_action_provider, menu);

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

		void CopyPrivateRawResourceToPubliclyAccessibleFile ()
		{
			Stream inputStream = null;
			Stream outputStream = null;

			try {
				inputStream =  Resources.OpenRawResource (Resource.Raw.robot);
				outputStream = OpenFileOutput (shared_file_name, FileCreationMode.WorldReadable | FileCreationMode.Append);
				byte[] buffer = new byte[1024];
				int length = 0;
				try {
					while ((length = inputStream.Read (buffer, 0, buffer.Length)) > 0) {
						outputStream.Write (buffer, 0, length);
					}
				} catch (IOException) {
					/* ignore */
				}
			} catch (FileNotFoundException) {
				/* ignore */
			} finally {
				try {
					inputStream.Close ();
				} catch (IOException) {
					/* ignore */
				}
				try {
					outputStream.Close ();
				} catch (IOException) {
					/* ignore */
				}
			}
		}
	}
}