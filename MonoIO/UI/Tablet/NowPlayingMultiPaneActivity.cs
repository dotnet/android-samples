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
	[Activity (Label = "@string/title_now_playing")]			
	public class NowPlayingMultiPaneActivity : BaseMultiPaneActivity
	{
		
		private SessionsFragment mSessionsFragment;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			
			Intent intent = new Intent();
			intent.SetData(ScheduleContract.Sessions.BuildSessionsAtDirUri(Java.Lang.JavaSystem.CurrentTimeMillis()));
	
			SetContentView(Resource.Layout.activity_now_playing);
	
			FragmentManager fm = SupportFragmentManager;
			mSessionsFragment = (SessionsFragment)fm.FindFragmentByTag("sessions");
			if (mSessionsFragment == null) {
				mSessionsFragment = new SessionsFragment();
				mSessionsFragment.Arguments = IntentToFragmentArguments(intent);
				fm.BeginTransaction()
	                    .Add(Resource.Id.fragment_container_sessions, mSessionsFragment, "sessions")
	                    .Commit();
			}
		}
		
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			
			ActivityHelper.SetupSubActivity();

			ViewGroup detailContainer = FindViewById<ViewGroup>(Resource.Id.fragment_container_now_playing_detail);
			if (detailContainer != null && detailContainer.ChildCount > 1) {
				FindViewById(Android.Resource.Id.Empty).Visibility = ViewStates.Gone;
			}
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch(string activityClassName)
		{
			FindViewById(Android.Resource.Id.Empty).Visibility = ViewStates.Gone;
	        if (Java.Lang.Class.FromType(typeof(SessionDetailActivity)).Name.Equals(activityClassName)) {
	            ClearSelectedItems();
	            return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionDetailFragment)),
	                    "session_detail",
	                    Resource.Id.fragment_container_now_playing_detail);
	        }
	        return null;
		}
		
		private void ClearSelectedItems()
		{
			if (mSessionsFragment != null) {
				mSessionsFragment.ClearCheckedPosition();
			}
		}
	}
}

