using System;
using Android.App;
using Android.Runtime;
using SpeedTracker.Db;

namespace SpeedTracker
{
	/**
	 * The {@link android.app.Application} class for the handset app.
	 */
	[Application]
	public class PhoneApplication : Application
	{
		private LocationDataManager mDataManager;

		public PhoneApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
		{
			// If OnCreate is overridden, the overridden c'tor will also be called.
		}

		public override void OnCreate()
		{
			base.OnCreate();
			var dbHelper = new LocationDbHelper(ApplicationContext);
			mDataManager = new LocationDataManager(dbHelper);
		}

		/**
		 * Returns an instance of {@link com.example.android.wearable.speedtracker.LocationDataManager}.
		 */
		public LocationDataManager GetDataManager()
		{
			return mDataManager;
		}
	}
}