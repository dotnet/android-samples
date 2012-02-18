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
    [Activity(Label="WordActivity")]
    public class WordActivity : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.word);

            //ActionBar actionBar = ActionBar;
            ActionBar.SetDisplayHomeAsUpEnabled (true);
            
            var uri = Intent.Data;
            var cursor = ManagedQuery (uri, null, null, null, null);
           
            if (cursor == null) {
                Finish ();
            } else {
                cursor.MoveToFirst ();

                var word = FindViewById<TextView> (Resource.Id.word);
                var definition = FindViewById<TextView> (Resource.Id.definition);
                
                int wIndex = cursor.GetColumnIndexOrThrow (DictionaryDatabase.KEY_WORD);
                int dIndex = cursor.GetColumnIndexOrThrow (DictionaryDatabase.KEY_DEFINITION);
                
                word.Text = cursor.GetString (wIndex);
                definition.Text = cursor.GetString (dIndex);
            }
        }
        
        public override bool OnCreateOptionsMenu (IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            
            MenuInflater.Inflate (Resource.Menu.options_menu, menu);
            
            var searchManager = (SearchManager)GetSystemService (Context.SearchService);
            var searchView = (SearchView)menu.FindItem (Resource.Id.search).ActionView;
            var searchableInfo = searchManager.GetSearchableInfo (ComponentName);          
            
            searchView.SetSearchableInfo (searchableInfo);
            searchView.SetIconifiedByDefault (false);

            return true;
        }
        
        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            switch (item.ItemId) {
            case Resource.Id.search:
                OnSearchRequested ();
                return true;
            case Android.Resource.Id.Home:
                var intent = new Intent (this, typeof(SearchableDictionary));
                intent.AddFlags (ActivityFlags.ClearTop);
                StartActivity (intent);
                return true;
            default:
                return false;
            }
        }
        
    }
}

