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

using FragmentManager = Android.Support.V4.App.FragmentManager;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO
{
	[Activity (Label = "@string/title_map")]
	public class MapMultiPaneActivity : BaseMultiPaneActivity, View.IOnClickListener, FragmentManager.IOnBackStackChangedListener
	{
		
		private static int POPUP_TYPE_SESSIONS = 1;
		private static int POPUP_TYPE_VENDORS = 2;
		
		private int mPopupType = -1;
		private bool mPauseBackStackWatcher = false;
		
		private FragmentManager mFragmentManager;
		private FragmentBreadCrumbs mFragmentBreadCrumbs;
		
		private MapFragment mMapFragment;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			
			SetContentView(Resource.Layout.activity_map);

			mFragmentManager = SupportFragmentManager;
			mFragmentManager.AddOnBackStackChangedListener(this);
	
			mFragmentBreadCrumbs = FindViewById<FragmentBreadCrumbs>(Resource.Id.breadcrumbs);
			mFragmentBreadCrumbs.SetActivity(this);
	
			mMapFragment = (MapFragment)mFragmentManager.FindFragmentByTag("map");
			if (mMapFragment == null) {
				mMapFragment = new MapFragment();
				mMapFragment.Arguments = IntentToFragmentArguments(Intent);
	
				mFragmentManager.BeginTransaction()
	                    .Add(Resource.Id.fragment_container_map, mMapFragment, "map")
	                    .Commit();
			}
	
			FindViewById<ImageButton>(Resource.Id.close_button).Click += (sender, e) => {
				ClearBackStack(SupportFragmentManager);	
			};
	
			UpdateBreadCrumb();
		}
		
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			
			ActivityHelper.SetupSubActivity();
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch(string activityClassName)
		{
			if (Java.Lang.Class.FromType(typeof(SessionsActivity)).Name.Equals(activityClassName)) {
				ClearBackStack(SupportFragmentManager);
				mPopupType = POPUP_TYPE_SESSIONS;
				ShowHideDetailAndPan(true);
				return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionsFragment)),
	                    "sessions",
	                    Resource.Id.fragment_container_map_detail);
			} else if (Java.Lang.Class.FromType(typeof(SessionDetailActivity)).Name.Equals(activityClassName)) {
				mPopupType = POPUP_TYPE_SESSIONS;
				ShowHideDetailAndPan(true);
				return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionDetailFragment)),
	                    "session_detail",
	                    Resource.Id.fragment_container_map_detail);
			} else if (Java.Lang.Class.FromType(typeof(VendorsActivity)).Name.Equals(activityClassName)) {
				ClearBackStack(SupportFragmentManager);
				mPopupType = POPUP_TYPE_VENDORS;
				ShowHideDetailAndPan(true);
				return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(VendorsFragment)),
	                    "vendors",
	                    Resource.Id.fragment_container_map_detail);
			} else if (Java.Lang.Class.FromType(typeof(VendorDetailActivity)).Name.Equals(activityClassName)) {
				mPopupType = POPUP_TYPE_VENDORS;
				ShowHideDetailAndPan(true);
				return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(VendorDetailFragment)),
	                    "vendor_detail",
	                    Resource.Id.fragment_container_map_detail);
			}
			return null;
		}
		
		protected override void OnBeforeCommitReplaceFragment(FragmentManager fm, Android.Support.V4.App.FragmentTransaction ft, Fragment fragment)
		{
			base.OnBeforeCommitReplaceFragment(fm, ft, fragment);
			
			if (fragment is SessionsFragment || fragment is VendorsFragment) {
				mPauseBackStackWatcher = true;
				ClearBackStack(fm);
				mPauseBackStackWatcher = false;
			}
			ft.AddToBackStack(null);
			UpdateBreadCrumb();
		}
		
		#region IOnClickListener implementation
		public void OnClick(View v)
		{
			mFragmentManager.PopBackStack();
		}
		#endregion
		
		
		private void ClearBackStack(FragmentManager fm)
		{
			while (fm.BackStackEntryCount > 0) {
				fm.PopBackStackImmediate();
			}
		}
		
		#region IOnBackStackChangedListener implementation
		public void OnBackStackChanged()
		{
			if (mPauseBackStackWatcher) {
				return;
			}
	
			if (mFragmentManager.BackStackEntryCount == 0) {
				ShowHideDetailAndPan(false);
			}
			UpdateBreadCrumb();
		}
		#endregion
		
		private void ShowHideDetailAndPan(bool show)
		{
			View detailPopup = FindViewById(Resource.Id.map_detail_popup);
			if (show != (detailPopup.Visibility == ViewStates.Visible)) {
				detailPopup.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
				mMapFragment.PanLeft(show ? 0.25f : -0.25f);
			}
		}
		
		public void UpdateBreadCrumb()
		{
			String title = (mPopupType == POPUP_TYPE_SESSIONS)
	                ? GetString(Resource.String.title_sessions)
	                : GetString(Resource.String.title_vendors);
			String detailTitle = (mPopupType == POPUP_TYPE_SESSIONS)
	                ? GetString(Resource.String.title_session_detail)
	                : GetString(Resource.String.title_vendor_detail);
	
			if (mFragmentManager.BackStackEntryCount >= 2) {
				mFragmentBreadCrumbs.SetParentTitle(title, title, this);
				mFragmentBreadCrumbs.SetTitle(detailTitle, detailTitle);
			} else {
				mFragmentBreadCrumbs.SetParentTitle((string) null, null, null);
				mFragmentBreadCrumbs.SetTitle(title, title);
			}
		}

		
	}
}