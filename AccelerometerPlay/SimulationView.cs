/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.Content;
using Android.Hardware;
using Android.Util;
using Android.Views;
using Android.Graphics;

namespace AccelerometerPlay
{
	class SimulationView : View, ISensorEventListener
	{
		// diameter of the balls in meters
		public static float BALL_DIAMETER = 0.004f;
		public static float BALL_DIAMETER_2 = BALL_DIAMETER * BALL_DIAMETER;

		// friction of the virtual table and air
		public static float FRICTION = 0.1f;

		private SensorManager sensor_manager;
		private Sensor accel_sensor;

		private float meters_to_pixels_x;
		private float meters_to_pixels_y;

		private Bitmap ball_bitmap;
		private Bitmap wood_bitmap;

		private PointF origin = new PointF ();
		private PointF sensor_values = new PointF ();
		
		private long sensor_timestamp;
		private long cpu_timestamp;

		private ParticleSystem particles;
		private Display display;

		public PointF Bounds { get; private set; }

		public SimulationView (Context context, SensorManager sensorManager, IWindowManager window)
			: base (context)
		{
			Bounds = new PointF ();

			// Get an accelorometer sensor
			sensor_manager = sensorManager;
			accel_sensor = sensor_manager.GetDefaultSensor (SensorType.Accelerometer);

			// Calculate screen size and dpi
			var metrics = new DisplayMetrics ();
			window.DefaultDisplay.GetMetrics (metrics);

			meters_to_pixels_x = metrics.Xdpi / 0.0254f;
			meters_to_pixels_y = metrics.Ydpi / 0.0254f;

			// Rescale the ball so it's about 0.5 cm on screen
			var ball = BitmapFactory.DecodeResource (Resources, Resource.Drawable.Ball);
			var dest_w = (int)(BALL_DIAMETER * meters_to_pixels_x + 0.5f);
			var dest_h = (int)(BALL_DIAMETER * meters_to_pixels_y + 0.5f);
			ball_bitmap = Bitmap.CreateScaledBitmap (ball, dest_w, dest_h, true);

			// Load the wood background texture
			var opts = new BitmapFactory.Options ();
			opts.InDither = true;
			opts.InPreferredConfig = Bitmap.Config.Rgb565;
			wood_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.Wood, opts);

			display = window.DefaultDisplay;
			particles = new ParticleSystem (this);
		}

		public void StartSimulation ()
		{
			// It is not necessary to get accelerometer events at a very high
			// rate, by using a slower rate (SENSOR_DELAY_UI), we get an
			// automatic low-pass filter, which "extracts" the gravity component
			// of the acceleration. As an added benefit, we use less power and
			// CPU resources.
			sensor_manager.RegisterListener (this, accel_sensor, SensorDelay.Ui);
		}

		public void StopSimulation ()
		{
			sensor_manager.UnregisterListener (this);
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			// Compute the origin of the screen relative
			// to the origin of the bitmap
			origin.Set ((w - ball_bitmap.Width) * 0.5f, (h - ball_bitmap.Height) * 0.5f);

			Bounds.X = (((float)w / (float)meters_to_pixels_x - BALL_DIAMETER) * 0.5f);
			Bounds.Y = (((float)h / (float)meters_to_pixels_y - BALL_DIAMETER) * 0.5f);

			Console.WriteLine (Bounds.X);
		}

		protected override void OnDraw (Canvas canvas)
		{
			// Draw the background
			canvas.DrawBitmap (wood_bitmap, 0, 0, null);

			// Compute the new position of our object, based on accelerometer
			// data and present time.			
			var now = sensor_timestamp + (DateTime.Now.Ticks - cpu_timestamp);
			particles.Update (sensor_values.X, sensor_values.Y, now);

			foreach (var ball in particles.Balls) {
				// We transform the canvas so that the coordinate system matches
				// the sensors coordinate system with the origin in the center
				// of the screen and the unit is the meter.
				var x = origin.X + ball.Location.X * meters_to_pixels_x;
				var y = origin.Y - ball.Location.Y * meters_to_pixels_y;

				canvas.DrawBitmap (ball_bitmap, x, y, null);
			}

			// Make sure to redraw asap
			Invalidate ();
		}


		#region ISensorEventListener Members
		public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy)
		{
		}

		public void OnSensorChanged (SensorEvent e)
		{
			if (e.Sensor.Type != SensorType.Accelerometer)
				return;

			// Record the accelerometer data, the event's timestamp as well as
			// the current time. The latter is needed so we can calculate the
			// "present" time during rendering. In this application, we need to
			// take into account how the screen is rotated with respect to the
			// sensors (which always return data in a coordinate space aligned
			// to with the screen in its native orientation).
			switch (display.Rotation) {
				case SurfaceOrientation.Rotation0:
					sensor_values.Set (e.Values[0], e.Values[1]);
					break;
				case SurfaceOrientation.Rotation90:
					sensor_values.Set (-e.Values[1], e.Values[0]);
					break;
				case SurfaceOrientation.Rotation180:
					sensor_values.Set (-e.Values[0], -e.Values[1]);
					break;
				case SurfaceOrientation.Rotation270:
					sensor_values.Set (e.Values[1], -e.Values[0]);
					break;
			}

			sensor_timestamp = e.Timestamp;
			cpu_timestamp = DateTime.Now.Ticks;
		}
		#endregion
	}
}