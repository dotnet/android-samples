using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using Android.Support.V4.App;
using Android.Preview.Support.V4.App;
using Android.Preview.Support.Wearable.Notifications;

namespace MultiPageSample
{
	[Activity (Label = "Multi-Page Notification", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			var button = FindViewById<Button> (Resource.Id.myButton);

			button.Click += delegate {
				var pendingIntent = PendingIntent.GetActivity (this, 100,
				                                               new Intent (this, typeof (MainActivity)),
				                                               PendingIntentFlags.UpdateCurrent);
				var largeIcon = BitmapFactory.DecodeResource (Resources, Resource.Drawable.monkey);

				var builder = new NotificationCompat.Builder (this)
					.SetSmallIcon (Resource.Drawable.ic_logo)
					.SetLargeIcon (largeIcon)
					.SetContentText ("Short Service announcement")
					.SetContentTitle ("Page 1")
					.SetContentIntent (pendingIntent);

				var secondPageStyle = new NotificationCompat.BigTextStyle();
				secondPageStyle.BigText("Xamarin.Android goes on your Android Wear too");

				var secondNotification = new NotificationCompat.Builder (this)
					.SetSmallIcon (Resource.Drawable.ic_logo)
					.SetLargeIcon (largeIcon)
					.SetContentTitle ("Page 2")
					.SetStyle (secondPageStyle)
					.Build ();

				var twoPageNotification = new WearableNotifications.Builder(builder)
					.AddPage(secondNotification)
					.Build();

				NotificationManagerCompat.From (this).Notify ("multi-page-notification", 1000, twoPageNotification);
			};
		}
	}
}


