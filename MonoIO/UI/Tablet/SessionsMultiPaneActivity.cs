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
using Android.Graphics;

namespace MonoIO
{
	[Activity(Label = "@string/title_sessions")]
	public class SessionsMultiPaneActivity : BaseMultiPaneActivity
	{
		private TracksDropdownFragment mTracksDropdownFragment;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			
			SetContentView(Resource.Layout.activity_sessions);

			Intent intent = new Intent();
			intent.SetData(ScheduleContract.Tracks.CONTENT_URI);
			intent.PutExtra(TracksFragment.EXTRA_NEXT_TYPE, TracksFragment.NEXT_TYPE_SESSIONS);
	
			FragmentManager fm = SupportFragmentManager;
			mTracksDropdownFragment = (TracksDropdownFragment)fm.FindFragmentById(Resource.Id.fragment_tracks_dropdown);
			mTracksDropdownFragment.ReloadFromArguments(IntentToFragmentArguments(intent));
		}
		
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			
			ActivityHelper.SetupSubActivity();

			ViewGroup detailContainer = FindViewById<ViewGroup>(Resource.Id.fragment_container_session_detail);
			if (detailContainer != null && detailContainer.ChildCount > 0) {
				FindViewById(Resource.Id.fragment_container_session_detail).SetBackgroundColor(Color.ParseColor("#ffffffff"));
			}
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch(string activityClassName)
		{
			if (Java.Lang.Class.FromType(typeof(SessionsActivity)).Name.Equals(activityClassName)) {
	            return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionsFragment)),
	                    "sessions",
	                    Resource.Id.fragment_container_sessions);
	        } else if (Java.Lang.Class.FromType(typeof(SessionDetailActivity)).Name.Equals(activityClassName)) {
	            FindViewById(Resource.Id.fragment_container_session_detail).SetBackgroundColor(Color.ParseColor("#ffffffff"));
	            return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(SessionDetailFragment)),
	                    "session_detail",
	                    Resource.Id.fragment_container_session_detail);
	        }
	        return null;
		}
	}
}

