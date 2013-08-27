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
 	* A list view example where the
 	* data comes from a cursor, and a
 	* SimpleCursorListAdapter is used to map each item to a two-line
 	* display.
 	*/
	[Activity (Label = "Views/Lists/03. Cursor (Phones)")]	
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class List3 : ListActivity
	{
		static string PhoneType   = ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type;
		static string PhoneNumber = ContactsContract.CommonDataKinds.Phone.Number;

		static string[] PHONE_PROJECTION = new String[] {
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Id,
			PhoneType,
			ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label,
			PhoneNumber			
		};

		 static int COLUMN_TYPE = 1;
		 static int COLUMN_LABEL = 2;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Get a cursor with all phones
			ICursor c = ContentResolver.Query (ContactsContract.CommonDataKinds.Phone.ContentUri,
			                                   PHONE_PROJECTION, null, null, null);
			StartManagingCursor (c);

			// Map Cursor columns to views defined in simple_list_item_2.xml
			var adapter = new SimpleCursorAdapter (this, Android.Resource.Layout.SimpleListItem2,
			                                       c, new String[] {PhoneType, PhoneNumber},
												   new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 });

			//Used to display a readable string for the phone type
			adapter.ViewBinder = new SimpleViewBinder (this);

			ListAdapter = adapter;
		}

		class SimpleViewBinder : Java.Lang.Object, SimpleCursorAdapter.IViewBinder
		{
			Context self;

			public SimpleViewBinder (Context s)
			{
				self = s;	
			}

			public bool SetViewValue (View view, ICursor cursor, int columnIndex)
			{
				//Let the adapter handle the binding if the column is not TYPE
                if (columnIndex != COLUMN_TYPE) {
                    return false;
                }

                int type = cursor.GetInt (COLUMN_TYPE);
                string label = null;

                //Custom type? Then get the custom label
				if (type == ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom) {
                    label = cursor.GetString (COLUMN_LABEL);
                }
                //Get the readable string
				string text = (String) ContactsContract.CommonDataKinds.Phone.GetTypeLabel (self.Resources, (PhoneDataKind)type, label);

                //Set text
                ((TextView) view).Text = text;

                return true;
			}
		}
	}
}

