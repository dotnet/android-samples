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
using Uri = Android.Net.Uri;
using Android.Database;
using Android.Provider;
using MonoIO.Utilities;
using MonoIO.UI;
using Android.Text;
using ListFragment = Android.Support.V4.App.ListFragment;
using Android.Util;
using Android.Graphics;

namespace MonoIO
{
	public class SessionsFragment : ListFragment, NotifyingAsyncQueryHandler.AsyncQueryListener
	{
		public static String EXTRA_SCHEDULE_TIME_STRING = "monoio.extra.SCHEDULE_TIME_STRING";
		private static String STATE_CHECKED_POSITION = "checkedPosition";
		private Uri mTrackUri;
		private ICursor mCursor;
		private CursorAdapter mAdapter;
		private int mCheckedPosition = -1;
		private bool mHasSetEmptyText = false;
		private NotifyingAsyncQueryHandler mHandler;
		private Handler mMessageQueueHandler = new Handler ();
		public SessionChangesObserver mSessionChangesObserver;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mHandler = new NotifyingAsyncQueryHandler (Activity.ContentResolver, this);
			mSessionChangesObserver = new SessionChangesObserver (this);
			ReloadFromArguments (Arguments);
			
		}
		
		public void ReloadFromArguments (Bundle arguments)
		{
			// Teardown from previous arguments
			if (mCursor != null) {
				Activity.StopManagingCursor (mCursor);
				mCursor = null;
			}
	
			mCheckedPosition = -1;
			ListAdapter = null;
	
			mHandler.CancelOperation (SearchQuery._TOKEN);
			mHandler.CancelOperation (SessionsQuery._TOKEN);
			mHandler.CancelOperation (TracksQuery._TOKEN);
	
			// Load new arguments
			Intent intent = BaseActivity.FragmentArgumentsToIntent (arguments);
			Uri sessionsUri = intent.Data;
			int sessionQueryToken;
	
			if (sessionsUri == null) {
				return;
			}
	
			String[] projection;
			if (!ScheduleContract.Sessions.IsSearchUri (sessionsUri)) {
				mAdapter = new SessionsAdapter (Activity, this);
				projection = SessionsQuery.PROJECTION;
				sessionQueryToken = SessionsQuery._TOKEN;
	
			} else {
				mAdapter = new SearchAdapter (Activity, this);
				projection = SearchQuery.PROJECTION;
				sessionQueryToken = SearchQuery._TOKEN;
			}
	
			ListAdapter = mAdapter;
	
			// Start background query to load sessions
			mHandler.StartQuery (sessionQueryToken, null, sessionsUri, projection, null, null, ScheduleContract.Sessions.DEFAULT_SORT);
	
			// If caller launched us with specific track hint, pass it along when
			// launching session details. Also start a query to load the track info.
			mTrackUri = (Android.Net.Uri)intent.GetParcelableExtra (SessionDetailFragment.EXTRA_TRACK);
			mTrackUri = null;
			if (mTrackUri != null) {
				mHandler.StartQuery (TracksQuery._TOKEN, mTrackUri, TracksQuery.PROJECTION);
			}
		}
		
		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			
			ListView.ChoiceMode = ChoiceMode.Single;

			if (savedInstanceState != null) {
				mCheckedPosition = savedInstanceState.GetInt (STATE_CHECKED_POSITION, -1);
			}
	
			if (!mHasSetEmptyText) {
				// Could be a bug, but calling this twice makes it become visible when it shouldn't
				// be visible.
				SetEmptyText (GetString (Resource.String.empty_sessions));
				mHasSetEmptyText = true;
			}
		}
		
		#region AsyncQueryListener implementation
		public void OnQueryComplete (int token, Java.Lang.Object cookie, Android.Database.ICursor cursor)
		{
			if (Activity == null) {
				return;
			}
	
			if (token == SessionsQuery._TOKEN || token == SearchQuery._TOKEN) {
				OnSessionOrSearchQueryComplete (cursor);
			} else if (token == TracksQuery._TOKEN) {
				OnTrackQueryComplete (cursor);
			} else {
				Log.Debug ("SessionsFragment/onQueryComplete", "Query complete, Not Actionable: " + token);
				cursor.Close ();
			}
		}
		#endregion
		
		/**
	     * Handle {@link SessionsQuery} {@link Cursor}.
	     */
		private void OnSessionOrSearchQueryComplete (ICursor cursor)
		{
			if (mCursor != null) {
				// In case cancelOperation() doesn't work and we end up with consecutive calls to this
				// callback.
				Activity.StopManagingCursor (mCursor);
				mCursor = null;
			}
	
			mCursor = cursor;
			Activity.StartManagingCursor (mCursor);
			mAdapter.ChangeCursor (mCursor);
			if (mCheckedPosition >= 0 && View != null) {
				ListView.SetItemChecked (mCheckedPosition, true);
			}
		}
	
		/**
	     * Handle {@link TracksQuery} {@link Cursor}.
	     */
		private void OnTrackQueryComplete (ICursor cursor)
		{
			try {
				if (!cursor.MoveToFirst ()) {
					return;
				}
	
				// Use found track to build title-bar
				ActivityHelper activityHelper = ((BaseActivity)Activity).ActivityHelper;
				String trackName = cursor.GetString (TracksQuery.TRACK_NAME);
				activityHelper.SetActionBarTitle (new Java.Lang.String (trackName));
				activityHelper.SetActionBarColor (new Color (cursor.GetInt (TracksQuery.TRACK_COLOR)));
				//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Tracks/" + trackName);
			} finally {
				cursor.Close ();
			}
		}
		
		public override void OnResume ()
		{
			base.OnResume ();
			mMessageQueueHandler.Post (mRefreshSessionsRunnable);
			Activity.ContentResolver.RegisterContentObserver (ScheduleContract.Sessions.CONTENT_URI, true, mSessionChangesObserver);
			if (mCursor != null) {
				mCursor.Requery ();
			}
		}
		
		void mRefreshSessionsRunnable ()
		{
			if (mAdapter != null) {
				// This is used to refresh session title colors.
				mAdapter.NotifyDataSetChanged ();
			}

			// Check again on the next quarter hour, with some padding to account for network
			// time differences.
			long nextQuarterHour = (SystemClock.UptimeMillis () / 900000 + 1) * 900000 + 5000;
			mMessageQueueHandler.PostAtTime (mRefreshSessionsRunnable, nextQuarterHour);
					
		}
			
		public override void OnPause ()
		{
			base.OnPause ();
			mMessageQueueHandler.RemoveCallbacks (mRefreshSessionsRunnable);
			Activity.ContentResolver.UnregisterContentObserver (mSessionChangesObserver);
		}
		
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt (STATE_CHECKED_POSITION, mCheckedPosition);
		}
		
		public override void OnListItemClick (Android.Widget.ListView l, Android.Views.View v, int position, long id)
		{
			Console.WriteLine ("On list item click!");
			// Launch viewer for specific session, passing along any track knowledge
			// that should influence the title-bar.
			ICursor cursor = (ICursor)mAdapter.GetItem (position);
			String sessionId = cursor.GetString (cursor.GetColumnIndex (ScheduleContract.Sessions.SESSION_ID));
			Uri sessionUri = ScheduleContract.Sessions.BuildSessionUri (sessionId);
			Intent intent = new Intent (Intent.ActionView, sessionUri);
			intent.PutExtra (SessionDetailFragment.EXTRA_TRACK, mTrackUri);
			((BaseActivity)Activity).OpenActivityOrFragment (intent);
	
			ListView.SetItemChecked (position, true);
			mCheckedPosition = position;
		}
		
		public void ClearCheckedPosition ()
		{
			if (mCheckedPosition >= 0) {
				ListView.SetItemChecked (mCheckedPosition, false);
				mCheckedPosition = -1;
			}
		}
		
		/**
	     * {@link CursorAdapter} that renders a {@link SessionsQuery}.
	     */
		private class SessionsAdapter : CursorAdapter
		{
			SessionsFragment _activity;

			public SessionsAdapter (Context context, SessionsFragment activity) : base(context, null)
			{
				_activity = activity;
			}
			
			public override Android.Views.View NewView (Context context, ICursor cursor, ViewGroup parent)
			{
				return _activity.Activity.LayoutInflater.Inflate (Resource.Layout.list_item_session, parent, false);
			}
			
			public override void BindView (Android.Views.View view, Context context, ICursor cursor)
			{
				TextView titleView = view.FindViewById<TextView> (Resource.Id.session_title);
				TextView subtitleView = view.FindViewById<TextView> (Resource.Id.session_subtitle);
	
				titleView.Text = cursor.GetString (SessionsQuery.TITLE);
	
				// Format time block this session occupies
				long blockStart = cursor.GetLong (SessionsQuery.BLOCK_START);
				long blockEnd = cursor.GetLong (SessionsQuery.BLOCK_END);
				string roomName = cursor.GetString (SessionsQuery.ROOM_NAME);
				string subtitle = UIUtils.FormatSessionSubtitle (blockStart, blockEnd, roomName, context);
	
				subtitleView.Text = subtitle;
				
				bool starred = cursor.GetInt (SessionsQuery.STARRED) != 0;
				view.FindViewById (Resource.Id.star_button).Visibility = starred ? ViewStates.Visible : ViewStates.Invisible;
	
				// Possibly indicate that the session has occurred in the past.
				UIUtils.SetSessionTitleColor (blockStart, blockEnd, titleView, subtitleView);
			}
		}
		
		/**
	     * {@link CursorAdapter} that renders a {@link SearchQuery}.
	     */
		private class SearchAdapter : CursorAdapter
		{
	        
			SessionsFragment _activity;

			public SearchAdapter (Context context, SessionsFragment activity) : base(context, null)
			{
				_activity = activity;
			}
			
			public override Android.Views.View NewView (Context context, ICursor cursor, ViewGroup parent)
			{
				return _activity.Activity.LayoutInflater.Inflate (Resource.Layout.list_item_session, parent, false);
			}
			
			public override void BindView (Android.Views.View view, Context context, ICursor cursor)
			{
				view.FindViewById<TextView> (Resource.Id.session_title).Text = cursor.GetString (SearchQuery.TITLE);
	
				String snippet = cursor.GetString (SearchQuery.SEARCH_SNIPPET);
	
				ISpannable styledSnippet = UIUtils.BuildStyledSnippet (new Java.Lang.String (snippet));
				view.FindViewById<TextView> (Resource.Id.session_subtitle).TextFormatted = styledSnippet;
	
				bool starred = cursor.GetInt (SearchQuery.STARRED) != 0;
				view.FindViewById (Resource.Id.star_button).Visibility = starred ? ViewStates.Visible : ViewStates.Invisible;
			}
		}
		
		public class SessionChangesObserver : ContentObserver
		{
			SessionsFragment _fragment;
 
			public SessionChangesObserver (SessionsFragment fragment) : base(new Handler())
			{
				_fragment = fragment;
			}
			
			public override void OnChange (bool selfChange)
			{
				_fragment.mCursor.Requery ();
			}	
		}
		
		/**
	     * {@link com.google.android.apps.iosched.provider.ScheduleContract.Sessions} query parameters.
	     */
		private class SessionsQuery
		{
			public static int _TOKEN = 0x1;
			public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Sessions.SESSION_ID,
	                ScheduleContract.Sessions.SESSION_TITLE,
	                ScheduleContract.Sessions.SESSION_STARRED,
	                ScheduleContract.Blocks.BLOCK_START,
	                ScheduleContract.Blocks.BLOCK_END,
	                ScheduleContract.Rooms.ROOM_NAME,
	        };
			public static int _ID = 0;
			public static int SESSION_ID = 1;
			public static int TITLE = 2;
			public static int STARRED = 3;
			public static int BLOCK_START = 4;
			public static int BLOCK_END = 5;
			public static int ROOM_NAME = 6;
		}
	
		/**
	     * {@link com.google.android.apps.iosched.provider.ScheduleContract.Tracks} query parameters.
	     */
		private class TracksQuery
		{
			public static int _TOKEN = 0x2;
			public static String[] PROJECTION = {
	                ScheduleContract.Tracks.TRACK_NAME,
	                ScheduleContract.Tracks.TRACK_COLOR,
	        };
			public static int TRACK_NAME = 0;
			public static int TRACK_COLOR = 1;
		}
	
		/** {@link com.google.android.apps.iosched.provider.ScheduleContract.Sessions} search query
	     * parameters. */
		private class SearchQuery
		{
			public static int _TOKEN = 0x3;
			public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Sessions.SESSION_ID,
	                ScheduleContract.Sessions.SESSION_TITLE,
	                ScheduleContract.Sessions.SEARCH_SNIPPET,
	                ScheduleContract.Sessions.SESSION_STARRED,
	        };
			public static int _ID = 0;
			public static int SESSION_ID = 1;
			public static int TITLE = 2;
			public static int SEARCH_SNIPPET = 3;
			public static int STARRED = 4;
		}
	}
}

