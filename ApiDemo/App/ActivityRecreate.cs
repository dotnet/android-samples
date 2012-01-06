/*
 * Copyright (C) 2010 The Android Open Source Project
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

// This sample only works on Android API 11+
#if __ANDROID_11__

using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo.App
{
	[Activity (Label = "App/Activity Recreate")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ActivityRecreate : Activity
	{
		int current_theme;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (bundle != null) {
				current_theme = bundle.GetInt ("theme");

				switch (current_theme) {
					case Android.Resource.Style.ThemeHoloLight:
						current_theme = Android.Resource.Style.ThemeHoloDialog;
						break;
					case Android.Resource.Style.ThemeHoloDialog:
						current_theme = Android.Resource.Style.ThemeHolo;
						break;
					default:
						current_theme = Android.Resource.Style.ThemeHoloLight;
						break;
				}

				SetTheme (current_theme);
			}

			SetContentView (Resource.Layout.ActivityRecreate);

			// Watch for button clicks
			var button = FindViewById<Button> (Resource.Id.recreate);
			button.Click += delegate { Recreate (); };
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			outState.PutInt ("theme", current_theme);
		}
	}
}
#endif