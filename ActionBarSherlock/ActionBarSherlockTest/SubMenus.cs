
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/submenus")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class SubMenus : SherlockActivity
	{
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			var subMenu1 = menu.AddSubMenu ("Action Item");
			subMenu1.Add ("Sample");
			subMenu1.Add ("Menu");
			subMenu1.Add ("Items");

			var subMenu1Item = subMenu1.Item;
			subMenu1Item.SetIcon (Resource.Drawable.ic_title_share_default);
			subMenu1Item.SetShowAsAction (MenuItem.ShowAsActionAlways | MenuItem.ShowAsActionWithText);

			var subMenu2 = menu.AddSubMenu ("Overflow Item");
			subMenu2.Add ("These");
			subMenu2.Add ("Are");
			subMenu2.Add ("Sample");
			subMenu2.Add ("Items");

			var subMenu2Item = subMenu2.Item;
			subMenu2Item.SetIcon (Resource.Drawable.ic_compose);

			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.text);
			FindViewById<TextView> (Resource.Id.text).SetText (Resource.String.submenus_content);
		}
	}
}
