using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace BoundServiceDemo
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		Button timestampButton;
		Button stopServiceButton;
		Button restartServiceButton;
		internal TextView timestampMessageTextView;

		TimestampServiceConnection serviceConnection;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);

			timestampButton = FindViewById<Button>(Resource.Id.get_timestamp_button);
			stopServiceButton = FindViewById<Button>(Resource.Id.stop_timestamp_service_button);
			restartServiceButton = FindViewById<Button>(Resource.Id.restart_timestamp_service_button);

			timestampMessageTextView = FindViewById<TextView>(Resource.Id.message_textview);
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (serviceConnection == null)
			{
				serviceConnection = new TimestampServiceConnection(this);
			}
		
			Intent serviceToStart = new Intent(this, typeof(TimestampService));
			BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
			restartServiceButton.Enabled = false;
		}

		protected override void OnResume()
		{
			base.OnResume();

			stopServiceButton.Click += StopServiceButton_Click;
			stopServiceButton.Enabled = true;

			timestampButton.Click += GetTimestampButton_Click;
			timestampButton.Enabled = true;
		}

		protected override void OnPause()
		{
			timestampButton.Click -= GetTimestampButton_Click;
			stopServiceButton.Click -= StopServiceButton_Click;
			restartServiceButton.Click -= RestartServiceButton_Click;

			base.OnPause();
		}

		protected override void OnStop()
		{
			UnbindService(serviceConnection);
			base.OnStop();
		}

		void GetTimestampButton_Click(object sender, System.EventArgs e)
		{
			if (serviceConnection.IsConnected)
			{
				timestampMessageTextView.Text = serviceConnection.Binder.Service.GetFormattedTimestamp();
			}
			else
			{
				timestampMessageTextView.SetText(Resource.String.service_not_connected);
			}
		}

		void StopServiceButton_Click(object sender, System.EventArgs e)
		{
			UnbindService(serviceConnection);

			timestampButton.Click -= GetTimestampButton_Click;
			timestampButton.Enabled = false;

			stopServiceButton.Click -= StopServiceButton_Click;
			stopServiceButton.Enabled = false;

			restartServiceButton.Click += RestartServiceButton_Click;
			restartServiceButton.Enabled = true;

			timestampMessageTextView.SetText(Resource.String.service_not_connected);
		}

		void RestartServiceButton_Click(object sender, System.EventArgs e)
		{
			Intent serviceToStart = new Intent(this, typeof(TimestampService));
			BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
			restartServiceButton.Enabled = false;

			stopServiceButton.Click += StopServiceButton_Click;
			stopServiceButton.Enabled = true;

			timestampButton.Click += GetTimestampButton_Click;
			timestampButton.Enabled = true;

			timestampMessageTextView.Text = string.Empty;
		}
	}
}

