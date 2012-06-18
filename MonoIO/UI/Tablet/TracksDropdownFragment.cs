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
using MonoIO.UI;
using Android.Database;
using MonoIO.Utilities;
using Fragment = Android.Support.V4.App.Fragment;
using Uri = Android.Net.Uri;
using Android.Graphics;

namespace MonoIO
{
	public class TracksDropdownFragment : Fragment, NotifyingAsyncQueryHandler.AsyncQueryListener, AdapterView.IOnItemClickListener, PopupWindow.IOnDismissListener
	{
		public static String EXTRA_NEXT_TYPE = "monoio.extra.NEXT_TYPE";
		public static String NEXT_TYPE_SESSIONS = "sessions";
		public static String NEXT_TYPE_VENDORS = "vendors";
		private bool mAutoloadTarget = true;
		private ICursor mCursor;
		private TracksAdapter mAdapter;
		private String mNextType;
		private ListPopupWindow mListPopupWindow;
		private ViewGroup mRootView;
		private TextView mTitle;
		private TextView mAbstract;
		private NotifyingAsyncQueryHandler mHandler;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			mHandler = new NotifyingAsyncQueryHandler (Activity.ContentResolver, this);
			mAdapter = new TracksAdapter (Activity);

			if (savedInstanceState != null) {
				// Prevent auto-load behavior on orientation change.
				mAutoloadTarget = false;
			}

			ReloadFromArguments (Arguments);
		}
		
		public void ReloadFromArguments (Bundle arguments)
		{
			// Teardown from previous arguments
			if (mListPopupWindow != null) {
				mListPopupWindow.SetAdapter (null);
			}
			if (mCursor != null) {
				Activity.StopManagingCursor (mCursor);
				mCursor = null;
			}
			mHandler.CancelOperation (TracksAdapter.TracksQuery._TOKEN);

			// Load new arguments
			Intent intent = BaseActivity.FragmentArgumentsToIntent (arguments);
			Uri tracksUri = intent.Data;
			if (tracksUri == null) {
				return;
			}

			mNextType = intent.GetStringExtra (EXTRA_NEXT_TYPE);

			// Filter our tracks query to only include those with valid results
			String[] projection = TracksAdapter.TracksQuery.PROJECTION;
			String selection = null;
			if (TracksFragment.NEXT_TYPE_SESSIONS.Equals (mNextType)) {
				// Only show tracks with at least one session
				projection = TracksAdapter.TracksQuery.PROJECTION_WITH_SESSIONS_COUNT;
				selection = ScheduleContract.Tracks.SESSIONS_COUNT + ">0";

			} else if (TracksFragment.NEXT_TYPE_VENDORS.Equals (mNextType)) {
				// Only show tracks with at least one vendor
				projection = TracksAdapter.TracksQuery.PROJECTION_WITH_VENDORS_COUNT;
				selection = ScheduleContract.Tracks.VENDORS_COUNT + ">0";
			}

			// Start background query to load tracks
			mHandler.StartQuery (TracksAdapter.TracksQuery._TOKEN, null, tracksUri, projection, selection, null, ScheduleContract.Tracks.DEFAULT_SORT);
		}
		
		public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			mRootView = (ViewGroup)inflater.Inflate (Resource.Layout.fragment_tracks_dropdown, null);
			mTitle = (TextView)mRootView.FindViewById (Resource.Id.track_title);
			mAbstract = (TextView)mRootView.FindViewById (Resource.Id.track_abstract);
	
			mRootView.Click += (sender, e) => {
				mListPopupWindow = new ListPopupWindow (Activity);
				mListPopupWindow.SetAdapter (mAdapter);
				mListPopupWindow.Modal = true;
				mListPopupWindow.SetContentWidth (400);
				mListPopupWindow.AnchorView = mRootView;
				mListPopupWindow.SetOnItemClickListener (this);
				mListPopupWindow.Show ();
				mListPopupWindow.SetOnDismissListener (this);
			};

			return mRootView;
		}
		
		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
		}
		
		public void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			if (Activity == null || cursor == null) {
				return;
			}

			mCursor = cursor;
			Activity.StartManagingCursor (mCursor);

			// If there was a last-opened track, load it. Otherwise load the first track.
			cursor.MoveToFirst ();
			String lastTrackID = UIUtils.GetLastUsedTrackID (Activity);
			if (lastTrackID != null) {
				while (!cursor.IsAfterLast) {
					if (lastTrackID.Equals (cursor.GetString (TracksAdapter.TracksQuery.TRACK_ID))) {
						break;
					}
					cursor.MoveToNext ();
				}

				if (cursor.IsAfterLast) {
					LoadTrack (null, mAutoloadTarget);
				} else {
					LoadTrack (cursor, mAutoloadTarget);
				}
			} else {
				LoadTrack (null, mAutoloadTarget);
			}

			mAdapter.SetHasAllItem (true);
			mAdapter.SetIsSessions (TracksFragment.NEXT_TYPE_SESSIONS.Equals (mNextType));
			mAdapter.ChangeCursor (mCursor);
		}
		
		#region IOnItemClickListener implementation
		public void OnItemClick (AdapterView parent, Android.Views.View view, int position, long id)
		{
			ICursor cursor = (ICursor)mAdapter.GetItem (position);
			LoadTrack (cursor, true);
	
			if (cursor != null) {
				UIUtils.SetLastUsedTrackID (Activity, cursor.GetString (TracksAdapter.TracksQuery.TRACK_ID));
			} else {
				UIUtils.SetLastUsedTrackID (Activity, ScheduleContract.Tracks.ALL_TRACK_ID);
			}
	
			if (mListPopupWindow != null) {
				mListPopupWindow.Dismiss ();
			}
		}
		#endregion
		
		public void LoadTrack (ICursor cursor, bool loadTargetFragment)
		{
			String trackId;
			Color trackColor;
			var res = Resources;
	
			if (cursor != null) {
				trackColor = new Color (cursor.GetInt (TracksAdapter.TracksQuery.TRACK_COLOR));
				trackId = cursor.GetString (TracksAdapter.TracksQuery.TRACK_ID);
	
				mTitle.Text = (cursor.GetString (TracksAdapter.TracksQuery.TRACK_NAME));
				mAbstract.Text = (cursor.GetString (TracksAdapter.TracksQuery.TRACK_ABSTRACT));
	
			} else {
				trackColor = res.GetColor (Resource.Color.all_track_color);
				trackId = ScheduleContract.Tracks.ALL_TRACK_ID;
	
				mTitle.SetText (TracksFragment.NEXT_TYPE_SESSIONS.Equals (mNextType)
	                    ? Resource.String.all_sessions_title
	                    : Resource.String.all_sandbox_title);
				mAbstract.SetText (TracksFragment.NEXT_TYPE_SESSIONS.Equals (mNextType)
	                    ? Resource.String.all_sessions_subtitle
	                    : Resource.String.all_sandbox_subtitle);
			}
	
			bool isDark = UIUtils.IsColorDark (trackColor);
			mRootView.SetBackgroundColor (trackColor);
	
			if (isDark) {
				mTitle.SetTextColor (res.GetColor (Resource.Color.body_text_1_inverse));
				mAbstract.SetTextColor (res.GetColor (Resource.Color.body_text_2_inverse));
				mRootView.FindViewById (Resource.Id.track_dropdown_arrow).SetBackgroundResource (Resource.Drawable.track_dropdown_arrow_light);
			} else {
				mTitle.SetTextColor (res.GetColor (Resource.Color.body_text_1));
				mAbstract.SetTextColor (res.GetColor (Resource.Color.body_text_2));
				mRootView.FindViewById (Resource.Id.track_dropdown_arrow).SetBackgroundResource (Resource.Drawable.track_dropdown_arrow_dark);
			}
	
			if (loadTargetFragment) {
				Intent intent = new Intent (Intent.ActionView);
				Uri trackUri = ScheduleContract.Tracks.BuildTrackUri (trackId);
				intent.PutExtra (SessionDetailFragment.EXTRA_TRACK, trackUri);
	
				if (NEXT_TYPE_SESSIONS.Equals (mNextType)) {
					if (cursor == null) {
						intent.SetData (ScheduleContract.Sessions.CONTENT_URI);
					} else {
						intent.SetData (ScheduleContract.Tracks.BuildSessionsUri (trackId));
					}
				} else if (NEXT_TYPE_VENDORS.Equals (mNextType)) {
					if (cursor == null) {
						intent.SetData (ScheduleContract.Vendors.CONTENT_URI);
					} else {
						intent.SetData (ScheduleContract.Tracks.BuildVendorsUri (trackId));
					}
				}
	
				((BaseActivity)Activity).OpenActivityOrFragment (intent);
			}
		}
	
		public void OnDismiss ()
		{
			mListPopupWindow = null;
		}
	}
}

