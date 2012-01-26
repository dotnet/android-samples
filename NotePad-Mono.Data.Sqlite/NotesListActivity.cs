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
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.Notepad
{
	// Displays a list of notes.
	[Activity (MainLauncher = true, Label = "@string/app_name", Icon = "@drawable/icon")]
	public class NotesListActivity : ListActivity
	{
		// Menu item ids
		public const int MENU_ITEM_DELETE = Menu.First;
		public const int MENU_ITEM_INSERT = Menu.First + 1;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetDefaultKeyMode (DefaultKey.Shortcut);

			// Inform the list we provide context menus for items
			ListView.SetOnCreateContextMenuListener (this);

			PopulateList ();
		}

		private void PopulateList ()
		{
			// Retrieve all our notes and put them in the list
			var notes = NoteRepository.GetAllNotes ();
			//var adapter = new ArrayAdapter<Note> (this, Resource.Layout.noteslist_item, notes.ToList ());
			var adapter = new NoteAdapter (this, this, Resource.Layout.NoteListRow, notes.ToArray ());
			ListAdapter = adapter;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			// Menu item to Add New Note
			menu.Add (0, MENU_ITEM_INSERT, 0, Resource.String.menu_insert)
				.SetShortcut ('3', 'a')
				.SetIcon (Android.Resource.Drawable.IcMenuAdd);

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case MENU_ITEM_INSERT:
					// Launch activity to insert a new item
					var intent = new Intent (this, typeof (NoteEditorActivity));
					intent.PutExtra ("note_id", -1);

					StartActivityForResult (intent, 0);
					return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		public override void OnCreateContextMenu (IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
		{
			var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
			var note = (Note)ListAdapter.GetItem (info.Position);

			// Add a menu item to delete the note
			menu.Add (0, MENU_ITEM_DELETE, 0, Resource.String.menu_delete);
		}

		public override bool OnContextItemSelected (IMenuItem item)
		{
			var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
			var note = (Note)ListAdapter.GetItem (info.Position);

			switch (item.ItemId) {
				case MENU_ITEM_DELETE: {
						// Delete the note that the context menu is for
						NoteRepository.DeleteNote (note);
						PopulateList ();
						return true;
				}
			}

			return false;
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			var selected = (Note)ListAdapter.GetItem (position);

			// Launch activity to view/edit the currently selected item
			var intent = new Intent (this, typeof (NoteEditorActivity));
			intent.PutExtra ("note_id", selected.Id);

			StartActivityForResult (intent, 0);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			// The only thing we care about is refreshing the list
			// in case it changed
			PopulateList ();
		}
	}
}
