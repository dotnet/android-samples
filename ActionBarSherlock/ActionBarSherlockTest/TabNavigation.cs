
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/tab_navigation")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class TabNavigation : SherlockActivity, SherlockActionBar.ITabListener
	{
		private TextView mSelected;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.tab_navigation);
			mSelected = FindViewById<TextView> (Resource.Id.text);

			SupportActionBar.NavigationMode = SherlockActionBar.NavigationModeTabs;
			for (int i = 1; i <= 3; i++) {
				var tab = SupportActionBar.NewTab ();
				tab.SetText ("Tab " + i);
				tab.SetTabListener (this);
				SupportActionBar.AddTab (tab);
			}
		}

		public void OnTabReselected (SherlockActionBar.Tab tab, FragmentTransaction transaction)
		{
		}

		public void OnTabSelected (SherlockActionBar.Tab tab, FragmentTransaction transaction)
		{
			mSelected.Text = "Selected: " + tab.Text;
		}

		public void OnTabUnselected (SherlockActionBar.Tab tab, FragmentTransaction transaction)
		{
		}
	}
}
