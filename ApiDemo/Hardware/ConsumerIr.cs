/*
 * Copyright (C) 20013The Android Open Source Project
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
using Android.Hardware;
using Android.Util;

namespace MonoDroid.ApiDemo
{
	/**
 	* App that transmit an IR code
 	* This demonstrates the Android.Hardware.ConsumerIrManager class.
 	*
 	* Hardware / Consumer IR
 	*/
	[Activity (Label = "Hardware/Consumer IR", Name = "monodroid.apidemo.ConsumerIr")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class ConsumerIr : Activity
	{
		static readonly String TAG = "ConsumerIrTest";
		TextView mFreqsText;
		ConsumerIrManager mCIR;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Get a reference to the ConsumerIrManager
			mCIR = (ConsumerIrManager)GetSystemService (Context.ConsumerIrService);

			// See assets/res/any/layout/consumer_ir.xml for this
			// view layout definition, which is being set here as
			// the content of our screen.
			SetContentView (Resource.Layout.consumer_ir);

			// Set the OnClickListener for the button so we see when it's pressed.
			FindViewById (Resource.Id.send_button).Click += mSendClickListener;
			FindViewById (Resource.Id.get_freqs_button).Click += mGetFreqsClickListener;
			mFreqsText = FindViewById <TextView> (Resource.Id.freqs_text);
		}

		void mSendClickListener (object sender, EventArgs e)
		{
			if (!mCIR.HasIrEmitter) {
				Log.Error (TAG, "No IR Emitter found\n");
				return;
			}

			// A pattern of alternating series of carrier on and off periods measured in
			// microseconds.
			int[] pattern = {1901, 4453, 625, 1614, 625, 1588, 625, 1614, 625, 442, 625, 442, 625,
				468, 625, 442, 625, 494, 572, 1614, 625, 1588, 625, 1614, 625, 494, 572, 442, 651,
				442, 625, 442, 625, 442, 625, 1614, 625, 1588, 651, 1588, 625, 442, 625, 494, 598,
				442, 625, 442, 625, 520, 572, 442, 625, 442, 625, 442, 651, 1588, 625, 1614, 625,
				1588, 625, 1614, 625, 1588, 625, 48958};

			// transmit the pattern at 38.4KHz
			mCIR.Transmit (38400, pattern);
		}

		void mGetFreqsClickListener (object sender, EventArgs e)
		{
			var sb = new StringBuilder ();

			if (!mCIR.HasIrEmitter) {
				mFreqsText.Text = "No IR Emitter found!";
				Log.Error (TAG, "No IR Emitter found!\n");
				return;
			}

			// Get the available carrier frequency ranges
			ConsumerIrManager.CarrierFrequencyRange[] freqs = mCIR.GetCarrierFrequencies ();
			sb.Append ("IR Carrier Frequencies:\n");
			foreach (var range in freqs) {
				sb.Append (string.Format ("    %d - %d\n", range.MinFrequency, range.MaxFrequency));
			}
			mFreqsText.Text = sb.ToString ();
		}
	}
}

