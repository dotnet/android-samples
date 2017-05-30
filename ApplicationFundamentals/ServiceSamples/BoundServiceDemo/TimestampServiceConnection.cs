using Android.Util;
using Android.OS;
using Android.Content;

namespace BoundServiceDemo
{
	public class TimestampServiceConnection : Java.Lang.Object, IServiceConnection, IGetTimestamp
	{
		static readonly string TAG = typeof(TimestampServiceConnection).FullName;

		MainActivity mainActivity;
		public TimestampServiceConnection(MainActivity activity)
		{
			IsConnected = false;
			Binder = null;
			mainActivity = activity;
		}

		public bool IsConnected { get; private set; }
		public TimestampBinder Binder { get; private set; }

		public void OnServiceConnected(ComponentName name, IBinder service)
		{
			Binder = service as TimestampBinder;
			IsConnected = this.Binder != null;

            string message = "onServiceConnected - ";
			Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

			if (IsConnected)
			{
                message = message + " bound to service " + name.ClassName;
                mainActivity.UpdateUiForBoundService();
			}
			else
			{
				message = message + " not bound to service " + name.ClassName;
                mainActivity.UpdateUiForUnboundService();
			}

            Log.Info(TAG, message);
            mainActivity.timestampMessageTextView.Text = message;

		}

		public void OnServiceDisconnected(ComponentName name)
		{
			Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
			IsConnected = false;
			Binder = null;
            mainActivity.UpdateUiForUnboundService();
		}

		public string GetFormattedTimestamp()
		{
			if (!IsConnected)
			{
				return null;
			}

			return Binder?.GetFormattedTimestamp();
		}
	}

}
