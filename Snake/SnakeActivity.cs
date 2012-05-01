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
using Android.App;
using Android.OS;
using Android.Widget;

namespace Mono.Samples.Snake
{
	/**
	 * Snake: a simple game that everyone can enjoy.
	 * 
	 * This is an implementation of the classic Game "Snake", in which you control a
	 * serpent roaming around the garden looking for apples. Be careful, though,
	 * because when you catch one, not only will you become longer, but you'll move
	 * faster. Running into yourself or the walls will end the game.
	 * 
	 */
	[Activity (Label = "Snake", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class SnakeActivity : Activity
	{
		private SnakeView snake_view;

		private static String ICICLE_KEY = "snake-view";

		// Called when Activity is first created. Turns off the title bar, sets up
		// the content views, and fires up the SnakeView.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.snake_layout);

			snake_view = FindViewById<SnakeView> (Resource.Id.snake);
			snake_view.SetTextView (FindViewById<TextView> (Resource.Id.text));

			if (savedInstanceState == null) {
				// We were just launched -- set up a new game
				snake_view.SetMode (GameMode.Ready);
			} else {
				// We are being restored
				Bundle map = savedInstanceState.GetBundle (ICICLE_KEY);

				if (map != null)
					snake_view.RestoreState (map);
				else
					snake_view.SetMode (GameMode.Paused);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Pause the game along with the activity
			snake_view.SetMode (GameMode.Paused);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			//Store the game state
			outState.PutBundle (ICICLE_KEY, snake_view.SaveState ());
		}
	}
}
