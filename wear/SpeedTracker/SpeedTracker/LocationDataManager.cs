using System.Collections.Generic;
using Java.Util;
using Shared;
using SpeedTracker.Db;

namespace SpeedTracker
{
	/**
	 * A class that wraps database access and provides a cache for various GPS data.
	 */
	public class LocationDataManager
	{
		private readonly Dictionary<string, List<LocationEntry>> mPointsMap = new Dictionary<string, List<LocationEntry>>();

		private LocationDbHelper mDbHelper;

		public LocationDataManager(LocationDbHelper dbHelper)
		{
			mDbHelper = dbHelper;
		}

		/**
		 * Returns a list of {@link com.xamarin.android.wearable.speedtracker.common.LocationEntry}
		 * objects for the day that the {@link java.util.Calendar} object points at. Internally it uses
		 * a cache to speed up subsequent calls. If there is no cached value, it gets the result from
		 * the database.
		 */
		public List<LocationEntry> GetPoints(Calendar calendar)
		{
			var day = Utils.GetHashedDay(calendar);
			lock (mPointsMap)
			{
				if (mPointsMap.ContainsKey(day)) return mPointsMap[day];
				// there is no cache for this day, so lets get it from DB
				var points = mDbHelper.Read(calendar);
				mPointsMap.Add(day, points);
			}
			return mPointsMap[day];
		}

		/**
		 * Clears the data for the day that the {@link java.util.Calendar} object falls on. This method
		 * removes the entries from the database and updates the cache accordingly.
		 */
		public int ClearPoints(Calendar calendar)
		{
			lock (mPointsMap)
			{
				var day = Utils.GetHashedDay(calendar);
				mPointsMap.Remove(day);
				return mDbHelper.Delete(day);
			}
		}

		/**
		 * Adds a {@link com.example.android.wearable.speedtracker.common.LocationEntry} point to the
		 * database and cache if it is a new point.
		 */
		public void AddPoint(LocationEntry entry)
		{
			lock (mPointsMap)
			{
				var points = GetPoints(entry.calendar);
				if (points == null || points.Count == 0)
				{
					mDbHelper.Insert(entry);
					if (points == null)
					{
						points = new List<LocationEntry>();
					}
					points.Add(entry);
					mPointsMap.Add(entry.day, points);
				}
				else
				{
					if (points.Contains(entry)) return;
					mDbHelper.Insert(entry);
					points.Add(entry);
				}
			}
		}
	}
}