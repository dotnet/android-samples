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

using ListFragment = Android.Support.V4.App.ListFragment;
using Fragment = Android.Support.V4.App.Fragment;

using Uri = Android.Net.Uri;
using MonoIO.UI;
using Android.Database;

namespace MonoIO
{
	public class TracksFragment : ListFragment, NotifyingAsyncQueryHandler.AsyncQueryListener
	{
		public static String EXTRA_NEXT_TYPE = "monoio.extra.NEXT_TYPE";

	    public static String NEXT_TYPE_SESSIONS = "sessions";
	    public static String NEXT_TYPE_VENDORS = "vendors";
	
	    private TracksAdapter mAdapter;
	    private NotifyingAsyncQueryHandler mHandler;
	    private String mNextType;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			
	        Intent intent = BaseActivity.FragmentArgumentsToIntent(Arguments);
	        Uri tracksUri = intent.Data;
	        mNextType = intent.GetStringExtra(EXTRA_NEXT_TYPE);
	
	        mAdapter = new TracksAdapter(Activity);
			ListAdapter = mAdapter;
	
	        // Filter our tracks query to only include those with valid results
	        String[] projection = TracksAdapter.TracksQuery.PROJECTION;
	        String selection = null;
	        if (NEXT_TYPE_SESSIONS.Equals(mNextType)) {
	            // Only show tracks with at least one session
	            projection = TracksAdapter.TracksQuery.PROJECTION_WITH_SESSIONS_COUNT;
	            selection = ScheduleContract.Tracks.SESSIONS_COUNT + ">0";
	            //AnalyticsUtils.getInstance(getActivity()).trackPageView("/Tracks");
	
	        } else if (NEXT_TYPE_VENDORS.Equals(mNextType)) {
	            // Only show tracks with at least one vendor
	            projection = TracksAdapter.TracksQuery.PROJECTION_WITH_VENDORS_COUNT;
	            selection = ScheduleContract.Tracks.VENDORS_COUNT + ">0";
	            //AnalyticsUtils.getInstance(getActivity()).trackPageView("/Sandbox");
	        }
	
	        // Start background query to load tracks
	        mHandler = new NotifyingAsyncQueryHandler(Activity.ContentResolver, this);
	        mHandler.StartQuery(tracksUri, projection, selection, null, ScheduleContract.Tracks.DEFAULT_SORT);
		}
		
		public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup) inflater.Inflate(Resource.Layout.fragment_list_with_spinner, null);
	
	        // For some reason, if we omit this, NoSaveStateFrameLayout thinks we are
	        // FILL_PARENT / WRAP_CONTENT, making the progress bar stick to the top of the activity.
	        root.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
	        return root;
		}
		
		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			ListView.ChoiceMode = ChoiceMode.Single;
		}
		

		#region AsyncQueryListener implementation
		public void OnQueryComplete (int token, Java.Lang.Object cookie, Android.Database.ICursor cursor)
		{
			Console.WriteLine ("OnQueryComplete!");
			if (Activity == null) {
	            return;
	        }
			
			Console.WriteLine (	"Is Sessions? = " + TracksFragment.NEXT_TYPE_SESSIONS.Equals(mNextType));
	
	        Activity.StartManagingCursor(cursor);
	        mAdapter.SetHasAllItem(true);
	        mAdapter.SetIsSessions(TracksFragment.NEXT_TYPE_SESSIONS.Equals(mNextType));
	        mAdapter.ChangeCursor(cursor);
		}
		#endregion
		
		public override void OnListItemClick (Android.Widget.ListView l, Android.Views.View v, int position, long id)
		{
			ICursor cursor = (ICursor) mAdapter.GetItem(position);
	        String trackId;
	
	        if (cursor != null) {
	            trackId = cursor.GetString(TracksAdapter.TracksQuery.TRACK_ID);
	        } else {
	            trackId = ScheduleContract.Tracks.ALL_TRACK_ID;
	        }
	
	        Intent intent = new Intent(Intent.ActionView);
	        Uri trackUri = ScheduleContract.Tracks.BuildTrackUri(trackId);
	        intent.PutExtra(SessionDetailFragment.EXTRA_TRACK, trackUri);
	
	        if (NEXT_TYPE_SESSIONS.Equals(mNextType)) {
	            if (cursor == null) {
	                intent.SetData(ScheduleContract.Sessions.CONTENT_URI);
	            } else {
	                intent.SetData(ScheduleContract.Tracks.BuildSessionsUri(trackId));
	            }
	        } else if (NEXT_TYPE_VENDORS.Equals(mNextType)) {
	            if (cursor == null) {
	                intent.SetData(ScheduleContract.Vendors.CONTENT_URI);
	            } else {
	                intent.SetData(ScheduleContract.Tracks.BuildVendorsUri(trackId));
	            }
	        }
			
	        ((BaseActivity) Activity).OpenActivityOrFragment(intent);
	
	        ListView.SetItemChecked(position, true);
		}
	}
}

