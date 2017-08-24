
using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace LocationUpdatesForegroundService
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
	[IntentFilter(new String[] { "com.xamarin.LocationUpdatesForegroundService.LocationUpdatesService" })]
	public class LocationUpdatesService : Service
	{
		const string PackageName = "com.google.android.gms.location.sample.locationupdatesforegroundservice";

		const string Tag = "LocationUpdatesService";

		const string ActionBroadcast = PackageName + ".broadcast";

		const string ExtraLocation = PackageName + ".location";
		const string ExtraStartedFromNotification = PackageName + ".started_from_notification";

		IBinder binder = new LocalBinder();

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
		bool mChangingConfiguration = false;

		NotificationManager mNotificationManager;

		/**
		 * Contains parameters used by {@link com.google.android.gms.location.FusedLocationProviderApi}.
		 */
		LocationRequest mLocationRequest;

		/**
		 * Provides access to the Fused Location Provider API.
		 */
		FusedLocationProviderClient mFusedLocationClient;

		/**
		 * Callback for changes in location.
		 */
		ILocationCallback mLocationCallback;

		Handler mServiceHandler;

		/**
		 * The current location.
		 */
		Android.Locations.Location mLocation;

		public LocationUpdatesService()
		{
		}

		public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			// start your service logic here

			// Return the correct StartCommandResult for the type of service you are building
			return StartCommandResult.NotSticky;
		}

		public override IBinder OnBind(Intent intent)
		{
			binder = new LocationUpdatesServiceBinder(this);
			return binder;
		}
	}

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
}
