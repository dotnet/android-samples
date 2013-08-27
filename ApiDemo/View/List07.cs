/*
 * Copyright (C) 2007 The Android Open Source Project
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
using Android.Provider;
using Android.Database;

namespace MonoDroid.ApiDemo
{
	/**
 	* A list view example where the data comes from a cursor.
 	*/
	[Activity (Label = "Views/Lists/07. Cursor (Phones)")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]				
	public class List7 : ListActivity
	{
		TextView mPhone;

		static string[] PHONE_PROJECTION = new string[] {
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Id,
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label,
			ContactsContract.CommonDataKinds.Phone.Number,
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName
		};

		const int COLUMN_PHONE_TYPE = 1;
		const int COLUMN_PHONE_LABEL = 2;
		const int COLUMN_PHONE_NUMBER = 3;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list_7);
			mPhone = FindViewById <TextView> (Resource.Id.phone);
			ListView.ItemClick += List7ItemSelected;


			// Get a cursor with all numbers.
			// This query will only return contacts with phone numbers
			ICursor c = ContentResolver.Query (ContactsContract.CommonDataKinds.Phone.ContentUri,
			                                   PHONE_PROJECTION, ContactsContract.CommonDataKinds.Phone.Number 
			                                   + " NOT NULL", null, null);
			StartManagingCursor(c);

			var adapter = new SimpleCursorAdapter (this, Android.Resource.Layout.SimpleListItem1, c,
			                                       new string[] {ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName},
												   new int[] {Android.Resource.Id.Text1});
			ListAdapter = adapter;
		}

		void List7ItemSelected (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (e.Position >= 0) {

				//Get current cursor
				var c = (ICursor) e.Parent.GetItemAtPosition (e.Position);
				int type = c.GetInt (COLUMN_PHONE_TYPE);
				string phone = c.GetString (COLUMN_PHONE_NUMBER);
				string label = null;

				//Custom type? Then get the custom label
				if (type == ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom) {
					label = c.GetString (COLUMN_PHONE_LABEL);
				}

				//Get the readable string
				var numberType = (String) ContactsContract.CommonDataKinds.Phone.GetTypeLabel (Resources, (PhoneDataKind)type, label);
				string text = numberType + ": " + phone;
				mPhone.Text = text;
			}
		}
	}
}

