/*
 * Copyright (C) 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
using Android.Webkit;
using Android.Print;

namespace MonoDroid.ApiDemo
{
	/**
 	* This class demonstrates how to implement HTML content printing
 	* from a WebView which is shown on the screen.
 	* 
 	* This activity shows a simple HTML content in a WebView
 	* and allows the user to print that content via an action in the
 	* action bar. The shown WebView is doing the printing.
 	*/
	[Activity (Label = "@string/print_html_from_screen")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class PrintHtmlFromScreen : Activity
	{
		WebView mWebView;
		bool mDataLoaded;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.print_html_from_screen);
			mWebView = FindViewById <WebView> (Resource.Id.web_view);

			// Important: Only enable the print option after the page is loaded.
			mWebView.SetWebViewClient (new MyWebViewClient (this));

			// Load an HTML page.
			mWebView.LoadUrl ("file:///android_res/raw/motogp_stats.html");
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			if (mDataLoaded) {
				MenuInflater.Inflate (Resource.Menu.print_custom_content, menu);
			}
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_print) {
				Print ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		void Print ()
		{
			// Get the print manager.
			var printManager = (PrintManager)GetSystemService (Context.PrintService);
			// Pass in the WebView's document adapter.
			printManager.Print ("MotoGP stats", mWebView.CreatePrintDocumentAdapter (), null);
		}

		class MyWebViewClient : WebViewClient
		{
			PrintHtmlFromScreen self;

			public MyWebViewClient (PrintHtmlFromScreen self) 
			{
				this.self = self;
			}

			public override void OnPageFinished (WebView view, string url)
			{
				// Data loaded, so now we want to show the print option.
				self.mDataLoaded = true;
				self.InvalidateOptionsMenu ();
			}
		}
	}
}

