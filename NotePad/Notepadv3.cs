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
    using Android.Content;
    using Android.Database;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

    [Activity(Label = "Notepadv3", MainLauncher = true, Icon = "@drawable/icon")] // Used for the manifest
    public class Notepadv3 : ListActivity
    {
        private const int ActivityCreate = 0;
        private const int ActivityEdit = 1;

        private const int InsertId = Menu.First;
        private const int DeleteId = Menu.First + 1;

        private NotesDbAdapter dbHelper;

        // Called when the activity is first created.
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.notes_list);
            
            this.dbHelper = new NotesDbAdapter(this);
            this.dbHelper.Open();
            this.FillData();
            
            RegisterForContextMenu(ListView);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            menu.Add(0, InsertId, 0, Resource.String.menu_insert);
            return true;
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case InsertId:
                    this.CreateNote();
                    return true;
            }

            return base.OnMenuItemSelected(featureId, item);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            menu.Add(0, DeleteId, 0, Resource.String.menu_delete);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case DeleteId:
                    var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
                    this.dbHelper.DeleteNote(info.Id);
                    this.FillData();
                    return true;
            }
            return base.OnContextItemSelected(item);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var i = new Intent(this, typeof(NoteEdit));
            i.PutExtra(NotesDbAdapter.KeyRowId, id);
            StartActivityForResult(i, ActivityEdit);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.FillData();
        }

        private void FillData()
        {
            ICursor notesCursor = this.dbHelper.FetchAllNotes();
            this.StartManagingCursor(notesCursor);

            // Create an array to specify the fields we want to display in the list (only TITLE)
            var from = new[] { NotesDbAdapter.KeyTitle };

            // and an array of the fields we want to bind those fields to (in this case just text1)
            var to = new[] { Resource.Id.text1 };

            // Now create a simple cursor adapter and set it to display
            var notes =
                new SimpleCursorAdapter(this, Resource.Layout.notes_row, notesCursor, from, to);
            this.ListAdapter = notes;
        }

        private void CreateNote()
        {
            var i = new Intent(this, typeof(NoteEdit));
            this.StartActivityForResult(i, ActivityCreate);
        }
    }
}