using System;
using Android.Gms.Location;

namespace Geofencing
{
	public class SimpleGeofence
	{
		// Instance variables
		readonly string mId;
		readonly double mLatitude;
		readonly double mLongitude;
		readonly float mRadius;
		long mExpirationDuration;
		int mTransitionType;

		// Instance field properies
		public string Id { get { return mId; } }
		public double Latitude { get { return mLatitude; } }
		public double Longitude { get { return mLongitude; } }
		public float Radius { get { return mRadius; } }
		public long ExpirationDuration { get { return mExpirationDuration; } }
		public int TransitionType { get { return mTransitionType; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Geofencing.SimpleGeofence"/> class.
		/// </summary>
		/// <param name="geofenceId">The Geofence's request ID</param>
		/// <param name="latitude">Latitude of the Geofence's center in degrees</param>
		/// <param name="longitude">Longitude of the Geofence's center in degrees</param>
		/// <param name="radius">Radius of the geofence circle in meters</param>
		/// <param name="expiration">Geofence expiration duration</param>
		/// <param name="transition">Type of Geofence transition</param>
		public SimpleGeofence (String geofenceId, double latitude, double longitude, float radius, long expiration, int transition)
		{
			// Set the instance fields from the constructor.
			this.mId = geofenceId;
			this.mLatitude = latitude;
			this.mLongitude = longitude;
			this.mRadius = radius;
			this.mExpirationDuration = expiration;
			this.mTransitionType = transition;
		}

		/// <summary>
		/// Creates a Location Services Geofenceo object from SimpleGeofence
		/// </summary>
		/// <returns>A Geofence object</returns>
		public IGeofence ToGeofence() {
			// Build a new Geofence object.
			return new GeofenceBuilder()
				.SetRequestId(mId)
				.SetTransitionTypes(mTransitionType)
				.SetCircularRegion(mLatitude, mLongitude, mRadius)
				.SetExpirationDuration(mExpirationDuration)
				.Build();
		}
	}
}

