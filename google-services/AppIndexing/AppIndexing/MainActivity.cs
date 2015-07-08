using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.AppIndexing;
using Android.Net;
using Android.Gms.Common.Apis;
using Android.Util;

namespace AppIndexing
{
	[Activity (Exported=true, LaunchMode = LaunchMode.SingleTop)]
	[IntentFilter (new [] {Android.Content.Intent.ActionView, Android.Content.Intent.ActionMain}, 
		Categories = new [] {
			Android.Content.Intent.CategoryLauncher,
			Android.Content.Intent.CategoryDefault,
			Android.Content.Intent.CategoryBrowsable
		},
		DataScheme = "http",
		DataHost = "example.com",
		DataPathPrefix = "/articles/"
	)]
	public class MainActivity : Activity
	{
		static readonly string Tag = typeof(MainActivity).Name;
		IGoogleApiClient mClient;
		string articleId;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);
			mClient = new GoogleApiClientBuilder(this).AddApi (AppIndex.APP_INDEX_API).Build ();
			OnNewIntent (Intent);
		}

		protected override void OnNewIntent (Android.Content.Intent intent)
		{
			var action = intent.Action;
			var data = intent.DataString;
			if (action == Android.Content.Intent.ActionView && string.IsNullOrEmpty(data)) {
				articleId = data.Substring(data.LastIndexOf("/") + 1);
				var deepLinkText = FindViewById<TextView>(Resource.Id.deep_link);
				deepLinkText.Text = data;
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (articleId != null) {
				// Connect your client
				mClient.Connect();

				// Define a title for your current page, shown in autocompletion UI
				const string title = "Sample Article";
				var appUri = Uri.Parse("android-app://com.google.developers.app-indexing.quickstart/http/www.example.com/articles/" + articleId);
				var webUrl = Uri.Parse("http://www.example.com/articles/" + articleId);

				// Call the App Indexing API view method
				var result = AppIndex.AppIndexApi.View(mClient, this,
					appUri, title, webUrl, null);
				result.SetResultCallback (new ResultCallback ());
			}
		}


		protected override void OnStop ()
		{
			base.OnStop ();
			if (articleId != null) {
				var appUri = Uri.Parse("android-app://com.google.developers.app-indexing.quickstart/http/www.example.com/articles/" + articleId);
				var result = AppIndex.AppIndexApi.ViewEnd(mClient, this, appUri);

				mClient.Disconnect();
			}
		}

		class ResultCallback : Java.Lang.Object, IResultCallback
		{
			public void OnResult (Java.Lang.Object x0)
			{
				var status = x0 as Statuses;
				if (status == null)
					return;
				if (status.IsSuccess) {
					Log.Debug(Tag, "App Indexing API: Recorded page view successfully.");
				} else {
					Log.Error(Tag, "App Indexing API: There was an error recording the page view."
						+ status.ToString());
				}
			}
		}
	}
}


