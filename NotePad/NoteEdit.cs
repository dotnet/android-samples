/*
 * Copyright (C) 2009 The Android Open Source Project
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

namespace NotePad
{
    using Android.App;
    using Android.Database;
    using Android.OS;
    using Android.Widget;
    using Java.Lang;

    [Activity(Label = "NoteEdit")]
    public class NoteEdit : Activity
    {
        private EditText titleText;
        private EditText bodyText;
        private Long rowId;
        private NotesDbAdapter dbHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.dbHelper = new NotesDbAdapter(this);
            this.dbHelper.Open();

            SetContentView(Resource.Layout.note_edit);
            SetTitle(Resource.String.edit_note);

            this.titleText = (EditText)FindViewById(Resource.Id.title);
            this.bodyText = (EditText)FindViewById(Resource.Id.body);

            var confirmButton = (Button)FindViewById(Resource.Id.confirm);

            this.rowId = ((savedInstanceState == null) ? null : savedInstanceState.GetSerializable(NotesDbAdapter.KeyRowId)) as Long;

            if (this.rowId == null)
            {
                var extras = Intent.Extras;
                this.rowId = extras != null ? new Long(extras.GetLong(NotesDbAdapter.KeyRowId))
                                        : null;
            }

            this.PopulateFields();
            confirmButton.Click += delegate
                {
                    SetResult(Result.Ok);
                    this.Finish();
                };
        }

        private void PopulateFields()
        {
            if (this.rowId == null)
            {
                return;
            }

            ICursor note = this.dbHelper.FetchNote(this.rowId.LongValue());
            this.StartManagingCursor(note);
            this.titleText.SetText(note.GetString(note.GetColumnIndexOrThrow(NotesDbAdapter.KeyTitle)), TextView.BufferType.Normal);
            this.bodyText.SetText(note.GetString(note.GetColumnIndexOrThrow(NotesDbAdapter.KeyBody)), TextView.BufferType.Normal);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            this.SaveState();
            outState.PutSerializable(NotesDbAdapter.KeyRowId, this.rowId);
        }


        protected override void OnPause()
        {
            base.OnPause();
            this.SaveState();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.PopulateFields();
        }

        private void SaveState()
        {
            string title = this.titleText.Text;
            string body = this.bodyText.Text;

            if (this.rowId == null)
            {
                long id = this.dbHelper.CreateNote(title, body);
                if (id > 0)
                {
                    this.rowId = new Long(id);
                }
            }
            else
            {
                this.dbHelper.UpdateNote(this.rowId.LongValue(), title, body);
            }
        }

    }
}