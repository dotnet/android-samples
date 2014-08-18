
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace AndroidLSamples
{
	[Activity (Label = "Notifications", ParentActivity=typeof(HomeActivity))]			
	public class NotificationsActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_notifications);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			var intent = new Intent (this, typeof(NotificationsActivity));
			var contentIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.CancelCurrent);

			var manager = GetSystemService (Context.NotificationService) as NotificationManager;


			FindViewById<Button>(Resource.Id.simple).Click += (sender, e) => {

				//Generate a notification with just short text and small icon
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetContentTitle("Daniel")
					.SetContentText("I went to the zoo and saw a monkey!")
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);
				var notification = builder.Build();
				manager.Notify(0, notification);
			};
				

			FindViewById<Button>(Resource.Id.simple_photo).Click += (sender, e) => {

				//Generate a notification with just short text, small icon & large icon
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there))
					.SetContentTitle("Daniel")
					.SetContentText("I went to the zoo and saw a monkey!")
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);
				var notification = builder.Build();
				manager.Notify(0, notification);
			};

			FindViewById<Button>(Resource.Id.extended).Click += (sender, e) => {

				//Extended with big text
				var message = "I went to the zoo and saw a monkey! And then I saw even more awesome stuff and itt was great and then we went back to see the monkeys again.";
				var style = new Notification.BigTextStyle();
				style.BigText(message);
				style.SetSummaryText(message);
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there))
					.SetStyle(style)
					.SetContentTitle("Daniel")
					.SetContentText(message)
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);
				var notification = builder.Build();
				manager.Notify(0, notification);
			};

			FindViewById<Button>(Resource.Id.extended_photo).Click += (sender, e) => {

				//Extended with big text
				var message = "I went to the zoo and saw a monkey! And then I saw even more awesome stuff and itt was great and then we went back to see the monkeys again.";
				var style = new Notification.BigPictureStyle();
				style.BigPicture(BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there));
				style.SetSummaryText(message);
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.evolve))
					.SetStyle(style)
					.SetContentTitle("Daniel")
					.SetContentText(message)
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);
				var notification = builder.Build();
				manager.Notify(0, notification);
			};
		}
	}
}

