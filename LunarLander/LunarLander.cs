/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *	  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using System;

/**
 * This is a simple LunarLander activity that houses a single LunarView. It
 * demonstrates...
 * <ul>
 * <li>animating by calling invalidate() from draw()
 * <li>loading and drawing resources
 * <li>handling onPause() in an animation
 * </ul>
 */
namespace Mono.Samples.LunarLander {
	public class LunarLander : Activity
	{
		const int MENU_EASY = 1;

		const int MENU_HARD = 2;

		const int MENU_MEDIUM = 3;

		const int MENU_PAUSE = 4;

		const int MENU_RESUME = 5;

		const int MENU_START = 6;

		const int MENU_STOP = 7;

		/** A handle to the thread that's actually running the animation. */
		private LunarView.LunarThread mLunarThread;

		/** A handle to the View in which the game is running. */
		private LunarView mLunarView;

		public LunarLander (IntPtr handle) : base (handle) {}

		/**
		 * Invoked during init to give the Activity a chance to set up its Menu.
		 * 
		 * @param menu the Menu to which entries may be added
		 * @return true
		 */
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			menu.Add(0, MENU_START, 0, R.@string.menu_start);
			menu.Add(0, MENU_STOP, 0, R.@string.menu_stop);
			menu.Add(0, MENU_PAUSE, 0, R.@string.menu_pause);
			menu.Add(0, MENU_RESUME, 0, R.@string.menu_resume);
			menu.Add(0, MENU_EASY, 0, R.@string.menu_easy);
			menu.Add(0, MENU_MEDIUM, 0, R.@string.menu_medium);
			menu.Add(0, MENU_HARD, 0, R.@string.menu_hard);

			return true;
		}

		/**
		 * Invoked when the user selects an item from the Menu.
		 * 
		 * @param item the Menu entry which was selected
		 * @return true if the Menu item was legit (and we consumed it), false
		 *		 otherwise
		 */
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case MENU_START:
				mLunarThread.DoStart ();
				return true;
			case MENU_STOP:
				mLunarThread.SetState (LunarView.LunarThread.STATE_LOSE,
					GetText (R.@string.message_stopped));
				return true;
			case MENU_PAUSE:
				mLunarThread.Pause ();
				return true;
			case MENU_RESUME:
				mLunarThread.Unpause ();
				return true;
			case MENU_EASY:
				mLunarThread.SetDifficulty (LunarView.LunarThread.DIFFICULTY_EASY);
				return true;
			case MENU_MEDIUM:
				mLunarThread.SetDifficulty (LunarView.LunarThread.DIFFICULTY_MEDIUM);
				return true;
			case MENU_HARD:
				mLunarThread.SetDifficulty (LunarView.LunarThread.DIFFICULTY_HARD);
				return true;
			}

			return false;
		}

		/**
		 * Invoked when the Activity is created.
		 * 
		 * @param savedInstanceState a Bundle containing state saved from a previous
		 *		execution, or null if this is a new execution
		 */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// turn off the window's title bar
			RequestWindowFeature (Window.FEATURE_NO_TITLE);

			// tell system to use the layout defined in our XML file
			SetContentView (R.layout.lunar_layout);

			// get handles to the LunarView from XML, and its LunarThread
			mLunarView = (LunarView) FindViewById (R.id.lunar);
			mLunarThread = mLunarView.Thread;

			// give the LunarView a handle to the TextView used for messages
			mLunarView.SetTextView((TextView) FindViewById (R.id.text));

			if (savedInstanceState == null) {
				// we were just launched: set up a new game
				mLunarThread.SetState (LunarView.LunarThread.STATE_READY);
				Log.W (this.GetType ().ToString (), "SIS is null");
			} else {
				// we are being restored: resume a previous game
				mLunarThread.RestoreState (savedInstanceState);
				Log.W (this.GetType ().ToString (), "SIS is nonnull");
			}
		}

		/**
		 * Invoked when the Activity loses user focus.
		 */
		protected override void OnPause()
		{
			base.OnPause ();
			mLunarView.Thread.Pause (); // pause game when Activity pauses
		}

		/**
		 * Notification that something is about to happen, to give the Activity a
		 * chance to save state.
		 * 
		 * @param outState a Bundle into which this Activity should save its state
		 */
		protected override void OnSaveInstanceState(Bundle outState)
		{
			// just have the View's thread save its state into our Bundle
			base.OnSaveInstanceState (outState);
			mLunarThread.SaveState (outState);
			Log.W (this.GetType ().ToString (), "SIS called");
		}
	}
}
