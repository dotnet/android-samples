/*
 * Copyright 2013 Chris Banes
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
// Under the same license as above.
//
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.ActionBarPullToRefresh.Library;
using Void = Java.Lang.Void;
using Object = Java.Lang.Object;

namespace ActionBarPullToRefreshSample
{
	/**
 * This sample shows how to use ActionBar-PullToRefresh with a
 * {@link android.widget.ListView ListView}, and manually creating (and attaching) a
 * {@link PullToRefreshAttacher} to the view.
 */
	[Activity (Label = "@string/activity_listview", MainLauncher = true)]
	public class ListViewActivity : ListActivity
	{
		private static string[] ITEMS = new string [] {"Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam",
			"Abondance", "Ackawi", "Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu",
			"Airag", "Airedale", "Aisy Cendre", "Allgauer Emmentaler", "Abbaye de Belloc",
			"Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi", "Acorn", "Adelost",
			"Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler"
		};
		private PullToRefreshAttacher mPullToRefreshAttacher;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			/**
         * Get ListView and give it an adapter to display the sample items
         */
			ListView listView = this.ListView;
			IListAdapter adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,
			                                         ITEMS);
			listView.Adapter = adapter;

			/**
         * Here we create a PullToRefreshAttacher manually without an Options instance.
         * PullToRefreshAttacher will manually create one using default values.
         */
			mPullToRefreshAttacher = new PullToRefreshAttacher (this, ListView);

			// Set Listener to know when a refresh should be started
			mPullToRefreshAttacher.Refresh += delegate {
				new MyTask (this).Execute ();
			};
		}

		class MyTask : AsyncTask<Void, Void, Void>
		{
			ListViewActivity owner;

			public MyTask (ListViewActivity owner)
			{
				this.owner = owner;
			}

			protected override Void RunInBackground (params Void[] @params)
			{
				try {
					System.Threading.Thread.Sleep (4000);
				} catch (Java.Lang.InterruptedException e) {
					Console.WriteLine (e);
				}
				return null;
			}

			protected override void OnPostExecute (Void result)
			{
				base.OnPostExecute (result);

				// Notify PullToRefreshAttacher that the refresh has finished
				owner.mPullToRefreshAttacher.SetRefreshComplete ();
			}
		}
	}
}


