using System;
using Android.App;
using Android.Gms.Wearable;
namespace FindMyPhoneSample
{
	[Service]
	[IntentFilter(new string[]{"com.google.android.gms.wearable.BIND_LISTENER"})]
	public class DisconnectListenerService : WearableListenerService
	{
		const string TAG = "ExampleFindPhoneApp";

		const int FORGOT_PHONE_NOTIFICATION_ID=1;

		public override void OnPeerDisconnected (INode p0)
		{
			Notification.Builder notificationBuilder = new Notification.Builder (this)
				.SetContentTitle ("Forgetting Something?")
				.SetContentText ("You may have left your phone behind.")
				.SetVibrate (new long[]{ 0, 200 })
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.SetLocalOnly (true)
				.SetPriority (NotificationPriority.Max);
			Notification card = notificationBuilder.Build ();
			((NotificationManager)GetSystemService (NotificationService))
				.Notify (FORGOT_PHONE_NOTIFICATION_ID, card);
		}

		public override void OnPeerConnected (INode p0)
		{
			((NotificationManager)GetSystemService (NotificationService))
				.Cancel (FORGOT_PHONE_NOTIFICATION_ID);
		}
	}
}

