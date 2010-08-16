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
using Android.OS;
using Android.Provider;

namespace Mono.Samples.Notepad
{
	public class NotesLiveFolder : Activity
	{
		/**
		* The URI for the Notes Live Folder content provider.
		*/
		public static Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse ("content://"
			+ Notes.AUTHORITY + "/live_folders/notes");

		public static Android.Net.Uri NOTE_URI = Android.Net.Uri.Parse ("content://"
			+ Notes.AUTHORITY + "/notes/#");

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Intent intent = Intent;
			String action = intent.Action;

			if (LiveFolders.ACTION_CREATE_LIVE_FOLDER == action) {
				// Build the live folder intent.
				Intent liveFolderIntent = new Intent ();

				liveFolderIntent.SetData (CONTENT_URI);
				liveFolderIntent.PutExtra (LiveFolders.EXTRA_LIVE_FOLDER_NAME,
					GetString (R.@string.live_folder_name));
				liveFolderIntent.PutExtra (LiveFolders.EXTRA_LIVE_FOLDER_ICON,
					Intent.ShortcutIconResource.FromContext (this,
						R.drawable.live_folder_notes));
				liveFolderIntent.PutExtra (LiveFolders.EXTRA_LIVE_FOLDER_DISPLAY_MODE,
					LiveFolders.DISPLAY_MODE_LIST);
				liveFolderIntent.PutExtra (LiveFolders.EXTRA_LIVE_FOLDER_BASE_INTENT,
					new Intent (Intent.ACTION_EDIT, NOTE_URI));

				// The result of this activity should be a live folder intent.
				SetResult (RESULT_OK, liveFolderIntent);
			} else {
				SetResult (RESULT_CANCELED);
			}

			Finish ();
		}

	}
}
