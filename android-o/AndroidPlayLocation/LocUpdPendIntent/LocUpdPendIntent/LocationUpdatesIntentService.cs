using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;

namespace LocUpdPendIntent
{
	/**
	 * Handles incoming location updates and displays a notification with the location data.
	 *
	 * For apps targeting API level 25 ("Nougat") or lower, location updates may be requested
	 * using {@link android.app.PendingIntent#getService(Context, int, Intent, int)} or
	 * {@link android.app.PendingIntent#getBroadcast(Context, int, Intent, int)}. For apps targeting
	 * API level O, only {@code getBroadcast} should be used.
	 *
	 *  Note: Apps running on "O" devices (regardless of targetSdkVersion) may receive updates
	 *  less frequently than the interval specified in the
	 *  {@link com.google.android.gms.location.LocationRequest} when the app is no longer in the
	 *  foreground.
	 */
	[Service(Label = "LocationUpdatesIntentService", Exported = false)]
	public class LocationUpdatesIntentService : IntentService
	{
		public const string ActionProcessUpdate = "com.xamarin.LocUpdPendIntent.action.PROCESS_UPDATES";
		const string Tag = "LocationUpdatesIntentService";

		public LocationUpdatesIntentService() : base(Tag) 
		{}

		protected override void OnHandleIntent(Intent intent)
		{
			if (intent != null)
			{
				var action = intent.Action;
				if (ActionProcessUpdate.Equals(action))
				{
					var result = LocationResult.ExtractResult(intent);
					if (result != null)
					{
						var locations = result.Locations;
						Utils.SetLocationUpdatesResult(this, locations);
						Utils.SendNotification(this, Utils.GetLocationResultTitle(this, locations));
						Log.Info(Tag, Utils.GetLocationUpdatesResult(this));
					}
				}
			}
		}
	}
}
