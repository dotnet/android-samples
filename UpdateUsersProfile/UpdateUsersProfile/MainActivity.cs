using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Database;

namespace UpdateUsersProfile
{
    [Activity(Label = "UpdateUsersProfile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the button for updating the user profile:
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate {

                // Give a name to the device owner's profile:
                NameOwner();

                // Read back the name: 
                if (ReadBackName())
                    // launch an activity to view the profile if reading the name works:
                    ViewProfile();
            };
        }

        // Give the device user a name: "John Doe"
        void NameOwner()
        {
            // Create the display name for the user's profile:
            ContentValues values = new ContentValues();
            values.Put (ContactsContract.Contacts.InterfaceConsts.DisplayName, "John Doe");

            // Insert the user's name. Note that the user's profile entry cannot be created explicitly
            // (attempting to do so will throw an exception):
            ContentResolver.Update (ContactsContract.Profile.ContentRawContactsUri, values, null, null);
        }

        // Read back the user name and print it to the console:
        bool ReadBackName()
        {
            // Get the URI for the user's profile:
            Android.Net.Uri uri = ContactsContract.Profile.ContentUri;

            // Setup the "projection" (columns we want) for only the display name:
            string[] projection = { ContactsContract.Contacts.InterfaceConsts.DisplayName };

            // Use a CursorLoader to retrieve the user's profile data:
            CursorLoader loader = new CursorLoader (this, uri, projection, null, null, null);
            ICursor cursor = (ICursor)loader.LoadInBackground();

            // Print the user name to the console if reading back succeeds:
            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    Console.WriteLine(cursor.GetString(cursor.GetColumnIndex(projection[0])));
                    return true;
                }
            }
            return false;
        }

        // Launch an intent that navigates to the user's profile:
        void ViewProfile ()
        {
            Intent intent = new Intent (Intent.ActionView, ContactsContract.Profile.ContentUri);
            StartActivity (intent);
        }
    }
}

