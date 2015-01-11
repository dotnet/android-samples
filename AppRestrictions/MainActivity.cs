/*
 * Copyright (C) 2013 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;

namespace AppRestrictions
{
	/**
 	* This is the main user interface of the App Restrictions sample app.  It demonstrates the use
	* of the App Restriction feature, which is available on Android 4.3 and above tablet devices
 	* with the multiuser feature.
 	*
 	* When launched under the primary User account, you can toggle between standard app restriction
 	* types and custom.  When launched under a restricted profile, this activity displays app
 	* restriction settings, if available.
 	*
 	* Follow these steps to exercise the feature:
 	* 1. If this is the primary user, go to Settings > Users.
 	* 2. Create a restricted profile, if one doesn't exist already.
 	* 3. Open the profile settings, locate the sample app, and tap the app restriction settings
 	*    icon. Configure app restrictions for the app.
 	* 4. In the lock screen, switch to the user's restricted profile, launch this sample app,
 	*    and see the configured app restrictions displayed.
 	*/
	[Activity (Label = "AppRestrictions", MainLauncher = true)]
	public class MainActivity : Activity
	{
		Bundle mRestrictionsBundle;
	
		// Checkbox to indicate whether custom or standard app restriction types are selected.
		CheckBox mCustomConfig;

		public static readonly String CUSTOM_CONFIG_KEY = "custom_config";

		private TextView mMultiEntryValue;
		private TextView mChoiceEntryValue;
		private TextView mBooleanEntryValue;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			mCustomConfig = FindViewById <CheckBox> (Resource.Id.custom_app_limits);
		
			bool customChecked = 
				PreferenceManager.GetDefaultSharedPreferences (this).GetBoolean (
					CUSTOM_CONFIG_KEY, false);

			if (customChecked) mCustomConfig.Checked = true;

			mMultiEntryValue = FindViewById <TextView> (Resource.Id.multi_entry_id);
			mChoiceEntryValue = FindViewById <TextView> (Resource.Id.choice_entry_id);
			mBooleanEntryValue = FindViewById <TextView> (Resource.Id.boolean_entry_id);

			/**
    		* Saves custom app restriction to the shared preference.
    	 	*
    		* This flag is used by {@code GetRestrictionsReceiver} to determine if a custom app
    	 	* restriction activity should be used.
    	 	*
    	 	* @param view
    	 	*/
			mCustomConfig.Click += delegate (object sender, EventArgs e) {
				var editor = PreferenceManager.GetDefaultSharedPreferences (this).Edit ();
				editor.PutBoolean (CUSTOM_CONFIG_KEY, mCustomConfig.Checked).Commit ();
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// If app restrictions are set for this package, when launched from a restricted profile,
			// the settings are available in the returned Bundle as key/value pairs.
			mRestrictionsBundle = ((UserManager) GetSystemService (Context.UserService))
					.GetApplicationRestrictions (PackageName);

			if (mRestrictionsBundle == null) {
				mRestrictionsBundle = new Bundle ();
			}

			// Reads and displays values from a boolean type restriction entry, if available.
			// An app can utilize these settings to restrict its content under a restricted profile.
			String booleanRestrictionValue =
				mRestrictionsBundle.ContainsKey (GetRestrictionsReceiver.KEY_BOOLEAN) ?
					mRestrictionsBundle.GetBoolean (GetRestrictionsReceiver.KEY_BOOLEAN) + "":
					GetString (Resource.String.na);
			mBooleanEntryValue.Text = booleanRestrictionValue;

			// Reads and displays values from a single choice restriction entry, if available.
			String singleChoiceRestrictionValue =
				mRestrictionsBundle.ContainsKey (GetRestrictionsReceiver.KEY_CHOICE) ?
					mRestrictionsBundle.GetString (GetRestrictionsReceiver.KEY_CHOICE) :
					GetString (Resource.String.na);
			mChoiceEntryValue.Text = singleChoiceRestrictionValue;

			// Reads and displays values from a multi-select restriction entry, if available.
			String[] multiSelectValues =
				mRestrictionsBundle.GetStringArray (GetRestrictionsReceiver.KEY_MULTI_SELECT);

			if (multiSelectValues == null || multiSelectValues.Length == 0) {
				mMultiEntryValue.Text = GetString (Resource.String.na);

			} else {
				String tempValue = "";
				foreach (String value in multiSelectValues) {
					tempValue = tempValue + value + " ";
				}
				mMultiEntryValue.Text = tempValue ;
			}
		}
	}
}

