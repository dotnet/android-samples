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
	public class SessionDetailFragment : Fragment, NotifyingAsyncQueryHandler.AsyncQueryListener
	{
		private static String TAG = "SessionDetailFragment";
	
		/**
	     * Since sessions can belong tracks, the parent activity can send this extra specifying a
	     * track URI that should be used for coloring the title-bar.
	     */
		public static String EXTRA_TRACK = "monoio.extra.TRACK";
		private static String TAG_SUMMARY = "summary";
		private static String TAG_NOTES = "notes";
		private static String TAG_LINKS = "links";
		private static StyleSpan sBoldSpan = new StyleSpan (TypefaceStyle.Bold);
		private String mSessionId;
		private Uri mSessionUri;
		private Uri mTrackUri;
		private String mTitleString;
		private String mHashtag;
		private String mUrl;
		private TextView mTagDisplay;
		private String mRoomId;
		private ViewGroup mRootView;
		private TabHost mTabHost;
		private TextView mTitle;
		private TextView mSubtitle;
		private CompoundButton mStarred;
		private TextView mAbstract;
		private TextView mRequirements;
		private NotifyingAsyncQueryHandler mHandler;
		private PackageChangesReceiver packageChangesReceiver;
		private bool mSessionCursor = false;
		private bool mSpeakersCursor = false;
		private bool mHasSummaryContent = false;
		
		public override void OnCreate (Bundle p0)
		{
			base.OnCreate (p0);
			
			Intent intent = BaseActivity.FragmentArgumentsToIntent (Arguments);
			mSessionUri = intent.Data;
			mTrackUri = ResolveTrackUri (intent);
			packageChangesReceiver = new PackageChangesReceiver (this);
			
			if (mSessionUri == null) {
				return;
			}
	
			mSessionId = ScheduleContract.Sessions.GetSessionId (mSessionUri);
			
			SetHasOptionsMenu (true);
		}
		
		public override void OnResume ()
		{
			base.OnResume ();
			
			UpdateNotesTab ();
	
			// Start listening for time updates to adjust "now" bar. TIME_TICK is
			// triggered once per minute, which is how we move the bar over time.
			IntentFilter filter = new IntentFilter ();
			filter.AddAction (Intent.ActionPackageAdded);
			filter.AddAction (Intent.ActionPackageRemoved);
			filter.AddAction (Intent.ActionPackageReplaced);
			filter.AddDataScheme ("package");
			Activity.RegisterReceiver (packageChangesReceiver, filter);
		}
		
		public override void OnPause ()
		{
			base.OnPause ();
			Activity.UnregisterReceiver (packageChangesReceiver);
		}
		
		public override void OnActivityCreated (Bundle p0)
		{
			base.OnActivityCreated (p0);
			
			if (mSessionUri == null) {
				return;
			}
	
			// Start background queries to load session and track details
			Uri speakersUri = ScheduleContract.Sessions.BuildSpeakersDirUri (mSessionId);
	
			mHandler = new NotifyingAsyncQueryHandler (Activity.ContentResolver, this);
			mHandler.StartQuery (SessionsQuery._TOKEN, mSessionUri, SessionsQuery.PROJECTION);
			mHandler.StartQuery (TracksQuery._TOKEN, mTrackUri, TracksQuery.PROJECTION);
			mHandler.StartQuery (SpeakersQuery._TOKEN, speakersUri, SpeakersQuery.PROJECTION);
		}
		
		public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
		{
			
			mRootView = (ViewGroup)inflater.Inflate (Resource.Layout.fragment_session_detail, null);
			mTabHost = mRootView.FindViewById<TabHost> (Android.Resource.Id.TabHost);
			mTabHost.Setup ();
	
			mTitle = mRootView.FindViewById<TextView> (Resource.Id.session_title);
			mSubtitle = mRootView.FindViewById<TextView> (Resource.Id.session_subtitle);
			mStarred = mRootView.FindViewById<CompoundButton> (Resource.Id.star_button);
			
	
			mStarred.CheckedChange += HandleCheckedChange;
			mStarred.Focusable = true;
			mStarred.Clickable = true;
	
			// Larger target triggers star toggle
			View starParent = mRootView.FindViewById<View> (Resource.Id.header_session);
			FractionalTouchDelegate.SetupDelegate (starParent, mStarred, new RectF (0.6f, 0f, 1f, 0.8f));
	
			mAbstract = mRootView.FindViewById<TextView> (Resource.Id.session_abstract);
			mRequirements = mRootView.FindViewById<TextView> (Resource.Id.session_requirements);
	
			SetupSummaryTab ();
			SetupNotesTab ();
			SetupLinksTab ();
	
			return mRootView;
		}
		
		/**
	     * Build and add "summary" tab.
	     */
		private void SetupSummaryTab ()
		{
			// Summary content comes from existing layout
			mTabHost.AddTab (mTabHost.NewTabSpec (TAG_SUMMARY)
	                .SetIndicator (BuildIndicator (Resource.String.session_summary))
	                .SetContent (Resource.Id.tab_session_summary));
		}
		
		/**
	     * Build a {@link View} to be used as a tab indicator, setting the requested string resource as
	     * its label.
	     *
	     * @param textRes
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
	     * Derive {@link com.google.android.apps.iosched.provider.ScheduleContract.Tracks#CONTENT_ITEM_TYPE}
	     * {@link Uri} based on incoming {@link Intent}, using
	     * {@link #EXTRA_TRACK} when set.
	     * @param intent
	     * @return Uri
	     */
		private Uri ResolveTrackUri (Intent intent)
		{
			Uri trackUri = (Uri)intent.GetParcelableExtra (EXTRA_TRACK);
			if (trackUri != null) {
				return trackUri;
			} else {
				return ScheduleContract.Sessions.BuildTracksDirUri (mSessionId);
			}
		}
		
		/**
	     * {@inheritDoc}
	     */
		public void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			if (Activity == null) {
				return;
			}
	
			if (token == SessionsQuery._TOKEN) {
				OnSessionQueryComplete (cursor);
			} else if (token == TracksQuery._TOKEN) {
				OnTrackQueryComplete (cursor);
			} else if (token == SpeakersQuery._TOKEN) {
				OnSpeakersQueryComplete (cursor);
			} else {
				cursor.Close ();
			}
		}
		
		/**
	     * Handle {@link SessionsQuery} {@link Cursor}.
	     */
		private void OnSessionQueryComplete (ICursor cursor)
		{
			try {
				mSessionCursor = true;
				if (!cursor.MoveToFirst ()) {
					return;
				}
	
				// Format time block this session occupies
				long blockStart = cursor.GetLong (SessionsQuery.BLOCK_START);
				long blockEnd = cursor.GetLong (SessionsQuery.BLOCK_END);
				String roomName = cursor.GetString (SessionsQuery.ROOM_NAME);
				String subtitle = UIUtils.FormatSessionSubtitle (blockStart, blockEnd, roomName, Activity);
	
				mTitleString = cursor.GetString (SessionsQuery.TITLE);
				mTitle.Text = mTitleString;
				mSubtitle.Text = subtitle;
	
				mUrl = cursor.GetString (SessionsQuery.URL);
				if (TextUtils.IsEmpty (mUrl)) {
					mUrl = "";
				}
	
				mHashtag = cursor.GetString (SessionsQuery.HASHTAG);
				mTagDisplay = mRootView.FindViewById<TextView> (Resource.Id.session_tags_button);
				if (!TextUtils.IsEmpty (mHashtag)) {
					// Create the button text
					SpannableStringBuilder sb = new SpannableStringBuilder ();
					sb.Append (GetString (Resource.String.tag_stream) + " ");
					int boldStart = sb.Length ();
					sb.Append (GetHashtagsString ());
					sb.SetSpan (sBoldSpan, boldStart, sb.Length (), SpanTypes.ExclusiveExclusive);
	
					mTagDisplay.Text = (string)sb;
					mTagDisplay.Click += (sender, e) => {
						Intent intent = new Intent (Activity, typeof(TagStreamActivity));
						intent.PutExtra (TagStreamFragment.EXTRA_QUERY, GetHashtagsString ());
						StartActivity (intent);	
					};
					
				} else {
					mTagDisplay.Visibility = ViewStates.Gone;
				}
	
				mRoomId = cursor.GetString (SessionsQuery.ROOM_ID);
	
				// Unregister around setting checked state to avoid triggering
				// listener since change isn't user generated.
				mStarred.CheckedChange += null;
				mStarred.Checked = cursor.GetInt (SessionsQuery.STARRED) != 0;
				mStarred.CheckedChange += HandleCheckedChange;
	
				String sessionAbstract = cursor.GetString (SessionsQuery.ABSTRACT);
				if (!TextUtils.IsEmpty (sessionAbstract)) {
					UIUtils.SetTextMaybeHtml (mAbstract, sessionAbstract);
					mAbstract.Visibility = ViewStates.Visible;
					mHasSummaryContent = true;
				} else {
					mAbstract.Visibility = ViewStates.Gone;
				}
	
				View requirementsBlock = mRootView.FindViewById (Resource.Id.session_requirements_block);
				String sessionRequirements = cursor.GetString (SessionsQuery.REQUIREMENTS);
				if (!TextUtils.IsEmpty (sessionRequirements)) {
					UIUtils.SetTextMaybeHtml (mRequirements, sessionRequirements);
					requirementsBlock.Visibility = ViewStates.Visible;
					mHasSummaryContent = true;
				} else {
					requirementsBlock.Visibility = ViewStates.Gone;
				}
	
				// Show empty message when all data is loaded, and nothing to show
				if (mSpeakersCursor && !mHasSummaryContent) {
					mRootView.FindViewWithTag (Android.Resource.Id.Empty).Visibility = ViewStates.Visible;
				}
	
				//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Sessions/" + mTitleString);
	
				UpdateLinksTab (cursor);
				UpdateNotesTab ();
	
			} finally {
				cursor.Close ();
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
				activityHelper.SetActionBarTitle (new Java.Lang.String (cursor.GetString (TracksQuery.TRACK_NAME)));
				activityHelper.SetActionBarColor (new Color (cursor.GetInt (TracksQuery.TRACK_COLOR)));
			} finally {
				cursor.Close ();
			}
		}
		
		private void OnSpeakersQueryComplete (ICursor cursor)
		{
			try {
				mSpeakersCursor = true;
				// TODO: remove any existing speakers from layout, since this cursor
				// might be from a data change notification.
				ViewGroup speakersGroup = mRootView.FindViewById<ViewGroup> (Resource.Id.session_speakers_block);
				LayoutInflater inflater = Activity.LayoutInflater;
	
				bool hasSpeakers = false;
	
				while (cursor.MoveToNext()) {
					String speakerName = cursor.GetString (SpeakersQuery.SPEAKER_NAME);
					if (TextUtils.IsEmpty (speakerName)) {
						continue;
					}
	
					String speakerImageUrl = cursor.GetString (SpeakersQuery.SPEAKER_IMAGE_URL);
					String speakerCompany = cursor.GetString (SpeakersQuery.SPEAKER_COMPANY);
					String speakerUrl = cursor.GetString (SpeakersQuery.SPEAKER_URL);
					String speakerAbstract = cursor.GetString (SpeakersQuery.SPEAKER_ABSTRACT);
	
					String speakerHeader = speakerName;
					if (!TextUtils.IsEmpty (speakerCompany)) {
						speakerHeader += ", " + speakerCompany;
					}
	
					var speakerView = inflater.Inflate (Resource.Layout.speaker_detail, speakersGroup, false);
					var speakerHeaderView = speakerView.FindViewById<TextView> (Resource.Id.speaker_header);
					var speakerImgView = speakerView.FindViewById<ImageView> (Resource.Id.speaker_image);
					var speakerUrlView = speakerView.FindViewById<TextView> (Resource.Id.speaker_url);
					var speakerAbstractView = speakerView.FindViewById<TextView> (Resource.Id.speaker_abstract);
					
					if (!TextUtils.IsEmpty (speakerImageUrl)) {
						BitmapUtils.FetchImage (Activity, new Java.Lang.String (speakerImageUrl), null, null, (result) => {
							Activity.RunOnUiThread (() => {
								speakerImgView.SetImageBitmap (result);
							});
						});
					}
	
					speakerHeaderView.Text = speakerHeader;
					UIUtils.SetTextMaybeHtml (speakerAbstractView, speakerAbstract);
	
					if (!TextUtils.IsEmpty (speakerUrl)) {
						UIUtils.SetTextMaybeHtml (speakerUrlView, speakerUrl);
						speakerUrlView.Visibility = ViewStates.Visible;
					} else {
						speakerUrlView.Visibility = ViewStates.Gone;
					}
	
					speakersGroup.AddView (speakerView);
					hasSpeakers = true;
					mHasSummaryContent = true;
				}
	
				speakersGroup.Visibility = (hasSpeakers ? ViewStates.Visible : ViewStates.Gone);
	
				// Show empty message when all data is loaded, and nothing to show
				if (mSessionCursor && !mHasSummaryContent) {
					mRootView.FindViewById (Android.Resource.Id.Empty).Visibility = ViewStates.Visible;
				}
			} finally {
				if (null != cursor) {
					cursor.Close ();
				}
			}
		}
		
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.session_detail_menu_items, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			String shareString;
			Intent intent;
	
			switch (item.ItemId) {
			case Resource.Id.menu_map:
				intent = new Intent (Activity.ApplicationContext, UIUtils.GetMapActivityClass (Activity));
				intent.PutExtra (MapFragment.EXTRA_ROOM, mRoomId);
				StartActivity (intent);
				return true;
	
			case Resource.Id.menu_share:
	                // TODO: consider bringing in shortlink to session
				shareString = GetString (Resource.String.share_template, mTitleString, GetHashtagsString (), mUrl);
				intent = new Intent (Intent.ActionSend);
				intent.SetType ("text/plain");
				intent.PutExtra (Intent.ExtraText, shareString);
				StartActivity (Intent.CreateChooser (intent, GetText (Resource.String.title_share)));
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void HandleCheckedChange (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var values = new ContentValues ();
			values.Put (ScheduleContract.Sessions.SESSION_STARRED, e.IsChecked ? 1 : 0);
			mHandler.StartUpdate (mSessionUri, values);
	
			// Because change listener is set to null during initialization, these won't fire on
			// pageview.
			//AnalyticsUtils.getInstance(getActivity()).trackEvent("Sandbox", isChecked ? "Starred" : "Unstarred", mTitleString, 0);	
		}
		
		
		/**
	     * Build and add "summary" tab.
	     */
		private void SetupLinksTab ()
		{
			// Summary content comes from existing layout
			mTabHost.AddTab (mTabHost.NewTabSpec (TAG_LINKS)
	                .SetIndicator (BuildIndicator (Resource.String.session_links))
	                .SetContent (Resource.Id.tab_session_links));
		}
	
		private void UpdateLinksTab (ICursor cursor)
		{
			ViewGroup container = mRootView.FindViewById<ViewGroup> (Resource.Id.links_container);
	
			// Remove all views but the 'empty' view
			int childCount = container.ChildCount;
			if (childCount > 1) {
				container.RemoveViews (1, childCount - 1);
			}
	
			LayoutInflater inflater = GetLayoutInflater (null);
	
			bool hasLinks = false;
			for (int i = 0; i < SessionsQuery.LINKS_INDICES.Length; i++) {
				String url = cursor.GetString (SessionsQuery.LINKS_INDICES [i]);
				if (!TextUtils.IsEmpty (url)) {
					hasLinks = true;
					ViewGroup linkContainer = (ViewGroup)inflater.Inflate (Resource.Layout.list_item_session_link, container, false);
					linkContainer.FindViewById<TextView> (Resource.Id.link_text).SetText (SessionsQuery.LINKS_TITLES [i]);
					int linkTitleIndex = i;
	                
					linkContainer.Click += (sender, e) => {
						FireLinkEvent (SessionsQuery.LINKS_TITLES [linkTitleIndex]);
						Intent intent = new Intent (Intent.ActionView, Android.Net.Uri.Parse (url));
						intent.AddFlags (ActivityFlags.ClearWhenTaskReset);
						StartActivity (intent);
					};
					
					container.AddView (linkContainer);
	
					// Create separator
					View separatorView = new ImageView (Activity);
					separatorView.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
					separatorView.SetBackgroundResource (Android.Resource.Drawable.DividerHorizontalBright);
					container.AddView (separatorView);
				}
			}
	
			container.FindViewById (Resource.Id.empty_links).Visibility = (hasLinks ? ViewStates.Gone : ViewStates.Visible);
		}
		
		/*
	     * Event structure:
	     * Category -> "Session Details"
	     * Action -> "Create Note", "View Note", etc
	     * Label -> Session's Title
	     * Value -> 0.
	     */
		public void FireNotesEvent (int actionId)
		{
			//AnalyticsUtils.getInstance(getActivity()).trackEvent("Session Details", getActivity().getString(actionId), mTitleString, 0);
		}
		
		/**
	     * Build and add "notes" tab.
	     */
		private void SetupNotesTab ()
		{
			// Make powered-by clickable
			mRootView.FindViewById<TextView> (Resource.Id.notes_powered_by).MovementMethod = LinkMovementMethod.Instance;
	
			// Setup tab
			mTabHost.AddTab (mTabHost.NewTabSpec (TAG_NOTES)
	                .SetIndicator (BuildIndicator (Resource.String.session_notes))
	                .SetContent (Resource.Id.tab_session_notes));
		}
		
		public void UpdateNotesTab ()
		{
			CatchNotesHelper helper = new CatchNotesHelper (Activity);
			bool notesInstalled = helper.IsNotesInstalledAndMinimumVersion ();
	
			Intent marketIntent = helper.NotesMarketIntent ();
			Intent newIntent = helper.CreateNoteIntent (GetString (Resource.String.note_template, mTitleString ?? "", GetHashtagsString ()));
	        
			Intent viewIntent = helper.ViewNotesIntent (GetHashtagsString ());
	
			// Set icons and click listeners
			(mRootView.FindViewById<ImageView> (Resource.Id.notes_catch_market_icon)).SetImageDrawable (
	                UIUtils.GetIconForIntent (Activity, marketIntent));
			(mRootView.FindViewById<ImageView> (Resource.Id.notes_catch_new_icon)).SetImageDrawable (
	                UIUtils.GetIconForIntent (Activity, newIntent));
			(mRootView.FindViewById<ImageView> (Resource.Id.notes_catch_view_icon)).SetImageDrawable (
	                UIUtils.GetIconForIntent (Activity, viewIntent));
	
			// Set click listeners
			mRootView.FindViewById (Resource.Id.notes_catch_market_link).Click += (sender, e) => {
				StartActivity (marketIntent);
				FireNotesEvent (Resource.String.notes_catch_market_title);
			};
			
			mRootView.FindViewById (Resource.Id.notes_catch_new_link).Click += (sender, e) => {
				StartActivity (newIntent);
				FireNotesEvent (Resource.String.notes_catch_new_title);
			};
			
			mRootView.FindViewById (Resource.Id.notes_catch_view_link).Click += (sender, e) => {
				StartActivity (viewIntent);
				FireNotesEvent (Resource.String.notes_catch_view_title);
			};
			
			// Show/hide elements
			mRootView.FindViewById (Resource.Id.notes_catch_market_link).Visibility = notesInstalled ? ViewStates.Gone : ViewStates.Visible;
			mRootView.FindViewById (Resource.Id.notes_catch_market_separator).Visibility = notesInstalled ? ViewStates.Gone : ViewStates.Visible;
	
			
			mRootView.FindViewById (Resource.Id.notes_catch_new_link).Visibility = !notesInstalled ? ViewStates.Gone : ViewStates.Visible;
			mRootView.FindViewById (Resource.Id.notes_catch_new_separator).Visibility = !notesInstalled ? ViewStates.Gone : ViewStates.Visible;
			
			
			mRootView.FindViewById (Resource.Id.notes_catch_view_link).Visibility = !notesInstalled ? ViewStates.Gone : ViewStates.Visible;
			mRootView.FindViewById (Resource.Id.notes_catch_view_separator).Visibility = !notesInstalled ? ViewStates.Gone : ViewStates.Visible;
		}
		
	
		/*
	     * Event structure:
	     * Category -> "Session Details"
	     * Action -> Link Text
	     * Label -> Session's Title
	     * Value -> 0.
	     */
		public void FireLinkEvent (int actionId)
		{
			//AnalyticsUtils.getInstance(getActivity()).trackEvent("Link Details", getActivity().getString(actionId), mTitleString, 0);
		}

		private String GetHashtagsString ()
		{
			if (!TextUtils.IsEmpty (mHashtag)) {
				return TagStreamFragment.DEFAULT_SEARCH + " #" + mHashtag;
			} else {
				return TagStreamFragment.DEFAULT_SEARCH;
			}
		}
		
		public class PackageChangesReceiver : BroadcastReceiver
		{
			SessionDetailFragment _fragment;

			public PackageChangesReceiver (SessionDetailFragment fragment)
			{
				_fragment = fragment;
			}
			
			public override void OnReceive (Context context, Intent intent)
			{
				_fragment.UpdateNotesTab ();
			}
		}
		
		/**
	     * {@link com.google.android.apps.iosched.provider.ScheduleContract.Sessions} query parameters.
	     */
		private class SessionsQuery
		{
			public static int _TOKEN = 0x1;
			public static String[] PROJECTION = {
	                ScheduleContract.Blocks.BLOCK_START,
	                ScheduleContract.Blocks.BLOCK_END,
	                ScheduleContract.Sessions.SESSION_LEVEL,
	                ScheduleContract.Sessions.SESSION_TITLE,
	                ScheduleContract.Sessions.SESSION_ABSTRACT,
	                ScheduleContract.Sessions.SESSION_REQUIREMENTS,
	                ScheduleContract.Sessions.SESSION_STARRED,
	                ScheduleContract.Sessions.SESSION_HASHTAG,
	                ScheduleContract.Sessions.SESSION_SLUG,
	                ScheduleContract.Sessions.SESSION_URL,
	                ScheduleContract.Sessions.SESSION_MODERATOR_URL,
	                ScheduleContract.Sessions.SESSION_YOUTUBE_URL,
	                ScheduleContract.Sessions.SESSION_PDF_URL,
	                ScheduleContract.Sessions.SESSION_FEEDBACK_URL,
	                ScheduleContract.Sessions.SESSION_NOTES_URL,
	                ScheduleContract.Sessions.ROOM_ID,
	                ScheduleContract.Rooms.ROOM_NAME,
	        };
			public static int BLOCK_START = 0;
			public static int BLOCK_END = 1;
			public static int LEVEL = 2;
			public static int TITLE = 3;
			public static int ABSTRACT = 4;
			public static int REQUIREMENTS = 5;
			public static int STARRED = 6;
			public static int HASHTAG = 7;
			public static int SLUG = 8;
			public static int URL = 9;
			public static int MODERATOR_URL = 10;
			public static int YOUTUBE_URL = 11;
			public static int PDF_URL = 12;
			public static int FEEDBACK_URL = 13;
			public static int NOTES_URL = 14;
			public static int ROOM_ID = 15;
			public static int ROOM_NAME = 16;
			public static int[] LINKS_INDICES = {
	                URL,
	                MODERATOR_URL,
	                YOUTUBE_URL,
	                PDF_URL,
	                FEEDBACK_URL,
	                NOTES_URL,
	        };
			public static int[] LINKS_TITLES = {
	                Resource.String.session_link_main,
	                Resource.String.session_link_moderator,
	                Resource.String.session_link_youtube,
	                Resource.String.session_link_pdf,
	                Resource.String.session_link_feedback,
	                Resource.String.session_link_notes,
	        };
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
	
		private class SpeakersQuery
		{
			public static int _TOKEN = 0x3;
			public static String[] PROJECTION = {
	                ScheduleContract.Speakers.SPEAKER_NAME,
	                ScheduleContract.Speakers.SPEAKER_IMAGE_URL,
	                ScheduleContract.Speakers.SPEAKER_COMPANY,
	                ScheduleContract.Speakers.SPEAKER_ABSTRACT,
	                ScheduleContract.Speakers.SPEAKER_URL,
	        };
			public static int SPEAKER_NAME = 0;
			public static int SPEAKER_IMAGE_URL = 1;
			public static int SPEAKER_COMPANY = 2;
			public static int SPEAKER_ABSTRACT = 3;
			public static int SPEAKER_URL = 4;
		}
	}
}

