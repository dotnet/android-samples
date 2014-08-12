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

		public override void OnMessageReceived(IMessageEvent messageEvent){
			if (messageEvent.Path.Equals (START_ACTIVITY_PATH)) {
				var launchMain = new Intent (this, typeof(MainActivity));
				launchMain.AddFlags (ActivityFlags.NewTask);
				StartActivity (launchMain);			
			}
		}
	}
}