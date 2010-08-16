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
	public class NoteEditorActivity : Activity
	{
		int count = 0;

		public NoteEditorActivity (IntPtr handle)
			: base (handle)
		{
		}

		private static string tag = "Notes";

		/**
		 * Standard projection for the interesting columns of a normal note.
		 */
		private static string[] PROJECTION = new String[] {
			Notes._ID, // 0
			Notes.NOTE, // 1
		};

		/** The index of the note column */
		private static int COLUMN_INDEX_NOTE = 1;

		// This is our state data that is stored when freezing.
		private static String ORIGINAL_CONTENT = "origContent";

		// Identifiers for our menu items.
		private const int REVERT_ID = MenuConsts.FIRST;
		private const int DISCARD_ID = MenuConsts.FIRST + 1;
		private const int DELETE_ID = MenuConsts.FIRST + 2;

		// The different distinct states the activity can be run in.
		private static int STATE_EDIT = 0;
		private static int STATE_INSERT = 1;

		private int mState;
		private bool mNoteOnly = false;
		private Android.Net.Uri mUri;
		private Cursor mCursor;
		private EditText mText;
		private String mOriginalContent;

		/**
		 * A custom EditText that draws lines between each line of text that is displayed.
		 */
		public class LinedEditText : EditText
		{
			private Rect mRect;
			private Paint mPaint;

			// we need this constructor for LayoutInflater
			public LinedEditText (Context context, AttributeSet attrs)
				: base (context, attrs)
			{
				mRect = new Rect ();
				mPaint = new Paint ();
				mPaint.SetStyle (Android.Graphics.Paint.Style.STROKE);
				mPaint.Color = Color.LTGRAY;
			}


			protected override void OnDraw (Canvas canvas)
			{
				int count = LineCount; ;
				Rect r = mRect;
				Paint paint = mPaint;

				for (int i = 0; i < count; i++) {
					int baseline = GetLineBounds (i, r);

					canvas.DrawLine (r.Left, baseline + 1, r.Right, baseline + 1, paint);
				}

				base.OnDraw (canvas);
			}
		}


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Intent intent = Intent;

			// Do some setup based on the action being performed.

			string action = intent.Action;

			if (Intent.ACTION_EDIT == action) {
				// Requested to edit: set that state, and the data being edited.
				mState = STATE_EDIT;
				mUri = intent.Data;
			} else if (Intent.ACTION_INSERT == action) {
				// Requested to insert: set that state, and create a new entry
				// in the container.
				mState = STATE_INSERT;
				mUri = ContentResolver.Insert (intent.Data, null);

				// If we were unable to create a new note, then just finish
				// this activity.  A RESULT_CANCELED will be sent back to the
				// original activity if they requested a result.
				if (mUri == null) {
					Log.E (tag, "Failed to insert new note into " + intent.Data);
					Finish ();
					return;
				}

				// The new entry was created, so assume all will end well and
				// set the result to be returned.
				SetResult (RESULT_OK, (new Intent ()).SetAction (mUri.ToString ()));

			} else {
				// Whoops, unknown action!  Bail.
				Log.E (tag, "Unknown action, exiting");
				Finish ();
				return;
			}

			// Set the layout for this activity.  You can find it in res/layout/note_editor.xml
			SetContentView (R.layout.note_editor);

			// The text view for our note, identified by its ID in the XML file.
			mText = (EditText)FindViewById (R.id.note);

			// Get the note!
			mCursor = ManagedQuery (mUri, PROJECTION, null, null, null);

			// If an instance of this activity had previously stopped, we can
			// get the original text it started with.
			if (savedInstanceState != null) {
				mOriginalContent = savedInstanceState.GetString (ORIGINAL_CONTENT);
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// If we didn't have any trouble retrieving the data, it is now
			// time to get at the stuff.
			if (mCursor != null) {
				// Make sure we are at the one and only row in the cursor.
				mCursor.MoveToFirst ();

				// Modify our overall title depending on the mode we are running in.
				if (mState == STATE_EDIT) {
					Title = GetText (R.@string.title_edit);
				} else if (mState == STATE_INSERT) {
					Title = GetText (R.@string.title_create);
				}

				// This is a little tricky: we may be resumed after previously being
				// paused/stopped.  We want to put the new text in the text view,
				// but leave the user where they were (retain the cursor position
				// etc).  This version of setText does that for us.
				String note = mCursor.GetString (COLUMN_INDEX_NOTE);
				mText.SetTextKeepState (note);

				// If we hadn't previously retrieved the original text, do so
				// now.  This allows the user to revert their changes.
				if (mOriginalContent == null) {
					mOriginalContent = note;
				}

			} else {
				Title = GetText (R.@string.error_title);
				mText.Text = GetText (R.@string.error_message);
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			// Save away the original text, so we still have it if the activity
			// needs to be killed while paused.
			outState.PutString (ORIGINAL_CONTENT, mOriginalContent);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// The user is going somewhere else, so make sure their current
			// changes are safely saved away in the provider.  We don't need
			// to do this if only editing.
			if (mCursor != null) {
				String text = mText.Text.ToString ();
				int length = text.Length;

				// If this activity is finished, and there is no text, then we
				// do something a little special: simply delete the note entry.
				// Note that we do this both for editing and inserting...  it
				// would be reasonable to only do it when inserting.
				if (IsFinishing && (length == 0) && !mNoteOnly) {
					SetResult (RESULT_CANCELED);
					DeleteNote ();

					// Get out updates into the provider.
				} else {
					ContentValues values = new ContentValues ();

					// This stuff is only done when working with a full-fledged note.
					if (!mNoteOnly) {
						// Bump the modification time to now.
						//TODO
						//values.Put (NotePad.Notes.MODIFIED_DATE, DateTime.Now.Millisecond);

						// If we are creating a new note, then we want to also create
						// an initial title for it.
						if (mState == STATE_INSERT) {
							String title = text.Substring (0, Math.Min (30, length));
							if (length > 30) {
								int lastSpace = title.LastIndexOf (' ');
								if (lastSpace > 0) {
									title = title.Substring (0, lastSpace);
								}
							}
							values.Put (Notes.TITLE, title);
						}
					}

					// Write our text back into the provider.
					values.Put (Notes.NOTE, text);

					// Commit all of our changes to persistent storage. When the update completes
					// the content provider will notify the cursor of the change, which will
					// cause the UI to be updated.
					ContentResolver.Update (mUri, values, null, null);
				}
			}
		}

		public override bool OnCreateOptionsMenu (Menu menu)
		{
			base.OnCreateOptionsMenu (menu);

			// Build the menus that are shown when editing.
			if (mState == STATE_EDIT) {
				menu.Add (0, REVERT_ID, 0, R.@string.menu_revert)
					.SetShortcut ('0', 'r')
					.SetIcon (android.R.drawable.ic_menu_revert);
				if (!mNoteOnly) {
					menu.Add (0, DELETE_ID, 0, R.@string.menu_delete)
						.SetShortcut ('1', 'd')
						.SetIcon (android.R.drawable.ic_menu_delete);
				}

				// Build the menus that are shown when inserting.
			} else {
				menu.Add (0, DISCARD_ID, 0, R.@string.menu_discard)
					.SetShortcut ('0', 'd')
					.SetIcon (android.R.drawable.ic_menu_delete);
			}

			// If we are working on a full note, then append to the
			// menu items for any other activities that can do stuff with it
			// as well.  This does a query on the system for any activities that
			// implement the ALTERNATIVE_ACTION for our data, adding a menu item
			// for each one that is found.
			if (!mNoteOnly) {
				Intent intent = new Intent (null, Intent.Data);
				intent.AddCategory (Intent.CATEGORY_ALTERNATIVE);
				menu.AddIntentOptions (MenuConsts.CATEGORY_ALTERNATIVE, 0, 0,
					new ComponentName (this, NoteEditor.@class), null, intent, 0, null);
			}

			return true;
		}

		public override bool OnOptionsItemSelected (MenuItem item)
		{
			// Handle all of the possible menu actions.
			switch (item.ItemId) {
				case DELETE_ID:
					DeleteNote ();
					Finish ();
					break;
				case DISCARD_ID:
					CancelNote ();
					break;
				case REVERT_ID:
					CancelNote ();
					break;
			}
			return base.OnOptionsItemSelected (item);
		}

		/**
		 * Take care of canceling work on a note.  Deletes the note if we
		 * had created it, otherwise reverts to the original text.
		 */
		private void CancelNote ()
		{
			if (mCursor != null) {
				if (mState == STATE_EDIT) {
					// Put the original note text back into the database
					mCursor.Close ();
					mCursor = null;
					ContentValues values = new ContentValues ();
					values.Put (Notes.NOTE, mOriginalContent);
					ContentResolver.Update (mUri, values, null, null);
				} else if (mState == STATE_INSERT) {
					// We inserted an empty note, make sure to delete it
					DeleteNote ();
				}
			}
			SetResult (RESULT_CANCELED);
			Finish ();
		}

		/**
		 * Take care of deleting a note.  Simply deletes the entry.
		 */
		private void DeleteNote ()
		{
			if (mCursor != null) {
				mCursor.Close ();
				mCursor = null;
				ContentResolver.Delete (mUri, null, null);
				mText.Text = "";
			}
		}
	}
}

