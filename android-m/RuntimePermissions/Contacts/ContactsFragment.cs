
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Database;

namespace RuntimePermissions
{
	/**
 	* Displays the first contact stored on the device and contains an option to add a dummy contact.
 	* 
 	* This Fragment is only used to illustrate that access to the Contacts ContentProvider API has
 	* been granted (or denied) as part of the runtime permissions model. It is not relevant for the
 	* use
 	* of the permissions API.
 	* 
 	* This fragments demonstrates a basic use case for accessing the Contacts Provider. The
 	* implementation is based on the training guide available here:
 	* https://developer.android.com/training/contacts-provider/retrieve-names.html
 	*/
	public class ContactsFragment : Fragment, LoaderManager.ILoaderCallbacks
	{
		static readonly string TAG = "Contacts";
		TextView messageText = null;

		static readonly string DUMMY_CONTACT_NAME = "__DUMMY CONTACT from runtime permissions sample";

		/**
     	* Projection for the content provider query includes the id and primary name of a contact.
     	*/
		static readonly string[] PROJECTION = { 
			ContactsContract.Contacts.InterfaceConsts.Id,
			ContactsContract.Contacts.InterfaceConsts.DisplayNamePrimary 
		};

		/**
     	* Sort order for the query. Sorted by primary name in ascending order.
     	*/
		static readonly string ORDER = ContactsContract.Contacts.InterfaceConsts.DisplayNamePrimary + " ASC";

		/**
     	* Creates a new instance of a ContactsFragment.
     	*/
		public static ContactsFragment NewInstance () 
		{
			return new ContactsFragment ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate (Resource.Layout.fragment_contacts, container, false);

			messageText = rootView.FindViewById <TextView> (Resource.Id.contact_message);

			// Register a listener to add a dummy contact when a button is clicked.
			var addButton = rootView.FindViewById <Button> (Resource.Id.contact_add);
			addButton.Click += (object sender, EventArgs e) => InsertDummyContact ();

			// Register a listener to display the first contact when a button is clicked.
			var loadButton = rootView.FindViewById <Button> (Resource.Id.contact_load);
			loadButton.Click += (object sender, EventArgs e) => LoadContact ();

			return rootView;
		}

		/**
     	* Restart the Loader to query the Contacts content provider to display the first contact.
     	*/
		void LoadContact () 
		{
			LoaderManager.RestartLoader (0, null, this);
		}

		#region ILoaderCallbacks Implementation
		/**
     	* Initializes a new CursorLoader that queries the ContactsContract.
     	*/
		public Loader OnCreateLoader (int i, Bundle bundle)
		{
			return new CursorLoader (Activity, ContactsContract.Contacts.ContentUri, PROJECTION, null, null, ORDER);
		}

		/**
     	* Dislays either the name of the first contact or a message.
     	*/
		public void OnLoadFinished (Loader loader, Java.Lang.Object data)
		{
			var cursor = data.JavaCast <ICursor> ();

			if (cursor != null) {
				int totalCount = cursor.Count;
				if (totalCount > 0) {
					cursor.MoveToFirst ();
					string name = cursor.GetString (cursor.GetColumnIndex (ContactsContract.Contacts.InterfaceConsts.DisplayName));
					messageText.Text = Resources.GetString (Resource.String.contacts_string, totalCount, name);
					CommonSampleLibrary.Log.Debug (TAG, "First contact loaded: " + name);
					CommonSampleLibrary.Log.Debug (TAG, "Total number of contacts: " + totalCount);
				} else {
					CommonSampleLibrary.Log.Debug (TAG, "List of contacts is empty.");
					messageText.Text = GetString (Resource.String.contacts_empty);
				}
			}
		}

		public void OnLoaderReset (Loader loader)
		{
			messageText.Text = GetString (Resource.String.contacts_empty);
		}
		#endregion

		/**
    	* Accesses the Contacts content provider directly to insert a new contact.
     	* <p>
     	* The contact is called "__DUMMY ENTRY" and only contains a name.
     	*/
		void InsertDummyContact ()
		{
			// Two operations are needed to insert a new contact.
			var operations = new List<ContentProviderOperation> (2);

			// First, set up a new raw contact.
			ContentProviderOperation.Builder op =
				ContentProviderOperation.NewInsert (ContactsContract.RawContacts.ContentUri)
					.WithValue (ContactsContract.RawContacts.InterfaceConsts.AccountType, null)
					.WithValue (ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
			operations.Add (op.Build ());

			// Next, set the name for the contact.
			op = ContentProviderOperation.NewInsert (ContactsContract.Data.ContentUri)
				.WithValueBackReference (ContactsContract.Data.InterfaceConsts.RawContactId, 0)
				.WithValue (ContactsContract.Data.InterfaceConsts.Mimetype,
					ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
				.WithValue (ContactsContract.CommonDataKinds.StructuredName.DisplayName, DUMMY_CONTACT_NAME);
			operations.Add (op.Build ());

			// Apply the operations.
			try {
				Activity.ContentResolver.ApplyBatch (ContactsContract.Authority, operations);
			} catch (RemoteException e) {
				CommonSampleLibrary.Log.Debug (TAG, "Could not add a new contact: " + e.Message);
			} catch (OperationApplicationException e) {
				CommonSampleLibrary.Log.Debug (TAG, "Could not add a new contact: " + e.Message);
			}
		}
	}
}

