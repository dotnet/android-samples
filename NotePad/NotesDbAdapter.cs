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
    using Android.Content;
    using Android.Database;
    using Android.Database.Sqlite;
    using Android.Util;

    public class NotesDbAdapter
    {

        public const string KeyTitle = "title";
        public const string KeyBody = "body";
        public const string KeyRowId = "_id";

        private const string Tag = "NotesDbAdapter";

        // Database creation sql statement
        private const string DatabaseCreate = 
            "create table notes (_id integer primary key autoincrement, "
            + "title text not null, body text not null);";

        private const string DBName = "data";
        private const string DatabaseTable = "notes";
        private const int DatabaseVersion = 2;

        private DatabaseHelper dbHelper;
        private SQLiteDatabase db;

        private readonly Context ctx;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesDbAdapter"/> class.
        /// Takes the context to allow the database to be opened/created
        /// </summary>
        /// <param name="ctx">the Context within which to work</param>
        public NotesDbAdapter(Context ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Open the notes database. If it cannot be opened, try to create a new
        /// instance of the database. If it cannot be created, throw an exception to
        /// signal the failure
        /// </summary>
        /// <returns>this (self reference, allowing this to be chained in an initialization call)</returns>
        /// <exception cref="SQLException">if the database could be neither opened or created</exception>
        public NotesDbAdapter Open()
        {
            this.dbHelper = new DatabaseHelper(this.ctx);
            this.db = this.dbHelper.WritableDatabase;
            return this;
        }

        public void Close()
        {
            this.dbHelper.Close();
        }

        /// <summary>
        /// Create a new note using the title and body provided. If the note is
        /// successfully created return the new rowId for that note, otherwise return
        /// a -1 to indicate failure.
        /// </summary>
        /// <param name="title">the title of the note</param>
        /// <param name="body">the body of the note</param>
        /// <returns>rowId or -1 if failed</returns>
        public long CreateNote(string title, string body)
        {
            var initialValues = new ContentValues();
            initialValues.Put(KeyTitle, title);
            initialValues.Put(KeyBody, body);

            return this.db.Insert(DatabaseTable, null, initialValues);
        }

        /// <summary>
        /// Delete the note with the given rowId
        /// </summary>
        /// <param name="rowId">id of note to delete</param>
        /// <returns>true if deleted, false otherwise</returns>
        public bool DeleteNote(long rowId)
        {
            return this.db.Delete(DatabaseTable, KeyRowId + "=" + rowId, null) > 0;
        }

        /// <summary>
        /// Return a Cursor over the list of all notes in the database
        /// </summary>
        /// <returns>A Cursor over all notes</returns>
        public ICursor FetchAllNotes()
        {
            return this.db.Query(DatabaseTable, new[] { KeyRowId, KeyTitle, KeyBody }, null, null, null, null, null);
        }

        /// <summary>
        /// Return a Cursor positioned at the note that matches the given rowId
        /// </summary>
        /// <param name="rowId">id of note to retrieve</param>
        /// <returns>A cursor positioned to matching note, if found</returns>
        /// <exception cref="SQLException">if note could not be found/retrieved</exception>
        public ICursor FetchNote(long rowId)
        {
            ICursor cursor = this.db.Query(
                true,
                DatabaseTable,
                new[] { KeyRowId, KeyTitle, KeyBody },
                KeyRowId + "=" + rowId,
                null,
                null,
                null,
                null,
                null);

            if (cursor != null)
            {
                cursor.MoveToFirst();
            }
            return cursor;
        }

        /// <summary>
        /// Update the note using the details provided. The note to be updated is
        /// specified using the rowId, and it is altered to use the title and body
        /// values passed in
        /// </summary>
        /// <param name="rowId">id of note to update</param>
        /// <param name="title">value to set note title to</param>
        /// <param name="body">value to set note body to</param>
        /// <returns>true if the note was successfully updated, false otherwise</returns>
        public bool UpdateNote(long rowId, string title, string body)
        {
            var args = new ContentValues();
            args.Put(KeyTitle, title);
            args.Put(KeyBody, body);

            return this.db.Update(DatabaseTable, args, KeyRowId + "=" + rowId, null) > 0;
        }

        private class DatabaseHelper : SQLiteOpenHelper
        {
            internal DatabaseHelper(Context context)
                : base(context, DBName, null, DatabaseVersion)
            {
            }

            public override void OnCreate(SQLiteDatabase db)
            {
                db.ExecSQL(DatabaseCreate);
            }

            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {
                Log.Wtf(Tag, "Upgrading database from version " + oldVersion + " to " + newVersion + ", which will destroy all old data");
                db.ExecSQL("DROP TABLE IF EXISTS notes");
                this.OnCreate(db);
            }
        }
    }
}