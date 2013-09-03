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
using Android.Util;

namespace MonoDroid.ApiDemo
{
	/**
 	* Sample code that invokes the speech recognition intent API.
 	*/
	[Activity (Label="@string/voice_recognition")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class VoiceRecognition : Activity
	{
		private static String TAG = "VoiceRecognition";
		private const int VOICE_RECOGNITION_REQUEST_CODE = 1234;
		private ListView mList;
		public Handler mHandler;
		private Spinner mSupportedLanguageView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mHandler = new Handler ();

			// Inflate our UI from its XML layout description.
			SetContentView (Resource.Layout.voice_recognition);

			// Get display items for later interaction
			Button speakButton = FindViewById <Button> (Resource.Id.btn_speak);

			mList = FindViewById <ListView> (Resource.Id.list);

			mSupportedLanguageView = FindViewById <Spinner> (Resource.Id.supported_languages);

			// Check to see if a recognition activity is present
			PackageManager pm = PackageManager;
			IList<ResolveInfo> activities = pm.QueryIntentActivities (new Intent (RecognizerIntent.ActionRecognizeSpeech), 0);

			if (activities.Count != 0)
				speakButton.Click += speakButton_Click;
			else {
				speakButton.Enabled = false;
				speakButton.Text = "Recognizer not present";
			}

			// Most of the applications do not have to handle the voice settings. If the application
			// does not require a recognition in a specific language (i.e., different from the system
			// locale), the application does not need to read the voice settings.
			RefreshVoiceSettings();
		}

		/**
     	* Handle the click on the start recognition button.
     	*/
		private void speakButton_Click (object sender, EventArgs e)
		{
			View v = (View)sender;

			if (v.Id == Resource.Id.btn_speak)
				StartVoiceRecognitionActivity ();
		}

		/**
     	* Fire an intent to start the speech recognition activity.
     	*/
		private void StartVoiceRecognitionActivity ()
		{
			Intent intent = new Intent (RecognizerIntent.ActionRecognizeSpeech);

			// Specify the calling package to identify your application
			intent.PutExtra (RecognizerIntent.ExtraCallingPackage, PackageName);

			// Display an hint to the user about what he should say.
			intent.PutExtra (RecognizerIntent.ExtraPrompt, "Voice Recognition Demo");

			// Given an hint to the recognizer about what the user is going to say
			intent.PutExtra (RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

			// Specify how many results you want to receive. The results will be sorted
			// where the first result is the one with higher confidence.
			intent.PutExtra (RecognizerIntent.ExtraMaxResults, 5);

			// Specify the recognition language. This parameter has to be specified only if the
			// recognition has to be done in a specific language and not the default one (i.e., the
			// system locale). Most of the applications do not have to set this parameter.
			if (mSupportedLanguageView.SelectedItem != null && mSupportedLanguageView.SelectedItem.ToString () != "Default") {
				intent.PutExtra (RecognizerIntent.ExtraLanguage,
				                mSupportedLanguageView.SelectedItem.ToString ());
			}

			StartActivityForResult (intent, VOICE_RECOGNITION_REQUEST_CODE);
		}

		/**
     	* Handle the results from the recognition activity.
     	*/
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == VOICE_RECOGNITION_REQUEST_CODE && resultCode == Result.Ok) {
				// Fill the list view with the strings the recognizer thought it could have heard
				IList<String> matches = data.GetStringArrayListExtra (RecognizerIntent.ExtraResults);
				mList.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, matches);
			}

			base.OnActivityResult (requestCode, resultCode, data);
		}

		void RefreshVoiceSettings ()
		{
			Log.Info (TAG, "Sending broadcast");
			SendOrderedBroadcast (RecognizerIntent.GetVoiceDetailsIntent (this), null,
			                     new SupportedLanguageBroadcastReceiver (this), null, Result.Ok, null, null);
		}

		public void UpdateSupportedLanguages (IList<String> languages)
		{
			// We add "Default" at the beginning of the list to simulate default language.
			languages.Add ("Default");

			var adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleSpinnerItem, languages);
			mSupportedLanguageView.Adapter = adapter;
		}

		public void UpdateLanguagePreference (String language)
		{
			TextView textView = FindViewById <TextView> (Resource.Id.language_preference);
			textView.Text = language;
		}

		/**
     	* Handles the response of the broadcast request about the recognizer supported languages.
     	*
     	* The receiver is required only if the application wants to do recognition in a specific
     	* language.
     	*/
		class SupportedLanguageBroadcastReceiver : BroadcastReceiver
		{
			VoiceRecognition self;

			public SupportedLanguageBroadcastReceiver (VoiceRecognition s)
			{
				self = s;
			}

			public override void OnReceive (Context context, Intent intent)
			{
				Log.Info (TAG, "Receiving broadcast " + intent);

				Bundle extra = GetResultExtras (false);

				if (ResultCode != Result.Ok) {
					self.mHandler.Post (() => {
						Toast.MakeText (self, "Error code:" + ResultCode, ToastLength.Short).Show ();
					});
				}

				if (extra == null) {
					self.mHandler.Post (() => {
						Toast.MakeText (self, "No extra", ToastLength.Short).Show ();
					});
				}

				else if (extra.ContainsKey (RecognizerIntent.ExtraSupportedLanguages)) {
					self.mHandler.Post (() => {
						self.UpdateSupportedLanguages (extra.GetStringArrayList (RecognizerIntent.ExtraSupportedLanguages));
					});
				}

				else if (extra.ContainsKey (RecognizerIntent.ExtraLanguagePreference)) {
					self.mHandler.Post (() => {
						self.UpdateLanguagePreference (extra.GetString (RecognizerIntent.ExtraLanguagePreference));
					});
				}
			}
		}
	}
}
