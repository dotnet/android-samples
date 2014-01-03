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
 	* from a WebView which is not shown on the screen.
 	* This activity shows a text prompt and when the user chooses the
 	* print option from the overflow menu an HTML page with content that
	* is not on the screen is printed via an off-screen WebView.
 	*/
	[Activity (Label = "@string/print_html_off_screen")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class PrintHtmlOffScreen : Activity
	{
		WebView mWebView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.print_html_off_screen);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.print_custom_content, menu);
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
			// Create a WebView and hold on to it as the printing will start when
			// load completes and we do not want the WebView to be garbage collected.
			mWebView = new WebView (this);

			// Important: Only after the page is loaded we will do the print.
			mWebView.SetWebViewClient (new MyWebViewClient (this));

			// Load an HTML page.
			mWebView.LoadUrl ("file:///android_res/raw/motogp_stats.html");
		}

		void DoPrint ()
		{
			// Get the print manager.
			var printManager = (PrintManager)GetSystemService (Context.PrintService);

			// Pass in our custom document adapter.
			printManager.Print ("MotoGP stats", new MyPrintDocumentAdapter (this), null);

		}

		// Create a wrapper PrintDocumentAdapter to clean up when done.
		class MyPrintDocumentAdapter : PrintDocumentAdapter
		{
			PrintDocumentAdapter mWrappedInstance;
			PrintHtmlOffScreen self;

			public MyPrintDocumentAdapter (PrintHtmlOffScreen self)
			{
				this.self = self;
				mWrappedInstance = self.mWebView.CreatePrintDocumentAdapter ();
			}

			public override void OnStart ()
			{
				mWrappedInstance.OnStart ();
			}

			public override void OnLayout (PrintAttributes oldAttributes, PrintAttributes newAttributes, 
				CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
			{
				mWrappedInstance.OnLayout(oldAttributes, newAttributes,
					cancellationSignal, callback, extras);
			}

			public override void OnWrite (PageRange[] pages, ParcelFileDescriptor destination,
				CancellationSignal cancellationSignal, WriteResultCallback callback)
			{
				mWrappedInstance.OnWrite (pages, destination, cancellationSignal, callback);
			}

			public override void OnFinish ()
			{
				mWrappedInstance.OnFinish();
				// Intercept the finish call to know when printing is done
				// and destroy the WebView as it is expensive to keep around.
				self.mWebView.Destroy();
				self.mWebView = null;
			}
		}

		class MyWebViewClient : WebViewClient
		{
			PrintHtmlOffScreen self;

			public MyWebViewClient (PrintHtmlOffScreen self) 
			{
				this.self = self;
			}

			public override void OnPageFinished (WebView view, string url)
			{
				// Important: Only after the page is loaded we will do the print.
				self.DoPrint ();
			}
		}
	}
}

