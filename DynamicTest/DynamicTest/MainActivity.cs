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
			var urls = new Dictionary<Uri,List<string>> ();
			var getUrl = new Uri ("https://api.github.com/users/mono/repos");
#if REACTIVE
			var hr = new HttpWebRequest (getUrl);
			var req = Observable.FromAsyncPattern<WebResponse> (hr.BeginGetResponse, hr.EndGetResponse);
			Observable.Defer (req).Subscribe (v => {
				var v = hr.GetResponse ();
				var json = (IEnumerable<JsonValue>) JsonValue.Load (v.GetResponseStream ());
#else
			var wc = new WebClient ();
			wc.DownloadStringCompleted += (sender, e) => {
				var v = e.Result;
				var json = (IEnumerable<JsonValue>) JsonValue.Parse (v);
#endif
#if REACTIVE
				json.ToObservable ().Select (j => j.AsDynamic ()).Subscribe (jitem => {
#else
				foreach (var item in json.Select (j => j.AsDynamic ())) {
#endif
					var uri = new Uri ((string) item.owner.avatar_url);
					var file = Path.Combine (baseDir, (string) item.id + new FileInfo (uri.LocalPath).Extension);
					if (!urls.ContainsKey (uri))
						urls.Add (uri, new List<string> () {file});
					else
						urls [uri].Add (file);
					data.Add (new JavaDictionary<string,object> () { {"Text", item.description}, {"Name", item.name}, {"Icon", Path.Combine (baseDir, file) }});
#if REACTIVE
				});
#else
				}
#endif
				urls.ToList ().ForEach (p => {
						var iwc = new WebClient ();
						iwc.DownloadDataCompleted += (isender, ie) => p.Value.ForEach (s => {
							using (var fs = File.Create (s))
								fs.Write (ie.Result, 0, ie.Result.Length);
						});
						iwc.DownloadDataAsync (p.Key);
					});
			
				var from = new string [] {"Text", "Name", "Icon"};
				var to = new int [] { Resource.Id.textMessage, Resource.Id.textName, Resource.Id.iconView};
			
				this.RunOnUiThread (() => {
					ListAdapter = new SimpleAdapter (this, data, Resource.Layout.ListItem, from, to);
				});
#if REACTIVE
			});
#else
			};
			wc.DownloadStringAsync (getUrl);
#endif
		}
	}
}


