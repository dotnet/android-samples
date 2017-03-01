using Android.App;
using Android.Content;
using Android.Gms.Iid;

namespace ClientApp
{
    // My listener for registration token refreshes:

    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class MyInstanceIDListenerService : InstanceIDListenerService
    {
        // When a token refresh happens, start my RegistrationIntentService:
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(RegistrationIntentService));
            StartService(intent);
        }
    }
}
