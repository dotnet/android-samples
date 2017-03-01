using Android.OS;
using Android.Util;
using Android.Content;
using System;

namespace MessengerClient
{
	public class TimestampServiceConnection : Java.Lang.Object, IServiceConnection
	{
		static readonly string TAG = typeof(TimestampServiceConnection).FullName;

		WeakReference<MainActivity> mainActivity;
		public TimestampServiceConnection(MainActivity activity)
		{
			IsConnected = false;
			mainActivity = new WeakReference<MainActivity>(activity);
		}

		public bool IsConnected { get; private set; }
		public Messenger Messenger { get; private set; }

		public MainActivity Activity
		{
			get
			{
				MainActivity activity;
				if (mainActivity.TryGetTarget(out activity))
				{
					return activity;
				}
				return null;
			}
		}

		public void OnServiceConnected(ComponentName name, IBinder service)
		{
			Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

			Messenger = new Messenger(service);

			IsConnected = this.Messenger != null;

			if (IsConnected)
			{
				Activity.timestampMessageTextView.SetText(Resource.String.service_started);
				Activity.sayHelloButton.Enabled = true;
				Activity.askForTimestampButton.Enabled = true;
				Activity.isStarting = false;
			}
			else
			{
				Activity.timestampMessageTextView.SetText(Resource.String.service_not_connected);
				Activity.sayHelloButton.Enabled = false;
				Activity.askForTimestampButton.Enabled = false;
			}

		}

		public void OnServiceDisconnected(ComponentName name)
		{
			Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
			IsConnected = false;
			Messenger = null;
			Activity.sayHelloButton.Enabled = false;
			Activity.askForTimestampButton.Enabled = false;
			Activity.timestampMessageTextView.SetText(Resource.String.service_disconnected);
		}
	}
}