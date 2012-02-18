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

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Database;

namespace SearchableDictionary
{
    [Activity(Label = "SearchableDictionary", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [IntentFilter(new string[]{"android.intent.action.SEARCH"})]
    [MetaData(("android.app.searchable"), Resource = "@xml/searchable")]
    public class SearchableDictionary : Activity
    {
        TextView textView;
        ListView listView;
        
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.main);
            
            textView = this.FindViewById<TextView> (Resource.Id.text);
            listView = this.FindViewById<ListView> (Resource.Id.list);
            
            HandleIntent (Intent);
        }
        
        void HandleIntent (Intent intent)
        {
            if (Intent.ActionView.Equals (intent.Action)) {
                // handles a click on a search suggestion; launches activity to show word
                Intent wordIntent = new Intent (this, typeof(WordActivity));
                wordIntent.SetData (intent.Data);
                StartActivity (wordIntent);
                Finish ();
            } else if (Intent.ActionSearch.Equals (intent.Action)) {
                string query = intent.GetStringExtra (SearchManager.Query);
                ShowResults (query); 
            }
        }
        
        protected override void OnNewIntent (Android.Content.Intent intent)
        {
            // Because this activity has set launchMode="singleTop", the system calls this method
            // to deliver the intent if this actvity is currently the foreground activity when
            // invoked again (when the user executes a search from this activity, we don't create
            // a new instance of this activity, so the system delivers the search intent here)
            HandleIntent (intent);
        }
        
        void ShowResults (string query)
        {
            var cursor = ManagedQuery (DictionaryProvider.CONTENT_URI, null, null, new string[] {query}, null);
            
            if (cursor == null) {
                // There are no results             
                textView.Text = GetString (Resource.String.no_results, query); 
            } else {
            
                int count = cursor.Count;
                var countString = Resources.GetQuantityString (Resource.Plurals.search_results, count, count, query);
                textView.Text = countString;
                
                string[] from = new string[] { DictionaryDatabase.KEY_WORD,
                    DictionaryDatabase.KEY_DEFINITION };
                
                int[] to = new int[] { Resource.Id.word,
                                  Resource.Id.definition };
    
                var words = new SimpleCursorAdapter (this, Resource.Layout.result, cursor, from, to);
                
                listView.SetAdapter (words);
                
                listView.ItemClick += (sender, e) => {
                    var wordIntent = new Intent (ApplicationContext, typeof(WordActivity));     
                    var data = Android.Net.Uri.WithAppendedPath (DictionaryProvider.CONTENT_URI, Java.Lang.String.ValueOf (e.Id));           
                    wordIntent.SetData (data);
                    StartActivity (wordIntent);
                };
            }
        }
        
        public override bool OnCreateOptionsMenu (IMenu menu)
        {
            MenuInflater.Inflate (Resource.Menu.options_menu, menu);
            
            var searchManager = (SearchManager)GetSystemService (Context.SearchService);
            var searchView = (SearchView)menu.FindItem (Resource.Id.search).ActionView;
            
            searchView.SetSearchableInfo (searchManager.GetSearchableInfo (ComponentName));
            searchView.SetIconifiedByDefault (false);
            
            return true;
        }
        
        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            switch (item.ItemId) {
            case Resource.Id.search:
                OnSearchRequested ();
                return true;
            default:
                return false;
            }
        }
    }
}