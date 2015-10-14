using System;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.App;
using Android.Provider;
using System.Collections.Generic;
using Android.Database;

namespace ContactsAdapterDemo {
    public class ContactsAdapter : BaseAdapter {
        List<Contact> contactList;
        Activity activity;
        
        public ContactsAdapter (Activity activity)
        {
            this.activity = activity;
            
            FillContacts ();
        }
        
        public override int Count {
            get { return contactList.Count; }
        }

        public override Java.Lang.Object GetItem (int position)
        {
            return null; // could wrap a Contact in a Java.Lang.Object to return it here if needed
        }

        public override long GetItemId (int position)
        {
            return contactList [position].Id;
        }
        
        public override View GetView (int position, View convertView, ViewGroup parent)
        {          
            var view = convertView ?? activity.LayoutInflater.Inflate (Resource.Layout.ContactListItem, parent, false);
            var contactName = view.FindViewById<TextView> (Resource.Id.ContactName);
            var contactImage = view.FindViewById<ImageView> (Resource.Id.ContactImage);
            
            contactName.Text = contactList [position].DisplayName;
            
            if (contactList [position].PhotoId == null) {
                
                contactImage = view.FindViewById<ImageView> (Resource.Id.ContactImage);
                contactImage.SetImageResource (Resource.Drawable.ContactImage);
                
            } else {
                
                var contactUri = ContentUris.WithAppendedId (ContactsContract.Contacts.ContentUri, contactList [position].Id);
                var contactPhotoUri = Android.Net.Uri.WithAppendedPath (contactUri, Contacts.Photos.ContentDirectory);
    
                contactImage.SetImageURI (contactPhotoUri);
            }
            return view;
        }
        
        void FillContacts ()
        {
            var uri = ContactsContract.Contacts.ContentUri;
            
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.Id, 
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.Contacts.InterfaceConsts.PhotoId
            };

            // ManagedQuery is deprecated in Honeycomb (3.0, API11)
            //var cursor = activity.ManagedQuery (uri, projection, null, null, null);

            // ContentResolver requires you to close the query yourself
            //var cursor = activity.ContentResolver.Query(uri, projection, null, null, null);

            // CursorLoader introduced in Honeycomb (3.0, API11)
            var loader = new CursorLoader(activity, uri, projection, null, null, null);
            var cursor = (ICursor)loader.LoadInBackground();

            contactList = new List<Contact> ();   
            
            if (cursor.MoveToFirst ()) {
                do {
                    contactList.Add (new Contact{
                        Id = cursor.GetLong (cursor.GetColumnIndex (projection [0])),
                        DisplayName = cursor.GetString (cursor.GetColumnIndex (projection [1])),
                        PhotoId = cursor.GetString (cursor.GetColumnIndex (projection [2]))
                    });
                } while (cursor.MoveToNext());
            }
        }
    }
}

