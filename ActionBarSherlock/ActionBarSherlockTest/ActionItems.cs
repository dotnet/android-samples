/*
 * Copyright (C) 2011 Jake Wharton
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

// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using Xamarin.ActionbarSherlockBinding.Views;
using Tab = Xamarin.ActionbarSherlockBinding.App.ActionBar.Tab;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ActionProvider = Xamarin.ActionbarSherlockBinding.Views.ActionProvider;
using ActionMode = Xamarin.ActionbarSherlockBinding.Views.ActionMode;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;
using Android.Provider;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/action_items")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class ActionItems : SherlockActivity
	{
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			//Used to put dark icons on light action bar
			bool isLight = SampleList.THEME == Resource.Style.Theme_Sherlock_Light;

			menu.Add ("Save")
				.SetIcon (isLight ? Resource.Drawable.ic_compose_inverse : Resource.Drawable.ic_compose)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom);

			menu.Add ("Search")
				.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			menu.Add ("Refresh")
				.SetIcon (isLight ? Resource.Drawable.ic_refresh_inverse : Resource.Drawable.ic_refresh)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			return true;
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.text);
			SetContent ((TextView)FindViewById (Resource.Id.text));
		}

		protected void SetContent (TextView view)
		{
			view.SetText (Resource.String.action_items_content);
		}
	}
}

