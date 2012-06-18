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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace MonoIO
{
	public class TracksAdapter : CursorAdapter
	{
		private int ALL_ITEM_ID = Int32.MaxValue;
		private Activity mActivity;
		private bool mHasAllItem;
		private int mPositionDisplacement;
		private bool mIsSessions = true;
		
		public TracksAdapter (Activity activity) : base(activity, null)
		{
			mActivity = activity;
		}
	
		public void SetHasAllItem (bool hasAllItem)
		{
			mHasAllItem = hasAllItem;
			mPositionDisplacement = mHasAllItem ? 1 : 0;
		}
	
		public void SetIsSessions (bool isSessions)
		{
			mIsSessions = isSessions;
		}
		
		public override int Count {
			get {
				return base.Count + mPositionDisplacement;
			}
		}
		
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			Console.WriteLine ("Position = " + position);
			if (mHasAllItem && position == 0) {
				if (convertView == null) {
					convertView = mActivity.LayoutInflater.Inflate (Resource.Layout.list_item_track, parent, false);
				}
	
				// Custom binding for the first item
				(convertView.FindViewById<TextView> (Android.Resource.Id.Text1)).Text = (
	                    "(" + mActivity.Resources.GetString (mIsSessions
	                            ? Resource.String.all_sessions_title
	                            : Resource.String.all_sandbox_title)
	                            + ")");
				convertView.FindViewById (Android.Resource.Id.Icon1).Visibility = ViewStates.Invisible;
	
				return convertView;
			}
			return base.GetView (position - mPositionDisplacement, convertView, parent);
		}
		
		public override Java.Lang.Object GetItem (int position)
		{
			if (mHasAllItem && position == 0) {
				return null;
			}
			
			return base.GetItem (position - mPositionDisplacement);
		}
		
		public override long GetItemId (int position)
		{
			if (mHasAllItem && position == 0) {
				return ALL_ITEM_ID;
			}
			return base.GetItemId (position - mPositionDisplacement);
		}
		
		public override bool IsEnabled (int position)
		{
			if (mHasAllItem && position == 0) {
				return true;
			}
			return base.IsEnabled (position - mPositionDisplacement);
		}
		
		public override int ViewTypeCount {
			get {
				// Add an item type for the "All" view.
				return base.ViewTypeCount + 1;
			}
		}
		
		public override int GetItemViewType (int position)
		{
			if (mHasAllItem && position == 0) {
				return this.ViewTypeCount - 1;
			}
			return base.GetItemViewType (position - mPositionDisplacement);
		}
		
		public override View NewView (Context context, Android.Database.ICursor cursor, ViewGroup parent)
		{
			return mActivity.LayoutInflater.Inflate (Resource.Layout.list_item_track, parent, false);
		}
		
		public override void BindView (View view, Context context, Android.Database.ICursor cursor)
		{
			TextView textView = view.FindViewById<TextView> (Android.Resource.Id.Text1);
			textView.Text = (cursor.GetString (TracksQuery.TRACK_NAME));
	
			// Assign track color to visible block
			ImageView iconView = view.FindViewById<ImageView> (Android.Resource.Id.Icon1);
			iconView.SetImageDrawable (new ColorDrawable (new Color (cursor.GetInt (TracksQuery.TRACK_COLOR))));
		}
		
		/** {@link com.google.android.apps.iosched.provider.ScheduleContract.Tracks} query parameters. */
		public class TracksQuery
		{
			public static int _TOKEN = 0x1;
			public static String[] PROJECTION = {
	                BaseColumns.Id,
	                ScheduleContract.Tracks.TRACK_ID,
	                ScheduleContract.Tracks.TRACK_NAME,
	                ScheduleContract.Tracks.TRACK_ABSTRACT,
	                ScheduleContract.Tracks.TRACK_COLOR,
	        };
			public static String[] PROJECTION_WITH_SESSIONS_COUNT = {
	                BaseColumns.Id,
	                ScheduleContract.Tracks.TRACK_ID,
	                ScheduleContract.Tracks.TRACK_NAME,
	                ScheduleContract.Tracks.TRACK_ABSTRACT,
	                ScheduleContract.Tracks.TRACK_COLOR,
	                ScheduleContract.Tracks.SESSIONS_COUNT,
	        };
			public static String[] PROJECTION_WITH_VENDORS_COUNT = {
	                BaseColumns.Id,
	                ScheduleContract.Tracks.TRACK_ID,
	                ScheduleContract.Tracks.TRACK_NAME,
	                ScheduleContract.Tracks.TRACK_ABSTRACT,
	                ScheduleContract.Tracks.TRACK_COLOR,
	                ScheduleContract.Tracks.VENDORS_COUNT,
	        };
			public static int _ID = 0;
			public static int TRACK_ID = 1;
			public static int TRACK_NAME = 2;
			public static int TRACK_ABSTRACT = 3;
			public static int TRACK_COLOR = 4;
		}
	}
}

