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
using Java.Lang;


namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Auto Complete/4. Contacts")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class AutoComplete4 : Activity
	{
		public static System.String[] CONTACT_PROJECTION = new System.String[] {
			ContactsContract.Contacts.InterfaceConsts.Id,
			ContactsContract.Contacts.InterfaceConsts.DisplayName
		};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.autocomplete_4);

			ICursor cursor = ContentResolver.Query (ContactsContract.Contacts.ContentUri, CONTACT_PROJECTION, null, null, null);

			ContactListAdapter adapter = new ContactListAdapter (this, cursor);

			AutoCompleteTextView textView = FindViewById <AutoCompleteTextView> (Resource.Id.edit);
			textView.Adapter = adapter;
		}
	}

	// XXX compiler bug in javac 1.5.0_07-164, we need to implement Filterable
	// to make compilation work
	public class ContactListAdapter : CursorAdapter, IFilterable
	{
		static int COLUMN_DISPLAY_NAME = 1;
		ContentResolver mContent;

		public ContactListAdapter (Context context, ICursor c) : base (context, c)
		{
			mContent = context.ContentResolver;
		}

		public override View NewView (Context context, Android.Database.ICursor cursor, ViewGroup parent)
		{
			LayoutInflater inflater = LayoutInflater.From (context);
			TextView view = (TextView) inflater.Inflate (
				Android.Resource.Layout.SimpleDropDownItem1Line, parent, false);
			view.Text = cursor.GetString (COLUMN_DISPLAY_NAME);

			return view;
		}

		public override void BindView (View view, Context context, Android.Database.ICursor cursor)
		{
			((TextView) view).Text = cursor.GetString (COLUMN_DISPLAY_NAME);
		}

		public override ICharSequence ConvertToStringFormatted (ICursor cursor)
		{
			var convertMe = new string[1];
			convertMe [0] = cursor.GetString (COLUMN_DISPLAY_NAME);

			var converted = CharSequence.ArrayFromStringArray (convertMe);
			return converted [0];
		}

		public override Android.Database.ICursor RunQueryOnBackgroundThread (Java.Lang.ICharSequence constraint)
		{
			IFilterQueryProvider filter = FilterQueryProvider;

			if (filter != null) {
				return filter.RunQuery (constraint);
			}

			Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath (
				ContactsContract.Contacts.ContentFilterUri,
				Android.Net.Uri.Encode (""));

			if (constraint != null) {
				uri = Android.Net.Uri.WithAppendedPath (
					ContactsContract.Contacts.ContentFilterUri,
					Android.Net.Uri.Encode (constraint.ToString ()));
			}

			return mContent.Query (uri, AutoComplete4.CONTACT_PROJECTION, null, null, null);
		}
	}
}

