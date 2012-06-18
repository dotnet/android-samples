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
using Android.Database.Sqlite;
using Android.Provider;
using Android.Util;

namespace MonoIO
{
	public class ScheduleDatabase : SQLiteOpenHelper
	{
		private static String TAG = "ScheduleDatabase";
	
	    private static String DATABASE_NAME = "schedule.db";
	
	    // NOTE: carefully update onUpgrade() when bumping database versions to make
	    // sure user data is saved.
	
	    protected const int VER_LAUNCH = 21;
	    protected const int VER_SESSION_FEEDBACK_URL = 22;
	    private static int VER_SESSION_NOTES_URL_SLUG = 23;
	
	    private static int DATABASE_VERSION = VER_SESSION_NOTES_URL_SLUG;
		
		public class Tables {
	        public static String BLOCKS = "blocks";
	        public static String TRACKS = "tracks";
	        public static String ROOMS = "rooms";
	        public static String SESSIONS = "sessions";
	        public static String SPEAKERS = "speakers";
	        public static String SESSIONS_SPEAKERS = "sessions_speakers";
	        public static String SESSIONS_TRACKS = "sessions_tracks";
	        public static String VENDORS = "vendors";
	
	        public static String SESSIONS_SEARCH = "sessions_search";
	        public static String VENDORS_SEARCH = "vendors_search";
	
	        public static String SEARCH_SUGGEST = "search_suggest";
	
	        public static String SESSIONS_JOIN_BLOCKS_ROOMS = "sessions "
	                + "LEFT OUTER JOIN blocks ON sessions.block_id=blocks.block_id "
	                + "LEFT OUTER JOIN rooms ON sessions.room_id=rooms.room_id";
	
	        public static String VENDORS_JOIN_TRACKS = "vendors "
	                + "LEFT OUTER JOIN tracks ON vendors.track_id=tracks.track_id";
	
	        public static String SESSIONS_SPEAKERS_JOIN_SPEAKERS = "sessions_speakers "
	                + "LEFT OUTER JOIN speakers ON sessions_speakers.speaker_id=speakers.speaker_id";
	
	        public static String SESSIONS_SPEAKERS_JOIN_SESSIONS_BLOCKS_ROOMS = "sessions_speakers "
	                + "LEFT OUTER JOIN sessions ON sessions_speakers.session_id=sessions.session_id "
	                + "LEFT OUTER JOIN blocks ON sessions.block_id=blocks.block_id "
	                + "LEFT OUTER JOIN rooms ON sessions.room_id=rooms.room_id";
	
	        public static String SESSIONS_TRACKS_JOIN_TRACKS = "sessions_tracks "
	                + "LEFT OUTER JOIN tracks ON sessions_tracks.track_id=tracks.track_id";
	
	        public static String SESSIONS_TRACKS_JOIN_SESSIONS_BLOCKS_ROOMS = "sessions_tracks "
	                + "LEFT OUTER JOIN sessions ON sessions_tracks.session_id=sessions.session_id "
	                + "LEFT OUTER JOIN blocks ON sessions.block_id=blocks.block_id "
	                + "LEFT OUTER JOIN rooms ON sessions.room_id=rooms.room_id";
	
	        public static String SESSIONS_SEARCH_JOIN_SESSIONS_BLOCKS_ROOMS = "sessions_search "
	                + "LEFT OUTER JOIN sessions ON sessions_search.session_id=sessions.session_id "
	                + "LEFT OUTER JOIN blocks ON sessions.block_id=blocks.block_id "
	                + "LEFT OUTER JOIN rooms ON sessions.room_id=rooms.room_id";
	
	        public static String VENDORS_SEARCH_JOIN_VENDORS_TRACKS = "vendors_search "
	                + "LEFT OUTER JOIN vendors ON vendors_search.vendor_id=vendors.vendor_id "
	                + "LEFT OUTER JOIN tracks ON vendors.track_id=tracks.track_id";
	
	    }
		
		protected class Triggers {
	        public static String SESSIONS_SEARCH_INSERT = "sessions_search_insert";
	        public static String SESSIONS_SEARCH_DELETE = "sessions_search_delete";
	        public static String SESSIONS_SEARCH_UPDATE = "sessions_search_update";
	
	        public static String VENDORS_SEARCH_INSERT = "vendors_search_insert";
	        public static String VENDORS_SEARCH_DELETE = "vendors_search_delete";
	    }
	
	    public class SessionsSpeakers {
	        public static String SESSION_ID = "session_id";
	        public static String SPEAKER_ID = "speaker_id";
	    }
	
	    public class SessionsTracks {
	        public static String SESSION_ID = "session_id";
	        public static String TRACK_ID = "track_id";
	    }
	
	    public class SessionsSearchColumns {
	        public static String SESSION_ID = "session_id";
	        public static String BODY = "body";
	    }
	
	    public class VendorsSearchColumns {
	        public static String VENDOR_ID = "vendor_id";
	        public static String BODY = "body";
	    }
		
		/** Fully-qualified field names. */
	    private class Qualified {
	        public static String SESSIONS_SEARCH_SESSION_ID = Tables.SESSIONS_SEARCH + "."
	                + SessionsSearchColumns.SESSION_ID;
	        public static String VENDORS_SEARCH_VENDOR_ID = Tables.VENDORS_SEARCH + "."
	                + VendorsSearchColumns.VENDOR_ID;
	
	        public static String SESSIONS_SEARCH = Tables.SESSIONS_SEARCH + "(" + SessionsSearchColumns.SESSION_ID
	                + "," + SessionsSearchColumns.BODY + ")";
	        public static String VENDORS_SEARCH = Tables.VENDORS_SEARCH + "(" + VendorsSearchColumns.VENDOR_ID + ","
	                + VendorsSearchColumns.BODY + ")";
	    }
	
	    /** {@code REFERENCES} clauses. */
	    private class References {
	        public static String BLOCK_ID = "REFERENCES " + Tables.BLOCKS + "(" + ScheduleContract.Blocks.BLOCK_ID + ")";
	        public static String TRACK_ID = "REFERENCES " + Tables.TRACKS + "(" + ScheduleContract.Tracks.TRACK_ID + ")";
	        public static String ROOM_ID = "REFERENCES " + Tables.ROOMS + "(" + ScheduleContract.Rooms.ROOM_ID + ")";
	        public static String SESSION_ID = "REFERENCES " + Tables.SESSIONS + "(" + ScheduleContract.Sessions.SESSION_ID + ")";
	        public static String SPEAKER_ID = "REFERENCES " + Tables.SPEAKERS + "(" + ScheduleContract.Speakers.SPEAKER_ID + ")";
	        public static String VENDOR_ID = "REFERENCES " + Tables.VENDORS + "(" + ScheduleContract.Vendors.VENDOR_ID + ")";
	    }
		
		private class Subquery {
	        /**
	         * Subquery used to build the {@link SessionsSearchColumns#BODY} string
	         * used for indexing {@link Sessions} content.
	         */
	        public static String SESSIONS_BODY = "(new." + ScheduleContract.Sessions.SESSION_TITLE
	                + "||'; '||new." + ScheduleContract.Sessions.SESSION_ABSTRACT
	                + "||'; '||" + "coalesce(new." + ScheduleContract.Sessions.SESSION_KEYWORDS + ", '')"
	                + ")";
	
	        /**
	         * Subquery used to build the {@link VendorsSearchColumns#BODY} string
	         * used for indexing {@link Vendors} content.
	         */
	        public static String VENDORS_BODY = "(new." + ScheduleContract.Vendors.VENDOR_NAME
	                + "||'; '||new." + ScheduleContract.Vendors.VENDOR_DESC
	                + "||'; '||new." + ScheduleContract.Vendors.VENDOR_PRODUCT_DESC + ")";
	    }
	
	    public ScheduleDatabase(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
		{
	    }
		
		public override void OnCreate (SQLiteDatabase db)
		{
			db.ExecSQL("CREATE TABLE " + Tables.BLOCKS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.BlocksColumns.BLOCK_ID + " TEXT NOT NULL,"
	                + ScheduleContract.BlocksColumns.BLOCK_TITLE + " TEXT NOT NULL,"
	                + ScheduleContract.BlocksColumns.BLOCK_START + " INTEGER NOT NULL,"
	                + ScheduleContract.BlocksColumns.BLOCK_END + " INTEGER NOT NULL,"
	                + ScheduleContract.BlocksColumns.BLOCK_TYPE + " TEXT,"
	                + "UNIQUE (" + ScheduleContract.BlocksColumns.BLOCK_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.TRACKS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.TracksColumns.TRACK_ID + " TEXT NOT NULL,"
	                + ScheduleContract.TracksColumns.TRACK_NAME + " TEXT,"
	                + ScheduleContract.TracksColumns.TRACK_COLOR + " INTEGER,"
	                + ScheduleContract.TracksColumns.TRACK_ABSTRACT + " TEXT,"
	                + "UNIQUE (" + ScheduleContract.TracksColumns.TRACK_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.ROOMS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.RoomsColumns.ROOM_ID + " TEXT NOT NULL,"
	                + ScheduleContract.RoomsColumns.ROOM_NAME + " TEXT,"
	                + ScheduleContract.RoomsColumns.ROOM_FLOOR + " TEXT,"
	                + "UNIQUE (" + ScheduleContract.RoomsColumns.ROOM_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.SESSIONS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.SyncColumns.UPDATED + " INTEGER NOT NULL,"
	                + ScheduleContract.SessionsColumns.SESSION_ID + " TEXT NOT NULL,"
	                + ScheduleContract.Sessions.BLOCK_ID + " TEXT " + References.BLOCK_ID + ","
	                + ScheduleContract.Sessions.ROOM_ID + " TEXT " + References.ROOM_ID + ","
	                + ScheduleContract.SessionsColumns.SESSION_LEVEL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_TITLE + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_ABSTRACT + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_REQUIREMENTS + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_KEYWORDS + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_HASHTAG + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_SLUG + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_MODERATOR_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_YOUTUBE_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_PDF_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_FEEDBACK_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_NOTES_URL + " TEXT,"
	                + ScheduleContract.SessionsColumns.SESSION_STARRED + " INTEGER NOT NULL DEFAULT 0,"
	                + "UNIQUE (" + ScheduleContract.SessionsColumns.SESSION_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.SPEAKERS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.SyncColumns.UPDATED + " INTEGER NOT NULL,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_ID + " TEXT NOT NULL,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_NAME + " TEXT,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_IMAGE_URL + " TEXT,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_COMPANY + " TEXT,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_ABSTRACT + " TEXT,"
	                + ScheduleContract.SpeakersColumns.SPEAKER_URL+ " TEXT,"
	                + "UNIQUE (" + ScheduleContract.SpeakersColumns.SPEAKER_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.SESSIONS_SPEAKERS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + SessionsSpeakers.SESSION_ID + " TEXT NOT NULL " + References.SESSION_ID + ","
	                + SessionsSpeakers.SPEAKER_ID + " TEXT NOT NULL " + References.SPEAKER_ID + ","
	                + "UNIQUE (" + SessionsSpeakers.SESSION_ID + ","
	                        + SessionsSpeakers.SPEAKER_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.SESSIONS_TRACKS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + SessionsTracks.SESSION_ID + " TEXT NOT NULL " + References.SESSION_ID + ","
	                + SessionsTracks.TRACK_ID + " TEXT NOT NULL " + References.TRACK_ID + ","
	                + "UNIQUE (" + SessionsTracks.SESSION_ID + ","
	                        + SessionsTracks.TRACK_ID + ") ON CONFLICT REPLACE)");
	
	        db.ExecSQL("CREATE TABLE " + Tables.VENDORS + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + ScheduleContract.SyncColumns.UPDATED + " INTEGER NOT NULL,"
	                + ScheduleContract.VendorsColumns.VENDOR_ID + " TEXT NOT NULL,"
	                + ScheduleContract.Vendors.TRACK_ID + " TEXT " + References.TRACK_ID + ","
	                + ScheduleContract.VendorsColumns.VENDOR_NAME + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_LOCATION + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_DESC + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_URL + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_PRODUCT_DESC + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_LOGO_URL + " TEXT,"
	                + ScheduleContract.VendorsColumns.VENDOR_STARRED + " INTEGER,"
	                + "UNIQUE (" + ScheduleContract.VendorsColumns.VENDOR_ID + ") ON CONFLICT REPLACE)");
	
	        CreateSessionsSearch(db);
	        CreateVendorsSearch(db);
	
	        db.ExecSQL("CREATE TABLE " + Tables.SEARCH_SUGGEST + " ("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + SearchManager.SuggestColumnText1 + " TEXT NOT NULL)");
		}
		
		/**
	     * Create triggers that automatically build {@link Tables#SESSIONS_SEARCH}
	     * as values are changed in {@link Tables#SESSIONS}.
	     */
	    private void CreateSessionsSearch(SQLiteDatabase db) {
	        // Using the "porter" tokenizer for simple stemming, so that
	        // "frustration" matches "frustrated."
	
	        db.ExecSQL("CREATE VIRTUAL TABLE " + Tables.SESSIONS_SEARCH + " USING fts3("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + SessionsSearchColumns.BODY + " TEXT NOT NULL,"
	                + SessionsSearchColumns.SESSION_ID
	                        + " TEXT NOT NULL " + References.SESSION_ID + ","
	                + "UNIQUE (" + SessionsSearchColumns.SESSION_ID + ") ON CONFLICT REPLACE,"
	                + "tokenize=porter)");
	
	        // TODO: handle null fields in body, which cause trigger to fail
	        // TODO: implement update trigger, not currently exercised
	
	        db.ExecSQL("CREATE TRIGGER " + Triggers.SESSIONS_SEARCH_INSERT + " AFTER INSERT ON "
	                + Tables.SESSIONS + " BEGIN INSERT INTO " + Qualified.SESSIONS_SEARCH + " "
	                + " VALUES(new." + ScheduleContract.Sessions.SESSION_ID + ", " + Subquery.SESSIONS_BODY + ");"
	                + " END;");
	
	        db.ExecSQL("CREATE TRIGGER " + Triggers.SESSIONS_SEARCH_DELETE + " AFTER DELETE ON "
	                + Tables.SESSIONS + " BEGIN DELETE FROM " + Tables.SESSIONS_SEARCH + " "
	                + " WHERE " + Qualified.SESSIONS_SEARCH_SESSION_ID + "=old." + ScheduleContract.Sessions.SESSION_ID
	                + ";" + " END;");
	
	        db.ExecSQL("CREATE TRIGGER " + Triggers.SESSIONS_SEARCH_UPDATE 
	                + " AFTER UPDATE ON " + Tables.SESSIONS
	                + " BEGIN UPDATE sessions_search SET " + SessionsSearchColumns.BODY  + " = "
	                + Subquery.SESSIONS_BODY + " WHERE session_id = old.session_id"
	                + "; END;");
	
	    }
		
		/**
	     * Create triggers that automatically build {@link Tables#VENDORS_SEARCH} as
	     * values are changed in {@link Tables#VENDORS}.
	     */
	    private void CreateVendorsSearch(SQLiteDatabase db) {
	        // Using the "porter" tokenizer for simple stemming, so that
	        // "frustration" matches "frustrated."
	
	        db.ExecSQL("CREATE VIRTUAL TABLE " + Tables.VENDORS_SEARCH + " USING fts3("
	                + BaseColumns.Id + " INTEGER PRIMARY KEY AUTOINCREMENT,"
	                + VendorsSearchColumns.BODY + " TEXT NOT NULL,"
	                + VendorsSearchColumns.VENDOR_ID
	                        + " TEXT NOT NULL " + References.VENDOR_ID + ","
	                + "UNIQUE (" + VendorsSearchColumns.VENDOR_ID + ") ON CONFLICT REPLACE,"
	                + "tokenize=porter)");
	
	        // TODO: handle null fields in body, which cause trigger to fail
	        // TODO: implement update trigger, not currently exercised
	
	        db.ExecSQL("CREATE TRIGGER " + Triggers.VENDORS_SEARCH_INSERT + " AFTER INSERT ON "
	                + Tables.VENDORS + " BEGIN INSERT INTO " + Qualified.VENDORS_SEARCH + " "
	                + " VALUES(new." + ScheduleContract.Vendors.VENDOR_ID + ", " + Subquery.VENDORS_BODY + ");"
	                + " END;");
	
	        db.ExecSQL("CREATE TRIGGER " + Triggers.VENDORS_SEARCH_DELETE + " AFTER DELETE ON "
	                + Tables.VENDORS + " BEGIN DELETE FROM " + Tables.VENDORS_SEARCH + " "
	                + " WHERE " + Qualified.VENDORS_SEARCH_VENDOR_ID + "=old." + ScheduleContract.Vendors.VENDOR_ID
	                + ";" + " END;");
	
	    }
		
		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			Log.Debug(TAG, "onUpgrade() from " + oldVersion + " to " + newVersion);

	        // NOTE: This switch statement is designed to handle cascading database
	        // updates, starting at the current version and falling through to all
	        // future upgrade cases. Only use "break;" when you want to drop and
	        // recreate the entire database.
	        int version = oldVersion;
	
	        switch (version) {
	            case VER_LAUNCH:
	                // Version 22 added column for session feedback URL.
	                db.ExecSQL("ALTER TABLE " + Tables.SESSIONS + " ADD COLUMN "
	                        + ScheduleContract.SessionsColumns.SESSION_FEEDBACK_URL + " TEXT");
	                version = VER_SESSION_FEEDBACK_URL;
					goto case VER_SESSION_FEEDBACK_URL;
	
	            case VER_SESSION_FEEDBACK_URL:
	                // Version 23 added columns for session official notes URL and slug.
	                db.ExecSQL("ALTER TABLE " + Tables.SESSIONS + " ADD COLUMN "
	                        + ScheduleContract.SessionsColumns.SESSION_NOTES_URL + " TEXT");
	                db.ExecSQL("ALTER TABLE " + Tables.SESSIONS + " ADD COLUMN "
	                        + ScheduleContract.SessionsColumns.SESSION_SLUG + " TEXT");
	                version = VER_SESSION_NOTES_URL_SLUG;
					break;
	        }
	
	        Log.Debug(TAG, "after upgrade logic, at version " + version);
	        if (version != DATABASE_VERSION) {
	            Log.Warn(TAG, "Destroying old data during upgrade");
	
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.BLOCKS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.TRACKS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.ROOMS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SESSIONS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SPEAKERS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SESSIONS_SPEAKERS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SESSIONS_TRACKS);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.VENDORS);
	
	            db.ExecSQL("DROP TRIGGER IF EXISTS " + Triggers.SESSIONS_SEARCH_INSERT);
	            db.ExecSQL("DROP TRIGGER IF EXISTS " + Triggers.SESSIONS_SEARCH_DELETE);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SESSIONS_SEARCH);
	
	            db.ExecSQL("DROP TRIGGER IF EXISTS " + Triggers.VENDORS_SEARCH_INSERT);
	            db.ExecSQL("DROP TRIGGER IF EXISTS " + Triggers.VENDORS_SEARCH_DELETE);
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.VENDORS_SEARCH);
	
	            db.ExecSQL("DROP TABLE IF EXISTS " + Tables.SEARCH_SUGGEST);
	
	            OnCreate(db);
	        }
		}
	}
}

