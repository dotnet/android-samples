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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace JetBoy
{
	[Activity (Label = "Jet Boy", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape, Icon = "@drawable/icon")]
	public class JetBoyActivity : Activity
	{
		// Handle to the thread that's actually running the animation.
		private JetBoyThread jetboy_thread;

		// GUI Widgets
		private Button start_button;
		private Button retry_button;
		private TextView text_view;
		private TextView timer_view;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Remove the title bar
			RequestWindowFeature (WindowFeatures.NoTitle);

			SetContentView (Resource.Layout.main);

			// Look up the happy shiny buttons
			start_button = FindViewById<Button> (Resource.Id.Button01);
			start_button.Click += new EventHandler (StartButton_Click);

			retry_button = FindViewById<Button> (Resource.Id.Button02);
			retry_button.Click += new EventHandler (RetryButton_Click);

			// Get handles for instruction text and game timer text
			text_view = FindViewById<TextView> (Resource.Id.text);
			timer_view = FindViewById<TextView> (Resource.Id.timer);

			// Get handles to the JetView from XML and the JET thread.
			JetBoyView jetboy_view = FindViewById<JetBoyView> (Resource.Id.JetBoyView);
			jetboy_view.Click += new EventHandler (jetboy_view_Click);

			jetboy_view.Initialize (timer_view, retry_button, text_view);
			jetboy_thread = jetboy_view.GetThread ();
		}

		private void RetryButton_Click (object sender, EventArgs e)
		{
			text_view.SetText (Resource.String.helpText);

			start_button.Text = "PLAY!";

			text_view.Visibility = ViewStates.Visible;
			start_button.Visibility = ViewStates.Visible;
			retry_button.Visibility = ViewStates.Invisible;

			jetboy_thread.GameState = GameState.Play;
		}

		private void StartButton_Click (object sender, EventArgs e)
		{
			switch (jetboy_thread.GameState) {
				case GameState.Start:
					// User hit the "START" button
					start_button.Text = "PLAY!";
					text_view.Visibility = ViewStates.Visible;
					text_view.SetText (Resource.String.helpText);

					jetboy_thread.GameState = GameState.Play;

					break;
				case GameState.Play:
					// User hit the "PLAY" button
					start_button.Visibility = ViewStates.Invisible;
					text_view.Visibility = ViewStates.Invisible;
					timer_view.Visibility = ViewStates.Visible;

					jetboy_thread.GameState = GameState.Running;

					break;
			}
		}

		// Forward key presses (other than back) to our game thread
		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
				return base.OnKeyDown (keyCode, e);
			else
				return jetboy_thread.DoKeyDown (keyCode, e);
		}

		public override bool OnKeyUp (Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
				return base.OnKeyUp (keyCode, e);
			else
				return jetboy_thread.DoKeyUp (keyCode, e);
		}

		private void jetboy_view_Click (object sender, EventArgs e)
		{
			jetboy_thread.DoClick ();
		}
	}
}