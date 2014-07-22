using System;
using Android.App;
using Android.Content;
using Android.Gms.Wearable;

namespace DelayedConfirmation
{
	[Service]
	[IntentFilter (new string[]{"com.google.android.gms.wearable.BIND_LISTENER"})]
	public class WearableMessageListenerService : WearableListenerService
	{
		private static readonly string START_ACTIVITY_PATH = "/start-activity";

		public void OnMessageRecieved(IMessageEvent messageEvent){
			if (messageEvent.Path.Equals (START_ACTIVITY_PATH)) {
				var startIntent = new Intent (this, typeof(MainActivity));
				startIntent.AddFlags (ActivityFlags.NewTask);
				StartActivity (startIntent);			
			}
		}
	}
}

