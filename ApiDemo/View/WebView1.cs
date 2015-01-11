/*
 * Copyright (C) 2007 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	/**
 	* Sample creating 1 webviews.
 	*/
	[Activity (Label = "Views/WebView")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class WebView1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.webview_1);

			var mimeType = "text/html";

			WebView wv;

			wv = FindViewById < WebView> (Resource.Id.wv1);
			wv.LoadData ("<a href=\"http://www.xamarin.com\">Hello Xamarin!</a>", mimeType, null);
		}
	}
}