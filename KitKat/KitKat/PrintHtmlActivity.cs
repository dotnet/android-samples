using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Print;

namespace KitKat
{
	[Activity (Label = "PrintHtmlActivity")]			
	public class PrintHtmlActivity : Activity
	{
		WebView webView;
		bool dataLoaded;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.PrintHtml);
			webView = FindViewById <WebView> (Resource.Id.webView);

			// Important: Only enable the print option after the page is loaded.
			webView.SetWebViewClient (new MyWebViewClient (this));

			// Load some HTML content
			webView.LoadData ("<!DOCTYPE html><html><body><p>Hello world!</p><p><em>This is some html.</em></p><p>Click on the three dots in the top right or press device menu button to print.</p></body></html>",
				"text/html", 
				"utf8");
		}

		// Build out the options menu (this appears in the top right of the UI)
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			if (dataLoaded) {
				// Add the print option to the menu. The metadata
				// for the print option is defined in the print.xml
				// file in the Resources/menu deirectory
				MenuInflater.Inflate (Resource.Menu.print, menu);
			}
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// call the PrintPage method when user selects the print option from the menu
			if (item.ItemId == Resource.Id.menu_print) {
				PrintPage ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void PrintPage ()
		{
			// Ensure google cloud print is installed, or direct users to the google play store
			if(CloudPrintInstalled()) {
				// Get the print manager.
				PrintManager printManager = (PrintManager)GetSystemService (Context.PrintService);
				// Pass in the WebView's document adapter.
				PrintDocumentAdapter printDocumentAdapter = webView.CreatePrintDocumentAdapter ();
				printManager.Print ("MyWebPage", printDocumentAdapter, null);
			} else {
				var uri = Android.Net.Uri.Parse ("market://details?id=com.google.android.apps.cloudprint");
				Intent storeIntent = new Intent(Intent.ActionView, uri);

				StartActivity(storeIntent);
			}
		}

		bool CloudPrintInstalled()
		{
			try {
				PackageManager.GetPackageInfo("com.google.android.apps.cloudprint", 0);
				return true;
			} 
			catch(Exception e) {
				return false;
			}
		}

		class MyWebViewClient : WebViewClient
		{
			PrintHtmlActivity caller;

			public MyWebViewClient (PrintHtmlActivity caller) 
			{
				this.caller = caller;
			}

			public override void OnPageFinished (WebView view, string url)
			{
				// Data loaded, so now we want to show the print option.
				caller.dataLoaded = true;

				// Tell the Activity that the number of options in the menu has changed, 
				// and that it needs to be reloaded
				caller.InvalidateOptionsMenu ();
			}
		}
	}
}

