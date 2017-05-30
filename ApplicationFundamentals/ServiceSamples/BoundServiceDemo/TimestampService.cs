using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;

namespace BoundServiceDemo
{
	[Service(Name="com.xamarin.ServicesDemo1")]
	public class TimestampService : Service, IGetTimestamp
	{
		static readonly string TAG = typeof(TimestampService).FullName;
		IGetTimestamp timestamper;

		public IBinder Binder { get; private set; }

		public override void OnCreate()
		{
			// This method is optional to implement
			base.OnCreate();
			Log.Debug(TAG, "OnCreate");
			timestamper = new UtcTimestamper();
		}

		public override IBinder OnBind(Intent intent)
		{
			// This method must always be implemented
			Log.Debug(TAG, "OnBind");
			this.Binder = new TimestampBinder(this);
			return this.Binder;
		}

		public override bool OnUnbind(Intent intent)
		{
			// This method is optional to implement
			Log.Debug(TAG, "OnUnbind");
			return base.OnUnbind(intent);
		}

		public override void OnDestroy()
		{
			// This method is optional to implement
			Log.Debug(TAG, "OnDestroy");
			Binder = null;
			timestamper = null;
			base.OnDestroy();
		}

		/// <summary>
		/// This method will return a formatted timestamp to the client.
		/// </summary>
		/// <returns>A string that details what time the service started and how long it has been running.</returns>
		public string GetFormattedTimestamp()
		{
			return timestamper?.GetFormattedTimestamp();
		}
	}
}
