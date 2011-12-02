//
// Copyright (C) 2007 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.IO;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Content/Read Asset")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class ReadAsset : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.read_asset);

			// Programmatically load text from an asset and place it into the
			// text view.  Note that the text we are loading is ASCII, so we
			// need to convert it to UTF-16.
			Stream input = Assets.Open ("read_asset.txt");

			string text;

			// Use a StreamReader to read the data
			using (StreamReader sr = new StreamReader (input))
				text = sr.ReadToEnd ();

			// Finally stick the string into the text view.
			TextView tv = FindViewById<TextView> (Resource.Id.text);
			tv.Text = text;
		}
	}
}
