using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo {

	[Service]
	public class LocalService : Service {
		private NotificationManager nm;
		private IBinder binder;

		public LocalService ()
		{
			binder = new LocalBinder (this);
		}

		// how's this work?
		public class LocalBinder : Binder {
			LocalService self;

			public LocalBinder (LocalService self)
			{
				this.self = self;
			}

			public LocalService Service {
				get {return self;}
			}
		}

		public override void OnCreate ()
		{
			nm = (NotificationManager) GetSystemService (NotificationService);
			ShowNotification ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Info ("LocalService", "Received start id " + startId + ": " + intent);
			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			nm.Cancel (Resource.String.local_service_started);
			Toast.MakeText (this, Resource.String.local_service_stopped, ToastLength.Short).Show ();
		}

		public override IBinder OnBind (Intent intent)
		{
			return binder;
		}

		void ShowNotification ()
		{
			var text = GetText (Resource.String.local_service_started);
			var sinceEpoch    = DateTime.UtcNow - new DateTime (1970, 1, 1);
			var msSinceEpoch  = (long) sinceEpoch.TotalMilliseconds;
			var notification  = new Notification (Resource.Drawable.stat_sample, text, msSinceEpoch);
			PendingIntent contentIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof (LocalServiceActivities.Controller)), 0);
			notification.SetLatestEventInfo (this, GetText (Resource.String.local_service_label), text, contentIntent);
			nm.Notify (Resource.String.local_service_started, notification);
		}
	}
}
