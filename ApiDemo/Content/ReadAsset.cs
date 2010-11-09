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
using Java.IO;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Content/Read Asset")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { Intent.CategorySampleCode })]
	public class ReadAsset : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.layout.read_asset);

			// Programmatically load text from an asset and place it into the
			// text view.  Note that the text we are loading is ASCII, so we
			// need to convert it to UTF-16.
			InputStream input = Assets.Open ("read_asset.txt");

			// We guarantee that the available method returns the total
			// size of the asset...  of course, this does mean that a single
			// asset can't be more than 2 gigs.
			int size = input.Available ();
			
			// Read the entire asset into a local byte buffer.
			byte[] buffer = new byte[size];
			input.Read (buffer);
			input.Close ();

			// Convert the buffer into a string.
			String text = System.Text.ASCIIEncoding.Default.GetString (buffer);

			// Finally stick the string into the text view.
			TextView tv = FindViewById<TextView> (Resource.id.text);
			tv.Text = text;
		}
	}
}
