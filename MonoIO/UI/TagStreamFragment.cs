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
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO.UI
{
	public class TagStreamFragment : Fragment
	{
		private string search_string;
		private static WebView web_view;
		private static View loading_spinner;
		public const string EXTRA_QUERY = "monoio.extra.QUERY";
		public const string DEFAULT_SEARCH = "monodroid";

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Intent intent = BaseActivity.FragmentArgumentsToIntent (Arguments);
			search_string = intent.GetStringExtra (EXTRA_QUERY);

			if (string.IsNullOrEmpty (search_string))
				search_string = DEFAULT_SEARCH;

			if (!search_string.StartsWith ("#"))
				search_string = "#" + search_string;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_webview_with_spinner, null);

			// For some reason, if we omit this, NoSaveStateFrameLayout thinks we are
			// FILL_PARENT / WRAP_CONTENT, making the progress bar stick to the top of the activity.
			root.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			
			loading_spinner = root.FindViewById<View> (Resource.Id.loading_spinner);
			web_view = root.FindViewById<WebView> (Resource.Id.webview);

			web_view.SetWebViewClient (new TagWebViewClient (Activity));

			web_view.Post (() => {
				web_view.Settings.JavaScriptEnabled = true;
				web_view.Settings.JavaScriptCanOpenWindowsAutomatically = false;

				try {
					var url = "http://twitter.com/#!/search/{0}";
					url = string.Format (url, Uri.EscapeDataString (search_string));

					web_view.LoadUrl (url);
				} catch (UnsupportedEncodingException ex) {
					Log.Error ("MonoIO", "Could not construct the realtime search URL", ex);
				}
			});
			

			return root;
		}

		public void Refresh ()
		{
			web_view.Reload ();
		}
		
		class TagWebViewClient : WebViewClient
		{
			public Activity _activity;
			
			public TagWebViewClient (Activity activity)
			{
				_activity = activity;	
			}
			
			public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon)
			{
				loading_spinner.Visibility = ViewStates.Visible;
				web_view.Visibility = ViewStates.Invisible;
			}
	
			public override void OnPageFinished (WebView view, string url)
			{
				loading_spinner.Visibility = ViewStates.Gone;
				web_view.Visibility = ViewStates.Visible;
			}
			
			public override bool ShouldOverrideUrlLoading (WebView view, string url)
			{
//				if (url.StartsWith("javascript")) {
				return false;
//	            }
//	
//	            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
//	            _activity.StartActivity(intent);
//	            return true;
			}
			
		}
	}
}