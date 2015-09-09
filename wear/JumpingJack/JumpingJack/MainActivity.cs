using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Support.V4.View;
using Android.Util;

using Java.Util;
using Java.Lang;
using Android.Support.V4.App;

namespace JumpingJack
{
	public class MyOnPageChangeListener : Java.Lang.Object,ViewPager.IOnPageChangeListener
	{
		MainActivity activity;
		public MyOnPageChangeListener(MainActivity activity)
		{
			this.activity = activity;
		}

		public void OnPageScrolled(int i, float v, int i2)
		{
		}

		public void OnPageSelected(int i)
		{
			activity.SetIndicator (i);
			activity.RenewTimer ();
		}

		public void OnPageScrollStateChanged(int i)
		{
		}
	}

	public class MyTimerTask2 : TimerTask
	{
		MainActivity activity;
		public MyTimerTask2(MainActivity activity)
		{
			this.activity = activity;
		}

		public override void Run()
		{
			if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
				Log.Debug (MainActivity.TAG, "Removing the FLAG_KEEP_SCREEN_ON flag to allow going to background");
			}
			activity.ResetFlag ();
		}
	}

	public class MyRunnable2 : Java.Lang.Object,IRunnable
	{
		MainActivity activity;
		public MyRunnable2(MainActivity activity)
		{
			this.activity = activity;
		}

		public void Run()
		{
			if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
				Log.Debug (MainActivity.TAG, "Resetting FLAG_KEEP_SCREEN_ON flag to allow going to background");
			}
			activity.Window.ClearFlags (WindowManagerFlags.KeepScreenOn);
			activity.Finish ();
		}
	}

	[Activity (Label = "JumpingJack", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : FragmentActivity, ISensorEventListener
	{
		public const string TAG = "JJMainActivity";

		//how long to keep the screen on when there is no activity
		const int SCREEN_ON_TIMEOUT_MS = 20000;

		//an up and down movement that takes longer than 2 seconds will not be registered
		const int TIME_THRESHOLD_NS = 2000000000; //in nanoseconds

		//if the gravity delta is greater than the threshold, it will count
		const float GRAVITY_THRESHOLD = 7.0f;

		SensorManager sensor_manager;
		Sensor sensor;
		int last_time = 0;
		bool up = false;
		int jump_counter = 0;
		ViewPager pager;
		CounterFragment counter_page;
		SettingsFragment setting_page;
		ImageView second_indicator;
		ImageView first_indicator;
		Timer timer;
		TimerTask timer_task;
		Handler handler;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.jj_layout);
			SetUpViews ();
			handler = new Handler ();
			jump_counter = Utils.GetCounterFromPreference (this);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);
			RenewTimer ();
			sensor_manager = (SensorManager)GetSystemService (Context.SensorService);
			sensor = sensor_manager.GetDefaultSensor (SensorType.Gravity);


		}

		private void SetUpViews()
		{
			pager = (ViewPager)FindViewById<ViewPager> (Resource.Id.pager);
			first_indicator = (ImageView)FindViewById (Resource.Id.indicator_0);
			second_indicator = (ImageView)FindViewById (Resource.Id.indicator_1);
			PagerAdapter adapter = new PagerAdapter (SupportFragmentManager);
			counter_page = new CounterFragment ();
			setting_page = new SettingsFragment (this);
			adapter.AddFragment (counter_page);
			adapter.AddFragment (setting_page);
			SetIndicator (0);
			pager.SetOnPageChangeListener (new MyOnPageChangeListener (this));
			pager.Adapter = adapter;

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (sensor_manager.RegisterListener (this, sensor, SensorDelay.Normal)) {
				if(Log.IsLoggable(TAG,LogPriority.Debug)) {
					Log.Debug (TAG, "Successfully registered for the sensor updates");
				}
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			sensor_manager.UnregisterListener (this);
			if (Log.IsLoggable (TAG, LogPriority.Debug)) {
				Log.Debug (TAG, "Unregistered for sensor events");
			}
		}

		public void OnSensorChanged(SensorEvent e)
		{
			DetectJump (e.Values [0], (int)e.Timestamp);
		}

		public void OnAccuracyChanged(Sensor sensor,int accuracy)
		{

		}

		public void OnAccuracyChanged(Sensor sensor, SensorStatus status)
		{

		}

		//checks if the x value of gravity is greater than the threshold to register a count
		//only count those which took less than 2 seconds and if it is an up and down movement
		private void DetectJump(float xValue, int timestamp)
		{
			if ((System.Math.Abs (xValue) > GRAVITY_THRESHOLD)) {
				if (timestamp - last_time < TIME_THRESHOLD_NS && up != (xValue > 0)) {
					OnJumpDetected (!up);
				}
				up = xValue > 0;
				last_time = timestamp;
			}
		}
			
		private void OnJumpDetected(bool up)
		{
			if (up)
				return;
			jump_counter++;
			SetCounter (jump_counter);
			RenewTimer ();
		}

		//Increment the counter and make the device vibrate once a multiple of 10 is reached
		private void SetCounter(int i)
		{
			counter_page.SetCounter (i);
			Utils.SaveCounterToPreference (this, i);
			if (i > 0 && i % 10 == 0)
				Utils.Vibrate (this, 0);
			
		}

		public void ResetCounter()
		{
			SetCounter (0);
			RenewTimer ();
		}

		public void RenewTimer()
		{
			if (null != timer)
				timer.Cancel ();
			timer_task = new MyTimerTask2 (this);
			timer = new Timer ();
			timer.Schedule (timer_task, SCREEN_ON_TIMEOUT_MS);

		}

		public void ResetFlag()
		{
			handler.Post (new MyRunnable2 (this));
		}

		public void SetIndicator(int i)
		{
			switch (i) {
			case 0:
				first_indicator.SetImageResource (Resource.Drawable.full_10);
				second_indicator.SetImageResource (Resource.Drawable.empty_10);
				break;
			case 1:
				first_indicator.SetImageResource (Resource.Drawable.empty_10);
				second_indicator.SetImageResource (Resource.Drawable.full_10);
				break;
			}
		}


	}
}


