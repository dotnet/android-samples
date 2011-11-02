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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Database;
using Mono.Data.Sqlite;

namespace MonoDroid.ContentProviderDemo
{
    class LocationContentProvider : ContentProvider
    {
        public static string PROVIDER_NAME = "monodroid.contentproviderdemo.LocationProvider";

        public static Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse("content://" + PROVIDER_NAME + "/locations");

        private const string DATABASE_NAME = "Locations";
        private const string DATABASE_TABLE_NAME = "Location";
        private const int DATABASE_VERSION = 1;
        private const string DEFAULT_SORT_ORDER = "name DESC";

        public const string _ID = "_id";
        public const string NAME = "name";
        public const string LONGITUDE = "longitude";
        public const string LATITUTDE = "latitude";

        private const int LOCATIONS = 1;
        private const int LOCATION_ID = 2;

        private const string LOCATION_TYPE = "vnd.android.cursor.dir/vnd.contentproviderdemo.location";
        private const string LOCATION_ITEM_TYPE = "vnd.android.cursor.item/vnd.contentproviderdemo.location";

        private static UriMatcher uriMatcher;
        private LocationContentProviderDatabaseHelper dbHelper;
        private static IDictionary<string, string> mProjectionMap;

        static LocationContentProvider()
        {
            mProjectionMap = new Dictionary<string, string>();
            mProjectionMap.Add(_ID, _ID);
            mProjectionMap.Add(NAME, NAME);
            mProjectionMap.Add(LONGITUDE, LONGITUDE);
            mProjectionMap.Add(LATITUTDE, LATITUTDE);

            uriMatcher = new UriMatcher(UriMatcher.NoMatch);
            uriMatcher.AddURI(PROVIDER_NAME, "locations", LOCATIONS);
            uriMatcher.AddURI(PROVIDER_NAME, "locations/#", LOCATION_ID);
        }

        private class LocationContentProviderDatabaseHelper : SQLiteOpenHelper
        {
            public LocationContentProviderDatabaseHelper(Context context)
                : base(context, DATABASE_NAME, null, DATABASE_VERSION)
            {
            }

            public override void OnCreate(SQLiteDatabase db)
            {
                db.ExecSQL(@"
                    CREATE TABLE " + DATABASE_TABLE_NAME + " ("
                        + _ID + " INTEGER PRIMARY KEY NOT NULL," // INTEGER PRIMARY KEY functions as auto increment in SQLite
                        + NAME + " TEXT NOT NULL,"
                        + LONGITUDE + " REAL NOT NULL,"
                        + LATITUTDE + " REAL NOT NULL)"
                        );
            }

            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {
                db.ExecSQL("DROP TABLE IF EXISTS " + DATABASE_TABLE_NAME);

                OnCreate(db);
            }
        }

        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            SQLiteDatabase db = dbHelper.WritableDatabase;
            int count;
            switch (uriMatcher.Match(uri))
            {
                case LOCATIONS:
                    count = db.Delete(DATABASE_TABLE_NAME, selection, selectionArgs);
                    break;

                case LOCATION_ID:
                    String locationId = uri.PathSegments.ElementAt(1);
                    string select = "";
                    if (selection != null)
                        if (selection.Length > 0) select = " AND (" + selection + ")";
                    count = db.Delete(
                        DATABASE_TABLE_NAME,
                        _ID + "=" + locationId + select,
                        selectionArgs);
                    break;

                default:
                    throw new ArgumentException("Unknown URI " + uri);
            }

            Context.ContentResolver.NotifyChange(uri, null);
            return count;
        }

        public override string GetType(Android.Net.Uri uri)
        {
            switch (uriMatcher.Match(uri))
            {
                case LOCATIONS:
                    return LOCATION_TYPE;
                case LOCATION_ID:
                    return LOCATION_ITEM_TYPE;
                default:
                    throw new ArgumentException("Unsupported URI: " + uri);
            }
        }

        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues initialValues)
        {
            // Validate the requested uri
            if (uriMatcher.Match(uri) != LOCATIONS)
            {
                throw new ArgumentException("Unknown URI " + uri);
            }

            ContentValues values;
            if (initialValues != null)
            {
                values = new ContentValues(initialValues);
            }
            else
            {
                values = new ContentValues();
            }

            // Make sure all NOT NULL fields are set
            if (values.ContainsKey(NAME) == false)
            {
                throw new SQLException("Failed to insert row into " + uri + ". Query is missing NAME!");
            }

            SQLiteDatabase db = dbHelper.WritableDatabase;
            long rowId = db.Insert(DATABASE_TABLE_NAME, NAME, values);
            if (rowId > 0)
            {
                Android.Net.Uri locationUri = ContentUris.WithAppendedId(CONTENT_URI, rowId);
                Context.ContentResolver.NotifyChange(locationUri, null);
                return locationUri;
            }

            throw new SQLException("Failed to insert row into " + uri);
        }

        public override bool OnCreate()
        {
            dbHelper = new LocationContentProviderDatabaseHelper(Context);
            return true;
        }

        public override ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            SQLiteQueryBuilder qb = new SQLiteQueryBuilder();
            qb.Tables = DATABASE_TABLE_NAME;

            switch (uriMatcher.Match(uri))
            {
                case LOCATIONS:
                    qb.SetProjectionMap(mProjectionMap);
                    break;
                case LOCATION_ID:
                    qb.SetProjectionMap(mProjectionMap);
                    qb.AppendWhere(_ID + "=" + uri.PathSegments.ElementAt(1));
                    break;
                default:
                    throw new ArgumentException("Unknown URI " + uri);
            }

            // If no sort order is specified use the default
            string orderBy;
            if (sortOrder.Length < 1)
            {
                orderBy = DEFAULT_SORT_ORDER;
            }
            else
            {
                orderBy = sortOrder;
            }

            // Get the database and run the query
            SQLiteDatabase db = dbHelper.ReadableDatabase;
            ICursor c = qb.Query(db, projection, selection, selectionArgs, null, null, orderBy);

            // Tell the cursor what uri to watch, so it knows when its source data changes
            c.SetNotificationUri(Context.ContentResolver, uri);

            return c;
        }

        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            SQLiteDatabase db = dbHelper.WritableDatabase;
            int count;
            switch (uriMatcher.Match(uri))
            {
                case LOCATIONS:
                    count = db.Update(DATABASE_TABLE_NAME, values, selection, selectionArgs);
                    break;

                case LOCATION_ID:
                    String locationId = uri.PathSegments.ElementAt(1);
                    string select = "";
                    if (selection != null)
                        if (selection.Length > 0) select = " AND (" + selection + ")";

                    count = db.Update(
                        DATABASE_TABLE_NAME,
                        values,
                        _ID + "=" + locationId + select,
                        selectionArgs);
                    break;

                default:
                    throw new ArgumentException("Unknown URI " + uri);
            }

            Context.ContentResolver.NotifyChange(uri, null);
            return count;
        }
    }
}