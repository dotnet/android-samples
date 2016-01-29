using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace ClientApp
{
    [Activity(Label = "Remote Notifications", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView msgText;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            msgText = FindViewById<TextView> (Resource.Id.msgText);

            // Check for Google Play Services on the device:
            if (IsPlayServicesAvailable ())
            {
                // Start the registration intent service; try to get a token:
                var intent = new Intent (this, typeof (RegistrationIntentService));
                StartService (intent);
            }
        }

        // Utility method to check for the presence of the Google Play Services APK:
        public bool IsPlayServicesAvailable ()
        {
            // These methods are moving to GoogleApiAvailability soon:
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
            if (resultCode != ConnectionResult.Success)
            {
                // Google Play Service check failed - display the error to the user:
                if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
                {
                    // Give the user a chance to download the APK:
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString (resultCode);
                }
                else
                {
                    msgText.Text = "Sorry, this device is not supported";
                    Finish ();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                return true;
            }
        }
    }
}

