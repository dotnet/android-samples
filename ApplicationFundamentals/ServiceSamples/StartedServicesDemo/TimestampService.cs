using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;

namespace StartedServicesDemo
{
	/// <summary>
	/// This is a sample started service. When the service is started, it will log a string that details how long 
	/// the service has been running (using Android.Util.Log). This service displays a notification in the notification
	/// tray while the service is active.
	/// </summary>
	[Service]
	public class TimestampService : Service
	{
		static readonly string TAG = typeof(TimestampService).FullName;
		static readonly int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
		static readonly int NOTIFICATION_ID = 10000;

		UtcTimestamper timestamper;
		bool isStarted;
		Handler handler;
		Action runnable;

		public override void OnCreate()
		{
			base.OnCreate();
			Log.Info(TAG, "OnCreate: the service is initializing.");

			timestamper = new UtcTimestamper();
			handler = new Handler();

			// This Action is only for demonstration purposes.
			runnable = new Action(() =>
							{
								if (timestamper != null)
								{
									Log.Debug(TAG, timestamper.GetFormattedTimestamp());
									handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
								}
							});
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			if (isStarted)
			{
				Log.Info(TAG, "OnStartCommand: This service has already been started.");
			}
			else
			{
				Log.Info(TAG, "OnStartCommand: The service is starting.");
				DispatchNotificationThatServiceIsRunning();
				handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
				isStarted = true;
			}

			// This tells Android not to restart the service if it is killed to reclaim resources.
			return StartCommandResult.NotSticky;
		}


		public override IBinder OnBind(Intent intent)
		{
			// Return null because this is a pure started service. A hybrid service would return a binder that would
			// allow access to the GetFormattedStamp() method.
			return null;
		}


		public override void OnDestroy()
		{
			// We need to shut things down.
			Log.Debug(TAG, GetFormattedTimestamp());
			Log.Info(TAG, "OnDestroy: The started service is shutting down.");

			// Stop the handler.
			handler.RemoveCallbacks(runnable);

			// Remove the notification from the status bar.
			var notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Cancel(NOTIFICATION_ID);

			timestamper = null;
			isStarted = false;
			base.OnDestroy();
		}

		/// <summary>
		/// This method will return a formatted timestamp to the client.
		/// </summary>
		/// <returns>A string that details what time the service started and how long it has been running.</returns>
		string GetFormattedTimestamp()
		{
			return timestamper?.GetFormattedTimestamp();
		}

		void DispatchNotificationThatServiceIsRunning()
		{
			Notification.Builder notificationBuilder = new Notification.Builder(this)
				.SetSmallIcon(Resource.Drawable.ic_stat_name)
				.SetContentTitle(Resources.GetString(Resource.String.app_name))
				.SetContentText(Resources.GetString(Resource.String.notification_text));

			var notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
		}
	}
}
