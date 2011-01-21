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
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace JetBoy
{
	public class MessageHandler : Handler
	{
		const string TAG = "JetBoy";

		private TextView timer_view;
		private Button retry_button;
		private TextView text_view;
		private Scoreboard scores;

		public MessageHandler (TextView timerView, Button retryButton, TextView textView, Scoreboard scores)
		{
			timer_view = timerView;
			retry_button = retryButton;
			text_view = textView;
			this.scores = scores;
		}

		public override void HandleMessage (Message msg)
		{
			// Ensure we have a valid message
			if (msg == null) {
				Log.Debug (TAG, "null msg");
				return;
			}

			if (msg.Data == null) {
				Log.Debug (TAG, "null msg.Data");
				return;
			}

			// Update the timer
			Log.Debug ("jpobst", msg.Data.GetString ("text"));
			timer_view.Text = msg.Data.GetString ("text");

			// If the user lost, set up the lost screen
			if (msg.Data.GetString ("STATE_LOSE") != null) {

				retry_button.Visibility = ViewStates.Visible;
				timer_view.Visibility = ViewStates.Invisible;
				text_view.Visibility = ViewStates.Visible;

				if (scores.HitTotal >= scores.SuccessThreshold) {
					text_view.SetText (Resource.String.winText);
				} else {
					string lost = string.Format ("Sorry, You Lose! You got {0}. You need {1} to win.", scores.HitTotal, scores.SuccessThreshold);
					text_view.Text = lost;
				}

				timer_view.Text = "1:12";
				text_view.SetHeight (20);
			}
		}
	}
}
