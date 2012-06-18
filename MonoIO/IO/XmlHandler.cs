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
using Org.XmlPull.V1;
using Java.IO;
using System.IO;
using System.Xml.Linq;

namespace MonoIO
{
	public abstract class XmlHandler
	{
		private String mAuthority;
	
	    public XmlHandler(String authority) 
		{
	        mAuthority = authority;
	    }
		
		public void ParseAndApply(XDocument input, ContentResolver resolver)
		{
			try {
	            List<ContentProviderOperation> batch = Parse(input, resolver);
	            resolver.ApplyBatch(mAuthority, batch);
	        } 
			catch (Exception e)
			{
	            // Failures like constraint violation aren't recoverable
	            // TODO: write unit tests to exercise full provider
	            // TODO: consider catching version checking asserts here, and then
	            // wrapping around to retry parsing again.
	            throw new Exception("Problem applying batch operation", e);
	        }	
		}
	
		/**
	     * Parse the given {@link XmlPullParser}, returning a set of
	     * {@link ContentProviderOperation} that will bring the
	     * {@link ContentProvider} into sync with the parsed data.
	     */
	    public abstract List<ContentProviderOperation> Parse(XDocument input, ContentResolver resolver);
	}
}
