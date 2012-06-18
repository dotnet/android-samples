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
using Android.Util;
using Java.Util;
using Android.Provider;
using Uri = Android.Net.Uri;
using Android.Database.Sqlite;

namespace MonoIO
{
	[ContentProvider(new [] {"monoio"}, Name = "monoio.ScheduleProvider", WritePermission = "monoio.permission.WRITE_SCHEDULE")]
	public class ScheduleProvider : ContentProvider
	{
		private static String TAG = "ScheduleProvider";
	    private static bool LOGV = Log.IsLoggable(TAG, LogPriority.Verbose);
	
	    private ScheduleDatabase mOpenHelper;
	
	    private static UriMatcher sUriMatcher = BuildUriMatcher();
	
	    private const int BLOCKS = 100;
	    private const int BLOCKS_BETWEEN = 101;
	    private const int BLOCKS_ID = 102;
	    private const int BLOCKS_ID_SESSIONS = 103;
	
	    private const int TRACKS = 200;
	    private const int TRACKS_ID = 201;
	    private const int TRACKS_ID_SESSIONS = 202;
	    private const int TRACKS_ID_VENDORS = 203;
	
	    private const int ROOMS = 300;
	    private const int ROOMS_ID = 301;
	    private const int ROOMS_ID_SESSIONS = 302;
	
	    private const int SESSIONS = 400;
	    private const int SESSIONS_STARRED = 401;
	    private const int SESSIONS_SEARCH = 402;
	    private const int SESSIONS_AT = 403;
	    private const int SESSIONS_ID = 404;
	    private const int SESSIONS_ID_SPEAKERS = 405;
	    private const int SESSIONS_ID_TRACKS = 406;
	
	    private const int SPEAKERS = 500;
	    private const int SPEAKERS_ID = 501;
	    private const int SPEAKERS_ID_SESSIONS = 502;
	
	    private const int VENDORS = 600;
	    private const int VENDORS_STARRED = 601;
	    private const int VENDORS_SEARCH = 603;
	    private const int VENDORS_ID = 604;
	
	    private const int SEARCH_SUGGEST = 800;
	
	    private static String MIME_XML = "text/xml";
		
		/**
	     * Build and return a {@link UriMatcher} that catches all {@link Uri}
	     * variations supported by this {@link ContentProvider}.
	     */
	    private static UriMatcher BuildUriMatcher() {
	        UriMatcher matcher = new UriMatcher(UriMatcher.NoMatch);
	        String authority = ScheduleContract.CONTENT_AUTHORITY;
	
	        matcher.AddURI(authority, "blocks", BLOCKS);
	        matcher.AddURI(authority, "blocks/between/*/*", BLOCKS_BETWEEN);
	        matcher.AddURI(authority, "blocks/*", BLOCKS_ID);
	        matcher.AddURI(authority, "blocks/*/sessions", BLOCKS_ID_SESSIONS);
	
	        matcher.AddURI(authority, "tracks", TRACKS);
	        matcher.AddURI(authority, "tracks/*", TRACKS_ID);
	        matcher.AddURI(authority, "tracks/*/sessions", TRACKS_ID_SESSIONS);
	        matcher.AddURI(authority, "tracks/*/vendors", TRACKS_ID_VENDORS);
	
	        matcher.AddURI(authority, "rooms", ROOMS);
	        matcher.AddURI(authority, "rooms/*", ROOMS_ID);
	        matcher.AddURI(authority, "rooms/*/sessions", ROOMS_ID_SESSIONS);
	
	        matcher.AddURI(authority, "sessions", SESSIONS);
	        matcher.AddURI(authority, "sessions/starred", SESSIONS_STARRED);
	        matcher.AddURI(authority, "sessions/search/*", SESSIONS_SEARCH);
	        matcher.AddURI(authority, "sessions/at/*", SESSIONS_AT);
	        matcher.AddURI(authority, "sessions/*", SESSIONS_ID);
	        matcher.AddURI(authority, "sessions/*/speakers", SESSIONS_ID_SPEAKERS);
	        matcher.AddURI(authority, "sessions/*/tracks", SESSIONS_ID_TRACKS);
	
	        matcher.AddURI(authority, "speakers", SPEAKERS);
	        matcher.AddURI(authority, "speakers/*", SPEAKERS_ID);
	        matcher.AddURI(authority, "speakers/*/sessions", SPEAKERS_ID_SESSIONS);
	
	        matcher.AddURI(authority, "vendors", VENDORS);
	        matcher.AddURI(authority, "vendors/starred", VENDORS_STARRED);
	        matcher.AddURI(authority, "vendors/search/*", VENDORS_SEARCH);
	        matcher.AddURI(authority, "vendors/*", VENDORS_ID);
	
	        matcher.AddURI(authority, "search_suggest_query", SEARCH_SUGGEST);
	
	        return matcher;
	    }
		
		public override bool OnCreate ()
		{
			Context context = Context;
	        mOpenHelper = new ScheduleDatabase(context);
	        return true;
		}
		
		public override string GetType (Android.Net.Uri uri)
		{
			int match = sUriMatcher.Match(uri);
	        switch (match) {
	            case BLOCKS:
	                return ScheduleContract.Blocks.CONTENT_TYPE;
	            case BLOCKS_BETWEEN:
	                return ScheduleContract.Blocks.CONTENT_TYPE;
	            case BLOCKS_ID:
	                return ScheduleContract.Blocks.CONTENT_ITEM_TYPE;
	            case BLOCKS_ID_SESSIONS:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case TRACKS:
	                return ScheduleContract.Tracks.CONTENT_TYPE;
	            case TRACKS_ID:
	                return ScheduleContract.Tracks.CONTENT_ITEM_TYPE;
	            case TRACKS_ID_SESSIONS:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case TRACKS_ID_VENDORS:
	                return ScheduleContract.Vendors.CONTENT_TYPE;
	            case ROOMS:
	                return ScheduleContract.Rooms.CONTENT_TYPE;
	            case ROOMS_ID:
	                return ScheduleContract.Rooms.CONTENT_ITEM_TYPE;
	            case ROOMS_ID_SESSIONS:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case SESSIONS:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case SESSIONS_STARRED:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case SESSIONS_SEARCH:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case SESSIONS_AT:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case SESSIONS_ID:
	                return ScheduleContract.Sessions.CONTENT_ITEM_TYPE;
	            case SESSIONS_ID_SPEAKERS:
	                return ScheduleContract.Speakers.CONTENT_TYPE;
	            case SESSIONS_ID_TRACKS:
	                return ScheduleContract.Tracks.CONTENT_TYPE;
	            case SPEAKERS:
	                return ScheduleContract.Speakers.CONTENT_TYPE;
	            case SPEAKERS_ID:
	                return ScheduleContract.Speakers.CONTENT_ITEM_TYPE;
	            case SPEAKERS_ID_SESSIONS:
	                return ScheduleContract.Sessions.CONTENT_TYPE;
	            case VENDORS:
	                return ScheduleContract.Vendors.CONTENT_TYPE;
	            case VENDORS_STARRED:
	                return ScheduleContract.Vendors.CONTENT_TYPE;
	            case VENDORS_SEARCH:
	                return ScheduleContract.Vendors.CONTENT_TYPE;
	            case VENDORS_ID:
	                return ScheduleContract.Vendors.CONTENT_ITEM_TYPE;
	            default:
	                throw new Exception("Unknown uri: " + uri);
	        }
		}
		
		public override Android.Database.ICursor Query (Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
		{
			if (LOGV) Log.Verbose(TAG, "query(uri=" + uri + ", proj=" + projection.ToString() + ")");
	        SQLiteDatabase db = mOpenHelper.ReadableDatabase;
	
	        int match = sUriMatcher.Match(uri);
	        switch (match) {
	            default: {
	                // Most cases are handled with simple SelectionBuilder
	                SelectionBuilder builder = BuildExpandedSelection(uri, match);
	                return builder.Where(selection, selectionArgs).Query(db, projection, sortOrder);
	            }
	            case SEARCH_SUGGEST: {
	                SelectionBuilder builder = new SelectionBuilder();
	
	                // Adjust incoming query to become SQL text match
	                selectionArgs[0] = selectionArgs[0] + "%";
	                builder.Table(ScheduleDatabase.Tables.SEARCH_SUGGEST);
	                builder.Where(selection, selectionArgs);
	                builder.Map(SearchManager.SuggestColumnQuery,
	                        SearchManager.SuggestColumnText1);
	
	                projection = new String[] { BaseColumns.Id, SearchManager.SuggestColumnText1,
	                        SearchManager.SuggestColumnQuery };
	
	                String limit = uri.GetQueryParameter(SearchManager.SuggestParameterLimit);
	                return builder.Query(db, projection, null, null, ScheduleContract.SearchSuggest.DEFAULT_SORT, limit);
	            }
	        }
		}
		
		public override Uri Insert (Uri uri, ContentValues values)
		{
			if (LOGV) Log.Verbose(TAG, "insert(uri=" + uri + ", values=" + values.ToString() + ")");
	        SQLiteDatabase db = mOpenHelper.WritableDatabase;
	        int match = sUriMatcher.Match(uri);
	        switch (match) {
	            case BLOCKS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.BLOCKS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Blocks.BuildBlockUri(values.GetAsString(ScheduleContract.Blocks.BLOCK_ID));
	            }
	            case TRACKS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.TRACKS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Tracks.BuildTrackUri(values.GetAsString(ScheduleContract.Tracks.TRACK_ID));
	            }
	            case ROOMS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.ROOMS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Rooms.BuildRoomUri(values.GetAsString(ScheduleContract.Rooms.ROOM_ID));
	            }
	            case SESSIONS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.SESSIONS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Sessions.BuildSessionUri(values.GetAsString(ScheduleContract.Sessions.SESSION_ID));
	            }
	            case SESSIONS_ID_SPEAKERS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.SESSIONS_SPEAKERS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Speakers.BuildSpeakerUri(values.GetAsString(ScheduleDatabase.SessionsSpeakers.SPEAKER_ID));
	            }
	            case SESSIONS_ID_TRACKS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.SESSIONS_TRACKS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Tracks.BuildTrackUri(values.GetAsString(ScheduleDatabase.SessionsTracks.TRACK_ID));
	            }
	            case SPEAKERS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.SPEAKERS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Speakers.BuildSpeakerUri(values.GetAsString(ScheduleContract.Speakers.SPEAKER_ID));
	            }
	            case VENDORS: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.VENDORS, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.Vendors.BuildVendorUri(values.GetAsString(ScheduleContract.Vendors.VENDOR_ID));
	            }
	            case SEARCH_SUGGEST: {
	                db.InsertOrThrow(ScheduleDatabase.Tables.SEARCH_SUGGEST, null, values);
	                Context.ContentResolver.NotifyChange(uri, null);
	                return ScheduleContract.SearchSuggest.CONTENT_URI;
	            }
	            default: {
	                throw new Exception("Unknown uri: " + uri);
	            }
	        }
		}
		
		public override int Update (Uri uri, ContentValues values, string selection, string[] selectionArgs)
		{
			if (LOGV) Log.Verbose(TAG, "update(uri=" + uri + ", values=" + values.ToString() + ")");
	        SQLiteDatabase db = mOpenHelper.WritableDatabase;
	        SelectionBuilder builder = BuildSimpleSelection(uri);
	        int retVal = builder.Where(selection, selectionArgs).Update(db, values);
	        Context.ContentResolver.NotifyChange(uri, null);
	        return retVal;
		}
		
		public override int Delete (Uri uri, string selection, string[] selectionArgs)
		{
			if (LOGV) Log.Verbose(TAG, "delete(uri=" + uri + ")");
	        SQLiteDatabase db = mOpenHelper.WritableDatabase;
	        SelectionBuilder builder = BuildSimpleSelection(uri);
	        int retVal = builder.Where(selection, selectionArgs).Delete(db);
	        Context.ContentResolver.NotifyChange(uri, null);
	        return retVal;
		}
		
		public override ContentProviderResult[] ApplyBatch (IList<ContentProviderOperation> operations)
		{
			SQLiteDatabase db = mOpenHelper.WritableDatabase;
	        db.BeginTransaction();
	        try {
	            int numOperations = operations.Count;
	            ContentProviderResult[] results = new ContentProviderResult[numOperations];
	            for (int i = 0; i < numOperations; i++) {
	                results[i] = operations[i].Apply(this, results, i);
	            }
	            db.SetTransactionSuccessful();
	            return results;
	        } finally {
	            db.EndTransaction();
	        }
		}
		
		/**
	     * Build a simple {@link SelectionBuilder} to match the requested
	     * {@link Uri}. This is usually enough to support {@link #insert},
	     * {@link #update}, and {@link #delete} operations.
	     */
	    private SelectionBuilder BuildSimpleSelection(Uri uri) {
	        SelectionBuilder builder = new SelectionBuilder();
	        int match = sUriMatcher.Match(uri);
	        switch (match) {
	            case BLOCKS: {
	                return builder.Table(ScheduleDatabase.Tables.BLOCKS);
	            }
	            case BLOCKS_ID: {
	                String blockId = ScheduleContract.Blocks.GetBlockId(uri);
	                return builder.Table(ScheduleDatabase.Tables.BLOCKS)
	                        .Where(ScheduleContract.Blocks.BLOCK_ID + "=?", blockId);
	            }
	            case TRACKS: {
	                return builder.Table(ScheduleDatabase.Tables.TRACKS);
	            }
	            case TRACKS_ID: {
	                String trackId = ScheduleContract.Tracks.GetTrackId(uri);
	                return builder.Table(ScheduleDatabase.Tables.TRACKS)
	                        .Where(ScheduleContract.Tracks.TRACK_ID + "=?", trackId);
	            }
	            case ROOMS: {
	                return builder.Table(ScheduleDatabase.Tables.ROOMS);
	            }
	            case ROOMS_ID: {
	                String roomId = ScheduleContract.Rooms.GetRoomId(uri);
	                return builder.Table(ScheduleDatabase.Tables.ROOMS)
	                        .Where(ScheduleContract.Rooms.ROOM_ID + "=?", roomId);
	            }
	            case SESSIONS: {
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS);
	            }
	            case SESSIONS_ID: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS)
	                        .Where(ScheduleContract.Sessions.SESSION_ID + "=?", sessionId);
	            }
	            case SESSIONS_ID_SPEAKERS: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_SPEAKERS)
	                        .Where(ScheduleContract.Sessions.SESSION_ID + "=?", sessionId);
	            }
	            case SESSIONS_ID_TRACKS: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_TRACKS)
	                        .Where(ScheduleContract.Sessions.SESSION_ID + "=?", sessionId);
	            }
	            case SPEAKERS: {
	                return builder.Table(ScheduleDatabase.Tables.SPEAKERS);
	            }
	            case SPEAKERS_ID: {
	                String speakerId = ScheduleContract.Speakers.GetSpeakerId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SPEAKERS)
	                        .Where(ScheduleContract.Speakers.SPEAKER_ID + "=?", speakerId);
	            }
	            case VENDORS: {
	                return builder.Table(ScheduleDatabase.Tables.VENDORS);
	            }
	            case VENDORS_ID: {
	                String vendorId = ScheduleContract.Vendors.GetVendorId(uri);
	                return builder.Table(ScheduleDatabase.Tables.VENDORS)
	                        .Where(ScheduleContract.Vendors.VENDOR_ID + "=?", vendorId);
	            }
	            case SEARCH_SUGGEST: {
	                return builder.Table(ScheduleDatabase.Tables.SEARCH_SUGGEST);
	            }
	            default: {
	                throw new Exception("Unknown uri: " + uri);
	            }
	        }
	    }
		
		/**
	     * Build an advanced {@link SelectionBuilder} to match the requested
	     * {@link Uri}. This is usually only used by {@link #query}, since it
	     * performs table joins useful for {@link Cursor} data.
	     */
	    private SelectionBuilder BuildExpandedSelection(Uri uri, int match) {
	        SelectionBuilder builder = new SelectionBuilder();
	        switch (match) {
	            case BLOCKS: {
	                return builder.Table(ScheduleDatabase.Tables.BLOCKS);
	            }
	            case BLOCKS_BETWEEN: {
	                var segments = uri.PathSegments;
	                String startTime = segments[2];
	                String endTime = segments[3];
	                return builder.Table(ScheduleDatabase.Tables.BLOCKS)
	                        .Map(ScheduleContract.Blocks.SESSIONS_COUNT, Subquery.BLOCK_SESSIONS_COUNT)
	                        .Map(ScheduleContract.Blocks.CONTAINS_STARRED, Subquery.BLOCK_CONTAINS_STARRED)
	                        .Where(ScheduleContract.Blocks.BLOCK_START + ">=?", startTime)
	                        .Where(ScheduleContract.Blocks.BLOCK_START + "<=?", endTime);
	            }
	            case BLOCKS_ID: {
	                String blockId = ScheduleContract.Blocks.GetBlockId(uri);
	                return builder.Table(ScheduleDatabase.Tables.BLOCKS)
	                        .Map(ScheduleContract.Blocks.SESSIONS_COUNT, Subquery.BLOCK_SESSIONS_COUNT)
	                        .Map(ScheduleContract.Blocks.CONTAINS_STARRED, Subquery.BLOCK_CONTAINS_STARRED)
	                        .Where(ScheduleContract.Blocks.BLOCK_ID + "=?", blockId);
	            }
	            case BLOCKS_ID_SESSIONS: {
	                String blockId = ScheduleContract.Blocks.GetBlockId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .Map(ScheduleContract.Blocks.SESSIONS_COUNT, Subquery.BLOCK_SESSIONS_COUNT)
	                        .Map(ScheduleContract.Blocks.CONTAINS_STARRED, Subquery.BLOCK_CONTAINS_STARRED)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.SESSION_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(Qualified.SESSIONS_BLOCK_ID + "=?", blockId);
	            }
	            case TRACKS: {
	                return builder.Table(ScheduleDatabase.Tables.TRACKS)
	                        .Map(ScheduleContract.Tracks.SESSIONS_COUNT, Subquery.TRACK_SESSIONS_COUNT)
	                        .Map(ScheduleContract.Tracks.VENDORS_COUNT, Subquery.TRACK_VENDORS_COUNT);
	            }
	            case TRACKS_ID: {
	                String trackId = ScheduleContract.Tracks.GetTrackId(uri);
	                return builder.Table(ScheduleDatabase.Tables.TRACKS)
	                        .Where(ScheduleContract.Tracks.TRACK_ID + "=?", trackId);
	            }
	            case TRACKS_ID_SESSIONS: {
	                String trackId = ScheduleContract.Tracks.GetTrackId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_TRACKS_JOIN_SESSIONS_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.SESSION_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(Qualified.SESSIONS_TRACKS_TRACK_ID + "=?", trackId);
	            }
	            case TRACKS_ID_VENDORS: {
	                String trackId = ScheduleContract.Tracks.GetTrackId(uri);
	                return builder.Table(ScheduleDatabase.Tables.VENDORS_JOIN_TRACKS)
	                        .MapToTable(ScheduleContract.Vendors._ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.TRACK_ID, ScheduleDatabase.Tables.VENDORS)
	                        .Where(Qualified.VENDORS_TRACK_ID + "=?", trackId);
	            }
	            case ROOMS: {
	                return builder.Table(ScheduleDatabase.Tables.ROOMS);
	            }
	            case ROOMS_ID: {
	                String roomId = ScheduleContract.Rooms.GetRoomId(uri);
	                return builder.Table(ScheduleDatabase.Tables.ROOMS)
	                        .Where(ScheduleContract.Rooms.ROOM_ID + "=?", roomId);
	            }
	            case ROOMS_ID_SESSIONS: {
	                String roomId = ScheduleContract.Rooms.GetRoomId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(Qualified.SESSIONS_ROOM_ID + "=?", roomId);
	            }
	            case SESSIONS: {
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS);
	            }
	            case SESSIONS_STARRED: {
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(ScheduleContract.Sessions.SESSION_STARRED + "=1", (string) null);
	            }
	            case SESSIONS_SEARCH: {
	                String query = ScheduleContract.Sessions.GetSearchQuery(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_SEARCH_JOIN_SESSIONS_BLOCKS_ROOMS)
	                        .Map(ScheduleContract.Sessions.SEARCH_SNIPPET, Subquery.SESSIONS_SNIPPET)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.SESSION_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(ScheduleDatabase.SessionsSearchColumns.BODY + " MATCH ?", query);
	            }
	            case SESSIONS_AT: {
	                var segments = uri.PathSegments;
	                String time = segments[2];
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(ScheduleContract.Sessions.BLOCK_START + "<=?", time)
	                        .Where(ScheduleContract.Sessions.BLOCK_END + ">=?", time);
	            }
	            case SESSIONS_ID: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_JOIN_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(Qualified.SESSIONS_SESSION_ID + "=?", sessionId);
	            }
	            case SESSIONS_ID_SPEAKERS: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_SPEAKERS_JOIN_SPEAKERS)
	                        .MapToTable(ScheduleContract.Speakers._ID, ScheduleDatabase.Tables.SPEAKERS)
	                        .MapToTable(ScheduleContract.Speakers.SPEAKER_ID, ScheduleDatabase.Tables.SPEAKERS)
	                        .Where(Qualified.SESSIONS_SPEAKERS_SESSION_ID + "=?", sessionId);
	            }
	            case SESSIONS_ID_TRACKS: {
	                String sessionId = ScheduleContract.Sessions.GetSessionId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_TRACKS_JOIN_TRACKS)
	                        .MapToTable(ScheduleContract.Tracks._ID, ScheduleDatabase.Tables.TRACKS)
	                        .MapToTable(ScheduleContract.Tracks.TRACK_ID, ScheduleDatabase.Tables.TRACKS)
	                        .Where(Qualified.SESSIONS_TRACKS_SESSION_ID + "=?", sessionId);
	            }
	            case SPEAKERS: {
	                return builder.Table(ScheduleDatabase.Tables.SPEAKERS);
	            }
	            case SPEAKERS_ID: {
	                String speakerId = ScheduleContract.Speakers.GetSpeakerId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SPEAKERS)
	                        .Where(ScheduleContract.Speakers.SPEAKER_ID + "=?", speakerId);
	            }
	            case SPEAKERS_ID_SESSIONS: {
	                String speakerId = ScheduleContract.Speakers.GetSpeakerId(uri);
	                return builder.Table(ScheduleDatabase.Tables.SESSIONS_SPEAKERS_JOIN_SESSIONS_BLOCKS_ROOMS)
	                        .MapToTable(ScheduleContract.Sessions._ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.SESSION_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.BLOCK_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .MapToTable(ScheduleContract.Sessions.ROOM_ID, ScheduleDatabase.Tables.SESSIONS)
	                        .Where(Qualified.SESSIONS_SPEAKERS_SPEAKER_ID + "=?", speakerId);
	            }
	            case VENDORS: {
	                return builder.Table(ScheduleDatabase.Tables.VENDORS_JOIN_TRACKS)
	                        .MapToTable(ScheduleContract.Vendors._ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.TRACK_ID, ScheduleDatabase.Tables.VENDORS);
	            }
	            case VENDORS_STARRED: {
	                return builder.Table(ScheduleDatabase.Tables.VENDORS_JOIN_TRACKS)
	                        .MapToTable(ScheduleContract.Vendors._ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.TRACK_ID, ScheduleDatabase.Tables.VENDORS)
	                        .Where(ScheduleContract.Vendors.VENDOR_STARRED + "=1", (string) null);
	            }
	            case VENDORS_SEARCH: {
	                String query = ScheduleContract.Vendors.GetSearchQuery(uri);
	                return builder.Table(ScheduleDatabase.Tables.VENDORS_SEARCH_JOIN_VENDORS_TRACKS)
	                        .Map(ScheduleContract.Vendors.SEARCH_SNIPPET, Subquery.VENDORS_SNIPPET)
	                        .MapToTable(ScheduleContract.Vendors._ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.VENDOR_ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.TRACK_ID, ScheduleDatabase.Tables.VENDORS)
	                        .Where(ScheduleDatabase.VendorsSearchColumns.BODY + " MATCH ?", query);
	            }
	            case VENDORS_ID: {
	                String vendorId = ScheduleContract.Vendors.GetVendorId(uri);
	                return builder.Table(ScheduleDatabase.Tables.VENDORS_JOIN_TRACKS)
	                        .MapToTable(ScheduleContract.Vendors._ID, ScheduleDatabase.Tables.VENDORS)
	                        .MapToTable(ScheduleContract.Vendors.TRACK_ID, ScheduleDatabase.Tables.VENDORS)
	                        .Where(ScheduleContract.Vendors.VENDOR_ID + "=?", vendorId);
	            }
	            default: {
	                throw new Exception("Unknown uri: " + uri);
	            }
	        }
	    }
		
		private class Subquery {
	        public static String BLOCK_SESSIONS_COUNT = "(SELECT COUNT(" + Qualified.SESSIONS_SESSION_ID + ") FROM "
	                + ScheduleDatabase.Tables.SESSIONS + " WHERE " + Qualified.SESSIONS_BLOCK_ID + "="
	                + Qualified.BLOCKS_BLOCK_ID + ")";
	
	        public static String BLOCK_CONTAINS_STARRED = "(SELECT MAX(" + Qualified.SESSIONS_STARRED + ") FROM "
	                + ScheduleDatabase.Tables.SESSIONS + " WHERE " + Qualified.SESSIONS_BLOCK_ID + "="
	                + Qualified.BLOCKS_BLOCK_ID + ")";
	
	        public static String TRACK_SESSIONS_COUNT = "(SELECT COUNT(" + Qualified.SESSIONS_TRACKS_SESSION_ID
	                + ") FROM " + ScheduleDatabase.Tables.SESSIONS_TRACKS + " WHERE "
	                + Qualified.SESSIONS_TRACKS_TRACK_ID + "=" + Qualified.TRACKS_TRACK_ID + ")";
	
	        public static String TRACK_VENDORS_COUNT = "(SELECT COUNT(" + Qualified.VENDORS_VENDOR_ID + ") FROM "
	                + ScheduleDatabase.Tables.VENDORS + " WHERE " + Qualified.VENDORS_TRACK_ID + "="
	                + Qualified.TRACKS_TRACK_ID + ")";
	
	        public static String SESSIONS_SNIPPET = "snippet(" + ScheduleDatabase.Tables.SESSIONS_SEARCH + ",'{','}','\u2026')";
	        public static String VENDORS_SNIPPET = "snippet(" + ScheduleDatabase.Tables.VENDORS_SEARCH + ",'{','}','\u2026')";
	    }
	
	    /**
	     * {@link ScheduleContract} fields that are fully qualified with a specific
	     * parent {@link Tables}. Used when needed to work around SQL ambiguity.
	     */
	    private class Qualified {
	        public static String SESSIONS_SESSION_ID = ScheduleDatabase.Tables.SESSIONS + "." + ScheduleContract.Sessions.SESSION_ID;
	        public static String SESSIONS_BLOCK_ID = ScheduleDatabase.Tables.SESSIONS + "." + ScheduleContract.Sessions.BLOCK_ID;
	        public static String SESSIONS_ROOM_ID = ScheduleDatabase.Tables.SESSIONS + "." + ScheduleContract.Sessions.ROOM_ID;
	
	        public static String SESSIONS_TRACKS_SESSION_ID = ScheduleDatabase.Tables.SESSIONS_TRACKS + "." + ScheduleDatabase.SessionsTracks.SESSION_ID;
	        public static String SESSIONS_TRACKS_TRACK_ID = ScheduleDatabase.Tables.SESSIONS_TRACKS + "." + ScheduleDatabase.SessionsTracks.TRACK_ID;
	
	        public static String SESSIONS_SPEAKERS_SESSION_ID = ScheduleDatabase.Tables.SESSIONS_SPEAKERS + "."
	                + ScheduleDatabase.SessionsSpeakers.SESSION_ID;
	        public static String SESSIONS_SPEAKERS_SPEAKER_ID = ScheduleDatabase.Tables.SESSIONS_SPEAKERS + "."
	                + ScheduleDatabase.SessionsSpeakers.SPEAKER_ID;
	
	        public static String VENDORS_VENDOR_ID = ScheduleDatabase.Tables.VENDORS + "." + ScheduleContract.Vendors.VENDOR_ID;
	        public static String VENDORS_TRACK_ID = ScheduleDatabase.Tables.VENDORS + "." + ScheduleContract.Vendors.TRACK_ID;
	
	        //@SuppressWarnings("hiding")
	        public static String SESSIONS_STARRED = ScheduleDatabase.Tables.SESSIONS + "." + ScheduleContract.Sessions.SESSION_STARRED;
	
	        public static String TRACKS_TRACK_ID = ScheduleDatabase.Tables.TRACKS + "." + ScheduleContract.Tracks.TRACK_ID;
	        public static String BLOCKS_BLOCK_ID = ScheduleDatabase.Tables.BLOCKS + "." + ScheduleContract.Blocks.BLOCK_ID;
	    }
	}
}

