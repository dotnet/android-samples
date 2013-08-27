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
using Android.Database;
using Android.Provider;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Gallery/2. People")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class Gallery2 : Activity
	{
		String[] CONTACT_PROJECTION = new String[] {
			ContactsContract.Contacts.InterfaceConsts.Id,
			ContactsContract.Contacts.InterfaceConsts.DisplayName
		};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.gallery_2);

			// Get a cursor with all people
			ICursor c = ContentResolver.Query (ContactsContract.Contacts.ContentUri, CONTACT_PROJECTION, null, null, null);
			StartManagingCursor (c);

			var adapter = new SimpleCursorAdapter (this,
			                                       // Use a template that displays a text view
			                                       Android.Resource.Layout.SimpleGalleryItem,
			                                       // Give the cursor to the list adatper
			                                        c,
			                                       // Map the NAME column in the people database to...
			                                        new String[] {ContactsContract.Contacts.InterfaceConsts.DisplayName},

			// The "text1" view defined in the XML template
			new int[] { Android.Resource.Id.Text1 });

			Gallery g = FindViewById <Gallery> (Resource.Id.gallery);
			g.Adapter = adapter;
		}
	}
}

