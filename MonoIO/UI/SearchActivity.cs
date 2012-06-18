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
using Android.Content.PM;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Graphics;

namespace MonoIO
{
	[Activity (Label = "@string/title_search", LaunchMode = LaunchMode.SingleTop)]
	[IntentFilter (new [] { Intent.ActionSearch }, Categories = new [] { Intent.CategoryDefault })]
	[MetaData("android.app.searchable", Resource="@xml/searchable")]
	public class SearchActivity : BaseMultiPaneActivity
	{
		public static String TAG_SESSIONS = "sessions";
		public static String TAG_VENDORS = "vendors";
		private String mQuery;
		private TabHost mTabHost;
		private TabWidget mTabWidget;
		private SessionsFragment mSessionsFragment;
		private VendorsFragment mVendorsFragment;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			Intent intent = Intent;        
			mQuery = intent.GetStringExtra (SearchManager.Query);
	
			SetContentView (Resource.Layout.activity_search);
	
			ActivityHelper.SetupActionBar (new Java.Lang.String (Title), new Color (0));
			var title = GetString (Resource.String.title_search_query, mQuery);
			ActivityHelper.SetActionBarTitle (new Java.Lang.String (title));
	
			mTabHost = FindViewById<TabHost> (Android.Resource.Id.TabHost);
			mTabWidget = FindViewById<TabWidget> (Android.Resource.Id.Tabs);
			mTabHost.Setup ();
	
			SetupSessionsTab ();
			SetupVendorsTab ();
		}
		
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			
			ActivityHelper.SetupSubActivity ();
	
			ViewGroup detailContainer = FindViewById<ViewGroup> (Resource.Id.fragment_container_search_detail);
			if (detailContainer != null && detailContainer.ChildCount > 1) {
				FindViewById (Android.Resource.Id.Empty).Visibility = ViewStates.Gone;
			}
		}
		
		protected override void OnNewIntent (Android.Content.Intent intent)
		{
			Intent = intent;
			mQuery = intent.GetStringExtra (SearchManager.Query);
	
			var title = GetString (Resource.String.title_search_query, mQuery);
			ActivityHelper.SetActionBarTitle (new Java.Lang.String (title));
	
			mTabHost.CurrentTab = 0;
	
			mSessionsFragment.ReloadFromArguments (GetSessionsFragmentArguments ());
			mVendorsFragment.ReloadFromArguments (GetVendorsFragmentArguments ());
		}
		
		/**
	     * Build and add "sessions" tab.
	     */
		private void SetupSessionsTab ()
		{
			// TODO: this is very inefficient and messy, clean it up
			FrameLayout fragmentContainer = new FrameLayout (this);
			fragmentContainer.Id = Resource.Id.fragment_sessions;
			fragmentContainer.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			FindViewById<ViewGroup> (Android.Resource.Id.TabContent).AddView (fragmentContainer);
	
			FragmentManager fm = SupportFragmentManager;
			mSessionsFragment = (SessionsFragment)fm.FindFragmentByTag ("sessions");
			if (mSessionsFragment == null) {
				mSessionsFragment = new SessionsFragment ();
				mSessionsFragment.Arguments = GetSessionsFragmentArguments ();
				fm.BeginTransaction ()
	                    .Add (Resource.Id.fragment_sessions, mSessionsFragment, "sessions")
	                    .Commit ();
			} else {
				mSessionsFragment.ReloadFromArguments (GetSessionsFragmentArguments ());
			}
	
			// Sessions content comes from reused activity
			mTabHost.AddTab (mTabHost.NewTabSpec (TAG_SESSIONS)
	                .SetIndicator (BuildIndicator (Resource.String.starred_sessions))
	                .SetContent (Resource.Id.fragment_sessions));
		}
		
		/**
	     * Build and add "vendors" tab.
	     */
		private void SetupVendorsTab ()
		{
			// TODO: this is very inefficient and messy, clean it up
			FrameLayout fragmentContainer = new FrameLayout (this);
			fragmentContainer.Id = Resource.Id.fragment_vendors;
			fragmentContainer.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			FindViewById<ViewGroup> (Android.Resource.Id.TabContent).AddView (fragmentContainer);
	
			FragmentManager fm = SupportFragmentManager;
			mVendorsFragment = (VendorsFragment)fm.FindFragmentByTag ("vendors");
			if (mVendorsFragment == null) {
				mVendorsFragment = new VendorsFragment ();
				mVendorsFragment.Arguments = GetVendorsFragmentArguments ();
				fm.BeginTransaction ()
	                    .Add (Resource.Id.fragment_vendors, mVendorsFragment, "vendors")
	                    .Commit ();
			} else {
				mVendorsFragment.ReloadFromArguments (GetVendorsFragmentArguments ());
			}
	
			// Vendors content comes from reused activity
			mTabHost.AddTab (mTabHost.NewTabSpec (TAG_VENDORS)
	                .SetIndicator (BuildIndicator (Resource.String.starred_vendors))
	                .SetContent (Resource.Id.fragment_vendors));
		}
		
		private Bundle GetSessionsFragmentArguments ()
		{
			return IntentToFragmentArguments (new Intent (Intent.ActionView, ScheduleContract.Sessions.BuildSearchUri (mQuery)));
		}
	
		private Bundle GetVendorsFragmentArguments ()
		{
			return IntentToFragmentArguments (new Intent (Intent.ActionView, ScheduleContract.Vendors.BuildSearchUri (mQuery)));
		}
		
		/**
	     * Build a {@link View} to be used as a tab indicator, setting the requested string resource as
	     * its label.
	     */
		private View BuildIndicator (int textRes)
		{
			TextView indicator = (TextView)LayoutInflater.Inflate (Resource.Layout.tab_indicator, mTabWidget, false);
			indicator.SetText (textRes);
			return indicator;
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch (string activityClassName)
		{
			if (FindViewById (Resource.Id.fragment_container_search_detail) != null) {
				// The layout we currently have has a detail container, we can add fragments there.
				FindViewById (Android.Resource.Id.Empty).Visibility = ViewStates.Gone;
				if (Java.Lang.Class.FromType (typeof(SessionDetailActivity)).Name.Equals (activityClassName)) {
					ClearSelectedItems ();
					return new FragmentReplaceInfo (
	                        Java.Lang.Class.FromType (typeof(SessionDetailFragment)),
	                        "session_detail",
	                        Resource.Id.fragment_container_search_detail);
				} else if (Java.Lang.Class.FromType (typeof(VendorDetailActivity)).Name.Equals (activityClassName)) {
					ClearSelectedItems ();
					return new FragmentReplaceInfo (
	                        Java.Lang.Class.FromType (typeof(VendorDetailActivity)),
	                        "vendor_detail",
	                        Resource.Id.fragment_container_search_detail);
				}
			}
			return null;
		}
		
		private void ClearSelectedItems ()
		{
			if (mSessionsFragment != null) {
				mSessionsFragment.ClearCheckedPosition ();
			}
			if (mVendorsFragment != null) {
				mVendorsFragment.ClearCheckedPosition ();
			}
		}
	}
}

