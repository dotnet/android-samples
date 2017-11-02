using Android.App;
using Android.OS;
using Android.Content;
using Android.Provider;

namespace NotificationChannels
{
    [Activity(Label = "NotificationChannels", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppTheme")]
    public class MainActivity : Activity
    {
        public static string TAG = typeof(MainActivity).Name;

        public const int NOTI_PRIMARY1 = 1100;
        public const int NOTI_PRIMARY2 = 1101;
        public const int NOTI_SECONDARY1 = 1200;
        public const int NOTI_SECONDARY2 = 1201;

        MainUI ui;

        NotificationHelper notificationHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            notificationHelper = new NotificationHelper(this);
            ui = new MainUI(FindViewById(Resource.Id.activity_main), this);
        }

        public void SendNotification(int id, string title)
        {
            Notification.Builder nb = null;
            switch (id)
            {
                case NOTI_PRIMARY1:
                    nb = notificationHelper.GetNotification1(title, GetString(Resource.String.primary1_body));
                    break;

                case NOTI_PRIMARY2:
                    nb = notificationHelper.GetNotification1(title, GetString(Resource.String.primary2_body));
                    break;

                case NOTI_SECONDARY1:
                    nb = notificationHelper.GetNotification2(title, GetString(Resource.String.secondary1_body));
                    break;

                case NOTI_SECONDARY2:
                    nb = notificationHelper.GetNotification2(title, GetString(Resource.String.secondary2_body));
                    break;
            }
            if (nb != null)
            {
                notificationHelper.Notify(id, nb);
            }
        }

        public void GoToNotificationSettings()
        {
            var i = new Intent(Settings.ActionAppNotificationSettings);
            i.PutExtra(Settings.ExtraAppPackage, PackageName);
            StartActivity(i);
        }

        public void GoToNotificationSettings(string channel)
        {
            var i = new Intent(Settings.ActionChannelNotificationSettings);
            i.PutExtra(Settings.ExtraAppPackage, PackageName);
            i.PutExtra(Settings.ExtraChannelId, channel);
            StartActivity(i);
        }

    }
}

