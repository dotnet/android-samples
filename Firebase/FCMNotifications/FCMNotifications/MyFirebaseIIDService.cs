using System;

using Android.App;
using Android.Util;

using Firebase.Iid;

namespace FCMNotifications
{
    [Service]
    [IntentFilter(new[] {"com.google.firebase.INSTANCE_ID_EVENT"})]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            // Add custom implementation, as needed.
        }
    }
}
