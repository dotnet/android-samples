using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;

using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Widget;



namespace AndroidSupportSample
{
	[Activity (Label = "@string/action_bar_usage", Theme = "@style/Theme.AppCompat")]				
	[Android.Runtime.Register ("android/support/sample/v7/ActionBarUsage")]
	public class ActionBarUsage : ActionBarActivity
	{
		TextView mSearchText;
		int mSortMode = -1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			mSearchText = new TextView(this);
			SetContentView(mSearchText);
		}

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			var inflater = MenuInflater;
			inflater.Inflate(Resource.Menu.actions, menu);
			var arg1 = menu.FindItem (Resource.Id.action_search);

			var test = new Android.Support.V7.Widget.SearchView (this);
			arg1.SetActionView (test);

			var searchView = MenuItemCompat.GetActionView (arg1);
			var searchView2 = searchView as Android.Support.V7.Widget.SearchView;
			searchView2.QueryTextChange += (sender, e) => {
				e.Handled = OnQueryTextChange (e.NewText);
			};

			searchView2.QueryTextSubmit += (sender, e) => {
				e.Handled = OnQueryTextSubmit (e.Query);
			};

			return true;
		}

		public override bool OnPrepareOptionsMenu (Android.Views.IMenu menu)
		{
			if (mSortMode != -1) {
				var icon = menu.FindItem (mSortMode).Icon;
				menu.FindItem(Resource.Id.action_sort).SetIcon (icon);
			}
			return base.OnPrepareOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			Toast.MakeText(this, "Selected Item: " + item.TitleFormatted, Android.Widget.ToastLength.Short).Show();
			return true;
		}

		[Java.Interop.Export ("onSort")]
		public void OnSort( Android.Views.IMenuItem item) {
			mSortMode = item.ItemId;
			// Request a call to onPrepareOptionsMenu so we can change the sort icon
			SupportInvalidateOptionsMenu();
		}

		public bool OnQueryTextChange (string newText)
		{
			newText = string.IsNullOrEmpty (newText) ? "" : "Query so far: " + newText;
			mSearchText.Text = newText;
			return true;
		}

		public bool OnQueryTextSubmit (string query)
		{
			Toast.MakeText(this,
			               "Searching for: " + query + "...", Android.Widget.ToastLength.Short).Show();
			return true;
		}


	}
}

