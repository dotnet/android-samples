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
using Xamarin.ActionBarPullToRefresh.Library.Delegates;
using System.Threading;
using System.Threading.Tasks;

namespace ActionBarPullToRefreshSample
{
	/**
 * This sample shows how to use ActionBar-PullToRefresh with a {@link android.widget.GridView
 * GridView}, and manually creating (and attaching) a {@link PullToRefreshAttacher} to the view.
 */
	[Activity (Label = "@string/activity_gridview")]
	public class GridViewActivity : Activity
	{
		private static string[] ITEMS = new string[] {"Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam",
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
			SetContentView (Resource.Layout.activity_gridview);

			GridView gridView = FindViewById<GridView> (Resource.Id.ptr_gridview);
			IListAdapter adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1, ITEMS);
			gridView.Adapter = adapter;

			// As we're modifying some of the options, create an instance of
			// PullToRefreshAttacher.Options
			PullToRefreshAttacher.Options ptrOptions = new PullToRefreshAttacher.Options ();

			// Here we make the refresh scroll distance to 75% of the GridView height
			ptrOptions.RefreshScrollDistance = 0.75f;

			/**
         * As GridView is an AbsListView derived class, we create a new AbsListViewDelegate
         * instance. You do NOT need to do this if you're using a supported scrollable Views. It is
         * merely in this sample to show you how to set a custom delegate.
         */
			ptrOptions.DelegateField = new AbsListViewDelegate ();

			// Here we customise the animations which are used when showing/hiding the header view
			ptrOptions.HeaderInAnimation = Resource.Animation.slide_in_top;
			ptrOptions.HeaderOutAnimation = Resource.Animation.slide_out_top;

			// Here we define a custom header layout which will be inflated and used
			ptrOptions.HeaderLayout = Resource.Layout.customised_header;

			// Here we define a custom header transformer which will alter the header based on the
			// current pull-to-refresh state
			ptrOptions.HeaderTransformer = new CustomisedHeaderTransformer ();

			// Here we create a PullToRefreshAttacher manually with the Options instance created above.
			mPullToRefreshAttacher = new PullToRefreshAttacher (this, gridView, ptrOptions);

			// Set Listener to know when a refresh should be started
			mPullToRefreshAttacher.Refresh += delegate {
				Task.Factory.StartNew(()=> { Thread.Sleep (5000); })
					.ContinueWith(task => { mPullToRefreshAttacher.SetRefreshComplete(); });
			};
		}
	}

	/**
     * Here's a customised header transformer which displays the scroll progress as text.
     */
	class CustomisedHeaderTransformer : PullToRefreshAttacher.HeaderTransformer
	{

		private TextView mMainTextView;
		private TextView mProgressTextView;

		public override void OnViewCreated (View headerView)
		{
			mMainTextView = headerView.FindViewById<TextView> (Resource.Id.ptr_text);
			mProgressTextView = headerView.FindViewById<TextView> (Resource.Id.ptr_text_secondary);
		}

		public override void OnReset ()
		{
			mMainTextView.Visibility = ViewStates.Visible;
			mMainTextView.SetText (Resource.String.pull_to_refresh_pull_label);

			mProgressTextView.Visibility = ViewStates.Gone;
			mProgressTextView.Text = "";
		}

		public override void OnPulled (float percentagePulled)
		{
			mProgressTextView.Visibility = ViewStates.Visible;
			mProgressTextView.Text = Math.Round (100f * percentagePulled) + "%";
		}

		public override void OnRefreshStarted ()
		{
			mMainTextView.SetText (Resource.String.pull_to_refresh_refreshing_label);
			mProgressTextView.Visibility = ViewStates.Gone;
		}
	}
}
