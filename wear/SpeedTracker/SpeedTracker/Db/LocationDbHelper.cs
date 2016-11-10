using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Provider;
using Android.Util;
using Java.Util;
using Shared;

namespace SpeedTracker.Db
{
	/**
	 * A helper class to set up the database that holds the GPS location information
	 */

	public class LocationDbHelper : SQLiteOpenHelper
	{
		private static readonly string TAG = "LocationDbHelper";

		public static readonly string TABLE_NAME = "location";
		public static readonly string COLUMN_NAME_DAY = "day";
		public static readonly string COLUMN_NAME_LATITUDE = "lat";
		public static readonly string COLUMN_NAME_LONGITUDE = "lon";
		public static readonly string COLUMN_NAME_TIME = "time";

		private static readonly string TEXT_TYPE = " TEXT";
		private static readonly string INTEGER_TYPE = " INTEGER";
		private static readonly string REAL_TYPE = " REAL";
		private static readonly string COMMA_SEP = ",";

		private static readonly string SQL_CREATE_ENTRIES =
			"CREATE TABLE " + TABLE_NAME + " ("
			+ BaseColumns.Id + " INTEGER PRIMARY KEY,"
			+ COLUMN_NAME_DAY + TEXT_TYPE + COMMA_SEP
			+ COLUMN_NAME_LATITUDE + REAL_TYPE + COMMA_SEP
			+ COLUMN_NAME_LONGITUDE + REAL_TYPE + COMMA_SEP
			+ COLUMN_NAME_TIME + INTEGER_TYPE
			+ " )";

		private static readonly string SQL_DELETE_ENTRIES = "DROP TABLE IF EXISTS " + TABLE_NAME;

		public static readonly int DATABASE_VERSION = 1;
		public static readonly string DATABASE_NAME = "Location.db";

		public LocationDbHelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
		{
		}

		public override void OnCreate(SQLiteDatabase db)
		{
			db.ExecSQL(SQL_CREATE_ENTRIES);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL(SQL_DELETE_ENTRIES);
			OnCreate(db);
		}

		/**
		 * Inserts a {@link com.xamarin.wearable.speedtracker.common.LocationEntry} item to the
		 * database.
		 */

		public long Insert(LocationEntry entry)
		{
			if (Log.IsLoggable(TAG, LogPriority.Debug))
			{
				Log.Debug(TAG, "Inserting a LocationEntry");
			}
			// Gets the data repository in write mode
			var db = WritableDatabase;

			// Create a new map of values, where column names are the keys
			var values = new ContentValues();
			values.Put(COLUMN_NAME_DAY, entry.day);
			values.Put(COLUMN_NAME_LONGITUDE, entry.longitude);
			values.Put(COLUMN_NAME_LATITUDE, entry.latitude);
			values.Put(COLUMN_NAME_TIME, entry.calendar.TimeInMillis);

			// Insert the new row, returning the primary key value of the new row
			return db.Insert(TABLE_NAME, "null", values);
		}

		/**
		 * Returns a list of {@link com.example.android.wearable.speedtracker.common.LocationEntry}
		 * objects from the database for a given day. The list can be empty (but not {@code null}) if
		 * there are no such items. This method looks at the day that the calendar argument points at.
		 */

		public List<LocationEntry> Read(Calendar calendar)
		{
			var db = ReadableDatabase;
			string[] projection =
			{
				COLUMN_NAME_LONGITUDE,
				COLUMN_NAME_LATITUDE,
				COLUMN_NAME_TIME
			};
			var day = Utils.GetHashedDay(calendar);

			// sort ASC based on the time of the entry
			var sortOrder = COLUMN_NAME_TIME + " ASC";
			var selection = COLUMN_NAME_DAY + " LIKE ?";

			var cursor = db.Query(
				TABLE_NAME, // The table to query
				projection, // The columns to return
				selection, // The columns for the WHERE clause
				new[] {day}, // The values for the WHERE clause
				null, // don't group the rows
				null, // don't filter by row groups
				sortOrder // The sort order
				);

			var result = new List<LocationEntry>();
			var count = cursor.Count;
			if (count > 0)
			{
				cursor.MoveToFirst();
				while (!cursor.IsAfterLast)
				{
					var cal = Calendar.Instance;
					cal.TimeInMillis = cursor.GetLong(2);
					var entry = new LocationEntry(cal, cursor.GetDouble(1), cursor.GetDouble(0));
					result.Add(entry);
					cursor.MoveToNext();
				}
			}
			cursor.Close();
			return result;
		}

		/**
		 * Deletes all the entries in the database for the given day. The argument {@code day} should
		 * match the format provided by {@link GetHashedDay()}
		 */

		public int Delete(string day)
		{
			var db = WritableDatabase;
			// Define 'where' part of the query.
			var selection = COLUMN_NAME_DAY + " LIKE ?";
			string[] selectionArgs = {day};
			return db.Delete(TABLE_NAME, selection, selectionArgs);
		}

		/**
		 * Deletes all the entries in the database for the day that the {@link java.util.Calendar}
		 * argument points at.
		 */
		public int Delete(Calendar calendar)
		{
			return Delete(Utils.GetHashedDay(calendar));
		}

	}

}