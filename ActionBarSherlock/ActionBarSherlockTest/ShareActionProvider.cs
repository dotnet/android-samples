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

// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using ShareActionProvider = Xamarin.ActionbarSherlockBinding.Widget.ShareActionProvider;
using Uri = Android.Net.Uri;

namespace Mono.ActionbarsherlockTest
{
	/**
 * This activity demonstrates how to use an {@link android.view.ActionProvider}
 * for adding functionality to the Action Bar. In particular this demo is adding
 * a menu item with ShareActionProvider as its action provider. The
 * ShareActionProvider is responsible for managing the UI for sharing actions.
 */
	[Activity (Label = "@string/share_action_providers")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class ShareActionProviders : SherlockActivity
	{
		private const String SHARED_FILE_NAME = "shared.png";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.text);
			FindViewById<TextView> (Resource.Id.text).SetText (Resource.String.share_action_providers_content);
			CopyPrivateRawResourceToPubliclyAccessibleFile ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate your menu.
			SupportMenuInflater.Inflate (Resource.Menu.share_action_provider, menu);

			// Set file with share history to the provider and set the share intent.
			var actionItem = menu.FindItem (Resource.Id.menu_item_share_action_provider_action_bar);
			var actionProvider = (ShareActionProvider)actionItem.ActionProvider;
			actionProvider.SetShareHistoryFileName (ShareActionProvider.DefaultShareHistoryFileName);
			// Note that you can set/change the intent any time,
			// say when the user has selected an image.
			actionProvider.SetShareIntent (CreateShareIntent ());

			//XXX: For now, ShareActionProviders must be displayed on the action bar
			// Set file with share history to the provider and set the share intent.
			//MenuItem overflowItem = menu.findItem(R.id.menu_item_share_action_provider_overflow);
			//ShareActionProvider overflowProvider =
			//    (ShareActionProvider) overflowItem.getActionProvider();
			//overflowProvider.setShareHistoryFileName(
			//    ShareActionProvider.DEFAULT_SHARE_HISTORY_FILE_NAME);
			// Note that you can set/change the intent any time,
			// say when the user has selected an image.
			//overflowProvider.setShareIntent(createShareIntent());

			return true;
		}

		/**
     * Creates a sharing {@link Intent}.
     *
     * @return The sharing intent.
     */
		private Intent CreateShareIntent ()
		{
			Intent shareIntent = new Intent (Intent.ActionSend);
			shareIntent.SetType ("image/*");
			Uri uri = Uri.FromFile (GetFileStreamPath ("shared.png"));
			shareIntent.PutExtra (Intent.ExtraStream, uri);
			return shareIntent;
		}

		/**
     * Copies a private raw resource content to a publicly readable
     * file such that the latter can be shared with other applications.
     */
		private void CopyPrivateRawResourceToPubliclyAccessibleFile ()
		{
			Stream inputStream = null;
			Stream outputStream = null;
			try {
				inputStream = Resources.OpenRawResource (Resource.Raw.robot);
				outputStream = OpenFileOutput (SHARED_FILE_NAME, FileCreationMode.WorldReadable | FileCreationMode.Append);
				byte[] buffer = new byte[1024];
				int length = 0;
				try {
					while ((length = inputStream.Read(buffer, 0, 1024)) > 0) {
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
				} catch (IOException ioe) {
					/* ignore */
				}
				try {
					outputStream.Close ();
				} catch (IOException ioe) {
					/* ignore */
				}
			}
		}
	}
}

