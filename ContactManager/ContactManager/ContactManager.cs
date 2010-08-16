/*
 * Copyright (C) 2009 The Android Open Source Project
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

namespace Novell.MonoDroid.Samples.ContactManager
{
	using Android.App;
	using Android.Content;
	using Android.Database;
	using Android.Net;
	using Android.OS;
	using Android.Provider;
	using Android.Util;
	using Android.Views;
	using Android.Widget;

	public class ContactManager : Activity
	{
		public static string TAG = "ContactManager";

		private Button mAddAccountButton;
		private ListView mContactList;
		private bool mShowInvisible;
		private CheckBox mShowInvisibleControl;

		/**
		 * Called when the activity is first created. Responsible for initializing the UI.
		 */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			Log.V (TAG, "Activity State: onCreate()");
			base.OnCreate (savedInstanceState);

			SetContentView (R.layout.contact_manager);

			// Obtain handles to UI objects
			mAddAccountButton = (Button) FindViewById (R.id.addContactButton);
			mContactList = (ListView) FindViewById (R.id.contactList);
			mShowInvisibleControl = (CheckBox) FindViewById (R.id.showInvisible);

			// Initialize class properties
			mShowInvisible = false;
			mShowInvisibleControl.Checked = mShowInvisible;

#if FUCK
        // Register handler for UI elements
        mAddAccountButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                Log.d(TAG, "mAddAccountButton clicked");
                launchContactAdder();
            }
        });
        mShowInvisibleControl.setOnCheckedChangeListener(new OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                Log.d(TAG, "mShowInvisibleControl changed: " + isChecked);
                mShowInvisible = isChecked;
                populateContactList();
            }
        });
#endif

			// Populate the contact list
			populateContactList ();
		}

		/**
		 * Populate the contact list based on account currently selected in the account spinner.
		 */
		private void populateContactList ()
		{
			// Build adapter with contact entries
			Cursor cursor = getContacts ();
			string[] fields = new string[] {
				// ContactsContract.Data.DISPLAY_NAME
				"DISPLAY_NAME"
			};

#if FIXME
			SimpleCursorAdapter adapter = new SimpleCursorAdapter (this, R.layout.contact_entry, cursor,
				fields, new int[] { R.id.contactEntryText });
			mContactList.SetAdapter (adapter);
#endif
		}

		/**
		 * Obtains the contact list for the currently selected account.
		 *
		 * @return A cursor for for accessing the contact list.
		 */
		private Cursor getContacts ()
		{
			// Run query
			Uri uri = ContactsContract.Contacts.CONTENT_URI;
			string[] projection = new string[] {
#if FIXME
				ContactsContract.Contacts._ID,
				ContactsContract.Contacts.DISPLAY_NAME
#else
				"_ID", "DISPLAY_NAME"
#endif
        };

#if FIXME
			string selection = ContactsContract.Contacts.IN_VISIBLE_GROUP + " = '" +
				(mShowInvisible ? "0" : "1") + "'";
			string[] selectionArgs = null;
			string sortOrder = ContactsContract.Contacts.DISPLAY_NAME + " COLLATE LOCALIZED ASC";

			return ManagedQuery (uri, projection, selection, selectionArgs, sortOrder);
#else
			throw new System.NotImplementedException ();
#endif
		}

		/**
		 * Launches the ContactAdder activity to add a new contact to the selected accont.
		 */
		protected void LaunchContactAdder ()
		{
#if FIXME
			Intent i = new Intent(this, ContactAdder.class);
			startActivity(i);
#endif
		}
	}
}
