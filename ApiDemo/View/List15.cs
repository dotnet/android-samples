/*
 * Copyright (C) 2010 The Android Open Source Project
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
 	* This demo illustrates the use of CHOICE_MODE_MULTIPLE_MODAL, a.k.a. selection mode on ListView.
 	*/
	[Activity (Label = "Views/Lists/15. Selection Mode", Name = "monodroid.apidemo.List15")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]						
	public class List15 : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.RequestFeature (WindowFeatures.ActionBar);

			ListView.ChoiceMode = (ChoiceMode)AbsListViewChoiceMode.MultipleModal;
			ListView.SetMultiChoiceModeListener (new ModeCallback (this));

			ListAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItemChecked, Cheeses.CheeseStrings);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActionBar.Subtitle = "Long press to start selection";
		}
	}

	class ModeCallback : Java.Lang.Object, ListView.IMultiChoiceModeListener
	{
		List15 self;

		public ModeCallback (List15 s)
		{
			self = s;
		}

		public bool OnCreateActionMode (ActionMode mode, IMenu menu)
		{
			self.MenuInflater.Inflate (Resource.Menu.list_select_menu, menu);
			mode.Title = "Select Items";
			SetSubtitle (mode);
			return true;
		}

		public bool OnPrepareActionMode (ActionMode mode, IMenu menu)
		{
			return true;
		}

		public bool OnActionItemClicked (ActionMode mode, IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.share:
				Toast.MakeText (self, "Shared " + self.ListView.CheckedItemCount +
				               " items", ToastLength.Short).Show ();
				mode.Finish ();
				break;

			default:
				Toast.MakeText (self, "Clicked " + item.TitleFormatted,
				               ToastLength.Short).Show ();
				break;
			}
			return true;
		}

		public void OnDestroyActionMode (ActionMode mode)
		{
		}

		public void OnItemCheckedStateChanged (ActionMode mode, int position, long id, bool check)
		{
			SetSubtitle (mode);
		}

		private void SetSubtitle (ActionMode mode)
		{
			int checkedCount =   self.ListView.CheckedItemCount;
			switch (checkedCount) {
			case 0:
				mode.Subtitle = null;
				break;

			case 1:
				mode.Subtitle = "One item selected";
				break;

			default:
				mode.Subtitle = "" + checkedCount + " items selected";
				break;
			}
		}
	}
}

