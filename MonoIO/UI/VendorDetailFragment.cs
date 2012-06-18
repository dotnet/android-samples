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
using Fragment = Android.Support.V4.App.Fragment;
using Android.Text.Style;
using Android.Graphics;
using MonoIO.UI;
using Uri = Android.Net.Uri;
using Android.Util;
using MonoIO.Utilities;
using Android.Text;
using Android.Database;
using Android.Text.Method;

namespace MonoIO
{
	/**
	 * A fragment that shows detail information for a sandbox company, including company name,
	 * description, product description, logo, etc.
	 */
	public class VendorDetailFragment : Fragment, NotifyingAsyncQueryHandler.AsyncQueryListener
	{
		private static String TAG = "VendorDetailFragment";
		private Uri mVendorUri;
		private String mTrackId;
		private ViewGroup mRootView;
		private TextView mName;
		private CompoundButton mStarred;
		private ImageView mLogo;
		private TextView mUrl;
		private TextView mDesc;
		private TextView mProductDesc;
		private String mNameString;
		private NotifyingAsyncQueryHandler mHandler;
		
		public override void OnCreate (Bundle p0)
		{
			base.OnCreate (p0);
			
			Intent intent = BaseActivity.FragmentArgumentsToIntent (Arguments);
			mVendorUri = intent.Data;
			if (mVendorUri == null) {
				return;
			}
	
			SetHasOptionsMenu (true);
		}
		
		public override void OnActivityCreated (Bundle p0)
		{
			base.OnActivityCreated (p0);
			
			if (mVendorUri == null) {
				return;
			}

			// Start background query to load vendor details
	
			mHandler = new NotifyingAsyncQueryHandler (Activity.ContentResolver, this);
			mHandler.StartQuery (mVendorUri, VendorsQuery.PROJECTION);
	
		}
		
		public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
		{
			
			mRootView = (ViewGroup)inflater.Inflate (Resource.Layout.fragment_vendor_detail, null);
			mName = mRootView.FindViewById<TextView> (Resource.Id.vendor_name);
			mStarred = mRootView.FindViewById<CompoundButton> (Resource.Id.star_button);
	
			mStarred.Focusable = true;
			mStarred.Clickable = true;
	
			// Larger target triggers star toggle
			View starParent = mRootView.FindViewById (Resource.Id.header_vendor);
				        
			FractionalTouchDelegate.SetupDelegate (starParent, mStarred, new RectF (0.6f, 0f, 1f, 0.8f));
	
			mLogo = mRootView.FindViewById<ImageView> (Resource.Id.vendor_logo);
			mUrl = mRootView.FindViewById<TextView> (Resource.Id.vendor_url);
			mDesc = mRootView.FindViewById<TextView> (Resource.Id.vendor_desc);
			mProductDesc = mRootView.FindViewById<TextView> (Resource.Id.vendor_product_desc);
	
			return mRootView;
		}
		
		/**
	     * Build a {@link android.view.View} to be used as a tab indicator, setting the requested string resource as
	     * its label.
	     *
	     * @return View
	     */
		private View BuildIndicator (int textRes)
		{
			TextView indicator = (TextView)Activity.LayoutInflater
				.Inflate (Resource.Layout.tab_indicator, mRootView.FindViewById<ViewGroup> (Android.Resource.Id.Tabs), false);
			indicator.SetText (textRes);
			return indicator;
		}
		
		
		/**
	     * {@inheritDoc}
	     */
		public void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			if (Activity == null) {
				return;
			}
	
			try {
				if (!cursor.MoveToFirst ()) {
					return;
				}
	
				mNameString = cursor.GetString (VendorsQuery.NAME);
				mName.Text = mNameString;
	
				// Unregister around setting checked state to avoid triggering
				// listener since change isn't user generated.
				mStarred.CheckedChange += null;
				mStarred.Checked = cursor.GetInt (VendorsQuery.STARRED) != 0;
				mStarred.CheckedChange += HandleCheckedChange;
	
				// Start background fetch to load vendor logo
				String logoUrl = cursor.GetString (VendorsQuery.LOGO_URL);
	
				if (!TextUtils.IsEmpty (logoUrl)) {
					BitmapUtils.FetchImage (Activity, new Java.Lang.String (logoUrl), null, null, (result) => {
						Activity.RunOnUiThread (() => {
							if (result == null) {
								mLogo.Visibility = ViewStates.Gone;
							} else {
								mLogo.Visibility = ViewStates.Visible;
								mLogo.SetImageBitmap (result);
							}
						});
					});
				}
	
				mUrl.Text = cursor.GetString (VendorsQuery.URL);
				mDesc.Text = cursor.GetString (VendorsQuery.DESC);
				mProductDesc.Text = cursor.GetString (VendorsQuery.PRODUCT_DESC);
	
				mTrackId = cursor.GetString (VendorsQuery.TRACK_ID);
	
				// Assign track details when found
				// TODO: handle vendors not attached to track
				ActivityHelper activityHelper = ((BaseActivity)Activity).ActivityHelper;
				activityHelper.SetActionBarTitle (new Java.Lang.String (cursor.GetString (VendorsQuery.TRACK_NAME)));
				activityHelper.SetActionBarColor (new Color (cursor.GetInt (VendorsQuery.TRACK_COLOR)));
	
				//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Sandbox/Vendors/" + mNameString);
	
			} finally {
				cursor.Close ();
			}
		}
		
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.map_menu_items, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_map) {
				// The room ID for the sandbox, in the map, is just the track ID
				Intent intent = new Intent (Activity.ApplicationContext, UIUtils.GetMapActivityClass (Activity));
				intent.PutExtra (MapFragment.EXTRA_ROOM, ParserUtils.TranslateTrackIdAliasInverse (mTrackId));
				StartActivity (intent);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void HandleCheckedChange (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var values = new ContentValues ();
			values.Put (ScheduleContract.Vendors.VENDOR_STARRED, e.IsChecked ? 1 : 0);
			mHandler.StartUpdate (mVendorUri, values);
	
			// Because change listener is set to null during initialization, these won't fire on
			// pageview.
			//AnalyticsUtils.getInstance(getActivity()).trackEvent("Sandbox", isChecked ? "Starred" : "Unstarred", mTitleString, 0);	
		}
		
		/**
	     * {@link com.google.android.apps.iosched.provider.ScheduleContract.Vendors} query parameters.
	     */
		private class VendorsQuery
		{
			public static String[] PROJECTION = {
	                ScheduleContract.Vendors.VENDOR_NAME,
	                ScheduleContract.Vendors.VENDOR_LOCATION,
	                ScheduleContract.Vendors.VENDOR_DESC,
	                ScheduleContract.Vendors.VENDOR_URL,
	                ScheduleContract.Vendors.VENDOR_PRODUCT_DESC,
	                ScheduleContract.Vendors.VENDOR_LOGO_URL,
	                ScheduleContract.Vendors.VENDOR_STARRED,
	                ScheduleContract.Vendors.TRACK_ID,
	                ScheduleContract.Tracks.TRACK_NAME,
	                ScheduleContract.Tracks.TRACK_COLOR,
	        };
			public static int NAME = 0;
			public static int LOCATION = 1;
			public static int DESC = 2;
			public static int URL = 3;
			public static int PRODUCT_DESC = 4;
			public static int LOGO_URL = 5;
			public static int STARRED = 6;
			public static int TRACK_ID = 7;
			public static int TRACK_NAME = 8;
			public static int TRACK_COLOR = 9;
		}
		
	}
}

