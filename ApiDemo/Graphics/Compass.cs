/*
 * Copyright (C) 2007 The Android Open Source Project
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
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/Compass")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Compass : GraphicsActivity
	{
		private SensorManager mSensorManager;
		private SampleView mView;
		private float[] mValues;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			mSensorManager = (SensorManager)GetSystemService (Context.SensorService);
			mView = new SampleView (this);

			SetContentView (mView);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			var ori = mSensorManager.GetDefaultSensor (SensorType.Orientation);

			mSensorManager.RegisterListener (mView, ori, SensorDelay.Fastest);
		}

		protected override void OnStop ()
		{
			mSensorManager.UnregisterListener (mView);
			base.OnStop ();
		}

		private class SampleView : View, ISensorEventListener
		{
			private Paint mPaint = new Paint ();
			private Path mPath = new Path ();
			private Compass compass;

			public SampleView (Context context) : base (context)
			{
				compass = (Compass)context;

				// Construct a wedge-shaped path
				mPath.MoveTo (0, -50);
				mPath.LineTo (-20, 60);
				mPath.LineTo (0, 50);
				mPath.LineTo (20, 60);
				mPath.Close ();
			}

			protected override void OnDraw (Canvas canvas)
			{
				Paint paint = mPaint;

				canvas.DrawColor (Color.White);

				paint.AntiAlias = true;
				paint.Color = Color.Black;
				paint.SetStyle (Paint.Style.Fill);

				int w = canvas.Width;
				int h = canvas.Height;
				int cx = w / 2;
				int cy = h / 2;

				canvas.Translate (cx, cy);

				if (compass.mValues != null)
					canvas.Rotate (-compass.mValues[0]);

				canvas.DrawPath (mPath, mPaint);
			}

			#region ISensorListener Members
			public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy)
			{
				// Do nothing
			}

			public void OnSensorChanged (SensorEvent e)
			{
				compass.mValues = e.Values.ToArray ();

				Invalidate ();
			}
			#endregion

		}
	}
}
