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
	// TODO: Disabled due to https://bugzilla.novell.com/show_bug.cgi?id=632427
	public class Sensors : Activity
	{
		private SensorManager mSensorManager;
		private GraphView mGraphView;

		public Sensors (IntPtr handle)
			: base (handle)
		{
		}

		private class GraphView : View, ISensorListener
		{
			private Bitmap mBitmap;
			private Paint mPaint = new Paint ();
			private Canvas mCanvas = new Canvas ();
			private Path mPath = new Path ();
			private RectF mRect = new RectF ();
			private float[] mLastValues = new float[3 * 2];
			private float[] mOrientationValues = new float[3];
			private int[] mColors = new int[3 * 2];
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
				mColors[0] = Color.Argb (192, 255, 64, 64);
				mColors[1] = Color.Argb (192, 64, 128, 64);
				mColors[2] = Color.Argb (192, 64, 64, 255);
				mColors[3] = Color.Argb (192, 64, 255, 255);
				mColors[4] = Color.Argb (192, 128, 64, 128);
				mColors[5] = Color.Argb (192, 255, 255, 64);

				mPaint.Flags = Paint.AntiAliasFlag;
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
						Paint paint = mPaint;
						Path path = mPath;
						int outer = Color.Argb (255, 192, 192, 192);
						int inner = Color.Argb (255, 255, 112, 16);

						if (mLastX >= mMaxX) {
							mLastX = 0;
							Canvas cavas = mCanvas;
							float yoffset = mYOffset;
							float maxx = mMaxX;
							float oneG = SensorManager.StandardGravity * mScale[0];
							paint.Color = Color.Argb (255, 170, 170, 170);
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
								canvas.Save (Canvas.MatrixSaveFlag);
								canvas.Translate (x, w * 0.5f + 4.0f);
								canvas.Save (Canvas.MatrixSaveFlag);
								paint.Color = outer;
								canvas.Scale (w, w);
								canvas.DrawOval (mRect, paint);
								canvas.Restore ();
								canvas.Scale (w - 5, w - 5);
								paint.Color = inner;
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
								canvas.Save (Canvas.MatrixSaveFlag);
								canvas.Translate (mWidth - (h * 0.5f + 4.0f), y);
								canvas.Save (Canvas.MatrixSaveFlag);
								paint.Color = outer;
								canvas.Scale (h, h);
								canvas.DrawOval (mRect, paint);
								canvas.Restore ();
								canvas.Scale (h - 5, h - 5);
								paint.Color = inner;
								canvas.Rotate (-values[i]);
								canvas.DrawPath (path, paint);
								canvas.Restore ();
								y += h0;
							}
						}

					}
				}
			}

			public void OnSensorChanged (int sensor, float[] values)
			{
				lock (this) {
					if (mBitmap != null) {
						Canvas canvas = mCanvas;
						Paint paint = mPaint;
						if (sensor == SensorManager.SensorAccelerometer) {
							for (int i = 0; i < 3; i++) {
								mOrientationValues[i] = values[i];
							}
						} else {
							float deltaX = mSpeed;
							float newX = mLastX + deltaX;

							int j = (sensor == SensorManager.SensorMagneticField) ? 1 : 0;
							for (int i = 0; i < 3; i++) {
								int k = i + j * 3;
								float v = mYOffset + values[i] * mScale[j];
								paint.Color = mColors[k];
								canvas.DrawLine (mLastX, mLastValues[k], newX, v, paint);
								mLastValues[k] = v;
							}
							if (sensor == SensorManager.SensorMagneticField)
								mLastX += mSpeed;
						}
						Invalidate ();
					}
				}
			}

			public void OnAccuracyChanged (int sensor, int accuracy)
			{
				// TODO Auto-generated method stub

			}
		}

		/**
		 * Initialization of the Activity after it is first created.  Must at least
		 * call {@link android.app.Activity#setContentView setContentView()} to
		 * describe what is to be displayed in the screen.
		 */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			// Be sure to call the super class.
			base.OnCreate (savedInstanceState);

			mSensorManager = (SensorManager)GetSystemService (SensorService);
			mGraphView = new GraphView (this.BaseContext);
			SetContentView (mGraphView);

			mSensorManager.RegisterListener (mGraphView,
				SensorManager.SensorAccelerometer |
				SensorManager.SensorMagneticField |
				SensorManager.SensorOrientation,
				SensorManager.SensorDelayFastest);

		}

		//protected override void OnResume ()
		//{
		//        base.OnResume ();

		//        mSensorManager.RegisterListener (mGraphView,
		//                SensorManager.SensorAccelerometer |
		//                SensorManager.SensorMagneticField |
		//                SensorManager.SensorOrientation,
		//                SensorManager.SensorDelayFastest);
		//}

		//protected override void OnStop() {
		//    mSensorManager.UnregisterListener(mGraphView);
		//    base.OnStop();
		//}
	}
}