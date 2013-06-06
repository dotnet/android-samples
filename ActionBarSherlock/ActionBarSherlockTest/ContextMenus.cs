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

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/context_menus")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class ContextMenus : SherlockActivity
	{
		public override bool OnCreateOptionsMenu (Xamarin.ActionbarSherlockBinding.Views.IMenu menu)
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

		public override bool OnOptionsItemSelected (Xamarin.ActionbarSherlockBinding.Views.IMenuItem item)
		{
			//This uses the imported MenuItem from ActionBarSherlock
			Toast.MakeText (this, "Got click: " + item.ToString (), ToastLength.Short).Show ();
			return true;
		}

		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			menu.Add ("One");
			menu.Add ("Two");
			menu.Add ("Three");
			menu.Add ("Four");
		}

		public override bool OnContextItemSelected (Android.Views.IMenuItem item)
		{
			//Note how this callback is using the fully-qualified class name
			Toast.MakeText (this, "Got click: " + item.ToString (), ToastLength.Short).Show ();
			return true;
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.context_menus);
			RegisterForContextMenu (FindViewById (Resource.Id.show_context_menu));
		}
	}
}

