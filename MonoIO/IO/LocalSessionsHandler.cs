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
using System.Xml;
using System.Xml.Linq;
using Uri = Android.Net.Uri;
using Android.Database;

namespace MonoIO
{
	public class LocalSessionsHandler : XmlHandler
	{
		public LocalSessionsHandler() : base(ScheduleContract.CONTENT_AUTHORITY)
		{
		}
		
		public override List<ContentProviderOperation> Parse (XDocument input, ContentResolver resolver)
		{
			List<ContentProviderOperation> batch = new List<ContentProviderOperation>();
	
			var sessions = from x in input.Descendants( "session" )
						select new SessionXml
						{
							Start = ParserUtils.ParseTime(x.Element("start").Value),
							End = ParserUtils.ParseTime(x.Element("end").Value),
							RoomId = ScheduleContract.Rooms.GenerateRoomId(x.Element("room").Value),
							TrackId = ScheduleContract.Tracks.GenerateTrackId(x.Element("track").Value),
					 		Title = x.Element("title").Value,
							SessionId = x.Element("id") != null ? x.Element("id").Value : null,
							Abstract = x.Element("abstract") != null ? x.Element("abstract").Value : null
						};
		
			Console.WriteLine ("Sessions = " + sessions.Count());
			
			foreach(var item in sessions)
				ParseSessions(item, batch, resolver);
	
	        return batch;
		}
		
		public static void ParseSessions(SessionXml input, List<ContentProviderOperation> batch, ContentResolver resolver)
		{
			
			ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ScheduleContract.Sessions.CONTENT_URI);
        	builder.WithValue(ScheduleContract.SyncColumns.UPDATED, 0);
			
			if (input.SessionId == null)
	            input.SessionId = ScheduleContract.Sessions.GenerateSessionId(input.Title);
	
			if(input.RoomId != null)
				builder.WithValue(ScheduleContract.Sessions.ROOM_ID, input.RoomId);
			
			if(input.Abstract != null)
				builder.WithValue(ScheduleContract.Sessions.SESSION_ABSTRACT, input.Abstract);
			else
				builder.WithValue(ScheduleContract.Sessions.SESSION_ABSTRACT, "");
			
	        builder.WithValue(ScheduleContract.Sessions.SESSION_ID, input.SessionId);
	        builder.WithValue(ScheduleContract.Sessions.SESSION_TITLE, input.Title);
	
	        // Use empty strings to make sure SQLite search trigger has valid data
	        // for updating search index.
	        builder.WithValue(ScheduleContract.Sessions.SESSION_REQUIREMENTS, "");
	        builder.WithValue(ScheduleContract.Sessions.SESSION_KEYWORDS, "");
	
	        input.BlockId = ParserUtils.FindBlock(input.Title, input.Start, input.End);
	        builder.WithValue(ScheduleContract.Sessions.BLOCK_ID, input.BlockId);
	
	        // Propagate any existing starred value
	        Uri sessionUri = ScheduleContract.Sessions.BuildSessionUri(input.SessionId);
	        int starred = QuerySessionStarred(sessionUri, resolver);
	        if (starred != -1) {
	            builder.WithValue(ScheduleContract.Sessions.SESSION_STARRED, starred);
	        }
	
	        batch.Add(builder.Build());
	
	        if (input.TrackId != null) {
	            // TODO: support parsing multiple tracks per session
	            Uri sessionTracks = ScheduleContract.Sessions.BuildTracksDirUri(input.SessionId);
	            batch.Add(ContentProviderOperation.NewInsert(sessionTracks)
	                    .WithValue(ScheduleDatabase.SessionsTracks.SESSION_ID, input.SessionId)
	                    .WithValue(ScheduleDatabase.SessionsTracks.TRACK_ID, input.TrackId).Build());
	        }
		}
		
		public static int QuerySessionStarred(Uri uri, ContentResolver resolver) {
	        String[] projection = { ScheduleContract.Sessions.SESSION_STARRED };
	        ICursor cursor = resolver.Query(uri, projection, null, null, null);
	        try {
	            if (cursor.MoveToFirst()) {
	                return cursor.GetInt(0);
	            } else {
	                return -1;
	            }
	        } finally {
	            cursor.Close();
	        }
	    }
		
		public class SessionXml
		{
			public string SessionId;
	        public string TrackId;
	        public long Start;
	        public long End;
	        public string Title;
			public string Abstract;
			public string RoomId;
			public string BlockId;
		}
	}
}

