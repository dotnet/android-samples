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
using Android.Provider;

using Uri = Android.Net.Uri;
using Android.Util;
using MonoIO.UI;
using Android.Database;
using Android.Text;
using MonoIO.Utilities;
using Fragment = Android.Support.V4.App.Fragment;
using ListFragment = Android.Support.V4.App.ListFragment;
using Android.Graphics;

namespace MonoIO
{
	public class VendorsFragment : ListFragment, NotifyingAsyncQueryHandler.AsyncQueryListener
	{
		private static String STATE_CHECKED_POSITION = "checkedPosition";
		private Uri mTrackUri;
		private ICursor mCursor;
		private CursorAdapter mAdapter;
		private int mCheckedPosition = -1;
		private bool mHasSetEmptyText = false;
		private NotifyingAsyncQueryHandler mHandler;
		public VendorChangesObserver mVendorChangesObserver;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			mHandler = new NotifyingAsyncQueryHandler (Activity.ContentResolver, this);
			mVendorChangesObserver = new VendorChangesObserver (this);
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
			mHandler.CancelOperation (VendorsQuery._TOKEN);
	
			// Load new arguments
			Intent intent = BaseActivity.FragmentArgumentsToIntent (arguments);
			Uri vendorsUri = intent.Data;
			int vendorQueryToken;
	
			if (vendorsUri == null) {
				return;
			}
	
			String[] projection;
			if (!ScheduleContract.Vendors.IsSearchUri (vendorsUri)) {
				mAdapter = new VendorsAdapter (Activity);
				projection = VendorsQuery.PROJECTION;
				vendorQueryToken = VendorsQuery._TOKEN;
	
			} else {
				Log.Debug ("VendorsFragment/reloadFromArguments", "A search URL definitely gets passed in.");
				mAdapter = new SearchAdapter (Activity);
				projection = SearchQuery.PROJECTION;
				vendorQueryToken = SearchQuery._TOKEN;
			}
	
			ListAdapter = mAdapter;
	
			// Start background query to load vendors
			mHandler.StartQuery (vendorQueryToken, null, vendorsUri, projection, null, null, ScheduleContract.Vendors.DEFAULT_SORT);
	
			// If caller launched us with specific track hint, pass it along when
			// launching vendor details. Also start a query to load the track info.
			mTrackUri = (Android.Net.Uri)intent.GetParcelableExtra (SessionDetailFragment.EXTRA_TRACK);
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
				SetEmptyText (GetString (Resource.String.empty_vendors));
				mHasSetEmptyText = true;
			}
		}
		
		public void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			if (Activity == null) {
				return;
			}
	
			if (token == VendorsQuery._TOKEN || token == SearchQuery._TOKEN) {
				OnVendorsOrSearchQueryComplete (cursor);
			} else if (token == TracksQuery._TOKEN) {
				OnTrackQueryComplete (cursor);
			} else {
				cursor.Close ();
			}
		}
		
		/**
	     * Handle {@link VendorsQuery} {@link Cursor}.
	     */
		private void OnVendorsOrSearchQueryComplete (ICursor cursor)
		{
			if (mCursor != null) {
				// In case cancelOperation() doesn't work and we end up with consecutive calls to this
				// callback.
				Activity.StopManagingCursor (mCursor);
				mCursor = null;
			}
	
			// TODO(romannurik): stopManagingCursor on detach (throughout app)
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
	
				//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Sandbox/Track/" + trackName);
	
			} finally {
				cursor.Close ();
			}
		}
		
		public override void OnResume ()
		{
			base.OnResume ();
			
			Activity.ContentResolver.RegisterContentObserver (ScheduleContract.Vendors.CONTENT_URI, true, mVendorChangesObserver);
			if (mCursor != null) {
				mCursor.Requery ();
			}
		}
		
		public override void OnPause ()
		{
			base.OnPause ();
			
			Activity.ContentResolver.UnregisterContentObserver (mVendorChangesObserver);
		}
		
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			
			outState.PutInt (STATE_CHECKED_POSITION, mCheckedPosition);
		}
		
		public override void OnListItemClick (Android.Widget.ListView l, Android.Views.View v, int position, long id)
		{
			// Launch viewer for specific vendor.
			ICursor cursor = (ICursor)mAdapter.GetItem (position);
			String vendorId = cursor.GetString (VendorsQuery.VENDOR_ID);
			Uri vendorUri = ScheduleContract.Vendors.BuildVendorUri (vendorId);
			((BaseActivity)Activity).OpenActivityOrFragment (new Intent (Intent.ActionView, vendorUri));
	
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
	     * {@link CursorAdapter} that renders a {@link VendorsQuery}.
	     */
		private class VendorsAdapter : CursorAdapter
		{
			Activity _activity;
			
			public VendorsAdapter (Activity context) : base(context, null)
			{
				_activity = context;
			}
	
			public override Android.Views.View NewView (Context context, ICursor cursor, ViewGroup parent)
			{
				return _activity.LayoutInflater.Inflate (Resource.Layout.list_item_vendor_oneline, parent, false);
			}
			
			public override void BindView (Android.Views.View view, Context context, ICursor cursor)
			{
				view.FindViewById<TextView> (Resource.Id.vendor_name).Text = cursor.GetString (VendorsQuery.NAME);
				bool starred = cursor.GetInt (VendorsQuery.STARRED) != 0;
				view.FindViewById (Resource.Id.star_button).Visibility = starred ? ViewStates.Visible : ViewStates.Invisible;
			}
		}
		
		/**
	     * {@link CursorAdapter} that renders a {@link SearchQuery}.
	     */
		private class SearchAdapter : CursorAdapter
		{
			
			public Activity _activity;
			
			public SearchAdapter (Activity context) : base(context, null)
			{
				_activity = context;
			}
			
			public override Android.Views.View NewView (Context context, ICursor cursor, ViewGroup parent)
			{
				return _activity.LayoutInflater.Inflate (Resource.Layout.list_item_vendor, parent, false);
			}
			
			public override void BindView (Android.Views.View view, Context context, ICursor cursor)
			{
				view.FindViewById<TextView> (Resource.Id.vendor_name).Text = cursor.GetString (SearchQuery.NAME);
				
				string snippet = cursor.GetString (SearchQuery.SEARCH_SNIPPET);
				ISpannable styledSnippet = UIUtils.BuildStyledSnippet (new Java.Lang.String (snippet));
				view.FindViewById<TextView> (Resource.Id.vendor_location).TextFormatted = styledSnippet;
				
				bool starred = cursor.GetInt (VendorsQuery.STARRED) != 0;
				view.FindViewById (Resource.Id.star_button).Visibility = starred ? ViewStates.Visible : ViewStates.Invisible;
			}
		}
		
		public class VendorChangesObserver : ContentObserver
		{
			VendorsFragment _fragment;
 
			public VendorChangesObserver (VendorsFragment fragment) : base(new Handler())
			{
				_fragment = fragment;
			}
			
			public override void OnChange (bool selfChange)
			{
				if (_fragment.mCursor != null)
					_fragment.mCursor.Requery ();
			}	
		}
		
		/**
	     * {@link com.google.android.apps.iosched.provider.ScheduleContract.Vendors} query parameters.
	     */
		private class VendorsQuery
		{
			public static int _TOKEN = 0x1;
			public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Vendors.VENDOR_ID,
	                ScheduleContract.Vendors.VENDOR_NAME,
	                ScheduleContract.Vendors.VENDOR_LOCATION,
	                ScheduleContract.Vendors.VENDOR_STARRED,
	        };
			public static int _ID = 0;
			public static int VENDOR_ID = 1;
			public static int NAME = 2;
			public static int LOCATION = 3;
			public static int STARRED = 4;
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
	
		/** {@link com.google.android.apps.iosched.provider.ScheduleContract.Vendors} search query
	     * parameters. */
		private class SearchQuery
		{
			public static int _TOKEN = 0x3;
			public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Vendors.VENDOR_ID,
	                ScheduleContract.Vendors.VENDOR_NAME,
	                ScheduleContract.Vendors.SEARCH_SNIPPET,
	                ScheduleContract.Vendors.VENDOR_STARRED,
	        };
			public static int _ID = 0;
			public static int VENDOR_ID = 1;
			public static int NAME = 2;
			public static int SEARCH_SNIPPET = 3;
			public static int STARRED = 4;
		}
	}
}

