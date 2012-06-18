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
	[Activity (Label = "@string/title_vendors")]			
	public class VendorsMultiPaneActivity : BaseMultiPaneActivity
	{
		private TracksDropdownFragment mTracksDropdownFragment;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			SetContentView(Resource.Layout.activity_vendors);

			Intent intent = new Intent();
			intent.SetData(ScheduleContract.Tracks.CONTENT_URI);
			intent.PutExtra(TracksFragment.EXTRA_NEXT_TYPE, TracksFragment.NEXT_TYPE_VENDORS);
	
			FragmentManager fm = SupportFragmentManager;
			mTracksDropdownFragment = (TracksDropdownFragment)fm.FindFragmentById(Resource.Id.fragment_tracks_dropdown);
			mTracksDropdownFragment.ReloadFromArguments(IntentToFragmentArguments(intent));
		}
		
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			
			ActivityHelper.SetupSubActivity();

			var detailContainer = FindViewById<ViewGroup>(Resource.Id.fragment_container_vendor_detail);
			if (detailContainer != null && detailContainer.ChildCount > 0) {
				FindViewById(Resource.Id.fragment_container_vendor_detail).SetBackgroundColor(Color.ParseColor("#ffffffff"));
			}
		}
		
		protected override FragmentReplaceInfo OnSubstituteFragmentForActivityLaunch(string activityClassName)
		{
			if (Java.Lang.Class.FromType(typeof(VendorsActivity)).Name.Equals(activityClassName)) {
	            return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(VendorsFragment)),
	                    "vendors",
	                    Resource.Id.fragment_container_vendors);
	        } else if (Java.Lang.Class.FromType(typeof(VendorDetailActivity)).Name.Equals(activityClassName)) {
	            FindViewById(Resource.Id.fragment_container_vendor_detail).SetBackgroundColor(Color.ParseColor("#ffffffff"));
	            return new FragmentReplaceInfo(
	                    Java.Lang.Class.FromType(typeof(VendorDetailFragment)),
	                    "vendor_detail",
	                    Resource.Id.fragment_container_vendor_detail);
	        }
	        return null;
		}
	}
}

