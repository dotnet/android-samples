/* 
 * Copyright (C) 2008 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label="App/Voice Recognition")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class VoiceRecognition : Activity
	{
		private const int VOICE_RECOGNITION_REQUEST_CODE = 1234;
		private ListView voice_list;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description.
			SetContentView (Resource.Layout.voice_recognition);

			// Get display items for later interaction
			Button speakButton = FindViewById<Button> (Resource.Id.btn_speak);

			voice_list = FindViewById<ListView> (Resource.Id.list);

			// Check to see if a recognition activity is present
			PackageManager pm = PackageManager;
			IList<ResolveInfo> activities = pm.QueryIntentActivities (new Intent (RecognizerIntent.ActionRecognizeSpeech), 0);

			if (activities.Count != 0)
				speakButton.Click += speakButton_Click;
			else {
				speakButton.Enabled = false;
				speakButton.Text = "Recognizer not present";
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == VOICE_RECOGNITION_REQUEST_CODE && resultCode == Result.Ok) {
				// Fill the list view with the strings the recognizer thought it could have heard
				IList<String> matches = data.GetStringArrayListExtra (RecognizerIntent.ExtraResults);
				voice_list.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, matches);
			}

			base.OnActivityResult (requestCode, resultCode, data);
		}

		private void speakButton_Click (object sender, EventArgs e)
		{
			View v = (View)sender;

			if (v.Id == Resource.Id.btn_speak)
				StartVoiceRecognitionActivity ();
		}

		private void StartVoiceRecognitionActivity ()
		{
			Intent intent = new Intent (RecognizerIntent.ActionRecognizeSpeech);

			intent.PutExtra (RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
			intent.PutExtra (RecognizerIntent.ExtraPrompt, "Voice Recognition Demo");

			StartActivityForResult (intent, VOICE_RECOGNITION_REQUEST_CODE);
		}
	}
}
