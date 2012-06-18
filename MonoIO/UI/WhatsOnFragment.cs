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
using Java.Lang;
using Math = Java.Lang.Math;
using Android.Text.Format;
using MonoIO.Utilities;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO.UI
{
	public class WhatsOnFragment : Fragment
	{
		private Handler messageHandler = new Handler();
		private TextView countdownTextView;
		private ViewGroup rootView;
		private IRunnable mCountdownRunnable;
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			mCountdownRunnable = new Runnable(() => {
				int remainingSec = (int) Math.Max(0,(UIUtils.CONFERENCE_START_MILLIS - JavaSystem.CurrentTimeMillis()) / 1000);
	            bool conferenceStarted = remainingSec == 0;
	
	            if (conferenceStarted) 
				{
	                // Conference started while in countdown mode, switch modes and
	                // bail on future countdown updates.
	                messageHandler.PostDelayed(() => {
						Refresh();
					}, 100);
	                return;
	            }
	
	            int secs = remainingSec % 86400;
	            int days = remainingSec / 86400;
	            string str = Resources.GetQuantityString(
	                    Resource.Plurals.whats_on_countdown_title, days, days,
	                    DateUtils.FormatElapsedTime(secs));
	            countdownTextView.Text = str;
	
	            // Repost ourselves to keep updating countdown
	            messageHandler.PostDelayed(mCountdownRunnable, 1000);	
			});
			
			rootView = (ViewGroup) inflater.Inflate (Resource.Layout.FragmentWhatsOn, container);
			Refresh();
			return rootView;
		}
		
		public override void OnAttach (Android.App.Activity p0)
		{
			base.OnAttach (p0);
		}
		
		public override void OnDetach ()
		{
			base.OnDetach ();
			messageHandler.RemoveCallbacks(mCountdownRunnable);
		}
		
		private void Refresh() 
		{
			messageHandler.RemoveCallbacks(mCountdownRunnable);
        	rootView.RemoveAllViews();
			
			long currentTimeMillis = UIUtils.GetCurrentTime(Activity);
	
	        // Show Loading... and load the view corresponding to the current state
	        if (currentTimeMillis < UIUtils.CONFERENCE_START_MILLIS) {
	            SetupBefore();
	        } else if (currentTimeMillis > UIUtils.CONFERENCE_END_MILLIS) {
	            SetupAfter();
	        } else {
	            SetupDuring();
	        }
			
			if (!UIUtils.IsHoneycombTablet(Activity)) 
			{
	            var separator = new View(Activity);
	            separator.LayoutParameters = new ViewGroup.LayoutParams(1, ViewGroup.LayoutParams.FillParent);
	            separator.SetBackgroundResource(Resource.Drawable.whats_on_separator);
	            rootView.AddView(separator);
	
	            View view = Activity.LayoutInflater.Inflate(Resource.Layout.whats_on_stream, rootView, false);
				view.Click += (sender, e) => {
					//AnalyticsUtils.getInstance(getActivity()).trackEvent("Home Screen Dashboard", "Click", "Realtime Stream", 0);
                    var intent = new Intent(Activity, typeof(TagStreamActivity));
                    StartActivity(intent);
				};
	            rootView.AddView(view);
	        }
		}
		
		private void SetupBefore() {
	        // Before conference, show countdown.
	        countdownTextView = (TextView) Activity.LayoutInflater.Inflate(Resource.Layout.whats_on_countdown, rootView, false);
	        rootView.AddView(countdownTextView);
	        messageHandler.Post(mCountdownRunnable);
	    }
	
	    private void SetupAfter() {
	        // After conference, show canned text.
	        Activity.LayoutInflater.Inflate(Resource.Layout.whats_on_thank_you, rootView, true);
	    }
	
	    private void SetupDuring ()
		{
			// Conference in progress, show "Now Playing" link.
			View view = Activity.LayoutInflater.Inflate (Resource.Layout.whats_on_now_playing, rootView, false);
			view.Click += (sender, e) => {
			
				if (UIUtils.IsHoneycombTablet(Activity)) {
	                    StartActivity(new Intent(Activity, typeof(NowPlayingMultiPaneActivity)));
                } else {
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(ScheduleContract.Sessions.BuildSessionsAtDirUri(Java.Lang.JavaSystem.CurrentTimeMillis()));
                    intent.PutExtra(Intent.ExtraTitle, GetString(Resource.String.title_now_playing));
                    StartActivity(intent);
                }
			};

			rootView.AddView(view);
		}
		
	}
}