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

namespace MonoDroid.ApiDemo
{
	/**
 	* A list view example with separators.
 	*/
	[Activity (Label = "Views/Lists/05. Separators")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class List5 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			ListAdapter = new MyListAdapter (this);
		}
	}

	class MyListAdapter : BaseAdapter 
	{
		Context self;

		public MyListAdapter (Context s)
		{
			self = s;
		}

		public override int Count {
			get {
				return mStrings.Length;
			}
		}

		public override bool AreAllItemsEnabled ()
		{
			return false;
		}

		public override bool IsEnabled (int position)
		{
			return !mStrings[position].StartsWith ("-");
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TextView tv;

			if (convertView == null) {
				tv = (TextView) LayoutInflater.From (self).Inflate (
					Android.Resource.Layout.SimpleExpandableListItem1, parent, false);
			} else {
				tv = (TextView) convertView;
			}
			tv.Text = mStrings[position];
			return tv;
		}

		string[] mStrings = {
			"----------",
			"----------",
			"Abbaye de Belloc",
			"Abbaye du Mont des Cats",
			"Abertam",
			"----------",
			"Abondance",
			"----------",
			"Ackawi",
			"Acorn",
			"Adelost",
			"Affidelice au Chablis",
			"Afuega'l Pitu",
			"Airag",
			"----------",
			"Airedale",
			"Aisy Cendre",
			"----------",
			"Allgauer Emmentaler",
			"Alverca",
			"Ambert",
			"American Cheese",
			"Ami du Chambertin",
			"----------",
			"----------",
			"Anejo Enchilado",
			"Anneau du Vic-Bilh",
			"Anthoriro",
			"----------",
			"----------"
		};
	}
}

