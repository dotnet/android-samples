/*
 * Copyright (C) 2016 The Android Open Source Project
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

using Android.App;
using Android.Widget;
using Android.OS;

namespace AppShortcuts
{
	[Activity(Label = "AppShortcuts", MainLauncher = true, Icon = "@mipmap/icon")]
	public class Main : ListActivity
	{
		public static string TAG = "ShortcutSample";

    	static string ID_ADD_WEBSITE = "add_website";

    	static string ACTION_ADD_WEBSITE = "com.example.android.shortcutsample.ADD_WEBSITE";
		//private MyAdapter myAdapter;

		private ShortcutHelper shortcutHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
		}
	}
}

