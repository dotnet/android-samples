using Android.App;
using Android.Gms.Location;
using Android.Util;
using Android.Content;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Graphics;

namespace Geofencing
{
    [Service]
    public class GeofenceTransitionsIntentService : IntentService
    {
        private const string TAG = "GeofenceTransitionsIS";

        public GeofenceTransitionsIntentService() : base(TAG)
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent.HasError)
            {
                var errorMessage = GeofenceErrorMessages.GetErrorString(this, geofencingEvent.ErrorCode);
                Log.Error(TAG, errorMessage);
                return;
            }

            int geofenceTransition = geofencingEvent.GeofenceTransition;

            if (geofenceTransition == Geofence.GeofenceTransitionEnter ||
                geofenceTransition == Geofence.GeofenceTransitionExit)
            {

                IList<IGeofence> triggeringGeofences = geofencingEvent.TriggeringGeofences;

                string geofenceTransitionDetails = GetGeofenceTransitionDetails(this, geofenceTransition, triggeringGeofences);

                SendNotification(geofenceTransitionDetails);
                Log.Info(TAG, geofenceTransitionDetails);
            }
            else
            {
                // Log the error.
                Log.Error(TAG, GetString(Resource.String.geofence_transition_invalid_type, new[] { new Java.Lang.Integer(geofenceTransition) }));
            }
        }

        string GetGeofenceTransitionDetails(Context context, int geofenceTransition, IList<IGeofence> triggeringGeofences)
        {
            string geofenceTransitionString = GetTransitionString(geofenceTransition);

            var triggeringGeofencesIdsList = new List<string>();
            foreach (IGeofence geofence in triggeringGeofences)
            {
                triggeringGeofencesIdsList.Add(geofence.RequestId);
            }
            var triggeringGeofencesIdsString = string.Join(", ", triggeringGeofencesIdsList);

            return geofenceTransitionString + ": " + triggeringGeofencesIdsString;
        }

        void SendNotification(string notificationDetails)
        {
            var notificationIntent = new Intent(ApplicationContext, typeof(MainActivity));

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(notificationIntent);

            var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var builder = new NotificationCompat.Builder(this);
            builder.SetSmallIcon(Resource.Drawable.icon)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon))
                .SetColor(Color.Red)
                .SetContentTitle(notificationDetails)
                .SetContentText(GetString(Resource.String.geofence_transition_notification_text))
                .SetContentIntent(notificationPendingIntent);

            builder.SetAutoCancel(true);

            var mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            mNotificationManager.Notify(0, builder.Build());
        }

        string GetTransitionString(int transitionType)
        {
            switch (transitionType)
            {
                case Geofence.GeofenceTransitionEnter:
                    return GetString(Resource.String.geofence_transition_entered);
                case Geofence.GeofenceTransitionExit:
                    return GetString(Resource.String.geofence_transition_exited);
                default:
                    return GetString(Resource.String.unknown_geofence_transition);
            }
        }
    }
}

