using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;

namespace ClientApp
{
    [Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived (string from, Bundle data)
        {
            // Extract the message received from GCM:
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);

            // Forward the received message in a local notification:
            SendNotification (message);
        }

        // Use Notification Builder to create and launch the notification:
        void SendNotification (string message)
        {
            var intent = new Intent (this, typeof(MainActivity));
            intent.AddFlags (ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder (this)
                .SetSmallIcon (Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle ("GCM Message")
                .SetContentText (message)
                .SetAutoCancel (true)
                .SetContentIntent (pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);
            notificationManager.Notify (0, notificationBuilder.Build ());
        }
    }
}
