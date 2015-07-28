using Android.Gms.Gcm.Iid;
using Android.Content;
using Android.App;

namespace GCMSample
{
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.gms.iid.InstanceID" })]
	public class MyInstanceIDListenerService: InstanceIDListenerService
	{
		private const string TAG = "MyInstanceIDLS";

		public override void OnTokenRefresh ()
		{
			var intent = new Intent (this, typeof(RegistrationIntentService));
			StartService (intent);
		}
	}
}

