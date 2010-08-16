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
using Java.Lang;

namespace Mono.Samples.Notepad
{

/**
 * Displays a list of notes. Will display notes from the {@link Uri}
 * provided in the intent if there is one, otherwise defaults to displaying the
 * contents of the {@link NotePadProvider}
 */
public class NotesList : ListActivity {
    private static String TAG = "NotesList";

    // Menu item ids
    public const int MENU_ITEM_DELETE = MenuConsts.FIRST;
    public const int MENU_ITEM_INSERT = MenuConsts.FIRST + 1;

    /**
     * The columns we are interested in from the database
     */
    private static String[] PROJECTION = new String[] {
            Notes._ID, // 0
            Notes.TITLE, // 1
    };

    /** The index of the title column */
    private static int COLUMN_INDEX_TITLE = 1;

    protected override void OnCreate(Bundle savedInstanceState) {
        base.OnCreate(savedInstanceState);

        SetDefaultKeyMode(DEFAULT_KEYS_SHORTCUT);

        // If no data was given in the intent (because we were started
        // as a MAIN activity), then use our default content provider.
        Intent intent = Intent;
        if (intent.Data == null) {
            intent.SetData (Notes.CONTENT_URI);
        }

        // Inform the list we provide context menus for items
        ListView.SetOnCreateContextMenuListener(this);

        // Perform a managed query. The Activity will handle closing and requerying the cursor
        // when needed.
        Cursor cursor = ManagedQuery(Intent.Data, PROJECTION, null, null,
                Notes.DEFAULT_SORT_ORDER);

        // Used to map notes entries from the database to views
        SimpleCursorAdapter adapter = new SimpleCursorAdapter(this, R.layout.noteslist_item, cursor,
                new String[] { Notes.TITLE }, new int[] { android.R.id.text1 });
        ListAdapter =adapter;
    }

    public override bool OnCreateOptionsMenu(Menu menu) {
        base.OnCreateOptionsMenu(menu);

        // This is our one standard application action -- inserting a
        // new note into the list.
        menu.Add(0, MENU_ITEM_INSERT, 0, R.@string.menu_insert)
                .SetShortcut('3', 'a')
                .SetIcon(android.R.drawable.ic_menu_add);

        // Generate any additional actions that can be performed on the
        // overall list.  In a normal install, there are no additional
        // actions found here, but this allows other applications to extend
        // our menu with their own actions.
        Intent intent = new Intent(null, Intent.Data);
        intent.AddCategory(Intent.CATEGORY_ALTERNATIVE);
        menu.AddIntentOptions(MenuConsts.CATEGORY_ALTERNATIVE, 0, 0,
                new ComponentName(this, NotesList.@class), null, intent, 0, null);

        return true;
    }

    public override bool OnPrepareOptionsMenu(Menu menu) {
        base.OnPrepareOptionsMenu(menu);
        bool haveItems = ListAdapter.Count > 0;

        // If there are any notes in the list (which implies that one of
        // them is selected), then we need to generate the actions that
        // can be performed on the current selection.  This will be a combination
        // of our own specific actions along with any extensions that can be
        // found.
        if (haveItems) {
            // This is the selected item.
            Android.Net.Uri uri = ContentUris.WithAppendedId(Intent.Data, SelectedItemId);

            // Build menu...  always starts with the EDIT action...
            Intent[] specifics = new Intent[1];
            specifics[0] = new Intent(Intent.ACTION_EDIT, uri);
            MenuItem[] items = new MenuItem[1];

            // ... is followed by whatever other actions are available...
            Intent intent = new Intent(null, uri);
            intent.AddCategory(Intent.CATEGORY_ALTERNATIVE);
            menu.AddIntentOptions(MenuConsts.CATEGORY_ALTERNATIVE, 0, 0, null, specifics, intent, 0,
                    items);

            // Give a shortcut to the edit action.
            if (items[0] != null) {
                items[0].SetShortcut('1', 'e');
            }
        } else {
            menu.RemoveGroup(MenuConsts.CATEGORY_ALTERNATIVE);
        }

        return true;
    }

    public override bool OnOptionsItemSelected(MenuItem item) {
        switch (item.ItemId) {
        case MENU_ITEM_INSERT:
            // Launch activity to insert a new item
            StartActivity(new Intent(Intent.ACTION_INSERT, Intent.Data));
            return true;
        }
        return base.OnOptionsItemSelected(item);
    }

    public override void OnCreateContextMenu (ContextMenu menu, View view, ContextMenuContextMenuInfo menuInfo)
    {
        AdapterView.AdapterContextMenuInfo info = null;
        try {
             info = (AdapterView.AdapterContextMenuInfo) menuInfo;
        } catch (ClassCastException e) {
            Log.E(TAG, "bad menuInfo", e);
            return;
        }

        Cursor cursor = (Cursor) ListAdapter.GetItem(info.Position);
        if (cursor == null) {
            // For some reason the requested item isn't available, do nothing
            return;
        }

        // Setup the menu header
        menu.SetHeaderTitle(cursor.GetString(COLUMN_INDEX_TITLE));

        // Add a menu item to delete the note
        menu.Add(0, MENU_ITEM_DELETE, 0, R.@string.menu_delete);
    }

    public override bool OnContextItemSelected(MenuItem item) {
        AdapterView.AdapterContextMenuInfo info;
        try {
             info = (AdapterView.AdapterContextMenuInfo) item.MenuInfo;
        } catch (ClassCastException e) {
            Log.E(TAG, "bad menuInfo", e);
            return false;
        }

        switch (item.ItemId) {
            case MENU_ITEM_DELETE: {
                // Delete the note that the context menu is for
                Android.Net.Uri noteUri = ContentUris.WithAppendedId(Intent.Data, info.Id);
                ContentResolver.Delete(noteUri, null, null);
                return true;
            }
        }
        return false;
    }

    protected override void OnListItemClick(ListView l, View v, int position, long id) {
        Android.Net.Uri uri = ContentUris.WithAppendedId(Intent.Data, id);

        String action = Intent.Action;
        if (Intent.ACTION_PICK == action || Intent.ACTION_GET_CONTENT == action) {
            // The caller is waiting for us to return a note selected by
            // the user.  The have clicked on one, so return it now.
            SetResult(RESULT_OK, new Intent().SetData (uri));
        } else {
            // Launch activity to view/edit the currently selected item
            StartActivity(new Intent(Intent.ACTION_EDIT, uri));
        }
    }
}}
