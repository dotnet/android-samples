using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using MessengerCore;

namespace MessengerService
{
	/// <summary>
	/// This is the Timestamp service class.
	/// </summary>
	[Service(Name = "com.xamarin.TimestampService", 
	         Exported=true, 
	         Permission="com.xamarin.xample.messengerservice.REQUEST_TIMESTAMP",
	         Process="com.xamarin.xample.messengerservice.timestampservice_process")]
    // Currently there is an issue with Xamarin.Android where the service
    // will crash on startup when attempting to run it in it's own process. 
    // See https://bugzilla.xamarin.com/show_bug.cgi?id=51940
	public class TimestampService : Service, IGetTimestamp
	{
		static readonly string TAG = typeof(TimestampService).FullName;
		IGetTimestamp timestamper;
		Messenger messenger ;

		public override void OnCreate()
		{
			base.OnCreate();
			Log.Debug(TAG, "OnCreate");
			timestamper = new UtcTimestamper();
			messenger = new Messenger(new TimestampRequestHandler(this));

			Log.Info(TAG, $"TimestampService is running in process id {Android.OS.Process.MyPid()}.");
		}

		public override IBinder OnBind(Intent intent)
		{
			Log.Debug(TAG, "OnBind");
			return messenger.Binder;
		}

		public override void OnDestroy()
		{
			Log.Debug(TAG, "OnDestroy");
			messenger.Dispose();
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
