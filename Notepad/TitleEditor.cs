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
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Database;
using Android.Graphics;
using Android.Util;

namespace Mono.Samples.Notepad
{
	public class TitleEditor : Activity, View.OnClickListener
	{
		/**
		* This is a special intent action that means "edit the title of a note".
		*/
		public static String EDIT_TITLE_ACTION = "com.android.notepad.action.EDIT_TITLE";

		/**
		 * An array of the columns we are interested in.
		 */
		private static String[] PROJECTION = new String[] {
			Notes._ID, // 0
			Notes.TITLE, // 1
		};

		/** Index of the title column */
		private static int COLUMN_INDEX_TITLE = 1;

		/**
		 * Cursor which will provide access to the note whose title we are editing.
		 */
		private Cursor mCursor;

		/**
		 * The EditText field from our UI. Keep track of this so we can extract the
		 * text when we are finished.
		 */
		private EditText mText;

		/**
		 * The content URI to the note that's being edited.
		 */
		private Android.Net.Uri mUri;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (R.layout.title_editor);

			// Get the uri of the note whose title we want to edit
			mUri = Intent.Data;

			// Get a cursor to access the note
			mCursor = ManagedQuery (mUri, PROJECTION, null, null, null);

			// Set up click handlers for the text field and button
			mText = (EditText)this.FindViewById (R.id.title);
			mText.SetOnClickListener (this);

			Button b = (Button)FindViewById (R.id.ok);
			b.SetOnClickListener (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Initialize the text with the title column from the cursor
			if (mCursor != null) {
				mCursor.MoveToFirst ();
				mText.Text = mCursor.GetString (COLUMN_INDEX_TITLE);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			if (mCursor != null) {
				// Write the title back to the note 
				ContentValues values = new ContentValues ();
				values.Put (Notes.TITLE, mText.Text.ToString ());
				ContentResolver.Update (mUri, values, null, null);
			}
		}

		public void onClick (View v)
		{
			// When the user clicks, just finish this activity.
			// onPause will be called, and we save our data there.
			Finish ();
		}

	}
}
