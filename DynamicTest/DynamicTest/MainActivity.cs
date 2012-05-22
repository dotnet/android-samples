using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
#if REACTIVE
using System.Reactive;
using System.Reactive.Linq;
#endif
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Path = System.IO.Path;

[assembly:UsesPermission (Android.Manifest.Permission.Internet)]

namespace DynamicTest
{
	[Activity (Label = "Mono Dynamic Test", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var baseDir = Path.Combine (Application.ApplicationInfo.DataDir, "image_cache");
			if (!Directory.Exists (baseDir))
				Directory.CreateDirectory (baseDir);

			var data = new List<IDictionary<string,object>> ();
			var hr = new HttpWebRequest (new Uri ("http://api.twitter.com/1/statuses/public_timeline.json"));
#if REACTIVE
			var req = Observable.FromAsyncPattern<WebResponse> (hr.BeginGetResponse, hr.EndGetResponse);
			Observable.Defer (req).Subscribe (v => {
#else
			{
				var v = hr.GetResponse ();
#endif
				var urls = new Dictionary<Uri,string> ();
				var json = (IEnumerable<JsonValue>)JsonValue.Load (v.GetResponseStream ());
#if REACTIVE
				json.ToObservable ().Select (j => j.AsDynamic ()).Subscribe (jitem => {
#else
				var items = from item in json select item.AsDynamic ();
				foreach (dynamic jitem in items) {
#endif
					var dic = new Dictionary<string,object> ();
					dic ["Text"] = (string) jitem.text;
					dic ["Name"] = (string) jitem.user.name;
					var uri = new Uri ((string) jitem.user.profile_image_url);
					var file = Path.Combine (baseDir, (string) jitem.id + new FileInfo (uri.LocalPath).Extension);
					urls.Add (uri, file);
					dic ["Icon"] = Path.Combine (baseDir, file);
					data.Add (dic);
#if REACTIVE
				});
#else
				}
#endif
				urls.ToList ().ForEach (p => new WebClient ().DownloadFileAsync (p.Key, p.Value));
			
				var from = new string [] {"Text", "Name", "Icon"};
				var to = new int [] { Resource.Id.textMessage, Resource.Id.textName, Resource.Id.iconView};
			
				this.RunOnUiThread (() => {
					ListAdapter = new SimpleAdapter (this, data, Resource.Layout.ListItem, from, to);
				});
#if REACTIVE
			});
#else
			}
#endif
		}
	}
}


