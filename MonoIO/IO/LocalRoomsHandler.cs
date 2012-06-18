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

namespace MonoIO
{
	public class LocalRoomsHandler : XmlHandler
	{
		public LocalRoomsHandler() : base(ScheduleContract.CONTENT_AUTHORITY)
		{
		}
		
		public override List<ContentProviderOperation> Parse (XDocument input, ContentResolver resolver)
		{
			List<ContentProviderOperation> batch = new List<ContentProviderOperation>();
	
			var rooms = from x in input.Descendants( "room" )
						select new RoomXml
						{
							Id = x.Element("id").Value,
							Name = x.Element("name").Value,
					 		Floor = x.Element("floor").Value
						};
		
			foreach(var item in rooms)
				ParseRoom(item, batch, resolver);
	
	        return batch;
		}
		
		public static void ParseRoom(RoomXml input, List<ContentProviderOperation> batch, ContentResolver resolver)
		{
			ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ScheduleContract.Rooms.CONTENT_URI);
	
	        builder.WithValue(ScheduleContract.Rooms.ROOM_ID, input.Id);
	        builder.WithValue(ScheduleContract.Rooms.ROOM_NAME, input.Name);
	        builder.WithValue(ScheduleContract.Rooms.ROOM_FLOOR, input.Floor);
			
			batch.Add(builder.Build());
		}
		
		public class RoomXml
		{
			public string Id;
			public string Name;
			public string Floor;
		}
	}
}

