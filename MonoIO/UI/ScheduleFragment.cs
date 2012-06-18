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
using Android.Text.Format;
using Android.Database;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.View;
using Math = Java.Lang.Math;
using MonoIO.UI.Widget;
using Android.Provider;
using Uri = Android.Net.Uri;
using Android.Graphics.Drawables;

namespace MonoIO.UI
{
	public class ScheduleFragment : Fragment, NotifyingAsyncQueryHandler.AsyncQueryListener, ObservableScrollView.OnScrollListener, View.IOnClickListener
	{
		public string TAG = "ScheduleFragment";
		
		/**
	     * Flags used with {@link android.text.format.DateUtils#formatDateRange}.
	     */
		private const FormatStyleFlags TIME_FLAGS = FormatStyleFlags.ShowDate | FormatStyleFlags.ShowWeekday | FormatStyleFlags.AbbrevWeekday;

    	private long TUE_START = ParserUtils.ParseTime("2011-05-10T00:00:00.000-07:00");
	    private long WED_START = ParserUtils.ParseTime("2011-05-11T00:00:00.000-07:00");
	
	    const int DISABLED_BLOCK_ALPHA = 100;
	
	    public Dictionary<string, int> TypeColumnMap = BuildTypeColumnMap();
	
	    // TODO: show blocks that don't fall into columns at the bottom
	
	    const string EXTRA_TIME_START = "monoio.extra.TIME_START";
	    const string EXTRA_TIME_END = "monoio.extra.TIME_END";
	
	    private NotifyingAsyncQueryHandler mHandler;
	
	    private Workspace mWorkspace;
	    private TextView mTitle;
	    private int mTitleCurrentDayIndex = -1;
	    private View mLeftIndicator;
	    private View mRightIndicator;
		
		public SessionChangesObserver mSessionChangesObserver;
		public MyBroadcastReceiver mReceiver;
		
		/**
	     * A helper class containing object references related to a particular day in the schedule.
	     */
	    private class Day : Java.Lang.Object 
		{
	        public ViewGroup RootView;
	        public ObservableScrollView ScrollView;
	        public View NowView;
	        public BlocksLayout BlocksView;
	
	        public int Index = -1;
	        public String Label = null;
	        public Uri BlocksUri = null;
	        public long TimeStart = -1;
	        public long TimeEnd = -1;
	    }
		
		private List<Day> mDays = new List<Day>();

	    private static Dictionary<string, int> BuildTypeColumnMap() {
	        var map = new Dictionary<string, int>();
			map.Add(ParserUtils.BlockTypeFood, 0);
			map.Add(ParserUtils.BlockTypeSession, 1);
			map.Add(ParserUtils.BlockTypeOfficeHours, 2);
	        return map;
	    }
		
		public override void OnCreate (Bundle p0)
		{
			base.OnCreate (p0);
			mHandler = new NotifyingAsyncQueryHandler(Activity.ContentResolver, this);
			mSessionChangesObserver = new SessionChangesObserver(this);
			mReceiver = new MyBroadcastReceiver(this);
			
	        SetHasOptionsMenu(true);
	        //AnalyticsUtils.getInstance(Activity).trackPageView("/Schedule");
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup) inflater.Inflate(Resource.Layout.fragment_schedule, null);

	        mWorkspace = root.FindViewById<Workspace>(Resource.Id.workspace);
	
	        mTitle = root.FindViewById<TextView>(Resource.Id.block_title);
	
	        mLeftIndicator = root.FindViewById(Resource.Id.indicator_left);
			mLeftIndicator.Touch += delegate(object sender, Android.Views.View.TouchEventArgs e) {
				if(((int)e.Event.Action & MotionEventCompat.ActionMask) == (int) MotionEventActions.Down)
				{
					mWorkspace.ScrollLeft();
					e.Handled = true;
				}
				e.Handled = false;
			};
			mLeftIndicator.Click += (sender, e) => {
				mWorkspace.ScrollLeft();
			};	
				
	        mRightIndicator = root.FindViewById(Resource.Id.indicator_right);
	        
			mRightIndicator.Touch += delegate(object sender, Android.Views.View.TouchEventArgs e) {
				if(((int)e.Event.Action & MotionEventCompat.ActionMask) == (int) MotionEventActions.Down)
				{
					mWorkspace.ScrollRight();
					e.Handled = true;
				}
				e.Handled = false;
			};
		
			mRightIndicator.Click += (sender, e) => {
				mWorkspace.ScrollRight();	
			};
			
	        SetupDay(inflater, TUE_START);
	        SetupDay(inflater, WED_START);
	
	        UpdateWorkspaceHeader(0);
			mWorkspace.SetOnScrollListener(new MyScrollListener(this), true);
			
	        return root;
		}
		
		public class MyScrollListener : Workspace.OnScrollListener
		{
			public ScheduleFragment _fragment;
			
			public MyScrollListener(ScheduleFragment fragment)
			{
				_fragment = fragment;
			}
			
			public void OnScroll (float screenFraction)
			{
				_fragment.UpdateWorkspaceHeader(Math.Round(screenFraction));
			}	
		}
		
	    public void UpdateWorkspaceHeader(int dayIndex) {
	        if (mTitleCurrentDayIndex == dayIndex) {
	            return;
	        }
	
	        mTitleCurrentDayIndex = dayIndex;
	        Day day = mDays[dayIndex];
	        mTitle.Text = day.Label;
	
	        mLeftIndicator.Visibility = (dayIndex != 0) ? ViewStates.Visible : ViewStates.Invisible;
			mRightIndicator.Visibility = (dayIndex < mDays.Count - 1) ? ViewStates.Visible : ViewStates.Invisible;
	    }
		
		private void SetupDay(LayoutInflater inflater, long startMillis) 
		{
	        Day day = new Day();
	
	        // Setup data
	        day.Index = mDays.Count;
	        day.TimeStart = startMillis;
	        day.TimeEnd = startMillis + DateUtils.DayInMillis;
	        day.BlocksUri = ScheduleContract.Blocks.BuildBlocksBetweenDirUri(day.TimeStart, day.TimeEnd);

	        // Setup views
	        day.RootView = (ViewGroup) inflater.Inflate(Resource.Layout.blocks_content, null);
	
	        day.ScrollView = day.RootView.FindViewById<ObservableScrollView>(Resource.Id.blocks_scroll);
	        day.ScrollView.SetOnScrollListener(this);
	
	        day.BlocksView = day.RootView.FindViewById<BlocksLayout>(Resource.Id.blocks);
	        day.NowView = day.RootView.FindViewById(Resource.Id.blocks_now);
	
	        day.BlocksView.DrawingCacheEnabled = true;
	        day.BlocksView.AlwaysDrawnWithCacheEnabled = true;
	
	        Java.Util.TimeZone.Default = UIUtils.ConferenceTimeZone;
	        day.Label = DateUtils.FormatDateTime(Activity, startMillis, TIME_FLAGS);
	
	        mWorkspace.AddView(day.RootView);
	        mDays.Add(day);
	    }
		
		public override void OnResume ()
		{
			base.OnResume ();
			
			// Since we build our views manually instead of using an adapter, we
	        // need to manually requery every time launched.
	        Requery();
	
	        Activity.ContentResolver.RegisterContentObserver(ScheduleContract.Sessions.CONTENT_URI, true, mSessionChangesObserver);
	
	        // Start listening for time updates to adjust "now" bar. TIME_TICK is
	        // triggered once per minute, which is how we move the bar over time.
	        IntentFilter filter = new IntentFilter();
	        filter.AddAction(Intent.ActionTimeTick);
	        filter.AddAction(Intent.ActionTimeChanged);
	        filter.AddAction(Intent.ActionTimezoneChanged);
	        Activity.RegisterReceiver(mReceiver, filter, null, new Handler());
		}
		
		private void Requery() {
	        foreach (var day in mDays) {
	            mHandler.StartQuery(0, day, day.BlocksUri, BlocksQuery.PROJECTION, null, null, ScheduleContract.Blocks.DEFAULT_SORT);
	        }
	    }
		
		public override void OnActivityCreated (Bundle p0)
		{
			base.OnActivityCreated (p0);
			
			Activity.RunOnUiThread(() => {
		    	UpdateNowView(true);	
			});
		}
		
		public override void OnPause ()
		{
			base.OnPause ();
			
			Activity.UnregisterReceiver(mReceiver);
        	Activity.ContentResolver.UnregisterContentObserver(mSessionChangesObserver);
		}
		
		#region AsyncQueryListener implementation
		public void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			if (Activity == null) {
	            return;
	        }
			
			Day day = (Day) cookie;
			
			// Clear out any existing sessions before inserting again
	        day.BlocksView.RemoveAllBlocks();
			
			try {
	            while (cursor.MoveToNext()) {
	                string type = cursor.GetString(BlocksQuery.BLOCK_TYPE);
	                
					// TODO: place random blocks at bottom of entire layout
					int column;
					try {
						column = TypeColumnMap[type];
					} catch {
						continue;
					}
	                
	                string blockId = cursor.GetString(BlocksQuery.BLOCK_ID);
	                string title = cursor.GetString(BlocksQuery.BLOCK_TITLE);
	                long start = cursor.GetLong(BlocksQuery.BLOCK_START);
	                long end = cursor.GetLong(BlocksQuery.BLOCK_END);
	                bool containsStarred = cursor.GetInt(BlocksQuery.CONTAINS_STARRED) != 0;
	
	                BlockView blockView = new BlockView(Activity, blockId, title, start, end, containsStarred, column);
	
	                int sessionsCount = cursor.GetInt(BlocksQuery.SESSIONS_COUNT);
					
	                if (sessionsCount > 0) {
	                    blockView.Click += HandleClick;
	                } else {
	                    blockView.Focusable = false;
	                    blockView.Enabled = false;
	                    LayerDrawable buttonDrawable = (LayerDrawable) blockView.Background;
	                    buttonDrawable.GetDrawable(0).SetAlpha(DISABLED_BLOCK_ALPHA);
	                    buttonDrawable.GetDrawable(2).SetAlpha(DISABLED_BLOCK_ALPHA);
	                }
	
	                day.BlocksView.AddView(blockView);
	            }
	        } finally {
	            cursor.Close();
	        }

		}

		void HandleClick (object view, EventArgs e)
		{
			Console.WriteLine ("On click!");
			if (view is BlockView) {
	            String title = ((BlockView)view).Text;
	            //AnalyticsUtils.getInstance(getActivity()).trackEvent(
	            //        "Schedule", "Session Click", title, 0);
	            String blockId = ((BlockView) view).GetBlockId();
	            Uri sessionsUri = ScheduleContract.Blocks.BuildSessionsUri(blockId);
	
	            Intent intent = new Intent(Intent.ActionView, sessionsUri);
	            intent.PutExtra(SessionsFragment.EXTRA_SCHEDULE_TIME_STRING, ((BlockView) view).GetBlockTimeString());
	            ((BaseActivity) Activity).OpenActivityOrFragment(intent);
	        }
		}
		#endregion
		
		#region IOnClickListener implementation
		public void OnClick (View view)
		{
			
		}
		#endregion
		
		/**
	     * Update position and visibility of "now" view.
	     */
	    private bool UpdateNowView(bool forceScroll) {
	        long now = UIUtils.GetCurrentTime(Activity);
	
	        Day nowDay = null; // effectively Day corresponding to today
	        foreach (var day in mDays) 
			{
	            if (now >= day.TimeStart && now <= day.TimeEnd) {
	                nowDay = day;
	                day.NowView.Visibility = ViewStates.Visible;
	            } else {
	                day.NowView.Visibility = ViewStates.Gone;
	            }
	        }
	
	        if (nowDay != null && forceScroll) {
	            // Scroll to show "now" in center
	            mWorkspace.SetCurrentScreen(nowDay.Index);
	            int offset = nowDay.ScrollView.Height / 2;
	            nowDay.NowView.RequestRectangleOnScreen(new Android.Graphics.Rect(0, offset, 0, offset), true);
	            nowDay.BlocksView.RequestLayout();
	            return true;
	        }
	
	        return false;
	    }
		
		#region OnScrollListener implementation
		public void OnScrollChanged (ObservableScrollView view)
		{
			// Keep each day view at the same vertical scroll offset.
	        int scrollY = view.ScrollY;
	        foreach (var day in mDays) {
	            if (day.ScrollView != view) {
	                day.ScrollView.ScrollTo(0, scrollY);
	            }
	        }
		}
		#endregion
		
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate(Resource.Menu.schedule_menu_items, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_now) {
	            if (!UpdateNowView(true)) {
	                Toast.MakeText(Activity, Resource.String.toast_now_not_visible, ToastLength.Short).Show();
	            }
	            return true;
	        }
			
			return base.OnOptionsItemSelected (item);
		}
		
		public class SessionChangesObserver : ContentObserver
		{
			ScheduleFragment _fragment;
 
			public SessionChangesObserver (ScheduleFragment fragment) : base(new Handler())
			{
				_fragment = fragment;
			}
			
			public override void OnChange (bool selfChange)
			{
				_fragment.Requery();
			}	
		}
		
		public class MyBroadcastReceiver : BroadcastReceiver
		{
			ScheduleFragment _fragment;
			public MyBroadcastReceiver(ScheduleFragment fragment)
			{
				_fragment = fragment;
			}
			
			public override void OnReceive (Context context, Intent intent)
			{
				Log.Debug(_fragment.TAG, "onReceive time update");
				_fragment.UpdateNowView(false);
			}
		}
	
	    private class BlocksQuery {
	        public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Blocks.BLOCK_ID,
	                ScheduleContract.Blocks.BLOCK_TITLE,
	                ScheduleContract.Blocks.BLOCK_START,
	                ScheduleContract.Blocks.BLOCK_END,
	                ScheduleContract.Blocks.BLOCK_TYPE,
	                ScheduleContract.Blocks.SESSIONS_COUNT,
	                ScheduleContract.Blocks.CONTAINS_STARRED,
	        };
	
	        public static int _ID = 0;
	        public static int BLOCK_ID = 1;
	        public static int BLOCK_TITLE = 2;
	        public static int BLOCK_START = 3;
	        public static int BLOCK_END = 4;
	        public static int BLOCK_TYPE = 5;
	        public static int SESSIONS_COUNT = 6;
	        public static int CONTAINS_STARRED = 7;
	    }
	}
}