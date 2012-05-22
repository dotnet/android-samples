//
// Copyright (C) 2007 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label="OS/Sensors")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Sensors : Activity
	{
		private SensorManager sensor_manager;
		private GraphView graph_view;

		// Initialization of the Activity after it is first created.  Must at least
		// call {@link android.app.Activity#setContentView setContentView()} to
		// describe what is to be displayed in the screen.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			// Be sure to call the base class.
			base.OnCreate (savedInstanceState);

			sensor_manager = (SensorManager)GetSystemService (SensorService);
			graph_view = new GraphView (this.BaseContext);
			SetContentView (graph_view);

			var accel = sensor_manager.GetDefaultSensor (SensorType.Accelerometer);
			var mag = sensor_manager.GetDefaultSensor (SensorType.MagneticField);
			var ori = sensor_manager.GetDefaultSensor (SensorType.Orientation);

			sensor_manager.RegisterListener (graph_view, accel, SensorDelay.Fastest);
			sensor_manager.RegisterListener (graph_view, mag, SensorDelay.Fastest);
			sensor_manager.RegisterListener (graph_view, ori, SensorDelay.Fastest);

		}

		protected override void OnResume ()
		{
			base.OnResume ();

			var accel = sensor_manager.GetDefaultSensor (SensorType.Accelerometer);
			var mag = sensor_manager.GetDefaultSensor (SensorType.MagneticField);
			var ori = sensor_manager.GetDefaultSensor (SensorType.Orientation);

			sensor_manager.RegisterListener (graph_view, accel, SensorDelay.Fastest);
			sensor_manager.RegisterListener (graph_view, mag, SensorDelay.Fastest);
			sensor_manager.RegisterListener (graph_view, ori, SensorDelay.Fastest);
		}

		protected override void OnStop ()
		{
			sensor_manager.UnregisterListener (graph_view);
			base.OnStop ();
		}

		private class GraphView : View, ISensorEventListener
		{
			private Bitmap mBitmap;
			private Paint paint = new Paint ();
			private Canvas mCanvas = new Canvas ();
			private Path mPath = new Path ();
			private RectF mRect = new RectF ();
			private float[] mLastValues = new float[3 * 2];
			private float[] mOrientationValues = new float[3];
			private Color[] mColors = new Color[3 * 2];
			private float mLastX;
			private float[] mScale = new float[2];
			private float mYOffset;
			private float mMaxX;
			private float mSpeed = 1.0f;
			private float mWidth;
			private float mHeight;

			public GraphView (Context context)
				: base (context)
			{
				mColors[0] = new Color (255, 64, 64, 192);
				mColors[1] = new Color (64, 128, 64, 192);
				mColors[2] = new Color (64, 64, 255, 192);
				mColors[3] = new Color (64, 255, 255, 192);
				mColors[4] = new Color (128, 64, 128, 192);
				mColors[5] = new Color (255, 255, 64, 192);

				paint.Flags = PaintFlags.AntiAlias;
				mRect.Set (-0.5f, -0.5f, 0.5f, 0.5f);
				mPath.ArcTo (mRect, 0, 180);
			}

			protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
			{
				mBitmap = Bitmap.CreateBitmap (w, h, Bitmap.Config.Rgb565);
				mCanvas.SetBitmap (mBitmap);
				mCanvas.DrawColor (Color.Black);
				mYOffset = h * 0.5f;
				mScale[0] = -(h * 0.5f * (1.0f / (SensorManager.StandardGravity * 2)));
				mScale[1] = -(h * 0.5f * (1.0f / (SensorManager.MagneticFieldEarthMax)));
				mWidth = w;
				mHeight = h;

				if (mWidth < mHeight)
					mMaxX = w;
				else
					mMaxX = w - 50;

				mLastX = mMaxX;

				base.OnSizeChanged (w, h, oldw, oldh);
			}


			protected override void OnDraw (Canvas canvas)
			{
				lock (this) {
					if (mBitmap != null) {
						Path path = mPath;
						var outer = new Color (192, 192, 192);
						var inner = new Color (255, 112, 16);

						if (mLastX >= mMaxX) {
							mLastX = 0;
							Canvas cavas = mCanvas;
							float yoffset = mYOffset;
							float maxx = mMaxX;
							float oneG = SensorManager.StandardGravity * mScale[0];
							paint.Color = new Color (170, 170, 170);
							cavas.DrawColor (Color.Black);
							cavas.DrawLine (0, yoffset, maxx, yoffset, paint);
							cavas.DrawLine (0, yoffset + oneG, maxx, yoffset + oneG, paint);
							cavas.DrawLine (0, yoffset - oneG, maxx, yoffset - oneG, paint);
						}
						canvas.DrawBitmap (mBitmap, 0, 0, null);

						float[] values = mOrientationValues;
						if (mWidth < mHeight) {
							float w0 = mWidth * 0.333333f;
							float w = w0 - 32;
							float x = w0 * 0.5f;
							for (int i = 0; i < 3; i++) {
								canvas.Save (SaveFlags.Matrix);
								canvas.Translate (x, w * 0.5f + 4.0f);
								canvas.Save (SaveFlags.Matrix);
								paint.Color = new Color(outer); //4.2
								canvas.Scale (w, w);
								canvas.DrawOval (mRect, paint);
								canvas.Restore ();
								canvas.Scale (w - 5, w - 5);
								paint.Color = new Color(inner); //4.2
								canvas.Rotate (-values[i]);
								canvas.DrawPath (path, paint);
								canvas.Restore ();
								x += w0;
							}
						} else {
							float h0 = mHeight * 0.333333f;
							float h = h0 - 32;
							float y = h0 * 0.5f;
							for (int i = 0; i < 3; i++) {
								canvas.Save (SaveFlags.Matrix);
								canvas.Translate (mWidth - (h * 0.5f + 4.0f), y);
								canvas.Save (SaveFlags.Matrix);
								paint.Color = new Color(outer); //4.2
								canvas.Scale (h, h);
								canvas.DrawOval (mRect, paint);
								canvas.Restore ();
								canvas.Scale (h - 5, h - 5);
								paint.Color = new Color(inner); //4.2
								canvas.Rotate (-values[i]);
								canvas.DrawPath (path, paint);
								canvas.Restore ();
								y += h0;
							}
						}

					}
				}
			}

			public void OnSensorChanged (SensorEvent e)
			{
				var sensor = e.Sensor;
				var values = e.Values;

				lock (this) {
					if (mBitmap != null) {
						Canvas canvas = mCanvas;
						if (sensor.Type == SensorType.Accelerometer) {
							for (int i = 0; i < 3; i++) {
								mOrientationValues[i] = values[i];
							}
						} else {
							float deltaX = mSpeed;
							float newX = mLastX + deltaX;

							int j = (sensor.Type == SensorType.MagneticField) ? 1 : 0;
							for (int i = 0; i < 3; i++) {
								int k = i + j * 3;
								float v = mYOffset + values[i] * mScale[j];
								paint.Color = new Color(mColors[k]); //4.2
								canvas.DrawLine (mLastX, mLastValues[k], newX, v, paint);
								mLastValues[k] = v;
							}
							if (sensor.Type == SensorType.MagneticField)
								mLastX += mSpeed;
						}
						Invalidate ();
					}
				}
			}

			public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy)
			{
				// Do nothing
			}
		}
	}
}
