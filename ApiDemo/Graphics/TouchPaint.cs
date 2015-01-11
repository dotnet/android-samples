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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace MonoDroid.ApiDemo
{
	/**
	* Demonstrates the handling of touch screen, stylus, mouse and trackball events to
	* implement a simple painting app.
	* 
	* Drawing with a touch screen is accomplished by drawing a point at the
	* location of the touch.  When pressure information is available, it is used
	* to change the intensity of the color.  When size and orientation information
	* is available, it is used to directly adjust the size and orientation of the
	* brush.
	* 
	* Drawing with a stylus is similar to drawing with a touch screen, with a
	* few added refinements.  First, there may be multiple tools available including
	* an eraser tool.  Second, the tilt angle and orientation of the stylus can be
	* used to control the direction of paint.  Third, the stylus buttons can be used
	* to perform various actions.  Here we use one button to cycle colors and the
	* other to airbrush from a distance.
	*
	* Drawing with a mouse is similar to drawing with a touch screen, but as with
	* a stylus we have extra buttons.  Here we use the primary button to draw,
	* the secondary button to cycle colors and the tertiary button to airbrush.
	*
	* Drawing with a trackball is a simple matter of using the relative motions
	* of the trackball to move the paint brush around.  The trackball may also
	* have a button, which we use to cycle through colors.
	*
	*/
	[Activity (Label = "Graphics/Touch Paint", ConfigurationChanges = ConfigChanges.Keyboard
		| ConfigChanges.KeyboardHidden | ConfigChanges.Navigation | ConfigChanges.Orientation
		| ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize,
		Theme = "@style/Theme.Black")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class TouchPaint : GraphicsActivity
	{
		/** Used as a pulse to gradually fade the contents of the window. */
		const int MSG_FADE = 1;

		/*		* Menu ID for the command to clear the window. */
		const int CLEAR_ID = Menu.First;

		/*		* Menu ID for the command to toggle fading. */
		const int FADE_ID = Menu.First + 1;

		/*		* How often to fade the contents of the window (in ms). */
		static readonly int FADE_DELAY = 100;

		/*		* Colors to cycle through. */
		static readonly int[] COLORS = new int[] {
			Color.White, Color.Red, Color.Yellow, Color.Green,
			Color.Cyan, Color.Blue, Color.Magenta,
		};

		/*		* Background color. */
		static readonly int BACKGROUND_COLOR = Color.Black;

		/*		* The view responsible for drawing the window. */
		PaintView mView;

		/*		* Is fading mode enabled? */
		bool mFading;

		MyHandler mHandler;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create and attach the view that is responsible for painting.
			mView = new PaintView (this);
			SetContentView (mView);
			mView.RequestFocus ();

			// Restore the fading option if we are being thawed from a
			// previously saved state.  Note that we are not currently remembering
			// the contents of the bitmap.
			if (bundle != null) {
				mFading = bundle.GetBoolean ("fading", true);
				mView.mColorIndex = bundle.GetInt ("color", 0);
			} else {
				mFading = true;
				mView.mColorIndex = 0;
			}

			mHandler = new MyHandler (this);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (0, CLEAR_ID, 0, "Clear");
			menu.Add (0, FADE_ID, 0, "Fade").SetCheckable (true);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			menu.FindItem (FADE_ID).SetChecked (mFading);
			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case CLEAR_ID:
				mView.Clear ();
				return true;
			case FADE_ID:
				mFading = !mFading;
				if (mFading) {
					StartFading ();
				} else {
					StopFading ();
				}
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// If fading mode is enabled, then as long as we are resumed we want
			// to run pulse to fade the contents.
			if (mFading) {
				StartFading ();
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			// Save away the fading state to restore if needed later.  Note that
			// we do not currently save the contents of the display.
			outState.PutBoolean ("fading", mFading);
			outState.PutInt ("color", mView.mColorIndex);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Make sure to never run the fading pulse while we are paused or
			// stopped.
			StopFading ();
		}

		/**
     	* Start up the pulse to fade the screen, clearing any existing pulse to
     	* ensure that we don't have multiple pulses running at a time.
     	*/
		void StartFading ()
		{
			mHandler.RemoveMessages (MSG_FADE);
			ScheduleFade ();
		}

		/**
     	* Stop the pulse to fade the screen.
     	*/
		void StopFading ()
		{
			mHandler.RemoveMessages (MSG_FADE);
		}

		/**
     	* Schedule a fade message for later.
     	*/
		void ScheduleFade ()
		{
			mHandler.SendMessageDelayed (mHandler.ObtainMessage (MSG_FADE), FADE_DELAY);
		}

		class MyHandler : Handler
		{
			TouchPaint self;

			public MyHandler (TouchPaint self)
			{
				this.self = self;
			}

			public override void HandleMessage (Message msg)
			{
				switch (msg.What) {
				// Upon receiving the fade pulse, we have the view perform a
				// fade and then enqueue a new message to pulse at the desired
				// next time.
				case MSG_FADE:
					self.mView.Fade ();
					self.ScheduleFade ();
					break;
				default:
					base.HandleMessage (msg);
					break;
				}
			}
		}

		enum PaintMode {
			Draw,
			Splat,
			Erase,
		}

		/**
     	* This view implements the drawing canvas.
     	*
     	* It handles all of the input events and drawing functions.
     	*/
		public class PaintView : View
		{
			static readonly int FADE_ALPHA = 0x06;
			static readonly int MAX_FADE_STEPS = 256 / (FADE_ALPHA / 2) + 4;
			static readonly int TRACKBALL_SCALE = 10;
			static readonly int SPLAT_VECTORS = 40;

			readonly Java.Util.Random mRandom = new Java.Util.Random ();
			Bitmap mBitmap;
			Canvas mCanvas;
			readonly Paint mPaint = new Paint ();
			readonly Paint mFadePaint = new Paint ();
			float mCurX;
			float mCurY;
			MotionEventButtonState mOldButtonState;
			int mFadeSteps = MAX_FADE_STEPS;

			/** The index of the current color to use. */
			public int mColorIndex;

			public PaintView (Context c) : base (c)
			{
				Init ();
			}

			public PaintView (Context c, IAttributeSet attrs) : base (c, attrs)
			{
				Init ();
			}

			void Init ()
			{
				Focusable = true;
				mPaint.AntiAlias = true;

				mFadePaint.Color = new Color (BACKGROUND_COLOR);
				mFadePaint.Alpha = FADE_ALPHA;
			}

			public void Clear ()
			{
				if (mCanvas != null) {
					mPaint.Color = new Color (BACKGROUND_COLOR);
					mCanvas.DrawPaint (mPaint);
					Invalidate ();

					mFadeSteps = MAX_FADE_STEPS;
				}
			}

			public void Fade ()
			{
				if (mCanvas != null && mFadeSteps < MAX_FADE_STEPS) {
					mCanvas.DrawPaint (mFadePaint);
					Invalidate ();

					mFadeSteps++;
				}
			}

			public void Text (string text)
			{
				if (mBitmap != null) {
					int width = mBitmap.Width;
					int height = mBitmap.Height;
					mPaint.Color = new Color (COLORS[mColorIndex]);
					mPaint.Alpha = 255;
					int size = height;
					mPaint.TextSize = size;
					var bounds = new Rect ();
					mPaint.GetTextBounds (text, 0, text.Length, bounds);
					int twidth = bounds.Width ();
					twidth += (twidth / 4);
					if (twidth > width) {
						size = (size*width)/twidth;
						mPaint.TextSize = size;
						mPaint.GetTextBounds (text, 0, text.Length, bounds);
					}
					Paint.FontMetrics fm = mPaint.GetFontMetrics ();
					mCanvas.DrawText (text, (width - bounds.Width ()) / 2,
						((height - size) / 2) - fm.Ascent, mPaint);
					mFadeSteps = 0;
					Invalidate ();
				}
			}

			protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
			{
				int curW = mBitmap != null ? mBitmap.Width : 0;
				int curH = mBitmap != null ? mBitmap.Height : 0;
				if (curW >= w && curH >= h) {
					return;
				}

				if (curW < w) curW = w;
				if (curH < h) curH = h;

				var newBitmap = Bitmap.CreateBitmap (curW, curH, Bitmap.Config.Argb8888);
				var newCanvas = new Canvas ();
				newCanvas.SetBitmap (newBitmap);
				if (mBitmap != null) {
					newCanvas.DrawBitmap (mBitmap, 0, 0, null);
				}
				mBitmap = newBitmap;
				mCanvas = newCanvas;
				mFadeSteps = MAX_FADE_STEPS;
			}

			protected override void OnDraw (Canvas canvas)
			{
				if (mBitmap != null) {
					canvas.DrawBitmap (mBitmap, 0, 0, null);
				}			
			}

			public override bool OnTrackballEvent (MotionEvent e)
			{
				MotionEventActions action = e.ActionMasked;
				if (action == MotionEventActions.Down) {
					// Advance color when the trackball button is pressed.
					AdvanceColor ();
				}

				if (action == MotionEventActions.Down || action == MotionEventActions.Move) {
					int N = e.HistorySize;
					float scaleX = e.XPrecision * TRACKBALL_SCALE;
					float scaleY = e.YPrecision * TRACKBALL_SCALE;
					for (int i = 0; i < N; i++) {
						MoveTrackball (e.GetHistoricalX (i) * scaleX,
							e.GetHistoricalY (i) * scaleY);
					}
					MoveTrackball (e.GetX () * scaleX, e.GetY () * scaleY);
				}
				return true;
			}

			void MoveTrackball (float deltaX, float deltaY)
			{
				int curW = mBitmap != null ? mBitmap.Width : 0;
				int curH = mBitmap != null ? mBitmap.Height : 0;

				mCurX = Math.Max (Math.Min (mCurX + deltaX, curW - 1), 0);
				mCurY = Math.Max (Math.Min (mCurY + deltaY, curH - 1), 0);
				Paint (PaintMode.Draw, mCurX, mCurY);
			}

			public override bool OnTouchEvent (MotionEvent e)
			{
				return OnTouchOrHoverEvent (e, true /*isTouch*/);		
			}

			public override bool OnHoverEvent (MotionEvent e)
			{
				return OnTouchOrHoverEvent (e, false /*isTouch*/);
			}

			bool OnTouchOrHoverEvent (MotionEvent e, bool isTouch)
			{
				MotionEventButtonState buttonState = e.ButtonState;
				MotionEventButtonState pressedButtons = buttonState & ~mOldButtonState;
				mOldButtonState = buttonState;

				if ((pressedButtons & MotionEventButtonState.Secondary) != 0) {
					// Advance color when the right mouse button or first stylus button
					// is pressed.
					AdvanceColor ();
				}

				PaintMode mode;
				if ((buttonState & MotionEventButtonState.Tertiary) != 0) {
					// Splat paint when the middle mouse button or second stylus button is pressed.
					mode = PaintMode.Splat;
				} else if (isTouch || (buttonState & MotionEventButtonState.Primary) != 0) {
					// Draw paint when touching or if the primary button is pressed.
					mode = PaintMode.Draw;
				} else {
					// Otherwise, do not paint anything.
					return false;
				}

				MotionEventActions action = e.ActionMasked;
				if (action ==  MotionEventActions.Down || action == MotionEventActions.Move
					|| action == MotionEventActions.HoverMove) {
					int N = e.HistorySize;
					int P = e.PointerCount;
					for (int i = 0; i < N; i++) {
						for (int j = 0; j < P; j++) {
							Paint (GetPaintModeForTool (e.GetToolType (j), mode),
								e.GetHistoricalX (j, i),
								e.GetHistoricalY(j, i),
								e.GetHistoricalPressure (j, i),
								e.GetHistoricalTouchMajor (j, i),
								e.GetHistoricalTouchMinor (j, i),
								e.GetHistoricalOrientation (j, i),
								e.GetHistoricalAxisValue (Axis.Distance, j, i),
								e.GetHistoricalAxisValue (Axis.Tilt, j, i));
						}
					}
					for (int j = 0; j < P; j++) {
						Paint (GetPaintModeForTool (e.GetToolType (j), mode),
							e.GetX (j),
							e.GetY (j),
							e.GetPressure (j),
							e.GetTouchMajor (j),
							e.GetTouchMinor (j),
							e.GetOrientation (j),
							e.GetAxisValue (Axis.Distance, j),
							e.GetAxisValue (Axis.Tilt, j));
					}
					mCurX = e.GetX ();
					mCurY = e.GetY ();
				}
				return true;
			}

			PaintMode GetPaintModeForTool (MotionEventToolType toolType, PaintMode defaultMode)
			{
				if (toolType == MotionEventToolType.Eraser) {
					return PaintMode.Erase;
				}
				return defaultMode;
			}

			void AdvanceColor ()
			{
				mColorIndex = (mColorIndex + 1) % COLORS.Length;
			}

			void Paint (PaintMode mode, float x, float y)
			{
				Paint (mode, x, y, 1.0f, 0, 0, 0, 0, 0);
			}

			void Paint (PaintMode mode, float x, float y, float pressure,
				float major, float minor, float orientation,
				float distance, float tilt)
			{
				if (mBitmap != null) {
					if (major <= 0 || minor <= 0) {
						// If size is not available, use a default value.
						major = minor = 16;
					}

					switch (mode) {
					case PaintMode.Draw:
						mPaint.Color = new Color (COLORS[mColorIndex]);
						mPaint.Alpha = Math.Min ((int)(pressure * 128), 255);
						DrawOval (mCanvas, x, y, major, minor, orientation, mPaint);
						break;

					case PaintMode.Erase:
						mPaint.Color = new Color (BACKGROUND_COLOR);
						mPaint.Alpha = Math.Min ((int)(pressure * 128), 255);
						DrawOval (mCanvas, x, y, major, minor, orientation, mPaint);
						break;

					case PaintMode.Splat:
						mPaint.Color = new Color (COLORS[mColorIndex]);
						mPaint.Alpha = 64;
						DrawSplat (mCanvas, x, y, orientation, distance, tilt, mPaint);
						break;
					}
				}
				mFadeSteps = 0;
				Invalidate ();
			}

			/**
         	* Draw an oval.
         	*
         	* When the orienation is 0 radians, orients the major axis vertically,
         	* angles less than or greater than 0 radians rotate the major axis left or right.
         	*/
			RectF mReusableOvalRect = new RectF ();
			void DrawOval (Canvas canvas, float x, float y, float major, float minor,
				float orientation, Paint paint)
			{
				canvas.Save (SaveFlags.Matrix);
				canvas.Rotate ((float) (orientation * 180 / Math.PI), x, y);
				mReusableOvalRect.Left = x - minor / 2;
				mReusableOvalRect.Right = x + minor / 2;
				mReusableOvalRect.Top = y - major / 2;
				mReusableOvalRect.Bottom = y + major / 2;
				canvas.DrawOval (mReusableOvalRect, paint);
				canvas.Restore ();
			}

			/**
         	* Splatter paint in an area.
         	*
         	* Chooses random vectors describing the flow of paint from a round nozzle
         	* across a range of a few degrees.  Then adds this vector to the direction
         	* indicated by the orientation and tilt of the tool and throws paint at
         	* the canvas along that vector.
        	*
         	* Repeats the process until a masterpiece is born.
         	*/
			private void DrawSplat (Canvas canvas, float x, float y, float orientation,
				float distance, float tilt, Paint paint)
			{
				float z = distance * 2 + 10;

				// Calculate the center of the spray.
				float nx = (float)(Math.Sin (orientation) * Math.Sin (tilt));
				float ny = (float)(- Math.Cos (orientation) * Math.Sin (tilt));
				float nz = (float)Math.Cos (tilt);
				if (nz < 0.05) {
					return;
				}
				float cd = z / nz;
				float cx = nx * cd;
				float cy = ny * cd;

				for (int i = 0; i < SPLAT_VECTORS; i++) {
					// Make a random 2D vector that describes the direction of a speck of paint
					// ejected by the nozzle in the nozzle's plane, assuming the tool is
					// perpendicular to the surface.
					double direction = mRandom.NextDouble () * Math.PI * 2;
					double dispersion = mRandom.NextGaussian () * 0.2;
					double vx = Math.Cos (direction) * dispersion;
					double vy = Math.Sin (direction) * dispersion;
					double vz = 1;

					// Apply the nozzle tilt angle.
					double temp = vy;
					vy = temp * Math.Cos (tilt) - vz * Math.Sin (tilt);
					vz = temp * Math.Sin (tilt) + vz * Math.Cos (tilt);

					// Apply the nozzle orientation angle.
					temp = vx;
					vx = temp * Math.Cos (orientation) - vy * Math.Sin (orientation);
					vy = temp * Math.Sin (orientation) + vy * Math.Cos (orientation);

					// Determine where the paint will hit the surface.
					if (vz < 0.05) {
						continue;
					}
					float pd = (float) (z / vz);
					float px = (float) (vx * pd);
					float py = (float) (vy * pd);

					// Throw some paint at this location, relative to the center of the spray.
					mCanvas.DrawCircle (x + px - cx, y + py - cy, 1.0f, paint);
				}
			}
		}
		
	}
}

