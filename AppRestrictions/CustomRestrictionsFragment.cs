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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace AppRestrictions
{
	/**
	* This fragment is included in {@code CustomRestrictionsActivity}.  It demonstrates how an app
	* can integrate its own custom app restriction settings with the restricted profile feature.
	*
	* This sample app maintains custom app restriction settings in shared preferences.  Your app
 	* can use other methods to maintain the settings.  When this activity is invoked
 	* (from Settings > Users > Restricted Profile), the shared preferences are used to initialize
 	* the custom configuration on the user interface.
 	*
 	* Three sample input types are shown: checkbox, single-choice, and multi-choice.  When the
 	* settings are modified by the user, the corresponding restriction entries are saved in the
 	* platform.  The saved restriction entries are retrievable when the app is launched under a
 	* restricted profile.
 	*/
	public class CustomRestrictionsFragment : PreferenceFragment
	{
		// Shared preference key for the boolean restriction.
		private static readonly String KEY_BOOLEAN_PREF = "pref_boolean";
		// Shared preference key for the single-select restriction.
		private static readonly String KEY_CHOICE_PREF = "pref_choice";
		// Shared preference key for the multi-select restriction.
		private static readonly String KEY_MULTI_PREF = "pref_multi";

		private List<IParcelable> mRestrictions;
		private Bundle mRestrictionsBundle;

		// Shared preferences for each of the sample input types.
		private CheckBoxPreference mBooleanPref;
		private ListPreference mChoicePref;
		private MultiSelectListPreference mMultiPref;

		// Restriction entries for each of the sample input types.
		private RestrictionEntry mBooleanEntry;
		private RestrictionEntry mChoiceEntry;
		private RestrictionEntry mMultiEntry;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			AddPreferencesFromResource (Resource.Xml.custom_prefs);

			// This sample app uses shared preferences to maintain app restriction settings.  Your app
			// can use other methods to maintain the settings.
			mBooleanPref = FindPreference (KEY_BOOLEAN_PREF) as CheckBoxPreference;
			mChoicePref = FindPreference (KEY_CHOICE_PREF) as ListPreference;
			mMultiPref = FindPreference (KEY_MULTI_PREF) as MultiSelectListPreference;

			mBooleanPref.PreferenceChange += OnPreferenceChange;
			mChoicePref.PreferenceChange += OnPreferenceChange;
			mMultiPref.PreferenceChange += OnPreferenceChange;

			RetainInstance = true;
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			Activity act = Activity; 

			/* BEGIN_INCLUDE (GET_CURRENT_RESTRICTIONS) */
			// Existing app restriction settings, if exist, can be retrieved from the Bundle.
			mRestrictionsBundle = act.Intent.GetBundleExtra (Intent.ExtraRestrictionsBundle);

			if (mRestrictionsBundle == null) {
				mRestrictionsBundle = ((UserManager) act.GetSystemService (Context.UserService))
							.GetApplicationRestrictions (Activity.PackageName);
			}

			if (mRestrictionsBundle == null) {
				mRestrictionsBundle = new Bundle ();
			}

			mRestrictions = (List<IParcelable>) act.Intent.GetParcelableArrayListExtra (Intent.ExtraRestrictionsList);
			/* END_INCLUDE (GET_CURRENT_RESTRICTIONS) */

			// Transfers the saved values into the preference hierarchy.
			if (mRestrictions != null) {
				foreach (RestrictionEntry entry in mRestrictions) {
					if (entry.Key.Equals (GetRestrictionsReceiver.KEY_BOOLEAN)) {
						mBooleanPref.Checked = entry.SelectedState;
						mBooleanEntry = entry;

					} else if (entry.Key.Equals (GetRestrictionsReceiver.KEY_CHOICE)) {
						mChoicePref.Value = entry.SelectedString;
						mChoiceEntry = entry;

					} else if (entry.Key.Equals (GetRestrictionsReceiver.KEY_MULTI_SELECT)) {
						List <String> list = new List <String> ();
						foreach (String value in entry.GetAllSelectedStrings ()) {
							list.Add (value);
						}
						mMultiPref.Values = list;
						mMultiEntry = entry;
					}
				}
			} else {
				mRestrictions = new List<IParcelable> ();

				// Initializes the boolean restriction entry and updates its corresponding shared
				// preference value.
				mBooleanEntry = new RestrictionEntry (GetRestrictionsReceiver.KEY_BOOLEAN, 
				                                      mRestrictionsBundle.GetBoolean (GetRestrictionsReceiver.KEY_BOOLEAN, false));
				mBooleanEntry.Type = RestrictionEntryType.Boolean;
				mBooleanPref.Checked = mBooleanEntry.SelectedState;

				// Initializes the single choice restriction entry and updates its corresponding
				// shared preference value.
				mChoiceEntry = new RestrictionEntry (GetRestrictionsReceiver.KEY_CHOICE, 
				                                     mRestrictionsBundle.GetString (GetRestrictionsReceiver.KEY_CHOICE));
				mChoiceEntry.Type = RestrictionEntryType.Choice;
				mChoicePref.Value = mChoiceEntry.SelectedString;

				// Initializes the multi-select restriction entry and updates its corresponding
				// shared preference value.
				mMultiEntry = new RestrictionEntry (GetRestrictionsReceiver.KEY_MULTI_SELECT,
					                                   mRestrictionsBundle.GetStringArray (GetRestrictionsReceiver.KEY_MULTI_SELECT));
				mMultiEntry.Type = RestrictionEntryType.MultiSelect;

				if (mMultiEntry.GetAllSelectedStrings() != null) {
					List <String> hset = new List <String> ();
					String[] values = mRestrictionsBundle.GetStringArray (GetRestrictionsReceiver.KEY_MULTI_SELECT);

					if (values != null) {
						foreach (String value in values) {
							hset.Add (value);
						}
					}
					mMultiPref.Values = hset;
				}
				mRestrictions.Add (mBooleanEntry);
				mRestrictions.Add (mChoiceEntry);
				mRestrictions.Add (mMultiEntry);
			}
			// Prepares result to be passed back to the Settings app when the custom restrictions
			// activity finishes.
			Intent intent = new Intent (Activity.Intent);
			intent.PutParcelableArrayListExtra (Intent.ExtraRestrictionsList, new List <IParcelable> (mRestrictions));
			Activity.SetResult (Result.Ok, intent);
		}

		public void OnPreferenceChange (object sender, Preference.PreferenceChangeEventArgs e) 
		{
			if (e.Preference == mBooleanPref) {
				mBooleanEntry.SelectedState = (bool) e.NewValue;

			} else if (e.Preference == mChoicePref) {
				mChoiceEntry.SelectedString = (string) e.NewValue;

			} else if (e.Preference == mMultiPref) {
				String[] selectedStrings = new String [((IList <string>) e.NewValue).Count];
				int i = 0;
				foreach (String value in (IList<string>) e.NewValue) {
					selectedStrings[i++] = value;
				}
				mMultiEntry.SetAllSelectedStrings (selectedStrings);
			}

			// Saves all the app restriction configuration changes from the custom activity.
			Intent intent = new Intent (Activity.Intent);
			intent.PutParcelableArrayListExtra (Intent.ExtraRestrictionsList, new List<IParcelable> (mRestrictions));
			Activity.SetResult (Result.Ok, intent);
		}
	}
}

