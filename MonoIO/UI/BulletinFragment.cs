/*
 * Copyright 2011 Google Inc.
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
using Android.Util;
using Android.Views;
using Android.Widget;
using MonoIO.Utilities;
using Android.Webkit;
using Java.Net;
using Java.IO;
using Android.Support.V4.App;
using Pattern = Java.Util.Regex.Pattern;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO.UI
{
	public class BulletinFragment : Fragment
	{
		private static Pattern sSiteUrlPattern = Pattern.Compile("google\\.com\\/events\\/io");
    	private string BULLETIN_URL = "http://www.google.com/events/io/2011/mobile_announcements.html";
		private static WebView webView;
		private static View loadingSpinner;

		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu(true);
			//AnalyticsUtils.getInstance(getActivity()).trackPageView("/Bulletin");
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_webview_with_spinner, null);

			// For some reason, if we omit this, NoSaveStateFrameLayout thinks we are
			// FILL_PARENT / WRAP_CONTENT, making the progress bar stick to the top of the activity.
			root.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			
			loadingSpinner = root.FindViewById<View> (Resource.Id.loading_spinner);
			webView = root.FindViewById<WebView> (Resource.Id.webview);

			webView.SetWebViewClient (new BulletinWebViewClient (Activity));

			webView.Post (() => {
				webView.Settings.JavaScriptEnabled = true;
				webView.Settings.JavaScriptCanOpenWindowsAutomatically = false;
				webView.LoadUrl (BULLETIN_URL);
			});
			
			return root;
		}
		
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
        	inflater.Inflate(Resource.Menu.refresh_menu_items, menu);
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_refresh) {
            	webView.Reload();
	            return true;
	        }
	        return base.OnOptionsItemSelected(item);
		}
		
		class BulletinWebViewClient : WebViewClient
		{
			public Activity _activity;
			
			public BulletinWebViewClient(Activity activity)
			{
				_activity = activity;	
			}
			
			public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon)
			{
				base.OnPageStarted(view, url, favicon);
				loadingSpinner.Visibility = ViewStates.Visible;
				webView.Visibility = ViewStates.Invisible;
			}
	
			public override void OnPageFinished (WebView view, string url)
			{
				base.OnPageFinished(view, url);
				loadingSpinner.Visibility = ViewStates.Gone;
				webView.Visibility = ViewStates.Visible;
			}
			
			public override bool ShouldOverrideUrlLoading (WebView view, string url)
			{
				if (sSiteUrlPattern.Matcher(url).Find()) 
				{
	                return false;
	            }
	
	            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
	        	_activity.StartActivity(intent);
	            return true;
			}
		}
	}
}