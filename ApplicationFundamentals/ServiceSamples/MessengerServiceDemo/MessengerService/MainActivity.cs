using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using MessengerCore;

namespace MessengerService
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		static readonly string TAG = typeof(MainActivity).FullName;
		internal TextView timestampMessageTextView;
		internal Button sayHelloButton;
		internal Button askForTimestampButton;
		internal bool isStarting = false;
		TimestampServiceConnection serviceConnection;
		Messenger activityMessenger;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);
			timestampMessageTextView = FindViewById<TextView>(Resource.Id.message_textview);

			activityMessenger = new Messenger(new MainActivityMessageHandler(this));
			serviceConnection = new TimestampServiceConnection(this);

			sayHelloButton = FindViewById<Button>(Resource.Id.say_hello_to_service_button);
			sayHelloButton.Click += SayHelloToService_Click;

			askForTimestampButton = FindViewById<Button>(Resource.Id.get_timestamp_button);
			askForTimestampButton.Click += AskForTimestampButton_Click;

			Log.Info(TAG, $"MainActivity is running in process id {Android.OS.Process.MyPid()}.");

		}

		protected override void OnStart()
		{
			base.OnStart();

			Intent serviceToStart = new Intent(this, typeof(TimestampService));
			BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
			isStarting = true;
			Log.Debug(TAG, "BindService has been called.");
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (isStarting)
			{
				timestampMessageTextView.SetText(Resource.String.service_starting);
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		void SayHelloToService_Click(object sender, EventArgs e)
		{
			if (serviceConnection.Messenger != null)
			{
				Message msg = Message.Obtain(null, Constants.SAY_HELLO_TO_TIMESTAMP_SERVICE);
				try
				{
					serviceConnection.Messenger.Send(msg);
				}
				catch (RemoteException ex)
				{
					Log.Error(TAG, ex, "There was a error trying to send the message.");
				}
			}
			else {
				Toast.MakeText(this, "Seems like we're not connected to the service - nobody around to say hello to.", ToastLength.Short).Show();
			}
		}

		void AskForTimestampButton_Click(object sender, System.EventArgs e)
		{
			if (!serviceConnection.IsConnected)
			{
				Log.Warn(TAG, "Not connected to the service, so can't ask for the timestamp.");
				return;
			}

			Message msg = Message.Obtain(null, Constants.GET_UTC_TIMESTAMP);
			msg.ReplyTo = activityMessenger;

			try
			{
				serviceConnection.Messenger.Send(msg);
				Log.Debug(TAG, "Requested the timestamp from the Service.");
			}
			catch (RemoteException ex)
			{
				Log.Error(TAG, ex, "There was a problem sending the message.");
			}
		}

		protected override void OnStop()
		{
			UnbindService(serviceConnection);
			base.OnStop();
		}


	}
}

