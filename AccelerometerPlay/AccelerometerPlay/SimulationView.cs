namespace AccelerometerPlay;

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
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Util;
using Android.Views;

class SimulationView : View, ISensorEventListener
{
	const string TAG = "APSV";

	// diameter of the balls in meters
	public static float BALL_DIAMETER = 0.004f;
	public static float BALL_DIAMETER_2 = BALL_DIAMETER * BALL_DIAMETER;

	// friction of the virtual table and air
	public static float FRICTION = 0.1f;

	SensorManager sensor_manager;
	Sensor accel_sensor;

	float meters_to_pixels_x;
	float meters_to_pixels_y;

	Bitmap ball_bitmap;
	Bitmap wood_bitmap;
	Bitmap wood_bitmap2;

	PointF origin = new PointF ();
	PointF sensor_values = new PointF ();
		
	long sensor_timestamp;
	long cpu_timestamp;

	ParticleSystem particles;
	Display display;

	public PointF Bounds { get; private set; }

	public SimulationView (Context context, SensorManager sensorManager, IWindowManager window)
		: base (context)
	{
		Bounds = new PointF ();

		// Get an accelorometer sensor
		sensor_manager = sensorManager;
		accel_sensor = sensor_manager.GetDefaultSensor (SensorType.Accelerometer) ?? throw new InvalidOperationException ("Unable to obtain default accelerometer sensor instance");

		// Calculate screen size and dpi
		DisplayMetrics metrics = context.Resources?.DisplayMetrics ?? throw new InvalidOperationException ("Unable to obtain display metrics");

		meters_to_pixels_x = metrics.Xdpi / 0.0254f;
		meters_to_pixels_y = metrics.Ydpi / 0.0254f;

		// Rescale the ball so it's about 0.5 cm on screen
		var ball = EnsureValidBitmap (
			BitmapFactory.DecodeResource (Resources, Resource.Mipmap.Ball),
			"Unable to decode the Ball bitmap resource"
		);
		var dest_w = (int)(BALL_DIAMETER * meters_to_pixels_x + 0.5f);
		var dest_h = (int)(BALL_DIAMETER * meters_to_pixels_y + 0.5f);
		ball_bitmap = EnsureValidBitmap (
			Bitmap.CreateScaledBitmap (ball, dest_w, dest_h, true),
			"Unable to create scaled ball bitmap"
		);

		// Load the wood background texture
		var opts = new BitmapFactory.Options ();

		// InDither is deprecated (and has no effect) since API24
		if (!OperatingSystem.IsAndroidVersionAtLeast(24)) {
			opts.InDither = true;
		}

		opts.InPreferredConfig = Bitmap.Config.Rgb565;
		wood_bitmap = EnsureValidBitmap (
			BitmapFactory.DecodeResource (Resources, Resource.Mipmap.Wood, opts),
			"Unable to decode the Wood bitmap resource (#1)"
		);
		wood_bitmap2 = EnsureValidBitmap (
			BitmapFactory.DecodeResource (Resources, Resource.Mipmap.Wood, opts),
			"Unable to decode the Wood bitmap resource (#1)"
		);

		display = window.DefaultDisplay ?? throw new InvalidOperationException ("Unable to obtain the default display instance");
		particles = new ParticleSystem (this);
	}

	Bitmap EnsureValidBitmap (Bitmap? bitmap, string errorMessage)
	{
		if (bitmap != null) {
			return bitmap;
		}

		throw new InvalidOperationException (errorMessage);
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

	protected override void OnDraw (Canvas? canvas)
	{
		// Draw the background
		if (canvas == null) {
			// Not much point in continuing since there's nothing to draw on
			Log.Warn (TAG, "Canvas is null in SimulationView.OnDraw, cannot paint");
			return;
		}

		canvas.DrawBitmap (wood_bitmap, 0, 0, null);
		canvas.DrawBitmap (wood_bitmap2, wood_bitmap.Width, 0, null);

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
	public void OnAccuracyChanged (Sensor? sensor, SensorStatus accuracy)
	{
	}

	public void OnSensorChanged (SensorEvent? e)
	{
		if (e == null || e.Sensor == null || e.Values == null) {
			Log.Warn (TAG, "Invalid event data in SimulationView.OnSensorChanged, cannot update");
			return;
		}

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
