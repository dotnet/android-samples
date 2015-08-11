/*
 * Copyright 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace ActionBarCompatListPopupMenu
{
	/// <summary>
	/// This ListFragment displays a list of cheeses, with a clickable view on each item whichs displays
	/// a <see cref="Android.Support.V7.Widget.PopupMenu"/> when clicked, allowing the user to
	/// remove the item from the list.
	/// </summary>
	public class PopupListFragment : ListFragment, View.IOnClickListener
	{
		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			// We want to allow modifications to the list so copy the dummy data array into an ArrayList
			List<string> items = new List<String> (Cheeses.CHEESES);

			// Set the ListAdapter
			ListAdapter = new PopupAdapter (items, this);
		}

		public override void OnListItemClick (ListView lValue, View vValue, int position, long idValue)
		{
			var item = (string)lValue.GetItemAtPosition (position);

			// Show a toast if the user clicks on an item
			Toast.MakeText (Activity, "Item Clicked: " + item, ToastLength.Short).Show ();
		}

		public void OnClick (View view)
		{
			// We need to post a Runnable to show the popup to make sure that the PopupMenu is
			// correctly positioned. The reason being that the view may change position before the
			// PopupMenu is shown.
			view.Post (new MyRunnable (view, this));
		}

		class MyRunnable : Java.Lang.Object, Java.Lang.IRunnable
		{
			readonly View myView;
			readonly PopupListFragment myPopupListFragment;

			public MyRunnable (View view, PopupListFragment popupListFragment)
			{
				myView = view;
				myPopupListFragment = popupListFragment;
			}

			public void Run ()
			{
				myPopupListFragment.ShowPopupMenu (myView);
			}
		}

		void ShowPopupMenu (View view)
		{
			var adapter = (PopupAdapter)ListAdapter;

			// Retrieve the clicked item from view's tag
			var item = (string)view.Tag;

			// Create a PopupMenu, giving it the clicked view for an anchor
			Android.Support.V7.Widget.PopupMenu popup = new Android.Support.V7.Widget.PopupMenu (Activity, view);

			// Inflate our menu resource into the PopupMenu's Menu
			popup.MenuInflater.Inflate (Resource.Menu.popup, popup.Menu);

			// Set a listener so we are notified if a menu item is clicked
			popup.SetOnMenuItemClickListener (new MyOnMenuItemClickListener (adapter, item));

			// Finally show the PopupMenu
			popup.Show ();
		}


		class MyOnMenuItemClickListener : Java.Lang.Object, Android.Support.V7.Widget.PopupMenu.IOnMenuItemClickListener
		{

			readonly PopupAdapter myAdapter;
			readonly string myItemTag;

			public MyOnMenuItemClickListener (PopupAdapter adapter, string itemTag)
			{
				myAdapter = adapter;
				myItemTag = itemTag;
			}

			public bool OnMenuItemClick (IMenuItem item)
			{
				switch (item.ItemId) {
				case Resource.Id.menu_remove:
					// Remove the item from the adapter
					myAdapter.Remove (myItemTag);
						
					return true;
				}
				return false;
			}
		}

		class PopupAdapter : ArrayAdapter<String>
		{
			readonly PopupListFragment myPopupListFragment;

			public PopupAdapter (List<string> items, PopupListFragment popupListFragment)
				: base (popupListFragment.Activity, Resource.Layout.list_item, Android.Resource.Id.Text1, items)
			{
				myPopupListFragment = popupListFragment;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				// Let ArrayAdapter inflate the layout and set the text
				View view = base.GetView (position, convertView, parent);

				// Retrieve the popup button from the inflated view
				View popupButton = view.FindViewById (Resource.Id.button_popup);

				// Set the item as the button's tag so it can be retrieved later
				popupButton.Tag = GetItem (position);

				// Set the fragment instance as the OnClickListener
				popupButton.SetOnClickListener (myPopupListFragment);

				// Finally return the view to be displayed
				return view;
			}
		}
	}
}

