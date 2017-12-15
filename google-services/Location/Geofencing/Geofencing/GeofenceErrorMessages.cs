using System;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;

namespace Geofencing
{
    public static class GeofenceErrorMessages
    {
        public static String GetErrorString(Context context, Exception ex)
        {
            if (ex is ApiException)
            {
                return GetErrorString(context, ((ApiException)ex).StatusCode);
            }
            else
            {
                return context.Resources.GetString(Resource.String.unknown_geofence_error);
            }
        }
        public static string GetErrorString(Context context, int errorCode)
        {
            var mResources = context.Resources;
            switch (errorCode)
            {
                case GeofenceStatusCodes.GeofenceNotAvailable:
                    return mResources.GetString(Resource.String.geofence_not_available);
                case GeofenceStatusCodes.GeofenceTooManyGeofences:
                    return mResources.GetString(Resource.String.geofence_too_many_geofences);
                case GeofenceStatusCodes.GeofenceTooManyPendingIntents:
                    return mResources.GetString(Resource.String.geofence_too_many_pending_intents);
                default:
                    return mResources.GetString(Resource.String.unknown_geofence_error);
            }
        }
    }
}

