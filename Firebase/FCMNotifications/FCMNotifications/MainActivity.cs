using Android.App;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;

using Firebase.Iid;
using Firebase.Messaging;

namespace FCMNotifications
{
    [Activity(Label = "FCMNotifications", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        TextView msgText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            msgText = FindViewById<TextView>(Resource.Id.msgText);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }


            CreateNotificationChannel();

            IsPlayServicesAvailable();


            var logTokenButton = FindViewById<Button>(Resource.Id.logTokenButton);
            logTokenButton.Click += delegate { Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token); };
            var subscribeButton = FindViewById<Button>(Resource.Id.subscribeButton);
            subscribeButton.Click += delegate
                                     {
                                         FirebaseMessaging.Instance.SubscribeToTopic("news");
                                         Log.Debug(TAG, "Subscribed to remote notifications");
                                     };
        }

        public bool IsPlayServicesAvailable()
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }

                return false;
            }

            msgText.Text = "Google Play Services is available.";
            return true;
        }


        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, "FCM Notifications", NotificationImportance.Default)
                          {
                              Description = "Firebase Cloud Messages appear in this channel"
                          };

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}
