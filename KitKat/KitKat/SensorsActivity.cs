using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Hardware;
using Android.Util;
using Android.Content.PM;

namespace KitKat
{
	[Activity (Label = "SensorsActivity")]			
	public class SensorsActivity : Activity, ISensorEventListener
	{
		Button trackButton;
		TextView stepCount;

		SensorManager senMgr;
		Sensor counter;
		float count = 0;
		bool visible;

		#region Lifecycle
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Sensors);

			trackButton = FindViewById<Button> (Resource.Id.trackButton);
			stepCount = FindViewById<TextView> (Resource.Id.stepCount);

			// create a sensor manager to schedule batches of sensor data
			senMgr = (SensorManager) GetSystemService (SensorService);

			// update state from orientation change
			if (bundle != null)
			{
				count = bundle.GetFloat ("step_count", 0);
				if (bundle.GetBoolean ("visible", false)) {
					visible = true;
					stepCount.Text = count.ToString ();
				}
				Log.Debug(GetType().FullName, "Recovered instance state");
			}


			// This button gets the user's step count since the last time the device was rebooted
			trackButton.Click += (o, e) => {
				// get the step counter sensor via the SensorManager
				counter = senMgr.GetDefaultSensor (SensorType.StepCounter);

				// button's been clicked, so counter visibility gets set to true
				visible = true;

				// This sensor is only available on Nexus 5 and Moto X at time of writing
				// The following line will check if the sensor is available explicitly:
				bool counterAvailabe = PackageManager.HasSystemFeature(PackageManager.FeatureSensorStepCounter);
				Log.Info("SensorManager", "Counter available");

				if (counterAvailabe && counter != null) {
					// Set sensor delay to normal, the default rate for batching sensor data
					senMgr.RegisterListener(this, counter, SensorDelay.Normal);
					Toast.MakeText(this,"Count sensor started",ToastLength.Long).Show();
				} else {
					Toast.MakeText(this, "Count sensor unavailable", ToastLength.Long).Show();
				}
			};

		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutFloat ("step_count", count);
			outState.PutBoolean ("visible", visible);
			base.OnSaveInstanceState (outState);
		}

		protected override void OnPause()
		{
			base.OnPause ();
			Log.Debug ("SensorActivity", "Activity entering background");

			// stop adding to the sensor count after app has exited
			senMgr.UnregisterListener(this); 
			Log.Debug ("SensorManager", "Unregistered listener");
		}

		protected override void OnStop()
		{
			base.OnStop();
			Log.Debug ("SensorActivity", "Activity is in the background");
		}

		protected override void OnResume()
		{
			base.OnResume();
			Log.Debug ("SensorActivity", "Activity has resumed");
		}

		#endregion

		#region SensorEventListener

		public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy)
		{
			Log.Info ("SensorManager", "Sensor accuracy changed");
		}

		public void OnSensorChanged (SensorEvent e)
		{
			count = e.Values [0];
			Log.Debug ("SensorManager", "Count updated to " + count.ToString());
			stepCount.Text = count.ToString();
		}

		#endregion
	}
}

