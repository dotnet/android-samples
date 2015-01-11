
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/tab_navigation_collapsed")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class TabNavigationCollapsed : TabNavigation
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			//The following two options trigger the collapsing of the main action bar view.
			//See the parent activity for the rest of the implementation
			SupportActionBar.SetDisplayShowHomeEnabled (false);
			SupportActionBar.SetDisplayShowTitleEnabled (false);
		}
	}
}

