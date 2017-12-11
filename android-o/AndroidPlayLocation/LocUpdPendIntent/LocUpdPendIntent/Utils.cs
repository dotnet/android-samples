using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Java.Lang;

namespace LocUpdPendIntent
{
	/**
	 * Utility methods used in this sample.
	 */
	public class Utils
	{
		public const string KeyLocationUpdatesRequested = "location-updates-requested";
		public const string KeyLocationUpdatesResult = "location-update-result";
		public const string ChannelId = "channel_01";

		public static void SetRequestingLocationUpdates(Context context, bool value)
		{
			PreferenceManager.GetDefaultSharedPreferences(context)
			        .Edit()
			        .PutBoolean(KeyLocationUpdatesRequested, value)
					.Apply();
		}

		public static bool GetRequestingLocationUpdates(Context context)
		{
			return PreferenceManager.GetDefaultSharedPreferences(context)
				    .GetBoolean(KeyLocationUpdatesRequested, false);
		}

		/**
	     * Posts a notification in the notification bar when a transition is detected.
	     * If the user clicks the notification, control goes to the MainActivity.
	     */
		public static void SendNotification(Context context, string notificationDetails)
		{
			// Create an explicit content Intent that starts the main Activity.
			var notificationIntent = new Intent(context, typeof(MainActivity));

	        notificationIntent.PutExtra("from_notification", true);

	        // Construct a task stack.
	        var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);

			// Add the main Activity to the task stack as the parent.
			stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));

	        // Push the content Intent onto the stack.
	        stackBuilder.AddNextIntent(notificationIntent);

	        // Get a PendingIntent containing the entire back stack.
			var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			// Get a notification builder that's compatible with platform versions >= 4
			NotificationCompat.Builder builder = new NotificationCompat.Builder(context);

			// Define the notification settings.
			builder.SetSmallIcon(Resource.Mipmap.ic_launcher)
	                // In a real app, you may want to use a library like Volley
	                // to decode the Bitmap.
	                .SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Mipmap.ic_launcher))
	                .SetColor(Color.Red)
	                .SetContentTitle("Location update")
	                .SetContentText(notificationDetails)
	                .SetContentIntent(notificationPendingIntent);

			// Dismiss notification once the user touches it.
			builder.SetAutoCancel(true);

	        // Get an instance of the Notification manager
			var mNotificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;

		    // Android O requires a Notification Channel.
		    if (Build.VERSION.SdkInt>= Build.VERSION_CODES.O)
		    {
		        string name = context.GetString(Resource.String.app_name);
		        // Create the channel for the notification
		        // Create the channel for the notification
		        NotificationChannel mChannel = new NotificationChannel(ChannelId, name, NotificationManager.ImportanceDefault);

		        // Set the Notification Channel for the Notification Manager.
		        mNotificationManager.CreateNotificationChannel(mChannel);

		        // Channel ID
		        builder.SetChannelId(ChannelId);
		    }

            // Issue the notification
            mNotificationManager.Notify(0, builder.Build());
	    }

		/**
	     * Returns the title for reporting about a list of {@link Location} objects.
	     *
	     * @param context The {@link Context}.
	     */
		public static string GetLocationResultTitle(Context context, IList<Location> locations)
		{
			var numLocationsReported = context.Resources.GetQuantityString(
					Resource.Plurals.num_locations_reported, locations.Count, locations.Count);
			return numLocationsReported + ": " + DateTime.Now.ToString();
		}

		/**
	     * Returns te text for reporting about a list of  {@link Location} objects.
	     *
	     * @param locations List of {@link Location}s.
	     */
		public static string GetLocationResultText(Context context, IList<Location> locations)
		{
			if (locations.Count == 0)
			{
				return context.GetString(Resource.String.unknown_location);
			}
			var sb = new StringBuilder();
			foreach (var location in locations)
			{
				sb.Append("(");
				sb.Append(location.Latitude);
				sb.Append(", ");
				sb.Append(location.Longitude);
				sb.Append(")");
				sb.Append("\n");
			}
			return sb.ToString();
		}

		public static void SetLocationUpdatesResult(Context context, IList<Location> locations)
		{
			PreferenceManager.GetDefaultSharedPreferences(context)
					.Edit()
			        .PutString(KeyLocationUpdatesResult, GetLocationResultTitle(context, locations)
							+ "\n" + GetLocationResultText(context, locations))
					.Apply();
		}

		public static string GetLocationUpdatesResult(Context context)
		{
			return PreferenceManager.GetDefaultSharedPreferences(context)
				    .GetString(KeyLocationUpdatesResult, "");
		}
	}
}
