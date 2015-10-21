using System;
using Android.App;
using Android.Util;
using System.Threading;
using Android.Content;
using Android.Widget;
using Android.OS;

namespace DemoService
{
	[Service]
	[IntentFilter(new String[]{"com.xamarin.DemoService"})]
	public class DemoService : Service
	{
		DemoServiceBinder binder;

		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug ("DemoService", "DemoService started");

			StartServiceInForeground ();

			DoWork ();

			return StartCommandResult.NotSticky;
		}

		void StartServiceInForeground ()
		{
			var ongoing = new Notification (Resource.Drawable.icon, "DemoService in foreground");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(DemoActivity)), 0);
			ongoing.SetLatestEventInfo (this, "DemoService", "DemoService is running in the foreground", pendingIntent);

			StartForeground ((int)NotificationFlags.ForegroundService, ongoing);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
            
			Log.Debug ("DemoService", "DemoService stopped");       
		}

		void SendNotification ()
		{
			var nMgr = (NotificationManager)GetSystemService (NotificationService);
			var notification = new Notification (Resource.Drawable.icon, "Message from demo service");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(DemoActivity)), 0);
			notification.SetLatestEventInfo (this, "Demo Service Notification", "Message from demo service", pendingIntent);
			nMgr.Notify (0, notification);
		}

		public void DoWork ()
		{
			Toast.MakeText (this, "The demo service has started", ToastLength.Long).Show ();

			var t = new Thread (() => {

				SendNotification ();

				Thread.Sleep (5000);

				Log.Debug ("DemoService", "Stopping foreground");
				StopForeground (true);

				StopSelf ();
			}
			);

			t.Start ();
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			binder = new DemoServiceBinder (this);
			return binder;
        }

		public string GetText ()
		{
			return "some text from the service";
		}
	}

	public class DemoServiceBinder : Binder
	{
		DemoService service;
    
		public DemoServiceBinder (DemoService service)
		{
			this.service = service;
		}

		public DemoService GetDemoService ()
		{
			return service;
		}
	}

}