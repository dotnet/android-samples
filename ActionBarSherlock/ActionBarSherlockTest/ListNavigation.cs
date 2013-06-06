
// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/list_navigation")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class ListNavigation : SherlockActivity, SherlockActionBar.IOnNavigationListener
	{
		private TextView mSelected;
		private String[] mLocations;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.list_navigation);
			mSelected = FindViewById<TextView> (Resource.Id.text);

			mLocations = Resources.GetStringArray (Resource.Array.locations);

			Context context = SupportActionBar.ThemedContext;
			var list = ArrayAdapter.CreateFromResource (context, Resource.Array.locations, Resource.Layout.sherlock_spinner_item);
			list.SetDropDownViewResource (Resource.Layout.sherlock_spinner_dropdown_item);

			SupportActionBar.NavigationMode = (SherlockActionBar.NavigationModeList);
			SupportActionBar.SetListNavigationCallbacks (list, this);
		}

		public bool OnNavigationItemSelected (int itemPosition, long itemId)
		{
			mSelected.Text = ("Selected: " + mLocations [itemPosition]);
			return true;
		}
	}
}
