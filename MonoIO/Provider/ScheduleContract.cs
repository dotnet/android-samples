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
using Android.Provider;
using Android.Text.Format;

namespace MonoIO
{
	/**
	 * Contract class for interacting with {@link ScheduleProvider}. Unless
	 * otherwise noted, all time-based fields are milliseconds since epoch and can
	 * be compared against {@link System#currentTimeMillis()}.
	 * <p>
	 * The backing {@link android.content.ContentProvider} assumes that {@link Uri} are generated
	 * using stronger {@link String} identifiers, instead of {@code int}
	 * {@link BaseColumns#_ID} values, which are prone to shuffle during sync.
	 */
	public class ScheduleContract
	{
		/**
	     * Special value for {@link SyncColumns#UPDATED} indicating that an entry
	     * has never been updated, or doesn't exist yet.
	     */
	    public const long UPDATED_NEVER = -2;
	
	    /**
	     * Special value for {@link SyncColumns#UPDATED} indicating that the last
	     * update time is unknown, usually when inserted from a local file source.
	     */
	    public const long UPDATED_UNKNOWN = -1;
	
	    public class SyncColumns {
	        /** Last time this entry was updated or synchronized. */
	        public static String UPDATED = "updated";
	    }
	
	    public class BlocksColumns  {
	        /** Unique string identifying this block of time. */
	        public static String BLOCK_ID = "block_id";
	        /** Title describing this block of time. */
	        public static String BLOCK_TITLE = "block_title";
	        /** Time when this block starts. */
	        public static String BLOCK_START = "block_start";
	        /** Time when this block ends. */
	        public static String BLOCK_END = "block_end";
	        /** Type describing this block. */
	        public static String BLOCK_TYPE = "block_type";
	    }
	
	    public class TracksColumns {
	        /** Unique string identifying this track. */
	        public static String TRACK_ID = "track_id";
	        /** Name describing this track. */
	        public static String TRACK_NAME = "track_name";
	        /** Color used to identify this track, in {@link Color#argb} format. */
	        public static String TRACK_COLOR = "track_color";
	        /** Body of text explaining this track in detail. */
	        public static String TRACK_ABSTRACT = "track_abstract";
	    }
	
	    public class RoomsColumns {
	        /** Unique string identifying this room. */
	        public static String ROOM_ID = "room_id";
	        /** Name describing this room. */
	        public static String ROOM_NAME = "room_name";
	        /** Building floor this room exists on. */
	        public static String ROOM_FLOOR = "room_floor";
	    }
	
	    public class SessionsColumns {
	        /** Unique string identifying this session. */
	        public static String SESSION_ID = "session_id";
	        /** Difficulty level of the session. */
	        public static String SESSION_LEVEL = "session_level";
	        /** Title describing this track. */
	        public static String SESSION_TITLE = "session_title";
	        /** Body of text explaining this session in detail. */
	        public static String SESSION_ABSTRACT = "session_abstract";
	        /** Requirements that attendees should meet. */
	        public static String SESSION_REQUIREMENTS = "session_requirements";
	        /** Kewords/tags for this session. */
	        public static String SESSION_KEYWORDS = "session_keywords";
	        /** Hashtag for this session. */
	        public static String SESSION_HASHTAG = "session_hashtag";
	        /** Slug (short name) for this session. */
	        public static String SESSION_SLUG = "session_slug";
	        /** Full URL to session online. */
	        public static String SESSION_URL = "session_url";
	        /** Link to Moderator for this session. */
	        public static String SESSION_MODERATOR_URL = "session_moderator_url";
	        /** Full URL to YouTube. */
	        public static String SESSION_YOUTUBE_URL = "session_youtube_url";
	        /** Full URL to PDF. */
	        public static String SESSION_PDF_URL = "session_pdf_url";
	        /** Full URL to speakermeter/external feedback URL. */
	        public static String SESSION_FEEDBACK_URL = "session_feedback_url";
	        /** Full URL to official session notes. */
	        public static String SESSION_NOTES_URL = "session_notes_url";
	        /** User-specific flag indicating starred status. */
	        public static String SESSION_STARRED = "session_starred";
			
			/** Time when this block starts. */
	        public static String BLOCK_START = "block_start";
	        /** Time when this block ends. */
	        public static String BLOCK_END = "block_end";
	    }
	
	    public class SpeakersColumns {
	        /** Unique string identifying this speaker. */
	        public static String SPEAKER_ID = "speaker_id";
	        /** Name of this speaker. */
	        public static String SPEAKER_NAME = "speaker_name";
	        /** Profile photo of this speaker. */
	        public static String SPEAKER_IMAGE_URL = "speaker_image_url";
	        /** Company this speaker works for. */
	        public static String SPEAKER_COMPANY = "speaker_company";
	        /** Body of text describing this speaker in detail. */
	        public static String SPEAKER_ABSTRACT = "speaker_abstract";
	        /** Full URL to the speaker's profile. */
	        public static String SPEAKER_URL = "speaker_url";
	    }
	
	    public class VendorsColumns {
	        /** Unique string identifying this vendor. */
	        public static String VENDOR_ID = "vendor_id";
	        /** Name of this vendor. */
	        public static String VENDOR_NAME = "vendor_name";
	        /** Location or city this vendor is based in. */
	        public static String VENDOR_LOCATION = "vendor_location";
	        /** Body of text describing this vendor. */
	        public static String VENDOR_DESC = "vendor_desc";
	        /** Link to vendor online. */
	        public static String VENDOR_URL = "vendor_url";
	        /** Body of text describing the product of this vendor. */
	        public static String VENDOR_PRODUCT_DESC = "vendor_product_desc";
	        /** Link to vendor logo. */
	        public static String VENDOR_LOGO_URL = "vendor_logo_url";
	        /** User-specific flag indicating starred status. */
	        public static String VENDOR_STARRED = "vendor_starred";
	    }
		
		public const String CONTENT_AUTHORITY = "monoio";

	    private static Uri BASE_CONTENT_URI = Uri.Parse("content://" + CONTENT_AUTHORITY);
	
	    private const String PATH_BLOCKS = "blocks";
	    private const String PATH_AT = "at";
	    private const String PATH_BETWEEN = "between";
	    private const String PATH_TRACKS = "tracks";
	    private const String PATH_ROOMS = "rooms";
	    private const String PATH_SESSIONS = "sessions";
	    private const String PATH_STARRED = "starred";
	    private const String PATH_SPEAKERS = "speakers";
	    private const String PATH_VENDORS = "vendors";
	    private const String PATH_EXPORT = "export";
	    private const String PATH_SEARCH = "search";
	    private const String PATH_SEARCH_SUGGEST = "search_suggest_query";

		
	    /**
	     * Blocks are generic timeslots that {@link Sessions} and other related
	     * events fall into.
	     */
	    public class Blocks : BlocksColumns {
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_BLOCKS).Build();
	
	        public const String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.block";
	        public const String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.block";
	
	        /** Count of {@link Sessions} inside given block. */
	        public const String SESSIONS_COUNT = "sessions_count";
	
	        /**
	         * Flag indicating that at least one {@link Sessions#SESSION_ID} inside
	         * this block has {@link Sessions#SESSION_STARRED} set.
	         */
	        public const String CONTAINS_STARRED = "contains_starred";
	
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = BlocksColumns.BLOCK_START + " ASC, " + BlocksColumns.BLOCK_END + " ASC";
	
	        /** Build {@link Uri} for requested {@link #BLOCK_ID}. */
	        public static Uri BuildBlockUri(String blockId) {
	            return CONTENT_URI.BuildUpon().AppendPath(blockId).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Sessions} associated
	         * with the requested {@link #BLOCK_ID}.
	         */
	        public static Uri BuildSessionsUri(String blockId) {
	            return CONTENT_URI.BuildUpon().AppendPath(blockId).AppendPath(PATH_SESSIONS).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Blocks} that occur
	         * between the requested time boundaries.
	         */
	        public static Uri BuildBlocksBetweenDirUri(long startTime, long endTime) {
	            return CONTENT_URI.BuildUpon().AppendPath(PATH_BETWEEN).AppendPath(
					Java.Lang.String.ValueOf(startTime)).AppendPath(Java.Lang.String.ValueOf(endTime)).Build();
	        }
	
	        /** Read {@link #BLOCK_ID} from {@link Blocks} {@link Uri}. */
	        public static String GetBlockId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        /**
	         * Generate a {@link #BLOCK_ID} that will always match the requested
	         * {@link Blocks} details.
	         */
	        public static String GenerateBlockId(long startTime, long endTime) {
	            startTime /= DateUtils.SecondInMillis;
	            endTime /= DateUtils.SecondInMillis;
	            return ParserUtils.SanitizeId(startTime + "-" + endTime);
	        }
	    }
		
		
		/**
	     * Tracks are overall categories for {@link Sessions} and {@link Vendors},
	     * such as "Android" or "Enterprise."
	     */
	    public class Tracks : TracksColumns {
	        public static Uri CONTENT_URI =
	                BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_TRACKS).Build();
	
	        public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.track";
	        public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.track";
	
	        /** Count of {@link Sessions} inside given track. */
	        public static String SESSIONS_COUNT = "sessions_count";
	        /** Count of {@link Vendors} inside given track. */
	        public static String VENDORS_COUNT = "vendors_count";
	
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = TracksColumns.TRACK_NAME + " ASC";
	
	        /** "All tracks" ID. */
	        public static String ALL_TRACK_ID = "all";
			
			public static String _ID = "_id";
	
	        /** Build {@link Uri} for requested {@link #TRACK_ID}. */
	        public static Uri BuildTrackUri(String trackId) {
	            return CONTENT_URI.BuildUpon().AppendPath(trackId).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Sessions} associated
	         * with the requested {@link #TRACK_ID}.
	         */
	        public static Uri BuildSessionsUri(String trackId) {
	            return CONTENT_URI.BuildUpon().AppendPath(trackId).AppendPath(PATH_SESSIONS).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Vendors} associated with
	         * the requested {@link #TRACK_ID}.
	         */
	        public static Uri BuildVendorsUri(String trackId) {
	            return CONTENT_URI.BuildUpon().AppendPath(trackId).AppendPath(PATH_VENDORS).Build();
	        }
	
	        /** Read {@link #TRACK_ID} from {@link Tracks} {@link Uri}. */
	        public static String GetTrackId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        /**
	         * Generate a {@link #TRACK_ID} that will always match the requested
	         * {@link Tracks} details.
	         */
	        public static String GenerateTrackId(String title) {
	            return ParserUtils.SanitizeId(title);
	        }
	    }
		
		/**
	     * Rooms are physical locations at the conference venue.
	     */
	    public class Rooms : RoomsColumns {
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_ROOMS).Build();
	
	        public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.room";
	        public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.room";
	
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = RoomsColumns.ROOM_FLOOR + " ASC, " + RoomsColumns.ROOM_NAME + " COLLATE NOCASE ASC";
	
	        /** Build {@link Uri} for requested {@link #ROOM_ID}. */
	        public static Uri BuildRoomUri(String roomId) {
	            return CONTENT_URI.BuildUpon().AppendPath(roomId).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Sessions} associated
	         * with the requested {@link #ROOM_ID}.
	         */
	        public static Uri BuildSessionsDirUri(String roomId) {
	            return CONTENT_URI.BuildUpon().AppendPath(roomId).AppendPath(PATH_SESSIONS).Build();
	        }
	
	        /** Read {@link #ROOM_ID} from {@link Rooms} {@link Uri}. */
	        public static String GetRoomId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        /**
	         * Generate a {@link #ROOM_ID} that will always match the requested
	         * {@link Rooms} details.
	         */
	        public static String GenerateRoomId(String room) {
	            return ParserUtils.SanitizeId(room);
	        }
	    }
		
		/**
	     * Each session is a block of time that has a {@link Tracks}, a
	     * {@link Rooms}, and zero or more {@link Speakers}.
	     */
	    public class Sessions : SessionsColumns//, BlocksColumns, RoomsColumns, SyncColumns, BaseColumns 
		{
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_SESSIONS).Build();
	        public static Uri CONTENT_STARRED_URI = CONTENT_URI.BuildUpon().AppendPath(PATH_STARRED).Build();
	
	        public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.session";
	        public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.session";
	
	        public static String BLOCK_ID = "block_id";
	        public static String ROOM_ID = "room_id";

			public static String _ID = "_id";
	
	        public static String SEARCH_SNIPPET = "search_snippet";

	        // TODO: shortcut primary track to offer sub-sorting here
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = BlocksColumns.BLOCK_START + " ASC," + SessionsColumns.SESSION_TITLE + " COLLATE NOCASE ASC";
	
	        /** Build {@link Uri} for requested {@link #SESSION_ID}. */
	        public static Uri BuildSessionUri(String sessionId) {
	            return CONTENT_URI.BuildUpon().AppendPath(sessionId).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Speakers} associated
	         * with the requested {@link #SESSION_ID}.
	         */
	        public static Uri BuildSpeakersDirUri(String sessionId) {
	            return CONTENT_URI.BuildUpon().AppendPath(sessionId).AppendPath(PATH_SPEAKERS).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Tracks} associated with
	         * the requested {@link #SESSION_ID}.
	         */
	        public static Uri BuildTracksDirUri(String sessionId) {
	            return CONTENT_URI.BuildUpon().AppendPath(sessionId).AppendPath(PATH_TRACKS).Build();
	        }
	
	        public static Uri BuildSessionsAtDirUri(long time) {
	            return CONTENT_URI.BuildUpon().AppendPath(PATH_AT).AppendPath(Java.Lang.String.ValueOf(time)).Build();
	        }
	
	        public static Uri BuildSearchUri(String query) {
	            return CONTENT_URI.BuildUpon().AppendPath(PATH_SEARCH).AppendPath(query).Build();
	        }
	
	        public static bool IsSearchUri(Uri uri) {
	            
				var pathSegments = uri.PathSegments;
	            return pathSegments.Count >= 2 && PATH_SEARCH.Equals(pathSegments[1]);
	        }
	
	        /** Read {@link #SESSION_ID} from {@link Sessions} {@link Uri}. */
	        public static String GetSessionId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        public static String GetSearchQuery(Uri uri) {
	            return uri.PathSegments[2];
	        }
	
	        /**
	         * Generate a {@link #SESSION_ID} that will always match the requested
	         * {@link Sessions} details.
	         */
	        public static String GenerateSessionId(String title) {
	            return ParserUtils.SanitizeId(title);
	        }
	    }
		
		/**
	     * Speakers are individual people that lead {@link Sessions}.
	     */
	    public class Speakers : SpeakersColumns//, SyncColumns, BaseColumns 
		{
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_SPEAKERS).Build();
	
	        public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.speaker";
	        public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.speaker";
	
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = SpeakersColumns.SPEAKER_NAME + " COLLATE NOCASE ASC";
	
			public static String _ID = "_id";
			
	        /** Build {@link Uri} for requested {@link #SPEAKER_ID}. */
	        public static Uri BuildSpeakerUri(String speakerId) {
	            return CONTENT_URI.BuildUpon().AppendPath(speakerId).Build();
	        }
	
	        /**
	         * Build {@link Uri} that references any {@link Sessions} associated
	         * with the requested {@link #SPEAKER_ID}.
	         */
	        public static Uri BuildSessionsDirUri(String speakerId) {
	            return CONTENT_URI.BuildUpon().AppendPath(speakerId).AppendPath(PATH_SESSIONS).Build();
	        }
	
	        /** Read {@link #SPEAKER_ID} from {@link Speakers} {@link Uri}. */
	        public static String GetSpeakerId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        /**
	         * Generate a {@link #SPEAKER_ID} that will always match the requested
	         * {@link Speakers} details.
	         */
	        public static String GenerateSpeakerId(String speakerLdap) {
	            return ParserUtils.SanitizeId(speakerLdap);
	        }
	    }
		
		/**
	     * Each vendor is a company appearing at the conference that may be
	     * associated with a specific {@link Tracks}.
	     */
	    public class Vendors : VendorsColumns {
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_VENDORS).Build();
	        public static Uri CONTENT_STARRED_URI = CONTENT_URI.BuildUpon().AppendPath(PATH_STARRED).Build();
	
	        public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.iosched.vendor";
	        public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.iosched.vendor";
	
	        /** {@link Tracks#TRACK_ID} that this vendor belongs to. */
	        public static String TRACK_ID = "track_id";
			public static String _ID = "_id";
	
	        public static String SEARCH_SNIPPET = "search_snippet";
	
	        /** Default "ORDER BY" clause. */
	        public static String DEFAULT_SORT = VendorsColumns.VENDOR_NAME + " COLLATE NOCASE ASC";
	
	        /** Build {@link Uri} for requested {@link #VENDOR_ID}. */
	        public static Uri BuildVendorUri(String vendorId) {
	            return CONTENT_URI.BuildUpon().AppendPath(vendorId).Build();
	        }
	
	        public static Uri BuildSearchUri(String query) {
	            return CONTENT_URI.BuildUpon().AppendPath(PATH_SEARCH).AppendPath(query).Build();
	        }
	
	        public static bool IsSearchUri(Uri uri) {
	            var pathSegments = uri.PathSegments;
	            return pathSegments.Count >= 2 && PATH_SEARCH.Equals(pathSegments[1]);
	        }
	
	        /** Read {@link #VENDOR_ID} from {@link Vendors} {@link Uri}. */
	        public static String GetVendorId(Uri uri) {
	            return uri.PathSegments[1];
	        }
	
	        public static String GetSearchQuery(Uri uri) {
	            return uri.PathSegments[2];
	        }
	
	        /**
	         * Generate a {@link #VENDOR_ID} that will always match the requested
	         * {@link Vendors} details.
	         */
	        public static String GenerateVendorId(String companyLogo) {
	            return ParserUtils.SanitizeId(companyLogo);
	        }
	    }
		
		public class SearchSuggest {
	        public static Uri CONTENT_URI = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_SEARCH_SUGGEST).Build();
	
	        public static String DEFAULT_SORT = SearchManager.SuggestColumnText1 + " COLLATE NOCASE ASC";
	    }
	
	}
}

