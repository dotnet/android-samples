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
using Android.Content.Res;

using Org.XmlPull.V1;
using Java.IO;
using System.Xml.Linq;

namespace MonoIO
{
	class LocalExecutor
	{
		private Resources mRes;
	    private ContentResolver mResolver;
	
	    public LocalExecutor(Resources res, ContentResolver resolver) {
	        mRes = res;
	        mResolver = resolver;
	    }
	
	    public void Execute(Context context, String assetName, XmlHandler handler)
		{
	        try {
	            var input = context.Assets.Open(assetName);
				var doc = XDocument.Load(input);
	            handler.ParseAndApply(doc, mResolver);
	        } catch (XmlPullParserException e) {
	            throw new Exception("Problem parsing local asset: " + assetName, e);
	        } catch (IOException e) {
	            throw new Exception("Problem parsing local asset: " + assetName, e);
	        }
	    }
	
	    public void Execute(int resId, XmlHandler handler)
		{
	        var input = mRes.GetXml(resId);
	        try {
				var doc = XDocument.Load(input);
	            handler.ParseAndApply(doc, mResolver);
	        } finally {
	            input.Close();
	        }
	    }
	}
}

