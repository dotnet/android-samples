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
using Android.Support.V4.App;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Database;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Text;

using SimpleCursorAdapter = Android.Support.V4.Widget.SimpleCursorAdapter;

namespace Support4
{
	[Activity (Label = "@string/loader_cursor_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class LoaderCursorSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var fm = SupportFragmentManager;

	        // Create the list fragment and add it as our sole content.
	        if (fm.FindFragmentById(Android.Resource.Id.Content) == null) {
	            var list = new CursorLoaderListFragment (this);
	            fm.BeginTransaction().Add(Android.Resource.Id.Content, list).Commit();
	        }
		}
		
		public class CursorLoaderListFragment : ListFragment, LoaderManager.ILoaderCallbacks
		{
			LoaderCursorSupport parent;

            public CursorLoaderListFragment() {}

			public CursorLoaderListFragment (LoaderCursorSupport parent)
			{
				this.parent = parent;
			}
			
			// This is the Adapter being used to display the list's data.
	        SimpleCursorAdapter _adapter;
	
	        // If non-null, this is the current filter the user has provided.
	        string _curFilter;
			
			public override void OnActivityCreated (Bundle p0)
			{
				base.OnActivityCreated (p0);
				
				// Give some text to display if there is no data.  In a real
	            // application this would come from a resource.
	            SetEmptyText("No phone numbers");
	
	            // We have a menu item to show in action bar.
	            SetHasOptionsMenu(true);
	
	            // Create an empty adapter we will use to display the loaded data.
	            _adapter = new SimpleCursorAdapter(Activity,
	                    Android.Resource.Layout.SimpleListItem1, null,
	                    new string[] { Android.Provider.Contacts.People.InterfaceConsts.DisplayName },
	                    new int[] {Android.Resource.Id.Text1});
	            
				ListAdapter = _adapter;
	
	            // Start out with a progress indicator.
	            SetListShown(false);
				
	
	            // Prepare the loader.  Either re-connect with an existing one,
	            // or start a new one.
				LoaderManager.InitLoader (0, null, this);
			}
			
			class MyOnQueryTextListenerCompat : SearchViewCompat.OnQueryTextListenerCompat
			{
				CursorLoaderListFragment parent;
				
				public MyOnQueryTextListenerCompat (CursorLoaderListFragment parent)
				{
					this.parent = parent;
				}
				
				public override bool OnQueryTextChange (string newText)
				{
                    // Called when the action bar search text has changed.  Update
                    // the search filter, and restart the loader to do a new query
                    // with this filter.
					parent._curFilter = !TextUtils.IsEmpty(newText) ? newText : null;
					parent.LoaderManager.RestartLoader (0, null, parent);
					return true;
				}
			}
			
			public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
			{
				// Place an action bar item for searching.
	            var item = menu.Add("Search");
	            item.SetIcon(Android.Resource.Drawable.IcMenuSearch);
				MenuCompat.SetShowAsAction(item, MenuItemCompat.ShowAsActionAlways);
	            var searchView = SearchViewCompat.NewSearchView(Activity);
	            if (searchView != null) {
	                SearchViewCompat.SetOnQueryTextListener (searchView, new MyOnQueryTextListenerCompat (this));
	                MenuItemCompat.SetActionView(item, searchView);
	            }
			}
			
			public override void OnListItemClick (ListView l, View v, int position, long id)
			{
				Console.WriteLine ("Item clicked: " + id);
			}
			
			// These are the Contacts rows that we will retrieve.
	        public string[] CONTACTS_SUMMARY_PROJECTION = new string[] {
	            Contacts.People.InterfaceConsts.Id,
	            Contacts.People.InterfaceConsts.DisplayName
	        };
			
			#region ILoaderCallbacks implementation
			public Android.Support.V4.Content.Loader OnCreateLoader (int p0, Bundle p1)
			{
				// This is called when a new Loader needs to be created.  This
	            // sample only has one Loader, so we don't care about the ID.
	            // First, pick the base URI to use depending on whether we are
	            // currently filtering.
	            Android.Net.Uri baseUri;
	            if (_curFilter != null) {
	                baseUri = Android.Net.Uri.WithAppendedPath(Contacts.People.ContentFilterUri, Android.Net.Uri.Encode(_curFilter));
	            } else {
	                baseUri = Contacts.People.ContentUri;
	            }
	
	            // Now create and return a CursorLoader that will take care of
	            // creating a Cursor for the data being displayed.
	            string select = "((" + Contacts.People.InterfaceConsts.DisplayName + " NOTNULL) AND ("
	                    + Contacts.People.InterfaceConsts.DisplayName + " != '' ))";
	            return new CursorLoader(Activity, baseUri,
	                    CONTACTS_SUMMARY_PROJECTION, select, null,
	                    Contacts.People.InterfaceConsts.DisplayName + " COLLATE LOCALIZED ASC");
			}
			
			public void OnLoadFinished (Android.Support.V4.Content.Loader loader, Java.Lang.Object data)
			{
				// Swap the new cursor in.  (The framework will take care of closing the
	            // old cursor once we return.)
	            _adapter.SwapCursor((ICursor) data);
				
	            // The list should now be shown.
				if (IsResumed)
					SetListShown(true);
				else
					SetListShownNoAnimation(true);
			}

			public void OnLoaderReset (Android.Support.V4.Content.Loader p0)
			{
				// This is called when the last Cursor provided to onLoadFinished()
	            // above is about to be closed.  We need to make sure we are no
	            // longer using it.
				_adapter.SwapCursor(null);
			}
			#endregion
			
		}

	}
}

