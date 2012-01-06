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

//
// Copyright (C) 2011 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0.
//
#if false // To enabled this sample, add "Resources/xml" directory to the project (under Resources directory). Note that it won't then build in earlier platforms than API Level 11(!)
#if __ANDROID_11__

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo.Preferences
{
	[Activity (Label = "@string/preference_with_headers")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class PreferenceWithHeaders : PreferenceActivity
	{
		public PreferenceWithHeaders ()
		{
		}
		
		protected override void OnCreate (Bundle savedInstance)
		{
			base.OnCreate (savedInstance);
			
			if (HasHeaders) {
				var button = new Button (this);
				button.Text = "Some action";
				this.SetListFooter (button);
			}
		}
		
		public override void OnBuildHeaders (IList<PreferenceActivity.Header> target)
		{
			LoadHeadersFromResource (Resource.Xml.preference_headers, target);
		}
		
		public class Prefs1Fragment : PreferenceFragment
		{
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				// Load the preferences from an XML resource
				AddPreferencesFromResource (Resource.Xml.fragmented_preferences);
			}
		}
		
		public class Prefs1FragmentInner : PreferenceFragment
		{
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				// Can retrieve arguments from preference XML.
				Log.Info ("args", "Arguments: " + Arguments);
				
				// Load the preferences from an XML resource
				AddPreferencesFromResource (Resource.Xml.fragmented_preferences_inner);
			}
		}

		public class Prefs2Fragment : PreferenceFragment
		{
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				// Can retrieve arguments from preference XML.
				Log.Info ("args", "Arguments: " + Arguments);

				// Load the preferences from an XML resource
				AddPreferencesFromResource (Resource.Xml.preference_dependencies);
			}
		}
	}
}

#endif
#endif
