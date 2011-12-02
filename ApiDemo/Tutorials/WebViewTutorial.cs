using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/WebView", Theme = "@android:style/Theme.NoTitleBar")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class WebViewTutorial : Activity
	{
		WebView web_view;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource  
			SetContentView (Resource.Layout.WebViewTutorial);

			web_view = FindViewById<WebView> (Resource.Id.webview);
			web_view.Settings.JavaScriptEnabled = true;
			web_view.LoadUrl ("http://www.google.com");

			web_view.SetWebViewClient (new HelloWebViewClient ()); 
		}

		public override bool OnKeyDown (Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			if (keyCode == Keycode.Back && web_view.CanGoBack ()) {
				web_view.GoBack ();
				return true;
			}

			return base.OnKeyDown (keyCode, e);
		}  
		private class HelloWebViewClient : WebViewClient
		{
			public override bool ShouldOverrideUrlLoading (WebView view, string url)
			{
				view.LoadUrl (url);
				return true;
			}
		} 
	}
}