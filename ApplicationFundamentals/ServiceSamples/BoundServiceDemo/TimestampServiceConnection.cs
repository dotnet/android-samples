using Android.App;
using Android.Util;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;

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
			Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

			if (IsConnected)
			{
				mainActivity.timestampMessageTextView.SetText(Resource.String.service_started);
			}
			else
			{
				mainActivity.timestampMessageTextView.SetText(Resource.String.service_not_connected);
			}

		}

		public void OnServiceDisconnected(ComponentName name)
		{
			Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
			IsConnected = false;
			Binder = null;
			mainActivity.timestampMessageTextView.SetText(Resource.String.service_not_connected);
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
