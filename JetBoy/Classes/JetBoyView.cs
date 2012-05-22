/*
 * Copyright (C) 2009 The Android Open Source Project
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
 * 
 */

using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace JetBoy
{
	public class JetBoyView : SurfaceView, ISurfaceHolderCallback
	{
		static String TAG = "JetBoy";

		private Scoreboard scores;

		// Handle to the thread that's actually running the animation.
		private JetBoyThread thread;

		// GUI Widgets
		private TextView timer_view;
		private Button retry_button;
		private TextView text_view;

		public JetBoyView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			scores = new Scoreboard ();

			// make sure we get key events
			Focusable = true; 
		}

		public void Initialize (TextView timerView, Button retryButton, TextView textView)
		{
			timer_view = timerView;
			retry_button = retryButton;
			text_view = textView;

			// register our interest in hearing about changes to our surface
			ISurfaceHolder holder = Holder;
			holder.AddCallback (this);
			
			// create thread only; it's started in SurfaceCreated()
			// except if used in the layout editor.
			if (IsInEditMode == false)
				thread = new JetBoyThread (holder, Context, scores, new MessageHandler (timer_view, retry_button, text_view, scores));
		}

		// If we lose focus, pause the game thread
		public override void OnWindowFocusChanged (bool hasWindowFocus)
		{
			if (!hasWindowFocus) {
				if (thread != null)
					thread.Pause ();

			}
		}

		// Return the animation thread
		public JetBoyThread GetThread ()
		{
			return thread;
		}

		#region ISurfaceHolderCallback Members
		public void SurfaceChanged (ISurfaceHolder holder, Format format, int width, int height)
		{
			thread.SetSurfaceSize (width, height);
		}

	    public void SurfaceCreated (ISurfaceHolder holder)
		{
			// Start the thread here so that we don't busy-wait in run()
			// waiting for the surface to be created
			thread.SetRunning (true);
			thread.Start ();
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			bool retry = true;
			thread.SetRunning (false);

			while (retry) {
				try {
					thread.Join ();
					retry = false;
				} catch (Exception) { }
			}
		}
		#endregion
	}
}
