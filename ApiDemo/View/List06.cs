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
 	* A list view example where the 
 	* data comes from a custom
 	* ListAdapter
 	*/
	[Activity (Label = "Views/Lists/06. ListAdapter Collapsed")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class List6 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Use our own list adapter
			ListAdapter = new SpeechListAdapter2 (this);

			ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e) {
				((SpeechListAdapter2) ListAdapter).Toggle (e.Position);
			};
		}
	}

	class SpeechListAdapter2 : BaseAdapter
	{
		Context self;

		public SpeechListAdapter2 (Context s)
		{
			self = s;
		}

		/**
         * The number of items in the list is determined by the number of speeches
         * in our array.
         */
		public override int Count {
			get {
				return Shakespeare.Titles.Length;
			}
		}

		/**
         * Since the data comes from an array, just returning
         * the index is sufficent to get at the data. If we
         * were using a more complex data structure, we
         * would return whatever object represents one 
         * row in the list.
         */
		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		/**
         * Use the array index as a unique id.
         */
		public override long GetItemId (int position)
		{
			return position;
		}

		/**
         * Make a SpeechView to hold each row.
         */
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			SpeechView2 sv;
			if (convertView == null) {
				sv = new SpeechView2 (self, Shakespeare.Titles[position], Shakespeare.Dialogue[position], mExpanded[position]);
			} else {
				sv = (SpeechView2)convertView;
				sv.Title = Shakespeare.Titles[position];
				sv.Dialogue = Shakespeare.Dialogue[position];
				sv.Expanded = mExpanded[position];
			}

			return sv;
		}

		public void Toggle (int position)
		{
			mExpanded[position] = !mExpanded[position];
			NotifyDataSetChanged ();
		}

		/**
         * Our data, part 3.
         */
		bool[] mExpanded = 
		{
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false   
		};
	}

	
	class SpeechView2 : LinearLayout
	{
		TextView mTitle;
		TextView mDialogue;

		public SpeechView2 (Context context, string title, string words, bool expanded) : base (context)
		{
			Orientation = Orientation.Vertical;

			// Here we build the child views in code. They could also have
			// been specified in an XML file.
			mTitle = new TextView (context);
			mTitle.Text = title;
			AddView (mTitle, new LinearLayout.LayoutParams (
				LayoutParams.MatchParent, LayoutParams.WrapContent));

			mDialogue = new TextView (context);
			mDialogue.Text = words;
			AddView (mDialogue, new LinearLayout.LayoutParams(
				LayoutParams.MatchParent, LayoutParams.WrapContent));

			mDialogue.Visibility = expanded ? ViewStates.Visible : ViewStates.Gone;

		}

		public string Title {
			set { mTitle.Text = value; }
		}

		public string Dialogue {
			set { mDialogue.Text = value; }
		}

		public bool Expanded {
			set { mDialogue.Visibility = value ? ViewStates.Visible : ViewStates.Gone; }
		}

	}
}

