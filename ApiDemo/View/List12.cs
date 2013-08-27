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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace MonoDroid.ApiDemo
{
	/**
 	* Demonstrates the using a list view in transcript mode
 	*/
	[Activity (Label = "Views/Lists/12. Transcript")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List12 : ListActivity
	{
		EditText mUserText;
		ArrayAdapter<string> mAdapter;
		List<string> mStrings = new List<string>();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list_12);

			mAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, mStrings);

			ListAdapter = mAdapter;

			mUserText = FindViewById <EditText> (Resource.Id.userText);

			mUserText.Click += delegate {
				SendText ();
			};

			mUserText.KeyPress += delegate (object sender, View.KeyEventArgs e) {
				if (e.Event.Action == KeyEventActions.Down) {
					switch (e.KeyCode) {
					case Keycode.DpadCenter:
					case Keycode.Enter:
						SendText ();
						break;
					case Keycode.Back:
						OnBackPressed ();
						break;
					}
				}
			};
		}

		void SendText ()
		{
			string text = mUserText.Text;
			mAdapter.Add (text);
			mUserText.Text = "";
		}
	}
}

