using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using System;

namespace ServicesDemo3
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		static readonly string TAG = typeof(MainActivity).FullName;

		Button stopServiceButton;
		Button startServiceButton;
		Intent startServiceIntent;
		Intent stopServiceIntent;
		bool isStarted = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);
			OnNewIntent(this.Intent);



			if (savedInstanceState != null)
			{
				isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
			}

			startServiceIntent = new Intent(this, typeof(TimestampService));
			startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);

			stopServiceIntent = new Intent(this, typeof(TimestampService));
			stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);


			stopServiceButton = FindViewById<Button>(Resource.Id.stop_timestamp_service_button);
			startServiceButton = FindViewById<Button>(Resource.Id.start_timestamp_service_button);
			if (isStarted)
			{
				stopServiceButton.Click += StopServiceButton_Click;
				stopServiceButton.Enabled = true;
				startServiceButton.Enabled = false;
			}
			else
			{
				startServiceButton.Click += StartServiceButton_Click;
				startServiceButton.Enabled = true;
				stopServiceButton.Enabled = false;
			}
		}

		protected override void OnNewIntent(Intent intent)
		{
			if (intent == null)
			{
				return;
			}

			var bundle = intent.Extras;
			if (bundle != null)
			{
				if (bundle.ContainsKey(Constants.SERVICE_STARTED_KEY) )
				{
					isStarted = true;
				}
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
			base.OnSaveInstanceState(outState);
		}

		protected override void OnDestroy()
		{
			//Log.Info(TAG, "Activity is being destroyed; stop the service.");

			//StopService(startServiceIntent);
			base.OnDestroy();
		}
		void StopServiceButton_Click(object sender, System.EventArgs e)
		{
			stopServiceButton.Click -= StopServiceButton_Click;
			stopServiceButton.Enabled = false;

			Log.Info(TAG, "User requested that the service be stopped.");
			StopService(stopServiceIntent);
			isStarted = false;

			startServiceButton.Click += StartServiceButton_Click;
			startServiceButton.Enabled = true;
		}

		void StartServiceButton_Click(object sender, System.EventArgs e)
		{
			startServiceButton.Enabled = false;
			startServiceButton.Click -= StartServiceButton_Click;

			StartService(startServiceIntent);
			Log.Info(TAG, "User requested that the service be started.");

			isStarted = true;
			stopServiceButton.Click += StopServiceButton_Click;

			stopServiceButton.Enabled = true;
		}
	}

	
}

