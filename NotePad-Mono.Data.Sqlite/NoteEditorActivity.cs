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
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace Mono.Samples.Notepad
{
	[Activity (Label = "Edit Note", ScreenOrientation = ScreenOrientation.Sensor, ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation)]
	public class NoteEditorActivity : Activity
	{
		private Note note;
		private EditText text_view;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set the layout for this activity.  You can find it in res/layout/note_editor.xml
			SetContentView (Resource.Layout.NoteEditor);

			// The text view for our note, identified by its ID in the XML file.
			text_view = FindViewById<EditText> (Resource.Id.note);

			// Get the note
			var note_id = Intent.GetLongExtra ("note_id", -1);

			if (note_id < 0)
				note = new Note ();
			else
				note = NoteRepository.GetNote (note_id);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// This is a little tricky: we may be resumed after previously being
			// paused/stopped.  We want to put the new text in the text view,
			// but leave the user where they were (retain the cursor position
			// etc).  This version of setText does that for us.
			text_view.SetTextKeepState (note.Body);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// If this was a new note and no content was added, don't save it
			if (IsFinishing && note.Id == -1 && text_view.Text.Length == 0)
				return;

			// Save the note
			note.Body = text_view.Text;
			NoteRepository.SaveNote (note);
		}
	}
}

