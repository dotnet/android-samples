using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;

namespace WatchFace
{
    [Activity(Label = "WatchFace", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var v = FindViewById<WatchViewStub>(Resource.Id.watch_view_stub);
            v.LayoutInflated += delegate
            {

                // Get our button from the layout resource,
                // and attach an event to it
                Button button = FindViewById<Button>(Resource.Id.myButton);

                button.Click += delegate
                {
                    var notification = new NotificationCompat.Builder(this)
                        .SetContentTitle("Button tapped")
                        .SetContentText("Button tapped " + count++ + " times!")
                        .SetSmallIcon(Android.Resource.Drawable.StatNotifyVoicemail)
                        .SetGroup("group_key_demo").Build();

                    var manager = NotificationManagerCompat.From(this);
                    manager.Notify(1, notification);
                    button.Text = "Check Notification!";
                };
            };
        }
    }
}

