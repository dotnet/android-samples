using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Net;
using System.Threading;
using AndroidX.AppCompat.App;

namespace AndroidPMiniDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public const string DEMO_CHANNEL = "com.xamarin.myapp.demo";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //------------------------------------------------------------------
            // Display Cutout Mode demo

            Button shortEdgesBtn = FindViewById<Button>(Resource.Id.short_btn);
            Button neverBtn = FindViewById<Button>(Resource.Id.never_btn);
            Button resetBtn = FindViewById<Button>(Resource.Id.reset_btn);

            shortEdgesBtn.Click += (sender, e) =>
            {
                Window.Attributes.LayoutInDisplayCutoutMode =
                        Android.Views.LayoutInDisplayCutoutMode.ShortEdges;
                Window.AddFlags(WindowManagerFlags.Fullscreen);
                Toast.MakeText(this, "Cutout Short Edges", ToastLength.Short).Show();
            };

            neverBtn.Click += (sender, e) =>
            { 
                Window.Attributes.LayoutInDisplayCutoutMode =
                        Android.Views.LayoutInDisplayCutoutMode.Never;
                Window.AddFlags(WindowManagerFlags.Fullscreen);
                Toast.MakeText(this, "Cutout Never", ToastLength.Short).Show();
            };

            resetBtn.Click += (sender, e) =>
            {
                Window.Attributes.LayoutInDisplayCutoutMode =
                        Android.Views.LayoutInDisplayCutoutMode.Never;
                Window.ClearFlags(WindowManagerFlags.Fullscreen);
                Toast.MakeText(this, "Reset to non Full-screen", ToastLength.Short).Show();
            };

            //------------------------------------------------------------------
            // Image Notification demo:

            Button notifBtn = FindViewById<Button>(Resource.Id.notification_btn);
            notifBtn.Click += (sender, e) =>
            {
                // Create a demo notification channel for notifications in this app:
                string chanName = GetString(Resource.String.noti_chan_demo);
                var importance = NotificationImportance.High;
                NotificationChannel chan = new NotificationChannel(DEMO_CHANNEL, chanName, importance);
                chan.EnableVibration(true);
                chan.LockscreenVisibility = NotificationVisibility.Public;
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(chan);

                // Create a Person object that represents the sender:
                Icon senderIcon = Icon.CreateWithResource(this, Resource.Drawable.sender_icon);
                Person fromPerson = new Person.Builder()
                    .SetIcon(senderIcon)
                    .SetName("Mark Sender")
                    .Build();

                // Create a text notification to send before the image notification:
                Notification.BigTextStyle textStyle = new Notification.BigTextStyle();
                string longTextMessage = "We've just arrived at the Colosseum, let me send";
                longTextMessage += " you a pic before we go inside.";
                textStyle.BigText(longTextMessage);

                // Notification builder using this style:
                Notification.Builder builder = new Notification.Builder(this, DEMO_CHANNEL)
                    .SetContentTitle("At the Colosseum")
                    .SetSmallIcon(Resource.Mipmap.ic_notification)
                    .SetStyle(textStyle)
                    .SetChannelId(DEMO_CHANNEL);

                // Publish the text notification:
                const int notificationId = 1000;
                notificationManager.Notify(notificationId, builder.Build());

                Thread.Sleep(5000);

                // Send Image notification ...........................................

                // Create a message from the sender with the image to send:
                Uri imageUri = Uri.Parse("android.resource://com.xamarin.pminidemo/drawable/example_image");
                Notification.MessagingStyle.Message message = new Notification.MessagingStyle
                        .Message("Here's a picture of where I'm currently standing", 0, fromPerson)
                        .SetData("image/", imageUri);

                // Add the message to a notification style:
                Notification.MessagingStyle style = new Notification.MessagingStyle(fromPerson)
                        .AddMessage(message);

                // Notification builder using this style:
                builder = new Notification.Builder(this, DEMO_CHANNEL)
                    .SetContentTitle("Tour of the Colosseum")
                    .SetContentText("We're standing in front of the Colosseum and I just took this shot!")
                    .SetSmallIcon(Resource.Mipmap.ic_notification)
                    .SetStyle(style)
                    .SetChannelId(DEMO_CHANNEL);

                // Publish the notification:
                const int notificationId2 = 1001;
                notificationManager.Notify(notificationId2, builder.Build());
            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}