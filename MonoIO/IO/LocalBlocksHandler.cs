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
	public class LocalBlocksHandler : XmlHandler
	{
		public LocalBlocksHandler() : base(ScheduleContract.CONTENT_AUTHORITY)
		{
		}
		
		public override List<ContentProviderOperation> Parse (XDocument input, ContentResolver resolver)
		{
			List<ContentProviderOperation> batch = new List<ContentProviderOperation>();
	
	        // Clear any existing static blocks, as they may have been updated.
	        String selection = ScheduleContract.Blocks.BLOCK_TYPE + "=? OR " + ScheduleContract.Blocks.BLOCK_TYPE +"=?";
	        String[] selectionArgs = {
	                ParserUtils.BlockTypeFood,
	                ParserUtils.BlockTypeOfficeHours
	        };
	        batch.Add(ContentProviderOperation.NewDelete(ScheduleContract.Blocks.CONTENT_URI).WithSelection(selection, selectionArgs).Build());

			var blocks = from x in input.Descendants( "block" )
						select new BlocksXml
						{
							StartTime = ParserUtils.ParseTime(x.Element("start").Value),
							EndTime = ParserUtils.ParseTime(x.Element("end").Value),
					 		Title = x.Element("title").Value,
							BlockType = x.Element("type") != null ? x.Element("type").Value : ""
						};
		
			foreach(var item in blocks)
				batch.Add(ParseBlock(item));
	
	        return batch;
		}
		
		public static ContentProviderOperation ParseBlock(BlocksXml input)
		{
			ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ScheduleContract.Blocks.CONTENT_URI);
			
			var blockId = ScheduleContract.Blocks.GenerateBlockId(input.StartTime, input.EndTime);
	
	        builder.WithValue(ScheduleContract.Blocks.BLOCK_ID, blockId);
	        builder.WithValue(ScheduleContract.Blocks.BLOCK_TITLE, input.Title);
	        builder.WithValue(ScheduleContract.Blocks.BLOCK_START, input.StartTime);
	        builder.WithValue(ScheduleContract.Blocks.BLOCK_END, input.EndTime);
	        builder.WithValue(ScheduleContract.Blocks.BLOCK_TYPE, input.BlockType);
			
			return builder.Build();
		}
		
		public class BlocksXml
		{
			public long StartTime = -1;
			public long EndTime = -1;
			public String Title;
			public string BlockType;
		}
	}
}

