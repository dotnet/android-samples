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
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content.Res;
using Android.Preferences;
using System.Collections;
using System.Collections.Generic;

namespace AppRestrictions
{
	[BroadcastReceiver (Name = "com.xamarin.apprestrictions.GetRestrictionsReciever")]
	[IntentFilter (new[] {Intent.ActionGetRestrictionEntries})]
	public class GetRestrictionsReceiver : BroadcastReceiver
	{
		static readonly String TAG = typeof (GetRestrictionsReceiver).Name;

		// Keys for referencing app restriction settings from the platform.
		public static readonly String KEY_BOOLEAN = "boolean_key";
		public static readonly String KEY_CHOICE = "choice_key";
		public static readonly String KEY_MULTI_SELECT = "multi_key";

		public override void OnReceive (Context context, Intent intent)
		{
			PendingResult result = GoAsync ();

			// If app restriction settings are already created, they will be included in the Bundle
			// as key/value pairs.
			Bundle existingRestrictions = intent.GetBundleExtra (Intent.ExtraRestrictionsBundle);
			Log.Info (TAG, "existingRestrictions = " + existingRestrictions);

			Thread thread = new Thread (new ThreadStart (delegate {
				CreateRestrictions (context, result, existingRestrictions);
			}));

			thread.Start ();	
		}

		// Initializes a boolean type restriction entry.
		public static void PopulateBooleanEntry (Resources res, RestrictionEntry entry)
		{
			entry.Type = RestrictionEntryType.Boolean;
			entry.Title = res.GetString (Resource.String.boolean_entry_title);
		}

		// Initializes a single choice type restriction entry.
		public static void PopulateChoiceEntry (Resources res, RestrictionEntry reSingleChoice)
		{
			String[] choiceEntries = res.GetStringArray (Resource.Array.choice_entry_entries);
			String[] choiceValues = res.GetStringArray (Resource.Array.choice_entry_values);

			if (reSingleChoice.SelectedString == null)
				reSingleChoice.SelectedString =  choiceValues [0];

			reSingleChoice.Title = res.GetString (Resource.String.choice_entry_title);
			reSingleChoice.SetChoiceEntries (choiceEntries);
			reSingleChoice.SetChoiceValues (choiceValues);
			reSingleChoice.Type = RestrictionEntryType.Choice;
		}

		// Initializes a multi-select type restriction entry.
		public static void PopulateMultiEntry (Resources res, RestrictionEntry reMultiSelect)
		{
			String[] multiEntries = res.GetStringArray (Resource.Array.multi_entry_entries);
			String[] multiValues = res.GetStringArray (Resource.Array.multi_entry_values);

			if (reMultiSelect.GetAllSelectedStrings() == null)
				reMultiSelect.SetAllSelectedStrings (new String[0]);

			reMultiSelect.Title = res.GetString (Resource.String.multi_entry_title);
			reMultiSelect.SetChoiceEntries (multiEntries);
			reMultiSelect.SetChoiceValues (multiValues);
			reMultiSelect.Type = RestrictionEntryType.MultiSelect;
		}

		// Demonstrates the creation of standard app restriction types: boolean, single choice, and
		// multi-select.
		List <IParcelable> InitRestrictions (Context context)
		{
			List <IParcelable> newRestrictions = new List <IParcelable> ();
			Resources res = context.Resources;

			RestrictionEntry reBoolean = new RestrictionEntry (KEY_BOOLEAN, false);
			PopulateBooleanEntry (res, reBoolean);
			newRestrictions.Add (reBoolean);

			RestrictionEntry reSingleChoice = new RestrictionEntry (KEY_CHOICE, (String) null);
			PopulateChoiceEntry (res, reSingleChoice);
			newRestrictions.Add (reSingleChoice);

			RestrictionEntry reMultiSelect = new RestrictionEntry (KEY_MULTI_SELECT, (String[]) null);
			PopulateMultiEntry (res, reMultiSelect);
			newRestrictions.Add (reMultiSelect);

			return newRestrictions;
		}

		private void CreateRestrictions (Context context, PendingResult result, Bundle existingRestrictions) 
		{
			// The incoming restrictions bundle contains key/value pairs representing existing app
			// restrictions for this package. In order to retain existing app restrictions, you need to
			// construct new restriction entries and then copy in any existing values for the new keys.
			List <IParcelable> newEntries = InitRestrictions (context);

			// If app restrictions were not previously configured for the package, create the default
			// restrictions entries and return them.
			if (existingRestrictions == null) {
				var extras = new Bundle ();
				extras.PutParcelableArrayList (Intent.ExtraRestrictionsList, newEntries);
				result.SetResult (Result.Ok, null, extras);
				result.Finish ();
				return;
			}

			// Retains current restriction settings by transferring existing restriction entries to
			// new ones.
			foreach (RestrictionEntry entry in newEntries) {
				String key = entry.Key;

				if (KEY_BOOLEAN.Equals (key)) {
					entry.SelectedState = existingRestrictions.GetBoolean (KEY_BOOLEAN);

				} else if (KEY_CHOICE.Equals (key)) {
					if (existingRestrictions.ContainsKey (KEY_CHOICE)) {
						entry.SelectedString = existingRestrictions.GetString (KEY_CHOICE);
					}
			
				} else if (KEY_MULTI_SELECT.Equals (key)) {
					if (existingRestrictions.ContainsKey (KEY_MULTI_SELECT)) {
						entry.SetAllSelectedStrings(existingRestrictions.GetStringArray (key));
					}
				}
			}

			var extra = new Bundle();

			// This path demonstrates the use of a custom app restriction activity instead of standard
			// types.  When a custom activity is set, the standard types will not be available under
			// app restriction settings.
			//
			// If your app has an existing activity for app restriction configuration, you can set it
			// up with the intent here.
			if (PreferenceManager.GetDefaultSharedPreferences (context)
			    .GetBoolean (MainActivity.CUSTOM_CONFIG_KEY, false)) {
				var customIntent = new Intent();
				customIntent.SetClass (context, typeof (CustomRestrictionsActivity));
				extra.PutParcelable (Intent.ExtraRestrictionsIntent, customIntent);
			}

			extra.PutParcelableArrayList (Intent.ExtraRestrictionsList, newEntries);
			result.SetResult (Result.Ok, null, extra);
			result.Finish ();
		}
	}
}

