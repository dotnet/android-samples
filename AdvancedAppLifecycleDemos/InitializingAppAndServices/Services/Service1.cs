using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace AppLifecycle.Services
{
	/// <summary>
	/// A simple service that generates a random letter every half a second.
	/// </summary>
	[Service]
	public class Service1 : Service
	{
		// delcarations
		private readonly string logTag = "Service1";
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

			Log.Debug (this.logTag, "Service1 Started");

			// let our users know that this service is running, because it'll be a foregrounded service
			// this isn't usually needed unless you're doing something like a music player app, because
			// services hardly every get recycle from memory pressure, especially sticky ones.
			var ongoingNotification = new Notification (Resource.Drawable.Icon, "Service1 Running Foreground");
			// the pending intent specifies the activity to launch when a user clicks on the notification
			// in this case, we want to take the user to the music player 
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(MainActivity)), 0);
			ongoingNotification.SetLatestEventInfo (this, logTag, "MainAppService is now runing in the foreground", pendingIntent);

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
			Log.Debug (this.logTag, "client bound to service1");

			// note: if we were using a messenger service, it would have it's own binder
			//this.binder = new MainAppServiceBinder (this);
			//return this.binder;
			return new ServiceBinder<Service1> (this);
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
				while (this.updating) {
					// generate a random letter
					int num = this.random.Next (0, 26); // Zero to 25
					char letter = (char)('a' + num);
					this.Updated (this, new UpdatingEventArgs () { Message = letter.ToString() });
					System.Threading.Thread.Sleep (500);
				}
			}).Start ();
		}

		public void StopUpdating ()
		{
			this.updating = false;
		}

	}
}

