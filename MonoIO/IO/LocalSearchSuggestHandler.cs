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
	public class LocalSearchSuggestHandler : XmlHandler
	{
		public LocalSearchSuggestHandler() : base(ScheduleContract.CONTENT_AUTHORITY)
		{
		}
		
		public override List<ContentProviderOperation> Parse (XDocument input, ContentResolver resolver)
		{
			List<ContentProviderOperation> batch = new List<ContentProviderOperation>();
			
			// Clear any existing suggestion words
        	batch.Add(ContentProviderOperation.NewDelete(ScheduleContract.SearchSuggest.CONTENT_URI).Build());

	
			var suggestions = from x in input.Descendants( "word" )
						select new TagXml
						{
							Word = x.Value
						};
		
			foreach(var item in suggestions)
				batch.Add(ContentProviderOperation.NewInsert(ScheduleContract.SearchSuggest.CONTENT_URI).WithValue(SearchManager.SuggestColumnText1, item.Word).Build());
	
	        return batch;
		}
		
		public class TagXml
		{
			public string Word;
		}
	}
}

