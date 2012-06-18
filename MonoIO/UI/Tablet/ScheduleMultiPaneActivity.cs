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
using Android.Graphics;

namespace MonoIO
{
	[Activity (Label = "@string/title_schedule")]		
	public class ScheduleMultiPaneActivity : BaseMultiPaneActivity, View.IOnClickListener, FragmentManager.IOnBackStackChangedListener
	{
		private FragmentManager mFragmentManager;
    	private FragmentBreadCrumbs mFragmentBreadCrumbs;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			SetContentView(Resource.Layout.activity_schedule);
	
			mFragmentManager = SupportFragmentManager;
			mFragmentBreadCrumbs = FindViewById<FragmentBreadCrumbs>(Resource.Id.breadcrumbs);
			mFragmentBreadCrumbs.SetActivity(this);
			mFragmentManager.AddOnBackStackChangedListener(this);
	
			UpdateBreadCrumb();
		}
		
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			
			ActivityHelper.SetupSubActivity();

			ViewGroup detailContainer = FindViewById<ViewGroup>(Resource.Id.fragment_container_schedule_detail);
			if (detailContainer != null && detailContainer.ChildCount > 0) {
				FindViewById(Resource.Id.fragment_container_schedule_detail).SetBackgroundColor(new Color(0));
			}
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch (string activityClassName)
		{
			if (Java.Lang.Class.FromType (typeof(SessionsActivity)).Name.Equals (activityClassName)) {
				SupportFragmentManager.PopBackStack ();
				FindViewById (Resource.Id.fragment_container_schedule_detail).SetBackgroundColor (new Color (0));
				return new FragmentReplaceInfo (
	                    Java.Lang.Class.FromType (typeof(SessionsFragment)),
	                    "sessions",
	                    Resource.Id.fragment_container_schedule_detail);
			} else if (Java.Lang.Class.FromType (typeof(SessionDetailActivity)).Name.Equals (activityClassName)) {
				FindViewById (Resource.Id.fragment_container_schedule_detail).SetBackgroundColor (new Color (0));
				return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionDetailFragment)),
	                    "session_detail",
	                    Resource.Id.fragment_container_schedule_detail);
			}
			return null;
		}
		
		protected override void OnBeforeCommitReplaceFragment(FragmentManager fm, Android.Support.V4.App.FragmentTransaction ft, Fragment fragment)
		{
			base.OnBeforeCommitReplaceFragment(fm, ft, fragment);
			
			if (fragment is SessionDetailFragment) {
				ft.AddToBackStack(null);
			} else if (fragment is SessionsFragment) {
				fm.PopBackStack();
			}
			UpdateBreadCrumb();
		}
		
		/**
	     * Handler for the breadcrumb parent.
	     */
	    public void OnClick(View view) {
	        mFragmentManager.PopBackStack();
	    }
	
	    public void OnBackStackChanged()
		{
			UpdateBreadCrumb();
		}
		
		public void UpdateBreadCrumb()
		{
			String title = GetString(Resource.String.title_sessions);
			String detailTitle = GetString(Resource.String.title_session_detail);
	
			if (mFragmentManager.BackStackEntryCount >= 1) {
				mFragmentBreadCrumbs.SetParentTitle(title, title, this);
				mFragmentBreadCrumbs.SetTitle(detailTitle, detailTitle);
			} else {
				mFragmentBreadCrumbs.SetParentTitle((string)null, null, null);
				mFragmentBreadCrumbs.SetTitle(title, title);
			}
		}

	}
}
