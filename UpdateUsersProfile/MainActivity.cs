using Android.Content;
using Android.Provider;
using Android.Database;
using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Content.PM;
using Android.Views;
using static Android.App.ActionBar;
using Android.Support.Design.Widget;

namespace UpdateUsersProfile
{
    [Activity(Label = "UpdateUsersProfile", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class MainActivity : Activity
    {
        public static readonly int REQUEST_CONTACTS = 1;

        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the button for updating the user profile:
            var button = FindViewById<Button>(Resource.Id.MyButton);

            ArgumentNullException.ThrowIfNull(button);

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
            var values = new ContentValues();
            values.Put(ContactsContract.Contacts.InterfaceConsts.DisplayName, "John Doe");

            // Insert the user's name. Note that the user's profile entry cannot be created explicitly
            // (attempting to do so will throw an exception):

            if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.M)
            {
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadContacts) == (int)Permission.Granted)
            {
                // We have permission, go ahead and access contacts
                if (ContactsContract.Profile.ContentRawContactsUri is not null)
                    ContentResolver?.Update(ContactsContract.Profile.ContentRawContactsUri, values, null, null);

            }
            else
            {
                // Contacts permission is not granted. Display rationale & request.

                ActivityCompat.RequestPermissions(this, new String[] { Android.Manifest.Permission.Camera }, REQUEST_CONTACTS);

            }
        }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_CONTACTS)
            {
                // Received permission result for camera permission.
                //Log.Info(TAG, "Received response for Location permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    //Log.Info(TAG, "Location permission has now been granted.");
                    NameOwner();
                }
            }
            //else
            //{
            //    if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.M)
            //    {
            //        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //    }
            //}
        }

        // Read back the user name and print it to the console:
        bool ReadBackName()
        {
            // Get the URI for the user's profile:
            Android.Net.Uri? uri = ContactsContract.Profile.ContentUri;

            ArgumentNullException.ThrowIfNull(uri);

            // Setup the "projection" (columns we want) for only the display name:
            string[] projection = { ContactsContract.Contacts.InterfaceConsts.DisplayName };

            // Use a CursorLoader to retrieve the user's profile data:
            var loader = new AndroidX.Loader.Content.CursorLoader(this, uri, projection, null, null, null);


            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadContacts) == (int)Permission.Granted)
                {
                    var cursor = (ICursor?)loader.LoadInBackground();

                    // Print the user name to the console if reading back succeeds:
                    if (cursor != null)
                    {
                        if (cursor.MoveToFirst())
                        {
                            Console.WriteLine(cursor.GetString(cursor.GetColumnIndex(projection[0])));
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        // Launch an intent that navigates to the user's profile:
        void ViewProfile()
        {
            Intent intent = new Intent(Intent.ActionView, ContactsContract.Profile.ContentUri);
            StartActivity(intent);
        }
    }
}