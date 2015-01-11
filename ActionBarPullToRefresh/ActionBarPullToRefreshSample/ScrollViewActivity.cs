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

using System.Threading;
using System.Threading.Tasks;

namespace ActionBarPullToRefreshSample
{
	/**
 * This sample shows how to use ActionBar-PullToRefresh with a
 * {@link android.widget.ScrollView ScrollView}. It utilises {@link PullToRefreshLayout} to setup
 * the ScrollView via XML. See the layout resource file for more information.
 * <p />
 * Once inflated, you can retrieve the {@link PullToRefreshAttacher} by calling
 * {@link PullToRefreshLayout#getAttacher(android.app.Activity, int) getAttacher(Activity, int)},
 * passing it the PullToRefreshLayout's ID. From there you can set your
 * {@link PullToRefreshAttacher.OnRefreshListener OnRefreshListener} as usual.
 */
	[Activity (Label = "@string/activity_scrollview")]
	public class ScrollViewActivity : Activity
	{

		private PullToRefreshAttacher mPullToRefreshAttacher;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_scrollview);

			// Retrieve PullToRefreshAttacher from PullToRefreshLayout
			mPullToRefreshAttacher = PullToRefreshLayout.GetAttacher (this, Resource.Id.ptr_layout);

			// Set Listener to know when a refresh should be started
			mPullToRefreshAttacher.Refresh += delegate {
				Task.Factory.StartNew(()=> { Thread.Sleep (5000); })
				.ContinueWith(task => { mPullToRefreshAttacher.SetRefreshComplete(); });
			};
		}
	}
}
