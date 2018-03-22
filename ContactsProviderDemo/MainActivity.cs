using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Database;

namespace ContactsProviderDemo
{
    [Activity (Label = "ContactsProviderDemo", MainLauncher = true)]
    public class MainActivity : Activity
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
            // Get the URI for the user's contacts:
            var uri = ContactsContract.Contacts.ContentUri;
            
            // Setup the "projection" (columns we want) for only the ID and display name:
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.Id, ContactsContract.Contacts.InterfaceConsts.DisplayName };

            // Use a CursorLoader to retrieve the user's contacts data:
            CursorLoader loader = new CursorLoader(this, uri, projection, null, null, null);
            ICursor cursor = (ICursor)loader.LoadInBackground();

            // Print the contact data (ID and DisplayName) to the console if reading back succeeds:
            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        Console.WriteLine("Contact ID: {0}, Contact Name: {1}",
                                           cursor.GetString(cursor.GetColumnIndex(projection[0])),
                                           cursor.GetString(cursor.GetColumnIndex(projection[1])));
                    } while (cursor.MoveToNext());
                }
            }
        }
        
        void UpdateProfile ()
        {   
            // Write to the profile
            var values = new ContentValues ();
            values.Put (ContactsContract.Contacts.InterfaceConsts.DisplayName, "John Doe");
            ContentResolver.Update (ContactsContract.Profile.ContentRawContactsUri, values, null, null);
            
            // Read the profile
            var uri = ContactsContract.Profile.ContentUri;
            
            // Setup the "projection" (column we want) for only the display name:
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.DisplayName };
            
            // Use a CursorLoader to retrieve the data:
            CursorLoader loader = new CursorLoader(this, uri, projection, null, null, null);
            ICursor cursor = (ICursor)loader.LoadInBackground();
            if (cursor != null)
            { 
                if (cursor.MoveToFirst ())
                {
                    Console.WriteLine(cursor.GetString (cursor.GetColumnIndex (projection [0])));
                }
            }
            
            // Navigate to the profile in the contacts app
            var intent = new Intent (Intent.ActionView, ContactsContract.Profile.ContentUri);            
            StartActivity (intent);    
        }
    }
}


