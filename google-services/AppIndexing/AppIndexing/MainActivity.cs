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
		GoogleApiClient mClient;
		string articleId;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);
			mClient = new GoogleApiClient.Builder (this).AddApi (AppIndex.APP_INDEX_API).Build ();
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

		protected override async void OnStart ()
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
				var result = await AppIndex.AppIndexApi.ViewAsync (mClient, this, appUri, title, webUrl, null);

                if (result.IsSuccess)
                    Log.Debug(Tag, "App Indexing API: Recorded page view successfully.");
                else
                    Log.Error(Tag, "App Indexing API: There was an error recording the page view." + result);
			}
		}


		protected override async void OnStop ()
		{
			base.OnStop ();
			if (articleId != null) {
				var appUri = Uri.Parse("android-app://com.google.developers.app-indexing.quickstart/http/www.example.com/articles/" + articleId);
				
                await AppIndex.AppIndexApi.ViewEndAsync (mClient, this, appUri);

				mClient.Disconnect();
			}
		}
	}
}


