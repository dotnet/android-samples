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
            timestampButton.Click += GetTimestampButton_Click;

			stopServiceButton = FindViewById<Button>(Resource.Id.stop_timestamp_service_button);
            stopServiceButton.Click += StopServiceButton_Click;

			restartServiceButton = FindViewById<Button>(Resource.Id.restart_timestamp_service_button);
            restartServiceButton.Click += RestartServiceButton_Click;

			timestampMessageTextView = FindViewById<TextView>(Resource.Id.message_textview);
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (serviceConnection == null)
			{
				serviceConnection = new TimestampServiceConnection(this);
			}
            DoBindService();
		}

		protected override void OnResume()
		{
			base.OnResume();
            if (serviceConnection.IsConnected) {
                UpdateUiForBoundService();
            }
            else {
                UpdateUiForUnboundService();
            }
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
            DoUnBindService();
			base.OnStop();
		}

        internal void UpdateUiForBoundService() {
            timestampButton.Enabled = true;
            stopServiceButton.Enabled = true;
            restartServiceButton.Enabled = false;

		}
        internal void UpdateUiForUnboundService() {
			timestampButton.Enabled = false;
			stopServiceButton.Enabled = false;
			restartServiceButton.Enabled = true;
		}

        void DoBindService() {
			Intent serviceToStart = new Intent(this, typeof(TimestampService));
			BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
			timestampMessageTextView.Text = "";
		}

        void DoUnBindService() {
            UnbindService(serviceConnection);
            restartServiceButton.Enabled = true;
            timestampMessageTextView.Text = "";
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
            DoUnBindService();
            UpdateUiForUnboundService();


		}

		void RestartServiceButton_Click(object sender, System.EventArgs e)
		{
            DoBindService();
            UpdateUiForBoundService();
		}
	}
}

