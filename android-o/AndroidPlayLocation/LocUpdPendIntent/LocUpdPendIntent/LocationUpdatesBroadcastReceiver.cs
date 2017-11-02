using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;

namespace LocUpdPendIntent
{
	/**
	 * Receiver for handling location updates.
	 *
	 * For apps targeting API level O
	 * {@link android.app.PendingIntent#getBroadcast(Context, int, Intent, int)} should be used when
	 * requesting location updates. Due to limits on background services,
	 * {@link android.app.PendingIntent#getService(Context, int, Intent, int)} should not be used.
	 *
	 *  Note: Apps running on "O" devices (regardless of targetSdkVersion) may receive updates
	 *  less frequently than the interval specified in the
	 *  {@link com.google.android.gms.location.LocationRequest} when the app is no longer in the
	 *  foreground.
	 */
	[BroadcastReceiver(Exported = true)]
	[IntentFilter (new string[] { ActionProcessUpdates })]
	public class LocationUpdatesBroadcastReceiver : BroadcastReceiver
	{
		const string Tag = "LUBroadcastReceiver";

    	public const string ActionProcessUpdates = "com.xamarin.LocUpdPendIntent.action.PROCESS_UPDATES";
		
		public override void OnReceive(Context context, Intent intent)
		{
			if (intent != null)
			{
				var action = intent.Action;
				if (ActionProcessUpdates.Equals(action))
				{
					var result = LocationResult.ExtractResult(intent);
					if (result != null)
					{
						var locations = result.Locations;
						Utils.SetLocationUpdatesResult(context, locations);
						Utils.SendNotification(context, Utils.GetLocationResultTitle(context, locations));
						Log.Info(Tag, Utils.GetLocationUpdatesResult(context));
					}
				}
			}
		}
	}
}
