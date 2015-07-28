using System;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Media;
using Android.Support.V4.App;
using Android.Util;
using Android.OS;

namespace GCMSample
{
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class MyGcmListenerService : GcmListenerService
	{
		const string TAG = "MyGcmListenerService";

		public override void OnMessageReceived (string from, Bundle data)
		{
			var message = data.GetString ("message");
			Log.Debug (TAG, "From: " + from);
			Log.Debug (TAG, "Message: " + message);

			SendNotification (message);
		}

		void SendNotification (string message)
		{
			var intent = new Intent (this, typeof(MainActivity));
			intent.AddFlags (ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

			var defaultSoundUri = RingtoneManager.GetDefaultUri (RingtoneType.Notification);

			var notificationBuilder = new NotificationCompat.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_stat_ic_notification)
				.SetContentTitle ("GCM Message")
				.SetContentText (message)
				.SetAutoCancel (true)
				.SetSound (defaultSoundUri)
				.SetContentIntent (pendingIntent);

			var notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);
			notificationManager.Notify (0, notificationBuilder.Build ());
		}
	}
}

