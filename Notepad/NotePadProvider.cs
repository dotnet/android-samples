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
using System.Collections.Generic;
using Android.Database.Sqlite;
using Android.Text;
using Java.Lang;
using Android.Content.Res;
using Android.Provider;

namespace Mono.Samples.Notepad
{
public class NotePadProvider : ContentProvider {

    private static String TAG = "NotePadProvider";

    private static String DATABASE_NAME = "note_pad.db";
    private static int DATABASE_VERSION = 2;
    private static String NOTES_TABLE_NAME = "notes";

    private static Dictionary<String, String> sNotesProjectionMap;
    private static Dictionary<String, String> sLiveFolderProjectionMap;

    private const int NOTES = 1;
    private const int NOTE_ID = 2;
    private const int LIVE_FOLDER_NOTES = 3;

    private static UriMatcher sUriMatcher;

    /**
     * This class helps open, create, and upgrade the database file.
     */
    private class DatabaseHelper : SQLiteOpenHelper {

        public DatabaseHelper (Context context) : base (context, DATABASE_NAME, null, DATABASE_VERSION)
	{
        }

        public override void OnCreate(SQLiteDatabase db) {
            db.ExecSQL("CREATE TABLE " + NOTES_TABLE_NAME + " ("
		    + Notes._ID + " INTEGER PRIMARY KEY,"
		    + Notes.TITLE + " TEXT,"
		    + Notes.NOTE + " TEXT,"
		    + Notes.CREATED_DATE + " INTEGER,"
		    + Notes.MODIFIED_DATE + " INTEGER"
                    + ");");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
            Log.W(TAG, "Upgrading database from version " + oldVersion + " to "
                    + newVersion + ", which will destroy all old data");
            db.ExecSQL("DROP TABLE IF EXISTS notes");
            OnCreate(db);
        }
    }

    private DatabaseHelper mOpenHelper;

    public override bool OnCreate() {
        mOpenHelper = new DatabaseHelper(Context);
        return true;
    }

    public override Cursor Query(Android.Net.Uri uri, String[] projection, String selection, String[] selectionArgs,
            String sortOrder) {
        SQLiteQueryBuilder qb = new SQLiteQueryBuilder();
        qb.Tables = NOTES_TABLE_NAME;

        switch (sUriMatcher.Match(uri)) {
        case NOTES:
		// TODO
            //qb.setProjectionMap(sNotesProjectionMap);
            break;

        case NOTE_ID:
            //qb.setProjectionMap(sNotesProjectionMap);
            //qb.AppendWhere(Notes._ID + "=" + uri.getPathSegments.get(1));
            break;

        case LIVE_FOLDER_NOTES:
            //qb.setProjectionMap(sLiveFolderProjectionMap);
            break;

        default:
            throw new IllegalArgumentException("Unknown URI " + uri);
        }

        // If no sort order is specified use the default
        String orderBy;
        if (TextUtils.IsEmpty(sortOrder)) {
            orderBy = Notes.DEFAULT_SORT_ORDER;
        } else {
            orderBy = sortOrder;
        }

        // Get the database and run the query
        SQLiteDatabase db = mOpenHelper.ReadableDatabase;
        Cursor c = qb.Query(db, projection, selection, selectionArgs, null, null, orderBy);

        // Tell the cursor what uri to watch, so it knows when its source data changes
        c.SetNotificationUri(Context.ContentResolver, uri);
        return c;
    }

    public override string  GetType(Android.Net.Uri uri)
{
        switch (sUriMatcher.Match(uri)) {
        case NOTES:
        case LIVE_FOLDER_NOTES:
            return Notes.CONTENT_TYPE;

        case NOTE_ID:
	    return Notes.CONTENT_ITEM_TYPE;

        default:
            throw new IllegalArgumentException("Unknown URI " + uri);
        }
    }

    public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues initialValues) {
        // Validate the requested uri
        if (sUriMatcher.Match(uri) != NOTES) {
            throw new IllegalArgumentException("Unknown URI " + uri);
        }

        ContentValues values;
        if (initialValues != null) {
            values = new ContentValues(initialValues);
        } else {
            values = new ContentValues();
        }

	// TODO
        long now = (long)DateTime.Now.Millisecond;

        // Make sure that the fields are all set
        if (values.ContainsKey(Notes.CREATED_DATE) == false) {
           //TODO
	    //values.Put(NotePad.Notes.CREATED_DATE, now);
        }

        if (values.ContainsKey(Notes.MODIFIED_DATE) == false) {
           // values.Put(NotePad.Notes.MODIFIED_DATE, now);
        }

        if (values.ContainsKey(Notes.TITLE) == false) {
            // TODO
	    //Resources r = Resources.System;
            //values.Put(NotePad.Notes.TITLE, r.GetString(android.R.@string.untitled));
        }

        if (values.ContainsKey(Notes.NOTE) == false) {
            values.Put(Notes.NOTE, "");
        }

        SQLiteDatabase db = mOpenHelper.WritableDatabase;
        long rowId = db.Insert(NOTES_TABLE_NAME, Notes.NOTE, values);
        if (rowId > 0) {
            Android.Net.Uri noteUri = ContentUris.WithAppendedId(Notes.CONTENT_URI, rowId);
            Context.ContentResolver.NotifyChange(noteUri, null);
            return noteUri;
        }

        throw new SQLException("Failed to insert row into " + uri);
    }

    public override int Delete(Android.Net.Uri uri, String where, String[] whereArgs) {
        SQLiteDatabase db = mOpenHelper.WritableDatabase;
        int count;
        switch (sUriMatcher.Match(uri)) {
        case NOTES:
            count = db.Delete(NOTES_TABLE_NAME, where, whereArgs);
            break;

        case NOTE_ID:
		// TODO
	    //String noteId = uri.getPathSegments().get(1);
	    //count = db.Delete(NOTES_TABLE_NAME, Notes._ID + "=" + noteId
	    //        + (!TextUtils.IsEmpty(where) ? " AND (" + where + ')' : ""), whereArgs);
            break;

        default:
            throw new IllegalArgumentException("Unknown URI " + uri);
        }

        Context.ContentResolver.NotifyChange(uri, null);
        
	// TODO
	return 1; //count;
    }

    public override int Update(Android.Net.Uri uri, ContentValues values, String where, String[] whereArgs) {
        SQLiteDatabase db = mOpenHelper.WritableDatabase;
        int count;
        switch (sUriMatcher.Match(uri)) {
        case NOTES:
            count = db.Update(NOTES_TABLE_NAME, values, where, whereArgs);
            break;

        case NOTE_ID:
	// TODO
	    //String noteId = uri.getPathSegments().get(1);
	    //count = db.update(NOTES_TABLE_NAME, values, Notes._ID + "=" + noteId
	    //        + (!TextUtils.isEmpty(where) ? " AND (" + where + ')' : ""), whereArgs);
            break;

        default:
            throw new IllegalArgumentException("Unknown URI " + uri);
        }

        Context.ContentResolver.NotifyChange(uri, null);

	// TODO
	return 1; //count;
    }

    static NotePadProvider () {
        sUriMatcher = new UriMatcher(UriMatcher.NO_MATCH);
        sUriMatcher.AddURI(Notes.AUTHORITY, "notes", NOTES);
        sUriMatcher.AddURI(Notes.AUTHORITY, "notes/#", NOTE_ID);
        sUriMatcher.AddURI(Notes.AUTHORITY, "live_folders/notes", LIVE_FOLDER_NOTES);

        sNotesProjectionMap = new Dictionary<String, String>();
	sNotesProjectionMap.Add (Notes._ID, Notes._ID);
	sNotesProjectionMap.Add (Notes.TITLE, Notes.TITLE);
	sNotesProjectionMap.Add (Notes.NOTE, Notes.NOTE);
	sNotesProjectionMap.Add (Notes.CREATED_DATE, Notes.CREATED_DATE);
	sNotesProjectionMap.Add (Notes.MODIFIED_DATE, Notes.MODIFIED_DATE);

        // Support for Live Folders.
        sLiveFolderProjectionMap = new Dictionary<String, String>();
	sLiveFolderProjectionMap.Add (LiveFolders._ID, Notes._ID + " AS " +
                LiveFolders._ID);
	sLiveFolderProjectionMap.Add (LiveFolders.NAME, Notes.TITLE + " AS " +
                LiveFolders.NAME);
        // Add more columns here for more robust Live Folders.
    }
}
}