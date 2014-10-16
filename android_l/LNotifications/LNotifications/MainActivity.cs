using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.OS;

namespace LNotifications
{
	// Launcher Activity for the L Notification samples application
	[Activity (Label = "LNotifications", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_notification);
			SetTitle (Resource.String.title_lnotification_activity);

			// Use ViewPager in the support library where possible.
			// At this time, the support library for L is not ready so using the depreciated method
			// to create tabs.
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.Tab tabHeadsUpNotification = ActionBar.NewTab ().SetText ("Heads Up");
			ActionBar.Tab tabVisibilityMetaData = ActionBar.NewTab ().SetText ("Visibility");
			ActionBar.Tab tabOtherMetaData = ActionBar.NewTab ().SetText ("Others");

			tabHeadsUpNotification.SetTabListener (new FragmentTabListener (HeadsUpNotificationFragment
				.NewInstance ()));
			tabVisibilityMetaData.SetTabListener (new FragmentTabListener (VisibilityMetaDataFragment
				.NewInstance ()));
			tabOtherMetaData.SetTabListener (new FragmentTabListener (OtherMetaDataFragment
				.NewInstance ()));

			ActionBar.AddTab (tabHeadsUpNotification, 0);
			ActionBar.AddTab (tabVisibilityMetaData, 1);
			ActionBar.AddTab (tabOtherMetaData, 2);

		}

		// TabListener that replaces a Fragment when a tab is clicked.
		private class FragmentTabListener : Java.Lang.Object,ActionBar.ITabListener
		{
			public Fragment fragment;
			public FragmentTabListener(Fragment frag)
			{
				this.fragment = frag;
			}

			public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
			{
			}

			public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
			{
				ft.Replace (Resource.Id.container, fragment);
			}

			public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft) 
			{
				ft.Remove (fragment);
			}
		}
	}
}


