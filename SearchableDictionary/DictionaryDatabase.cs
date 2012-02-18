/*
 * Copyright (C) 2010 The Android Open Source Project
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Util;
using Android.Text;

namespace SearchableDictionary
{
    public class DictionaryDatabase
    {
        static String TAG = "DictionaryDatabase";

        //The columns we'll include in the dictionary table
        
        public static readonly String KEY_WORD = Android.App.SearchManager.SuggestColumnText1;
        public static readonly String KEY_DEFINITION = Android.App.SearchManager.SuggestColumnText2;
        static String DATABASE_NAME = "dictionary";
        static String FTS_VIRTUAL_TABLE = "FTSdictionary";
        static int DATABASE_VERSION = 2;
        DictionaryOpenHelper databaseOpenHelper;
        static Dictionary<string, string> mColumnMap = BuildColumnMap ();
        
        public DictionaryDatabase (Context context)
        {
            databaseOpenHelper = new DictionaryOpenHelper (context);
        }
        
        static Dictionary<string, string> BuildColumnMap ()
        {
            Dictionary<string, string> map = new Dictionary<string, string> ();
            map.Add (KEY_WORD, KEY_WORD);
            map.Add (KEY_DEFINITION, KEY_DEFINITION);
            map.Add (Android.Provider.BaseColumns.Id, "rowid AS " +
                Android.Provider.BaseColumns.Id);
            map.Add (SearchManager.SuggestColumnIntentDataId, "rowid AS " +
                SearchManager.SuggestColumnIntentDataId);
            map.Add (SearchManager.SuggestColumnShortcutId, "rowid AS " +
                SearchManager.SuggestColumnShortcutId);
            return map;
        }
        
        public Android.Database.ICursor GetWord (String rowId, String[] columns)
        {
            String selection = "rowid = ?";
            String[] selectionArgs = new String[] {rowId};

            return Query (selection, selectionArgs, columns);
        }
        
        public Android.Database.ICursor GetWordMatches (String query, String[] columns)
        {
            String selection = KEY_WORD + " MATCH ?";
            String[] selectionArgs = new String[] {query + "*"};

            return Query (selection, selectionArgs, columns);
        }
        
        Android.Database.ICursor Query (String selection, String[] selectionArgs, String[] columns)
        {
            var builder = new SQLiteQueryBuilder ();
            builder.Tables = FTS_VIRTUAL_TABLE;
            builder.SetProjectionMap (mColumnMap);

            var cursor = builder.Query (databaseOpenHelper.ReadableDatabase,
                columns, selection, selectionArgs, null, null, null);

            if (cursor == null) {
                return null;
            } else if (!cursor.MoveToFirst ()) {
                cursor.Close ();
                return null;
            }
            return cursor;
        }

        class DictionaryOpenHelper : SQLiteOpenHelper
        {
            Context helperContext;
            SQLiteDatabase database;
            static String FTS_TABLE_CREATE =
                    "CREATE VIRTUAL TABLE " + FTS_VIRTUAL_TABLE +
                    " USING fts3 (" +
                    KEY_WORD + ", " +
                    KEY_DEFINITION + ");";
            
            public DictionaryOpenHelper (Context context): base(context, DATABASE_NAME, null, DATABASE_VERSION)
            {
                helperContext = context;
            }
            
            public override void OnCreate (SQLiteDatabase db)
            {
                database = db;
                database.ExecSQL (FTS_TABLE_CREATE);
                LoadDictionary ();
            }
            
            void LoadDictionary ()
            {
                new Thread (() => {
                    try {
                        LoadWords ();
                    } catch (Exception e) {
                        throw new Java.Lang.RuntimeException (e.Message);
                    }
                }).Start ();
            }
            
            void LoadWords ()
            {
                Log.Debug (TAG, "Loading words...");
                
                var resources = helperContext.Resources;
                var inputStream = resources.OpenRawResource (Resource.Raw.definitions);

                using (var reader = new System.IO.StreamReader(inputStream)) {
          
                    try {
                        String line;
                        while ((line = reader.ReadLine()) != null) {
                            String[] strings = TextUtils.Split (line, "-");
                            if (strings.Length < 2)
                                continue;
                            long id = AddWord (strings [0].Trim (), strings [1].Trim ());
                            if (id < 0) {
                                Log.Error (TAG, "unable to add word: " + strings [0].Trim ());
                            }
                        }
                    } finally {
                        reader.Close ();
                    }
                }
                
                Log.Debug (TAG, "DONE loading words.");
            }
            
            public long AddWord (String word, String definition)
            {
                var initialValues = new ContentValues ();
                initialValues.Put (KEY_WORD, word);
                initialValues.Put (KEY_DEFINITION, definition);

                return database.Insert (FTS_VIRTUAL_TABLE, null, initialValues);
            }

            public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
            {
                Log.Warn (TAG, "Upgrading database from version " + oldVersion + " to "
                    + newVersion + ", which will destroy all old data");
                db.ExecSQL ("DROP TABLE IF EXISTS " + FTS_VIRTUAL_TABLE);
                OnCreate (db);
            }

        }
    }
}

