
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
		Spinner spinner;
		Switch highPriority;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_notifications);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
			highPriority = FindViewById<Switch> (Resource.Id.high_priority);

			var intent = new Intent (this, typeof(NotificationsActivity));
			var contentIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.CancelCurrent);

			var manager = GetSystemService (Context.NotificationService) as NotificationManager;


			FindViewById<Button>(Resource.Id.simple).Click += (sender, e) => {

				manager.Cancel(0);

				//Generate a notification with just short text and small icon
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetContentTitle("Daniel")
					.SetContentText("I went to the zoo and saw a monkey!")
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);

				SetMetadata(builder);

				var notification = builder.Build();
				if(highPriority.Checked)
					notification.Defaults |= NotificationDefaults.Sound;
				manager.Notify(0, notification);
			};
				

			FindViewById<Button>(Resource.Id.simple_photo).Click += (sender, e) => {

				manager.Cancel(1);

				var icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there);
				//Generate a notification with just short text, small icon & large icon
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetLargeIcon(icon)
					.SetContentTitle("Daniel")
					.SetContentText("I went to the zoo and saw a monkey!")
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);

				SetMetadata(builder);

				var notification = builder.Build();
				if(highPriority.Checked)
					notification.Defaults |= NotificationDefaults.Sound;
				manager.Notify(1, notification);

				icon.Recycle();
			};

			FindViewById<Button>(Resource.Id.extended).Click += (sender, e) => {

				manager.Cancel(2);

				var icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there);

				//Extended with big text
				var message = "I went to the zoo and saw a monkey! And then I saw even more awesome stuff and itt was great and then we went back to see the monkeys again.";
				var style = new Notification.BigTextStyle();
				style.BigText(message);
				style.SetSummaryText(message);
				var builder = new Notification.Builder(this)
					.SetContentIntent(contentIntent)
					.SetSmallIcon(Resource.Drawable.ic_notification)
					.SetLargeIcon(icon)
					.SetStyle(style)
					.SetContentTitle("Daniel")
					.SetContentText(message)
					.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
					.SetAutoCancel(true);

				SetMetadata(builder);

				var notification = builder.Build();
				if(highPriority.Checked)
					notification.Defaults |= NotificationDefaults.Sound;
				manager.Notify(2, notification);
				icon.Recycle();
			};

			FindViewById<Button>(Resource.Id.extended_photo).Click += (sender, e) => {

				manager.Cancel(3);

				//Extended with big text
				var message = "I went to the zoo and saw a monkey! And then I saw even more awesome stuff and itt was great and then we went back to see the monkeys again.";
				var style = new Notification.BigPictureStyle();
				var icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.over_there);

				style.BigPicture(icon);
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

				SetMetadata(builder);

				var notification = builder.Build();
				if(highPriority.Checked)
					notification.Defaults |= NotificationDefaults.Sound;
				manager.Notify(3, notification);

				icon.Recycle();
			};

			spinner = FindViewById<Spinner>(Resource.Id.spinner_visibility);
			// Create an ArrayAdapter using the string array and a default spinner layout
			var adapter = ArrayAdapter.CreateFromResource(this,
				Resource.Array.notification_priority, Android.Resource.Layout.SimpleSpinnerItem);
			// Specify the layout to use when the list of choices appears
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			// Apply the adapter to the spinner
			spinner.Adapter = adapter;
		}

		private void SetMetadata(Notification.Builder builder)
		{


			if ((int)Build.VERSION.SdkInt >= 16) {


				builder.SetPriority(highPriority.Checked ? (int)NotificationPriority.High : (int)NotificationPriority.Default);
			}

			if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.Lollipop) {
				builder.SetCategory (Notification.CategoryMessage);
				var visibility = NotificationVisibility.Public;
				if(spinner.SelectedItemPosition == 1)
					visibility = NotificationVisibility.Private;
				else if(spinner.SelectedItemPosition == 2)
					visibility = NotificationVisibility.Secret;
					
				builder.SetVisibility(visibility);
			}

		}
	}
}

