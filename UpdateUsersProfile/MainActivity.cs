using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;

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
            values.Put(ContactsContract.Contacts.InterfaceConsts.DisplayName, "Jonathan Peppers");

            // Insert the user's name. Note that the user's profile entry cannot be created explicitly
            // (attempting to do so will throw an exception):
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadContacts) == Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteContacts) == Permission.Granted)
            {
                // We have permission, go ahead and access contacts
                ContentResolver?.Update(ContactsContract.Profile.ContentRawContactsUri!, values, null);
            }
            else
            {
                // Contacts permission is not granted. Display rationale & request.
                ActivityCompat.RequestPermissions(this, new String[] { Android.Manifest.Permission.ReadContacts, Android.Manifest.Permission.WriteContacts }, REQUEST_CONTACTS);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_CONTACTS)
            {
                // Check if the required permission(s) have been granted
                if (grantResults.All(r => r == Permission.Granted))
                {
                    NameOwner();
                }
            }
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
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadContacts) == Permission.Granted)
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
            return false;
        }

        // Launch an intent that navigates to the user's profile:
        void ViewProfile()
        {
            var intent = new Intent(Intent.ActionView, ContactsContract.Profile.ContentUri);
            StartActivity(intent);
        }
    }
}