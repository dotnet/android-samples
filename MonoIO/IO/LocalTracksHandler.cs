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
using Android.Graphics;

namespace MonoIO
{
	public class LocalTracksHandler : XmlHandler
	{
		public LocalTracksHandler() : base(ScheduleContract.CONTENT_AUTHORITY)
		{
		}
		
		public override List<ContentProviderOperation> Parse (XDocument input, ContentResolver resolver)
		{
			List<ContentProviderOperation> batch = new List<ContentProviderOperation>();
	
			batch.Add(ContentProviderOperation.NewDelete(ScheduleContract.Tracks.CONTENT_URI).Build());
			
			var tracks = from x in input.Descendants( "track" )
							select new TrackXml
							{
								Name = x.Element("name").Value,
								Color = Color.ParseColor(x.Element("color").Value),
								Abstract = x.Element("abstract").Value
							};
		
			foreach(var item in tracks)
				batch.Add(ParseTracks(item));
	
	        return batch;
		}
		
		public static ContentProviderOperation ParseTracks(TrackXml input)
		{
			ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ScheduleContract.Tracks.CONTENT_URI);
        	
			input.TrackId = ParserUtils.SanitizeId(input.Name);
			
			builder.WithValue(ScheduleContract.Tracks.TRACK_ID, input.TrackId);
			builder.WithValue(ScheduleContract.Tracks.TRACK_NAME, input.Name);
			builder.WithValue(ScheduleContract.Tracks.TRACK_COLOR, input.Color);
			builder.WithValue(ScheduleContract.Tracks.TRACK_ABSTRACT, input.Abstract);
			
			return builder.Build();
		}
		
		public class TrackXml
		{
			public string TrackId;
			public string Name;
	        public int Color;
	        public string Abstract;
		}
	}
}

