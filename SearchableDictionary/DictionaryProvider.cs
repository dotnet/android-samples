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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SearchableDictionary
{
    [ContentProvider(new string[]{"searchabledictionary.DictionaryProvider"})]
    public class DictionaryProvider : ContentProvider
    {
        public static readonly String AUTHORITY = "searchabledictionary.DictionaryProvider";
        public static readonly Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse ("content://" + AUTHORITY + "/dictionary");
        
        // MIME types used for searching words or looking up a single definition
        public static readonly String WORDS_MIME_TYPE = ContentResolver.CursorDirBaseType + "/vnd.SearchableDictionary.SearchableDictionary";
        public static readonly String DEFINITION_MIME_TYPE = ContentResolver.CursorItemBaseType + "/vnd.SearchableDictionary.SearchableDictionary";
        
        DictionaryDatabase dictionary;
        
        // UriMatcher stuff
        const int SEARCH_WORDS = 0;
        const int GET_WORD = 1;
        const int SEARCH_SUGGEST = 2;
        const int REFRESH_SHORTCUT = 3;
        static UriMatcher uriMatcher = BuildUriMatcher ();
        
        static UriMatcher BuildUriMatcher ()
        {
            var matcher = new UriMatcher (UriMatcher.NoMatch);
            
            // to get definitions...
            matcher.AddURI (AUTHORITY, "dictionary", SEARCH_WORDS);
            matcher.AddURI (AUTHORITY, "dictionary/#", GET_WORD);
            
            // to get suggestions...
            matcher.AddURI (AUTHORITY, SearchManager.SuggestUriPathQuery, SEARCH_SUGGEST);
            matcher.AddURI (AUTHORITY, SearchManager.SuggestUriPathQuery + "/*", SEARCH_SUGGEST);
            
            /* The following are unused in this implementation, but if we include
             * SearchManager.SuggestColumnShortcutId as a column in our suggestions table, we
             * could expect to receive refresh queries when a shortcutted suggestion is displayed in
             * Quick Search Box, in which case, the following Uris would be provided and we
             * would return a cursor with a single item representing the refreshed suggestion data.
             */
            matcher.AddURI (AUTHORITY, SearchManager.SuggestUriPathShortcut, REFRESH_SHORTCUT);
            matcher.AddURI (AUTHORITY, SearchManager.SuggestUriPathShortcut + "/*", REFRESH_SHORTCUT);
            return matcher;
        }
        
        public override bool OnCreate ()
        {
            dictionary = new DictionaryDatabase (Context);
            return true;
        }
        
        public override Android.Database.ICursor Query (Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            switch (uriMatcher.Match (uri)) {
            case SEARCH_SUGGEST:
                if (selectionArgs == null) {
                    throw new Java.Lang.IllegalArgumentException (
                      "selectionArgs must be provided for the Uri: " + uri);
                }
                return GetSuggestions (selectionArgs [0]);
            case SEARCH_WORDS:
                if (selectionArgs == null) {
                    throw new Java.Lang.IllegalArgumentException (
                      "selectionArgs must be provided for the Uri: " + uri);
                }
                return Search (selectionArgs [0]);
            case GET_WORD:
                return GetWord (uri);
            case REFRESH_SHORTCUT:
                return RefreshShortcut (uri);
            default:
                throw new Java.Lang.IllegalArgumentException ("Unknown Uri: " + uri);
            }
            
        }
        
        Android.Database.ICursor GetSuggestions (String query)
        {
            query = query.ToLower ();
            String[] columns = new String[] {
                Android.Provider.BaseColumns.Id,
                DictionaryDatabase.KEY_WORD,
                DictionaryDatabase.KEY_DEFINITION,
            /* SearchManager.SuggestColumnShortcutId,
               (only if you want to refresh shortcuts) */
                SearchManager.SuggestColumnIntentDataId
            };

            return dictionary.GetWordMatches (query, columns);
        }
  
        Android.Database.ICursor Search (String query)
        {
            query = query.ToLower ();
            String[] columns = new String[] {
                Android.Provider.BaseColumns.Id,
                DictionaryDatabase.KEY_WORD,
                DictionaryDatabase.KEY_DEFINITION
            };

            return dictionary.GetWordMatches (query, columns);
        }
        
        Android.Database.ICursor GetWord (Android.Net.Uri uri)
        {
            String rowId = uri.LastPathSegment;
            String[] columns = new String[] {
                DictionaryDatabase.KEY_WORD,
                DictionaryDatabase.KEY_DEFINITION};

            return dictionary.GetWord (rowId, columns);
        }
        
        Android.Database.ICursor RefreshShortcut (Android.Net.Uri uri)
        {
            /* This won't be called with the current implementation, but if we include
             * SearchManager.SuggestColumnShortcutId as a column in our suggestions table, we
             * could expect to receive refresh queries when a shortcutted suggestion is displayed in
             * Quick Search Box. In which case, this method will query the table for the specific
             * word, using the given item Uri and provide all the columns originally provided with the
             * suggestion query.
             */
            String rowId = uri.LastPathSegment;
            String[] columns = new String[] {
                Android.Provider.BaseColumns.Id,
                DictionaryDatabase.KEY_WORD,
                DictionaryDatabase.KEY_DEFINITION,
                SearchManager.SuggestColumnShortcutId,
                SearchManager.SuggestColumnIntentDataId
            };

            return dictionary.GetWord (rowId, columns);
        }
        
        public override String GetType (Android.Net.Uri uri)
        {
            switch (uriMatcher.Match (uri)) {
            case SEARCH_WORDS:
                return WORDS_MIME_TYPE;
            case GET_WORD:
                return DEFINITION_MIME_TYPE;
            case SEARCH_SUGGEST:
                return SearchManager.SuggestMimeType;
            case REFRESH_SHORTCUT:
                return SearchManager.ShortcutMimeType;
            default:
                throw new Java.Lang.IllegalArgumentException ("Unknown URL " + uri);
            }
        }
        
        public override int Delete (Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            throw new Java.Lang.UnsupportedOperationException ();
        }

        public override Android.Net.Uri Insert (Android.Net.Uri uri, ContentValues values)
        {
            throw new Java.Lang.UnsupportedOperationException ();
        }

        public override int Update (Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new Java.Lang.UnsupportedOperationException ();
        }
    }
}

