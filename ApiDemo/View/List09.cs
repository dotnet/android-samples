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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.Lang;

namespace MonoDroid.ApiDemo
{
	/**
 	* Another variation of the list of cheeses. In this case, we use
 	* {@link AbsListView#setOnScrollListener(AbsListView.OnScrollListener) 
 	* AbsListView#setOnItemScrollListener(AbsListView.OnItemScrollListener)} to display the
 	* first letter of the visible range of cheeses.
 	*/
	[Activity (Label = "Views/Lists/09. Array (Overlay)")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List9 : ListActivity
	{
		Action mRemoveWindow;
		Handler mHandler = new Handler ();
		private TextView mDialogText;
		private bool mShowing;
		private bool mReady;
		private char mPrevLetter = char.MinValue;
		private string[] mStrings = Cheeses.CheeseStrings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			mRemoveWindow = delegate { RemoveWindow (); };

			// Use an existing ListAdapter that will map an array
			// of strings to TextViews
			ListAdapter = new ArrayAdapter<string> (this,Android.Resource.Layout.SimpleListItem1, mStrings);

			ListView.Scroll += OnListScroll;

			LayoutInflater inflate = (LayoutInflater)GetSystemService (Context.LayoutInflaterService);

			mDialogText = (TextView) inflate.Inflate (Resource.Layout.list_position, null);
			mDialogText.Visibility = ViewStates.Invisible;

			mHandler.Post (() => {
				mReady = true;
				var lp = new WindowManagerLayoutParams (
					WindowManagerLayoutParams.WrapContent, WindowManagerLayoutParams.WrapContent,
					WindowManagerTypes.Application,
					WindowManagerFlags.NotTouchable
					| WindowManagerFlags.NotFocusable,
					Format.Translucent);

				WindowManager.AddView (mDialogText, lp);
			});
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			mReady = true;

		}

		protected override void OnPause ()
		{
			base.OnPause ();
			RemoveWindow ();
			mReady = false;
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			WindowManager.RemoveView (mDialogText);
			mReady = false;
		}

		void OnListScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			if (mReady) {
				char firstLetter = mStrings[e.FirstVisibleItem][0];

				if (!mShowing && firstLetter != mPrevLetter) {
					mShowing = true;
					mDialogText.Visibility = ViewStates.Visible;
				}

				mDialogText.Text = firstLetter.ToString ();
				mHandler.RemoveCallbacks (mRemoveWindow);
				mHandler.PostDelayed (mRemoveWindow, 3000);
				mPrevLetter = firstLetter;
			}
		}

		void RemoveWindow ()
		{
			if (mShowing) {
				mShowing = false;
				mDialogText.Visibility = ViewStates.Invisible;
			}
		}
	}
}

