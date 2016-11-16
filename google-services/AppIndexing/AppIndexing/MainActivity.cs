﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.AppIndexing;
using Android.Net;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Util;
using Java.Lang;

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
		DataHost = "www.example.com",
		DataPathPrefix = "/articles/"
	)]
	public class MainActivity : AppCompatActivity
	{
		// Define a title for your current page, shown in autocompletion UI
		const string Title = "Sample Article";
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
			if (!Android.Content.Intent.ActionView.Equals(action) || string.IsNullOrEmpty(data)) return;
			articleId = data.Substring(data.LastIndexOf("/") + 1);
			var deepLinkText = FindViewById<TextView>(Resource.Id.deep_link);
			deepLinkText.Text = data;
		}

		protected override async void OnStart ()
		{
			base.OnStart ();
			if (articleId == null) return;
			// Connect your client
			mClient.Connect();

			var baseUrl = Uri.Parse("http://www.example.com/articles/");
			var appUri = baseUrl.BuildUpon().AppendPath(articleId).Build();

			var viewAction = Action.NewAction(Action.TypeView, Title, appUri);

			// Call the App Indexing API view method
			var result = await AppIndex.AppIndexApi.Start(mClient, viewAction);
			if (result.Status.IsSuccess)
				Log.Debug(Tag, "App Indexing API: Recorded page view successfully.");
			else
				Log.Error(Tag, "App Indexing API: There was an error recording the page view." + result);
		}

		protected override async void OnStop ()
		{
			base.OnStop ();
			if (articleId == null) return;
			var baseUrl = Uri.Parse("http://www.example.com/articles/");
			var appUri = baseUrl.BuildUpon().AppendPath(articleId).Build();

			var viewAction = Action.NewAction(Action.TypeView, Title, appUri);
			var result = await AppIndex.AppIndexApi.End(mClient, viewAction);

			await AppIndex.AppIndexApi.ViewEndAsync (mClient, this, appUri);
			if (result.Status.IsSuccess)
				Log.Debug(Tag, "App Indexing API: Indexed recipe view end successfully.");
			else
				Log.Error(Tag, "App Indexing API: There was an error indexing the recipe view." + result.Status);

			mClient.Disconnect();
		}
	}
}


