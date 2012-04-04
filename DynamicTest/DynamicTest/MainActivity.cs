using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
#if REACTIVE
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

namespace DynamicTest
{
	[Activity (Label = "Mono Dynamic Test", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var wait = new ManualResetEvent (false);
			var data = new List<IDictionary<string,object>> ();
			var hr = new HttpWebRequest (new Uri ("http://api.twitter.com/1/statuses/public_timeline.json"));
			#if REACTIVE
												var req = Observable.FromAsyncPattern<WebResponse> (hr.BeginGetResponse, hr.EndGetResponse);
			Observable.Defer (req)/*.SubscribeOn (Application.SynchronizationContext)*/.Subscribe (v => {
			#else
			{
				var v = hr.GetResponse ();
				#endif
				var json = (JsonArray)JsonValue.Load (v.GetResponseStream ());
				var items = from item in (IEnumerable<JsonValue>)json select item.AsDynamic ();
				foreach (dynamic jitem in items) {
					var dic = new Dictionary<string,object> ();
					dic ["Text"] = jitem.text;
					dic ["Name"] = jitem.user.name;
					// FIXME: needs to provide correct image
					dic ["Image"] = jitem.user.profile_image_url;
					data.Add (dic);
				}
			
				var from = new string [] {"Text", "Name", "Icon"};
				var to = new int [] { Resource.Id.textMessage, Resource.Id.textName, Resource.Id.iconView};
			
				this.RunOnUiThread (() => {
					ListAdapter = new SimpleAdapter (this, data, Resource.Layout.ListItem, from, to);
					Toast.MakeText (this, "list updated", ToastLength.Short);
				});
#if REACTIVE
			});
#else
			}
#endif
			Toast.MakeText (this, "retrieving public timeline...", ToastLength.Short);
		}
	}
}


