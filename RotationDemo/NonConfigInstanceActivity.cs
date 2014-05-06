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

						var twitter = new Twitter ();
						ParseResults (twitter.GetTweets ());
				}

				void ParseResults (string jsonData)
				{
						var j = (JsonObject)JsonObject.Parse (jsonData);

						var results = (from result in (JsonArray)j ["statuses"]
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
}