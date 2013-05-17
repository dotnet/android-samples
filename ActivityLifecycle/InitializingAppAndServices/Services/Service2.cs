using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace AppLifecycle.Services
{
	/// <summary>
	/// A simple service that returns a random number every half a second
	/// </summary>
	[Service]
	public class Service2 : Service
	{
		// delcarations
		private readonly string logTag = "Service2";
		protected bool updating;
		Random random = new Random(DateTime.Now.Millisecond);

		public event EventHandler<UpdatingEventArgs> Updated = delegate {};

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (this.logTag, "OnCreate called in MainAppService");
		}

		/// <summary>
		/// By overriding this, we're specifying that the service is a _Started Service_, and therefore, we're
		/// supposed to manage it's lifecycle (shutting it down, for instance).
		/// </summary>
		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			base.OnStartCommand (intent, flags, startId);

			Log.Debug (this.logTag, "Service2 Started");

			// create the notification
			var ongoingNotification = new Notification (Resource.Drawable.Icon, "Service2 Running Foreground");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(MainActivity)), 0);
			ongoingNotification.SetLatestEventInfo (this, logTag, "Service2 is now runing in the foreground", pendingIntent);

			// start our service foregrounded, that way it won't get cleaned up from memory pressure
			StartForeground ((int)NotificationFlags.ForegroundService, ongoingNotification);

			// tell the OS that if this service ever gets killed, to redilever the intent when it's started
			return StartCommandResult.RedeliverIntent;
		}

		/// <summary>
		/// Called the first time that the OS wants a binding reference to the service.
		/// note that this will only be called the first time, so it's ok to create a new 
		/// binder object in here.
		/// </summary>
		public override IBinder OnBind (Intent intent)
		{
			Log.Debug (this.logTag, "client bound to service");
			return new ServiceBinder<Service2> (this);
		}

		public void StartUpdating()
		{
			// in case it gets called twice
			if (this.updating) {
				Log.Debug(this.logTag, "Service1.StartUpdating, Already Updating");
				return;
			}

			new Task (() => {
				this.updating = true;
				while(this.updating) {
					int num = this.random.Next(0, 1000);
					this.Updated ( this, new UpdatingEventArgs () { Message = num.ToString() } );
					System.Threading.Thread.Sleep(500);
				}
			}).Start ();
		}

		public void StopUpdating ()
		{
			this.updating = false;
		}
	}
}

