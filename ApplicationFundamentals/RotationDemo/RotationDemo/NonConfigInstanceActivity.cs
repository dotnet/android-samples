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
using System.Net;
using System.IO;
using System.Json;

namespace RotationDemo
{
	[Activity (Label = "NonConfigInstanceActivity")]			
	public class NonConfigInstanceActivity : ListActivity
	{
		TweetListWrapper _savedInstance;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var tweetsWrapper = LastNonConfigurationInstance as TweetListWrapper;
			
			if (tweetsWrapper != null)
				PopulateTweetList (tweetsWrapper.Tweets);
			else
				SearchTwitter ("xamarin");
		}
	
		public override Java.Lang.Object OnRetainNonConfigurationInstance ()
		{
			base.OnRetainNonConfigurationInstance ();			
			return _savedInstance;
		}
					
		public void SearchTwitter (string text)
		{
			Console.WriteLine ("call twitter");
			
			string searchUrl = String.Format("http://search.twitter.com/search.json?q={0}&rpp=10&include_entities=false&result_type=mixed", text);
			
			Console.WriteLine(searchUrl);
			
			var httpReq = (HttpWebRequest)HttpWebRequest.Create (new Uri (searchUrl));
			httpReq.BeginGetResponse (new AsyncCallback (ResponseCallback), httpReq);
		}

		void ResponseCallback (IAsyncResult ar)
		{
			var httpReq = (HttpWebRequest)ar.AsyncState;
			
			using (var httpRes = (HttpWebResponse)httpReq.EndGetResponse (ar)) {
				ParseResults (httpRes);
			}
		}
		
		void ParseResults (HttpWebResponse httpRes)
		{
			var s = httpRes.GetResponseStream ();
            
			var j = (JsonObject)JsonObject.Load (s);
            
			var results = (from result in (JsonArray)j ["results"]
                let jResult = result as JsonObject
                select jResult ["text"].ToString ()).ToArray ();
            
			RunOnUiThread (() => {
				PopulateTweetList (results);
			});
		}
		
		void PopulateTweetList (string[] results)
		{
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.ItemView, results);
			_savedInstance = new TweetListWrapper{Tweets=results};
		}
		
		class TweetListWrapper : Java.Lang.Object
		{
			public string[] Tweets { get; set; }
		}
	}
	
//	[Activity (Label = "NonConfigInstanceActivity")]			
//	public class NonConfigInstanceActivity : Activity
//	{
//	    #region DON'T DO THIS AS IT WILL LEAK THE ACTIVITY
//		
//		TextView _textView;
//				
//		protected override void OnCreate (Bundle bundle)
//		{
//			base.OnCreate (bundle);
//			
//			var tv = LastNonConfigurationInstance as TextView; //TextViewWrapper;
//			
//			if (tv != null) {
//				_textView = tv; //.TxtVw;
//				var parent = _textView.Parent as FrameLayout;
//				parent.RemoveView (_textView);
//			} else {
//				_textView = new TextView (this);	
//				_textView.Text = "Don't return this TextView from OnRetainNonConfigurationInstance";
//			}
//			
//			SetContentView (_textView);
//		}
//		
//		public override Java.Lang.Object OnRetainNonConfigurationInstance ()
//		{
//			base.OnRetainNonConfigurationInstance ();	
//			return _textView;
//		}
//		
//		#endregion
//	}
}