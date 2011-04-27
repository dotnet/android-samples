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

using System;
using Android.App;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Widget;

namespace ContactManager
{
	[Activity (Label = "List Contacts", MainLauncher = true)]
	public class ContactManager : Activity
	{
		private ListView contact_list;
		private bool show_invisible;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.ContactManager);

			// Obtain handles to UI objects
			CheckBox show_invisible_checkbox = FindViewById<CheckBox> (Resource.Id.showInvisible);
			
			contact_list = FindViewById<ListView> (Resource.Id.contactList);

			// Initialize class properties
			show_invisible_checkbox.Checked = show_invisible;

			// Register handler for UI elements
			show_invisible_checkbox.CheckedChange += (o, e) => {
				show_invisible = show_invisible_checkbox.Checked;
				PopulateContactList ();
			};

			// Populate the contact list
			PopulateContactList ();
		}

		// Populate the contact list based on account currently selected in the account spinner.
		private void PopulateContactList ()
		{
			// Build adapter with contact entries
			ICursor cursor = GetContacts ();

			string[] fields = new string[] { 
				ContactsContract.ContactsColumnsConsts.DisplayName
			};

			SimpleCursorAdapter adapter = new SimpleCursorAdapter (this, Resource.Layout.ContactEntry, cursor,
				fields, new int[] { Resource.Id.contactEntryText });

			contact_list.Adapter = adapter;
		}

		// Obtains the contact list for the currently selected account.
		private ICursor GetContacts ()
		{
			// Run query
			Android.Net.Uri uri = ContactsContract.Contacts.ContentUri;

			String[] projection = new String[] {
				BaseColumnsConsts.Id,
				ContactsContract.ContactsColumnsConsts.DisplayName
		        };

			String selection = string.Format ("{0} = '{1}'", ContactsContract.ContactsColumnsConsts.InVisibleGroup, show_invisible ? "0" : "1");
			String[] selectionArgs = null;
			String sortOrder = string.Format ("{0} COLLATE LOCALIZED ASC", ContactsContract.ContactsColumnsConsts.DisplayName);

			return ManagedQuery (uri, projection, selection, selectionArgs, sortOrder);
		}
	}
}

