using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Java.Lang;
using Task = Android.Gms.Tasks.Task;

namespace LocUpdFgService
{
	/**
	 * A bound and started service that is promoted to a foreground service when location updates have
	 * been requested and all clients unbind.
	 *
	 * For apps running in the background on "O" devices, location is computed only once every 10
	 * minutes and delivered batched every 30 minutes. This restriction applies even to apps
	 * targeting "N" or lower which are run on "O" devices.
	 *
	 * This sample show how to use a long-running service for location updates. When an activity is
	 * bound to this service, frequent location updates are permitted. When the activity is removed
	 * from the foreground, the service promotes itself to a foreground service, and location updates
	 * continue. When the activity comes back to the foreground, the foreground service stops, and the
	 * notification assocaited with that service is removed.
	 */
	[Service(Label = "LocationUpdatesService", Enabled = true, Exported = true)]
	[IntentFilter(new string[] { "com.xamarin.LocUpdFgService.LocationUpdatesService" })]
	public class LocationUpdatesService : Service
	{
		const string LocationPackageName = "com.xamarin.LocUpdFgService";

		public string Tag = "LocationUpdatesService";

		string ChannelId = "channel_01";

        public const string ActionBroadcast = LocationPackageName + ".broadcast";

		public const string ExtraLocation = LocationPackageName + ".location";
		const string ExtraStartedFromNotification = LocationPackageName + ".started_from_notification";

		IBinder Binder;

		/**
	     * The desired interval for location updates. Inexact. Updates may be more or less frequent.
	     */
		const long UpdateIntervalInMilliseconds = 10000;

		/**
		 * The fastest rate for active location updates. Updates will never be more frequent
		 * than this value.
		 */
		const long FastestUpdateIntervalInMilliseconds = UpdateIntervalInMilliseconds / 2;

		/**
		 * The identifier for the notification displayed for the foreground service.
		 */
		const int NotificationId = 12345678;

		/**
		 * Used to check whether the bound activity has really gone away and not unbound as part of an
		 * orientation change. We create a foreground service notification only if the former takes
		 * place.
		 */
		bool ChangingConfiguration = false;

		NotificationManager NotificationManager;

		/**
		 * Contains parameters used by {@link com.google.android.gms.location.FusedLocationProviderApi}.
		 */
		LocationRequest LocationRequest;

		/**
		 * Provides access to the Fused Location Provider API.
		 */
		FusedLocationProviderClient FusedLocationClient;

		/**
		 * Callback for changes in location.
		 */
		LocationCallback LocationCallback;

		Handler ServiceHandler;

		/**
		 * The current location.
		 */
		public Location Location;

		public LocationUpdatesService()
		{
			Binder = new LocationUpdatesServiceBinder(this);
		}

		class LocationCallbackImpl : LocationCallback
		{
			public LocationUpdatesService Service { get; set; }
			public override void OnLocationResult(LocationResult result)
			{
				base.OnLocationResult(result);
				Service.OnNewLocation(result.LastLocation);
			}
		}

		public override void OnCreate()
		{
			FusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);

			LocationCallback = new LocationCallbackImpl { Service = this };

			CreateLocationRequest();
            GetLastLocation();

			HandlerThread handlerThread = new HandlerThread(Tag);
			handlerThread.Start();
			ServiceHandler = new Handler(handlerThread.Looper);
			NotificationManager = (NotificationManager) GetSystemService(NotificationService);

		    if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
		    {
		        string name = GetString(Resource.String.app_name);
		        NotificationChannel mChannel = new NotificationChannel(ChannelId, name, NotificationManager.ImportanceDefault);
		        NotificationManager.CreateNotificationChannel(mChannel);
		    }
        }

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Info(Tag, "Service started");
			var startedFromNotification = intent.GetBooleanExtra(ExtraStartedFromNotification, false);

			// We got here because the user decided to remove location updates from the notification.
			if (startedFromNotification)
			{
				RemoveLocationUpdates();
				StopSelf();
			}
			// Tells the system to not try to recreate the service after it has been killed.
			return StartCommandResult.NotSticky;
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			ChangingConfiguration = true;
		}

		public override IBinder OnBind(Intent intent)
		{
			// Called when a client (MainActivity in case of this sample) comes to the foreground
			// and binds with this service. The service should cease to be a foreground service
			// when that happens.
			Log.Info(Tag, "in onBind()");
			StopForeground(true);
			ChangingConfiguration = false;
			return Binder;
		}

		public override void OnRebind(Intent intent)
		{
			// Called when a client (MainActivity in case of this sample) returns to the foreground
			// and binds once again with this service. The service should cease to be a foreground
			// service when that happens.
			Log.Info(Tag, "in onRebind()");
			StopForeground(true);
			ChangingConfiguration = false;
			base.OnRebind(intent);
		}

		public override bool OnUnbind(Intent intent)
		{
			Log.Info(Tag, "Last client unbound from service");

			// Called when the last client (MainActivity in case of this sample) unbinds from this
			// service. If this method is called due to a configuration change in MainActivity, we
			// do nothing. Otherwise, we make this service a foreground service.
			if (!ChangingConfiguration && Utils.RequestingLocationUpdates(this))
			{
				Log.Info(Tag, "Starting foreground service");
				/*
				// TODO(developer). If targeting O, use the following code.
				if (Build.VERSION.SDK_INT == Build.VERSION_CODES.O) {
					mNotificationManager.startServiceInForeground(new Intent(this,
							LocationUpdatesService.class), NOTIFICATION_ID, getNotification());
				} else {
					startForeground(NOTIFICATION_ID, getNotification());
				}
				 */
				StartForeground(NotificationId, GetNotification());
			}
			return true; // Ensures onRebind() is called when a client re-binds.
		}

		public override void OnDestroy()
		{
			ServiceHandler.RemoveCallbacksAndMessages(null);
		}

		/**
	     * Makes a request for location updates. Note that in this sample we merely log the
	     * {@link SecurityException}.
	     */
		public void RequestLocationUpdates()
		{
			Log.Info(Tag, "Requesting location updates");
			Utils.SetRequestingLocationUpdates(this, true);
			StartService(new Intent(ApplicationContext, typeof(LocationUpdatesService)));
	        try {
	            FusedLocationClient.RequestLocationUpdates(LocationRequest, LocationCallback, Looper.MyLooper());
	        } catch (SecurityException unlikely) {
	            Utils.SetRequestingLocationUpdates(this, false);
				Log.Error(Tag, "Lost location permission. Could not request updates. " + unlikely);
			}
		}

		/**
	     * Removes location updates. Note that in this sample we merely log the
	     * {@link SecurityException}.
	     */
		public void RemoveLocationUpdates()
		{
			Log.Info(Tag, "Removing location updates");
			try
			{
				FusedLocationClient.RemoveLocationUpdates(LocationCallback);
				Utils.SetRequestingLocationUpdates(this, false);
				StopSelf();
			}
			catch (SecurityException unlikely)
			{
				Utils.SetRequestingLocationUpdates(this, true);
				Log.Error(Tag, "Lost location permission. Could not remove updates. " + unlikely);
			}
		}

        /**
	     * Returns the {@link NotificationCompat} used as part of the foreground service.
	     */
        Notification GetNotification()
        {
            Intent intent = new Intent(this, typeof(LocationUpdatesService));

            var text = Utils.GetLocationText(Location);

            // Extra to help us figure out if we arrived in onStartCommand via the notification or not.
            intent.PutExtra(ExtraStartedFromNotification, true);

            // The PendingIntent that leads to a call to onStartCommand() in this service.
            var servicePendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // The PendingIntent to launch activity.
            var activityPendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .AddAction(Resource.Drawable.ic_launch, GetString(Resource.String.launch_activity),
                    activityPendingIntent)
                .AddAction(Resource.Drawable.ic_cancel, GetString(Resource.String.remove_location_updates),
                    servicePendingIntent)
                .SetContentText(text)
                .SetContentTitle(Utils.GetLocationTitle(this))
                .SetOngoing(true)
                .SetPriority((int) NotificationPriority.High)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetTicker(text)
                .SetWhen(JavaSystem.CurrentTimeMillis());

            if (Build.VERSION.SdkInt>= Build.VERSION_CODES.O)
            {
                builder.SetChannelId(ChannelId);
            }

            return builder.Build();
        }

        private void GetLastLocation()
        {
            try
            {
                FusedLocationClient.LastLocation.AddOnCompleteListener(new GetLastLocationOnCompleteListener { Service = this });
            }
            catch (SecurityException unlikely)
            {
                Log.Error(Tag, "Lost location permission." + unlikely);
            }
        }

        public void OnNewLocation(Location location)
		{
			Log.Info(Tag, "New location: " + location);

			Location = location;

			// Notify anyone listening for broadcasts about the new location.
			Intent intent = new Intent(ActionBroadcast);
			intent.PutExtra(ExtraLocation, location);
			LocalBroadcastManager.GetInstance(ApplicationContext).SendBroadcast(intent);

			// Update notification content if running as a foreground service.
			if (ServiceIsRunningInForeground(this))
			{
				NotificationManager.Notify(NotificationId, GetNotification());
			}
		}

		/**
	     * Sets the location request parameters.
	     */
		void CreateLocationRequest()
		{
			LocationRequest = new LocationRequest();
			LocationRequest.SetInterval(UpdateIntervalInMilliseconds);
			LocationRequest.SetFastestInterval(FastestUpdateIntervalInMilliseconds);
			LocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
		}

		/**
	     * Returns true if this is a foreground service.
	     *
	     * @param context The {@link Context}.
	     */
		public bool ServiceIsRunningInForeground(Context context)
		{
			var manager = (ActivityManager) context.GetSystemService(ActivityService);
			foreach (var service in manager.GetRunningServices(Integer.MaxValue))
			{
				if (Class.Name.Equals(service.Service.ClassName))
				{
					if (service.Foreground)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	/**
	 * Class used for the client Binder.  Since this service runs in the same process as its
	 * clients, we don't need to deal with IPC.
	 */
	public class LocationUpdatesServiceBinder : Binder
	{
		readonly LocationUpdatesService service;

		public LocationUpdatesServiceBinder(LocationUpdatesService service)
		{
			this.service = service;
		}

		public LocationUpdatesService GetLocationUpdatesService()
		{
			return service;
		}
	}

    public class GetLastLocationOnCompleteListener : Object, IOnCompleteListener
    {
        public LocationUpdatesService Service { get; set; }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful && task.Result != null)
            {
                Service.Location = (Location)task.Result;
            }
            else
            {
                Log.Warn(Service.Tag, "Failed to get location.");
            }
        }
    }
}
