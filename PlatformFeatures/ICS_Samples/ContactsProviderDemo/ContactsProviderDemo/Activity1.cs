using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;

namespace ContactsProviderDemo
{
    [Activity (Label = "ContactsProviderDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Main);

            var getContactsButton = FindViewById<Button> (Resource.Id.getContactsButton);
            
            getContactsButton.Click += delegate {
                GetContacts (); };
            
            var updateProfileButton = FindViewById<Button> (Resource.Id.updateProfileButton);
            
            updateProfileButton.Click += delegate {
                UpdateProfile (); };
        }
        
        void GetContacts ()
        {
            var uri = ContactsContract.Contacts.ContentUri;
            
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.Id, ContactsContract.Contacts.InterfaceConsts.DisplayName };
            
            var cursor = ManagedQuery (uri, projection, null, null, null);
            
            if (cursor.MoveToFirst ()) {
                do {
                    Console.WriteLine ("Contact ID: {0}, Contact Name: {1}", 
                                       cursor.GetString (cursor.GetColumnIndex (projection [0])),
                                       cursor.GetString (cursor.GetColumnIndex (projection [1])));
                    
                } while (cursor.MoveToNext());
            }
        }
        
        void UpdateProfile ()
        {   
            // write to the profile
            var values = new ContentValues ();
            
            values.Put (ContactsContract.Contacts.InterfaceConsts.DisplayName, "John Doe");
            
            ContentResolver.Update (ContactsContract.Profile.ContentRawContactsUri, values, null, null);
            
            // read the profile
            var uri = ContactsContract.Profile.ContentUri;
            
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.DisplayName };
            
            var cursor = ManagedQuery (uri, projection, null, null, null);

            if (cursor.MoveToFirst ()) {
                Console.WriteLine(cursor.GetString (cursor.GetColumnIndex (projection [0])));
            }
            
            // navigate to the profile in the people app
            var intent = new Intent (Intent.ActionView, ContactsContract.Profile.ContentUri);            
            StartActivity (intent);    
        }
    }
}


