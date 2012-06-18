/*
 * Copyright 2011 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Support.V4.App;
using MonoIO.Utilities;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO.UI
{
	public class DashboardFragment : Fragment
	{
		public void FireTrackerEvent(string label) 
		{
	       // AnalyticsUtils.getInstance(getActivity()).trackEvent(
	       //       "Home Screen Dashboard", "Click", label, 0);
	    }
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.FragmentDashboard, container);
			
			root.FindViewById<Button> (Resource.Id.home_btn_schedule).Click += (sender, e) => {
				FireTrackerEvent ("Schedule");
				
				if (UIUtils.IsHoneycombTablet(Activity)) {
					StartActivity(new Intent(Activity, typeof(ScheduleMultiPaneActivity)));
				} else {
					StartActivity(new Intent(Activity, typeof(ScheduleActivity)));	
				}
			};
			
			root.FindViewById<Button>(Resource.Id.home_btn_sessions).Click += (sender, e) => {
				FireTrackerEvent("Sessions");
				// Launch sessions list
			
				if (UIUtils.IsHoneycombTablet(Activity)) {
					StartActivity(new Intent(Activity, typeof(SessionsMultiPaneActivity)));
				} else {
					Intent intent = new Intent(Intent.ActionView, ScheduleContract.Tracks.CONTENT_URI);
					intent.PutExtra(Intent.ExtraTitle, GetString(Resource.String.title_session_tracks));
					intent.PutExtra(TracksFragment.EXTRA_NEXT_TYPE, TracksFragment.NEXT_TYPE_SESSIONS);
					StartActivity(intent);
				}				
			};
			
			root.FindViewById<Button>(Resource.Id.home_btn_starred).Click += (sender, e) => {
				FireTrackerEvent("Starred");
				StartActivity(new Intent(Activity, typeof(StarredActivity)));
			};
			
			root.FindViewById<Button>(Resource.Id.home_btn_vendors).Click += (sender, e) => {
				FireTrackerEvent("Sandbox");
				// Launch vendors list
				
				if (UIUtils.IsHoneycombTablet(Activity)) {
					StartActivity(new Intent(Activity, typeof(VendorsMultiPaneActivity)));
				} else {
					Intent intent = new Intent(Intent.ActionView, ScheduleContract.Tracks.CONTENT_URI);
					intent.PutExtra(Intent.ExtraTitle, GetString(Resource.String.title_session_tracks));
					intent.PutExtra(TracksFragment.EXTRA_NEXT_TYPE, TracksFragment.NEXT_TYPE_SESSIONS);
					StartActivity(intent);
				}

			};
		
			root.FindViewById<Button>(Resource.Id.home_btn_map).Click += (sender, e) => {
				FireTrackerEvent("Map");
				StartActivity(new Intent(Activity, UIUtils.GetMapActivityClass(Activity)));

			};
	
			root.FindViewById<Button>(Resource.Id.home_btn_announcements).Click += (sender, e) => {
				FireTrackerEvent("Bulletin");
				var intent = new Intent(Activity, typeof(BulletinActivity));
				StartActivity(intent);
			};
	            
			return root;
		}
	}
}