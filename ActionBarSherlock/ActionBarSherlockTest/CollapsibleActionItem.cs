/*
 * Copyright (C) 2012 Scott Kennedy
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
	[Activity (Label = "@string/collapsible")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class CollapsibleActionItem : SherlockActivity
	{
		public override bool OnCreateOptionsMenu (Xamarin.ActionbarSherlockBinding.Views.IMenu menu)
		{
			//Used to put dark icons on light action bar
			bool isLight = SampleList.THEME == Resource.Style.Theme_Sherlock_Light;

			menu.Add ("Search")
				.SetIcon (isLight ? Resource.Drawable.ic_search_inverse : Resource.Drawable.ic_search)
					.SetActionView (Resource.Layout.collapsible_edittext)
					.SetShowAsAction (MenuItem.ShowAsActionAlways | MenuItem.ShowAsActionCollapseActionView);

			return true;
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.text);
			FindViewById<TextView> (Resource.Id.text).SetText (Resource.String.collapsible_content);
		}
	}
}
