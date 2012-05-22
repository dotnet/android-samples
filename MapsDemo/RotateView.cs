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
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Views;

namespace MonoDroid.Samples.MapsDemo
{
	class RotateView : ViewGroup, ISensorEventListener
	{
		const float SQ2 = 1.414213562373095f;
		const float ROTATE_TOLERANCE = 2.0f;

		private float heading = 0;
		private object lock_obj = new object ();

		public RotateView (Context context)
			: base (context)
		{
		}

		// When the sensor tells us we have moved, update
		public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
		{
		}

		public void OnSensorChanged (SensorEvent e)
		{
			lock (lock_obj) {
				var values = e.Values;

				if (Math.Abs (heading - values[0]) > ROTATE_TOLERANCE) {
					heading = values[0];
					Invalidate ();
				}
				((IDisposable) values).Dispose ();
			}
		}

		// Rotate the canvas before drawing
		protected override void DispatchDraw (Canvas canvas)
		{
			canvas.Save (SaveFlags.Matrix);
			canvas.Rotate (-heading, Width * 0.5f, Height * 0.5f);

			base.DispatchDraw (canvas);

			canvas.Restore ();
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			int width = Width;
			int height = Height;
			int count = ChildCount;

			for (int i = 0; i < count; i++) {
				View view = GetChildAt (i);

				int childWidth = view.MeasuredWidth;
				int childHeight = view.MeasuredHeight;
				int childLeft = (width - childWidth) / 2;
				int childTop = (height - childHeight) / 2;

				view.Layout (childLeft, childTop, childLeft + childWidth, childTop + childHeight);
			}
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int w = GetDefaultSize (SuggestedMinimumWidth, widthMeasureSpec);
			int h = GetDefaultSize (SuggestedMinimumHeight, heightMeasureSpec);
			int sizeSpec;

			if (w > h)
				sizeSpec = MeasureSpec.MakeMeasureSpec ((int)(w * SQ2), MeasureSpecMode.Exactly);
			else
				sizeSpec = MeasureSpec.MakeMeasureSpec ((int)(h * SQ2), MeasureSpecMode.Exactly);

			int count = ChildCount;

			for (int i = 0; i < count; i++)
				GetChildAt (i).Measure (sizeSpec, sizeSpec);

			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
		}

		public override bool DispatchTouchEvent (MotionEvent ev)
		{
			// TODO: rotate events too
			return base.DispatchTouchEvent (ev);
		}
	}
}
