using Android.Content;
using Android.OS;
using Android.Util;

namespace MessengerService
{
	public class TimestampServiceConnection : Java.Lang.Object, IServiceConnection
	{
		static readonly string TAG = typeof(TimestampServiceConnection).FullName;

		MainActivity mainActivity;
		public TimestampServiceConnection(MainActivity activity)
		{
			IsConnected = false;
			mainActivity = activity;
		}

		public bool IsConnected { get; private set; }
		public Messenger Messenger { get; private set; }

		public void OnServiceConnected(ComponentName name, IBinder service)
		{
			Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

			Messenger = new Messenger(service);

			IsConnected = this.Messenger != null;

			if (IsConnected)
			{
				mainActivity.timestampMessageTextView.SetText(Resource.String.service_started);
				mainActivity.sayHelloButton.Enabled = true;
				mainActivity.askForTimestampButton.Enabled = true;
				mainActivity.isStarting = false;
			}
			else
			{
				mainActivity.timestampMessageTextView.SetText(Resource.String.service_not_connected);
				mainActivity.sayHelloButton.Enabled = false;
				mainActivity.askForTimestampButton.Enabled = false;
			}

		}

		public void OnServiceDisconnected(ComponentName name)
		{
			Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
			IsConnected = false;
			Messenger = null;
			mainActivity.sayHelloButton.Enabled = false;
			mainActivity.askForTimestampButton.Enabled = false;
			mainActivity.timestampMessageTextView.SetText(Resource.String.service_disconnected);
		}
	}
}
