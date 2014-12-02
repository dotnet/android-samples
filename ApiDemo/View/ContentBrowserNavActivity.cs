/*
 * Copyright (C) 2013 The Android Open Source Project
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
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Java.Interop;

namespace MonoDroid.ApiDemo
{
	/**
 	* This activity demonstrates how to use system UI flags to implement
 	* a content browser style of UI (such as a book reader) that hides the
 	* nav bar as well as the status bar.
 	*/
	[Activity (Label = "Views/System UI Visibility/Content Browser Nav Bar", 
		Name = "monodroid.apidemo.ContentBrowserNavActivity",
		Theme = "@android:style/Theme.Holo.Light.DarkActionBar", 
		UiOptions = UiOptions.SplitActionBarWhenNarrow)]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ContentBrowserNavActivity : Activity, SearchView.IOnQueryTextListener, ActionBar.ITabListener
	{
		/**
     	* Implementation of a view for displaying immersive content, using system UI
     	* flags to transition in and out of modes where the user is focused on that
     	* content.
     	*/
		[Register ("monodroid.apidemo.ContentBrowserNavActivity_Content")]
		public class Content : ScrollView, View.IOnSystemUiVisibilityChangeListener, View.IOnClickListener
		{
			TextView mText;
			TextView mTitleView;
			SeekBar mSeekView;
			SystemUiFlags mBaseSystemUiVisibility = SystemUiFlags.Fullscreen
				| SystemUiFlags.LayoutStable | SystemUiFlags.HideNavigation;
			StatusBarVisibility mLastSystemUiVis;
			Action mNavHider;

			public Content (Context context, IAttributeSet attrs) : base (context, attrs)
			{
				mNavHider = () => SetNavVisibility (false);
				mText = new TextView (context);
				mText.SetTextSize (ComplexUnitType.Dip, 16);
				mText.Text = context.GetString (Resource.String.alert_dialog_two_buttons2ultra_msg);
				mText.Clickable = false;
				mText.SetOnClickListener (this);
				mText.SetTextIsSelectable (true);
				AddView (mText, new ViewGroup.LayoutParams (
					ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

				SetOnSystemUiVisibilityChangeListener (this);
			}

			public void Init (TextView title, SeekBar seek)
			{
				// This called by the containing activity to supply the surrounding
				// state of the content browser that it will interact with.
				mTitleView = title;
				mSeekView = seek;
				SetNavVisibility (true);
			}

			public void OnSystemUiVisibilityChange (StatusBarVisibility visibility)
			{
				// Detect when we go out of low-profile mode, to also go out
				// of full screen.  We only do this when the low profile mode
				// is changing from its last state, and turning off.
				StatusBarVisibility diff = mLastSystemUiVis ^ visibility;
				mLastSystemUiVis = visibility;
				if (((int)diff&(int)SystemUiFlags.LowProfile) != 0
					&& ((int)visibility&(int)SystemUiFlags.LowProfile) == 0) {
					SetNavVisibility (true);
				}
			}

			protected override void OnWindowVisibilityChanged (ViewStates visibility)
			{
				// When we become visible, we show our navigation elements briefly
				// before hiding them.
				SetNavVisibility (true);
				Handler.PostDelayed (mNavHider, 2000);
			}

			protected override void OnScrollChanged (int l, int t, int oldl, int oldt)
			{
				// When the user scrolls, we hide navigation elements.
				SetNavVisibility (false);
			}

			public void OnClick (View v)
			{
				// When the user clicks, we toggle the visibility of navigation elements.
				StatusBarVisibility curVis = SystemUiVisibility;
				SetNavVisibility (((int)curVis&(int)SystemUiFlags.LowProfile) != 0);
			}

			public SystemUiFlags BaseSystemUiVisibility {
				set { mBaseSystemUiVisibility = value; }
			}

			void SetNavVisibility (bool visible)
			{
				SystemUiFlags newVis = mBaseSystemUiVisibility;
				if (!visible) {
					newVis |= SystemUiFlags.LowProfile | SystemUiFlags.Fullscreen
					| SystemUiFlags.HideNavigation | SystemUiFlags.Immersive;
				}
				bool changed = (int)newVis == (int)SystemUiVisibility;

				// Unschedule any pending event to hide navigation if we are
				// changing the visibility, or making the UI visible.
				if (changed || visible) {
					var h = Handler;
					if (h != null) {
						h.RemoveCallbacks (mNavHider);
					}
				}

				// Set the new desired visibility.
				SystemUiVisibility = (StatusBarVisibility)newVis;
				mTitleView.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
				mSeekView.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
			}
		}

		Content mContent;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.RequestFeature (WindowFeatures.ActionBarOverlay);

			SetContentView (Resource.Layout.content_browser_nav);
			mContent = FindViewById <Content> (Resource.Id.content);
			mContent.Init (FindViewById <TextView> (Resource.Id.title),
				FindViewById <SeekBar> (Resource.Id.seekbar));

			ActionBar.AddTab (ActionBar.NewTab ().SetText ("Tab 1").SetTabListener (this));
			ActionBar.AddTab (ActionBar.NewTab ().SetText ("Tab 2").SetTabListener (this));
			ActionBar.AddTab (ActionBar.NewTab ().SetText ("Tab 3").SetTabListener (this));
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.content_actions, menu);
			var searchView = (SearchView)menu.FindItem (Resource.Id.action_search).ActionView;
			searchView.SetOnQueryTextListener (this);

			// Set file with share history to the provider and set the share intent.
			IMenuItem actionItem = menu.FindItem (Resource.Id.menu_item_share_action_provider_action_bar);
			var actionProvider = (ShareActionProvider)actionItem.ActionProvider;
			actionProvider.SetShareHistoryFileName (ShareActionProvider.DefaultShareHistoryFileName);
			// Note that you can set/change the intent any time,
			// say when the user has selected an image.
			var shareIntent = new Intent (Intent.ActionSend);
			shareIntent.SetType ("image/*");
			var uri = Android.Net.Uri.FromFile (GetFileStreamPath ("shared.png"));
			shareIntent.PutExtra (Intent.ExtraStream, uri);
			actionProvider.SetShareIntent (shareIntent);
			return true;
		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
		}

		/**
     	* This method is declared in the menu.
     	*/
		[Export]
		public void onSort (IMenuItem item)
		{
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.show_tabs:
				ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
				item.SetChecked (true);
				return true;
			case Resource.Id.hide_tabs:
				ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
				item.SetChecked (true);
				return true;
			case Resource.Id.stable_layout:
				item.SetChecked (!item.IsChecked);
				mContent.BaseSystemUiVisibility = item.IsChecked
					?  SystemUiFlags.LayoutFullscreen
					| SystemUiFlags.LayoutStable
					: SystemUiFlags.LayoutFullscreen;
				return true;
			}
			return false;
		}

		#region SearchView.IOnQueryTextListener members
		public bool OnQueryTextChange (string newText)
		{
			return true;
		}

		public bool OnQueryTextSubmit (string query)
		{
			Toast.MakeText (this, "Searching for: " + query + "...", ToastLength.Short).Show();
			return true;
		}
		#endregion


		#region ActionBar.ITabListener members
		public void OnTabSelected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public void OnTabUnselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}

		public void OnTabReselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
		}
		#endregion
	}
}

