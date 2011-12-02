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
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "OS/Morse Code")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class MorseCode : Activity
	{
		// Our text view 
		private TextView mTextView;

		// Initialization of the Activity after it is first created.  Must at least
		// call {@link android.app.Activity#setContentView setContentView()} to
		// describe what is to be displayed in the screen.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			// Be sure to call the base class.
			base.OnCreate (savedInstanceState);

			// See assets/res/any/layout/hello_world.xml for this
			// view layout definition, which is being set here as
			// the content of our screen.
			SetContentView (Resource.Layout.morse_code);

			// Set the Click event for the button so we see when it's pressed.
			((Button)FindViewById (Resource.Id.button)).Click += MorseCode_Click;

			// Save the text view so we don't have to look it up each time
			mTextView = (TextView)FindViewById (Resource.Id.text);
		}

		private void MorseCode_Click (object sender, EventArgs e)
		{
			// Get the text out of the view
			String text = mTextView.Text.ToString ();

			// convert it using the function defined above.  See the docs for
			// android.os.Vibrator for more info about the format of this array
			long[] pattern = MorseCodeConverter.GetPattern (text);

			// Start the vibration
			Vibrator vibrator = (Vibrator)GetSystemService (Context.VibratorService);
			vibrator.Vibrate (pattern, -1);
		}
	}
}
