/*
 * Copyright (C) 2012 The Android Open Source Project
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

namespace Contactables
{

	/**
	 * Simple one-activity app that takes a search term via the Action Bar
	 * and uses it as a query to search the contacts database via the Contactables
	 * table.
	 */
	[Activity (Label = "Contactables", MainLauncher = true, Theme = "@style/Theme.Sample", 
	           LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	[MetaData ("android.app.searchable", Resource = "@xml/searchable")]
	[IntentFilter (new[] {Intent.ActionSearch})]
	public class MainActivity : Activity
	{
		public static readonly int CONTACT_QUERY_LOADER = 0;
		public static readonly String QUERY_KEY = "query";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			if (Intent != null) {
				HandleIntent (Intent);
			}
		}

		protected override void OnNewIntent (Intent intent)
		{
			HandleIntent (intent);
		}

		/**
		 * Assuming this activity was started with a new intent, process the incoming information and
		 * react accordingly.
		 * @param intent
		 */
		void HandleIntent (Intent intent)
		{
			// Special processing of the incoming intent only occurs if the if the action specified
			// by the intent is ACTION_SEARCH.
			if (Intent.ActionSearch.Equals (intent.Action)) {

				// SearchManager.QUERY is the key that a SearchManager will use to send a query string
				// to an Activity.
				String query = intent.GetStringExtra (SearchManager.Query);

				// We need to create a bundle containing the query string to send along to the
				// LoaderManager, which will be handling querying the database and returning results.
				Bundle bundle = new Bundle();
				bundle.PutString (QUERY_KEY, query);

				ContactablesLoaderCallbacks loaderCallbacks = new ContactablesLoaderCallbacks (this);

				// Start the loader with the new query, and an object that will handle all callbacks.
				LoaderManager.RestartLoader (CONTACT_QUERY_LOADER, bundle, loaderCallbacks);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate the menu; this adds items to the action bar if it is present.
			MenuInflater.Inflate (Resource.Menu.main, menu);

			// Associate searchable configuration with the SearchView
			var searchManager = (SearchManager) GetSystemService (Context.SearchService);
			var searchView = (SearchView) menu.FindItem (Resource.Id.search).ActionView;
			searchView.SetSearchableInfo (searchManager.GetSearchableInfo (ComponentName));

			return true;
		}
	}
}


