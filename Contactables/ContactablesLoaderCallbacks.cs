/*
 * Copyright (C) 2012 The Android Open Source Project
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
using Android.Database;
using Android.Provider;
using Android.Util;

namespace Contactables
{
	/**
 	* Helper class to handle all the callbacks that occur when interacting with loaders.  Most of the
 	* interesting code in this sample app will be in this file.
 	*/
	public class ContactablesLoaderCallbacks : Java.Lang.Object, LoaderManager.ILoaderCallbacks
	{
		Context mContext;

		public static readonly String QUERY_KEY = "query";

		public static readonly String TAG = "ContactablesLoaderCallbacks";

		public ContactablesLoaderCallbacks (Context context)
		{
			mContext = context;
		}

		public Loader OnCreateLoader (int loaderIndex, Bundle args)
		{
			// Where the Contactables table excels is matching text queries,
			// not just data dumps from Contacts db.  One search term is used to query
			// display name, email address and phone number.  In this case, the query was extracted
			// from an incoming intent in the handleIntent() method, via the
			// intent.getStringExtra() method.

			String query = args.GetString (QUERY_KEY);
			var uri = Android.Net.Uri.WithAppendedPath (ContactsContract.CommonDataKinds.Contactables.ContentFilterUri, query);

			// Easy way to limit the query to contacts with phone numbers.
			String selection = ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.HasPhoneNumber + " = " + 1;

			// Sort results such that rows for the same contact stay together.
			String sortBy = ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.LookupKey;

			return new CursorLoader (
				mContext,  // Context
				uri,       // URI representing the table/resource to be queried
				null,      // projection - the list of columns to return.  Null means "all"
				selection, // selection - Which rows to return (condition rows must match)
				null,      // selection args - can be provided separately and subbed into selection.
				sortBy);   // string specifying sort order

		}

		public void OnLoadFinished (Loader loader, Java.Lang.Object data)
		{
			var cursor = data.JavaCast<ICursor> ();

			TextView tv = ((Activity) mContext).FindViewById <TextView> (Resource.Id.sample_output);

			// Reset text in case of a previous query
			tv.Text = mContext.GetText (Resource.String.intro_message) + "\n\n";

			if (cursor.Count == 0) {
				return;
			}

			// Pulling the relevant value from the cursor requires knowing the column index to pull
			// it from.

			int phoneColumnIndex =  cursor.GetColumnIndex (ContactsContract.CommonDataKinds.Phone.Number);
			int emailColumnIndex = cursor.GetColumnIndex (ContactsContract.CommonDataKinds.Email.Address);
			int nameColumnIndex = cursor.GetColumnIndex (ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.DisplayName);
			int lookupColumnIndex = cursor.GetColumnIndex (ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.LookupKey);
			int typeColumnIndex = cursor.GetColumnIndex (ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.Mimetype);


			cursor.MoveToFirst ();
			// Lookup key is the easiest way to verify a row of data is for the same
			// contact as the previous row.
			String lookupKey = "";
			do {

				String currentLookupKey = cursor.GetString (lookupColumnIndex);
				if (!lookupKey.Equals (currentLookupKey)) {
					String displayName = cursor.GetString (nameColumnIndex);
					tv.Append (displayName + "\n");
					lookupKey = currentLookupKey;
				}


				// The data type can be determined using the mime type column.
				String mimeType = cursor.GetString (typeColumnIndex);
				if (mimeType.Equals (ContactsContract.CommonDataKinds.Phone.ContentItemType)) {
					tv.Append ("\tPhone Number: " + cursor.GetString (phoneColumnIndex) + "\n");
				} else if (mimeType.Equals (ContactsContract.CommonDataKinds.Email.ContentItemType)) {
					tv.Append ("\tEmail Address: " + cursor.GetString (emailColumnIndex) + "\n");
				}


				// Look at DDMS to see all the columns returned by a query to Contactables.
				// Behold, the firehose!
				foreach (String column in cursor.GetColumnNames ()) {
					  Log.Debug (TAG, column + column + ": " +
					      cursor.GetString (cursor.GetColumnIndex (column)) + "\n");
				}
			} while (cursor.MoveToNext ());
		}
	
		public void OnLoaderReset (Loader cursorLoader)
		{
		}
	}
}

