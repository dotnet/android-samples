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

using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.Snake
{
	// SnakeView: implementation of a simple game of Snake
	public class SnakeView : TileView
	{
		private static String TAG = "SnakeView";
		private static Random RNG = new Random ();

		// Current game mode (running, paused, etc)
		private GameMode mode = GameMode.Ready;

		// Current direction the snake is headed.
		private Direction mDirection = Direction.North;
		private Direction mNextDirection = Direction.North;

		// The number of apples captured
		private long mScore = 0;

		// Number of milliseconds between snake movements. This will 
		// decrease as apples are captured.
		private int mMoveDelay = 600;

		// Tracks the absolute time when the snake last moved, and is used
		// to determine if a move should be made based on mMoveDelay.
		private long mLastMove;

		// Text shows to the user in some run states
		private TextView mStatusText;

		// List of Coordinates that make up the snake's body
		private List<Coordinate> snake_trail = new List<Coordinate> ();

		// Secret locations of the juicy apples the snake craves.
		private List<Coordinate> apples = new List<Coordinate> ();

		// a simple handler that we can use to cause animation to happen.
		private RefreshHandler mRedrawHandler;

		#region Constructors
		public SnakeView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			InitSnakeView ();
			mRedrawHandler = new RefreshHandler (this);
		}

		public SnakeView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			InitSnakeView ();
			mRedrawHandler = new RefreshHandler (this);
		}

		private void InitSnakeView ()
		{
			Focusable = true;
			FocusableInTouchMode = true;

			// Initialize and load our tile bitmaps
			ResetTiles (4);

			LoadTile (TileType.Red, Resources.GetDrawable (Resource.Drawable.redstar));
			LoadTile (TileType.Yellow, Resources.GetDrawable (Resource.Drawable.yellowstar));
			LoadTile (TileType.Green, Resources.GetDrawable (Resource.Drawable.greenstar));

			Click += new EventHandler (SnakeView_Click);
		}
		#endregion

		#region Game Initialization
		private void InitNewGame ()
		{
			snake_trail.Clear ();
			apples.Clear ();

			// For now we're just going to load up a short default
			// eastbound snake that's just turned north
			snake_trail.Add (new Coordinate (7, 7));
			snake_trail.Add (new Coordinate (6, 7));
			snake_trail.Add (new Coordinate (5, 7));
			snake_trail.Add (new Coordinate (4, 7));
			snake_trail.Add (new Coordinate (3, 7));
			snake_trail.Add (new Coordinate (2, 7));

			mNextDirection = Direction.North;

			// Two apples to start with
			AddRandomApple ();
			AddRandomApple ();

			mMoveDelay = 600;
			mScore = 0;
		}

		// Selects a random location within the garden that is not currently covered
		// by the snake. Currently _could_ go into an infinite loop if the snake
		// currently fills the garden, but we'll leave discovery of this prize to a
		// truly excellent snake-player.
		private void AddRandomApple ()
		{
			Coordinate newCoord = null;
			bool found = false;

			while (!found) {
				// Choose a new location for our apple
				int newX = 1 + RNG.Next (x_tile_count - 2);
				int newY = 1 + RNG.Next (y_tile_count - 2);

				newCoord = new Coordinate (newX, newY);

				// Make sure it's not already under the snake
				bool collision = false;

				int snakelength = snake_trail.Count;
				for (int index = 0; index < snakelength; index++) {
					if (snake_trail[index] == newCoord) {
						collision = true;
					}
				}

				// if we're here and there's been no collision, then we have
				// a good location for an apple. Otherwise, we'll circle back
				// and try again
				found = !collision;
			}

			if (newCoord == null)
				Log.Error (TAG, "Somehow ended up with a null newCoord!");

			apples.Add (newCoord);
		}

		// Sets the TextView that will be used to give information
		// (such as "Game Over" to the user.
		public void SetTextView (TextView newView)
		{
			mStatusText = newView;
		}
		#endregion

		#region Game Logic
		// Handles key events in the game. Update the direction our snake is traveling
		// based on the DPAD. Ignore events that would cause the snake to immediately
		// turn back on itself.
		public override bool OnKeyDown (Keycode keyCode, KeyEvent msg)
		{
			if (keyCode == Keycode.DpadUp || keyCode == Keycode.VolumeUp) {
				if (mode == GameMode.Ready | mode == GameMode.Lost) {
					// At the beginning of the game, or the end of a
					// previous one, we should start a new game.
					InitNewGame ();

					SetMode (GameMode.Running);
					Update ();

					return true;
				}

				if (mode == GameMode.Paused) {
					// If the game is merely paused, we should
					// just continue where we left off.
					SetMode (GameMode.Running);
					Update ();

					return true;
				}

				if (keyCode == Keycode.VolumeUp) {
					mNextDirection = (Direction) (((int)mDirection + 1) % 4);
				} else {
					if (mDirection != Direction.South)
						mNextDirection = Direction.North;
				}

				return true;
			}

			if (keyCode == Keycode.DpadDown) {
				if (mDirection != Direction.North)
					mNextDirection = Direction.South;

				return true;
			}

			if (keyCode == Keycode.VolumeDown) {
				mNextDirection = (Direction) (((int)mDirection - 1) % 4);

				return true;
			}

			if (keyCode == Keycode.DpadLeft) {
				if (mDirection != Direction.East)
					mNextDirection = Direction.West;

				return true;
			}

			if (keyCode == Keycode.DpadRight) {
				if (mDirection != Direction.West)
					mNextDirection = Direction.East;

				return (true);
			}

			return base.OnKeyDown (keyCode, msg);
		}

		private void SnakeView_Click (object sender, EventArgs e)
		{
			// Let you start the game without having a d-pad.  You
			// still won't be able to play the game, but you can
			// see it in action.
			if (mode == GameMode.Ready | mode == GameMode.Lost) {
				// At the beginning of the game, or the end of a
				// previous one, we should start a new game.
				InitNewGame ();

				SetMode (GameMode.Running);
				Update ();
			}
		}

		// Updates the current mode of the application (RUNNING or PAUSED or the like)
		// as well as sets the visibility of textview for notification
		public void SetMode (GameMode newMode)
		{
			GameMode oldMode = mode;
			mode = newMode;

			if (newMode == GameMode.Running & oldMode != GameMode.Running) {
				mStatusText.Visibility = ViewStates.Invisible;
				Update ();

				return;
			}

			var str = "";

			if (newMode == GameMode.Paused)
				str = Resources.GetText (Resource.String.mode_pause);
			else if (newMode == GameMode.Ready)
				str = Resources.GetText (Resource.String.mode_ready);
			else if (newMode == GameMode.Lost) {
				var lose_prefix = Resources.GetString (Resource.String.mode_lose_prefix);
				var lose_suffix = Resources.GetString (Resource.String.mode_lose_suffix);
				str = string.Format ("{0}{1}{2}", lose_prefix, mScore, lose_suffix);
			}

			mStatusText.Text = str;
			mStatusText.Visibility = ViewStates.Visible;
		}

		// Handles the basic update loop, checking to see if we are in the running
		// state, determining if a move should be made, updating the snake's location.
		public void Update ()
		{
			if (mode == GameMode.Running) {
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

				if (now - mLastMove > mMoveDelay) {
					ClearTiles ();
					UpdateWalls ();
					UpdateSnake ();
					UpdateApples ();
					mLastMove = now;
				}

				mRedrawHandler.Sleep (mMoveDelay);
			}
		}

		// Draws some walls.
		private void UpdateWalls ()
		{
			for (int x = 0; x < x_tile_count; x++) {
				SetTile (TileType.Green, x, 0);
				SetTile (TileType.Green, x, y_tile_count - 1);
			}

			for (int y = 1; y < y_tile_count - 1; y++) {
				SetTile (TileType.Green, 0, y);
				SetTile (TileType.Green, x_tile_count - 1, y);
			}
		}

		// Draws some apples.
		private void UpdateApples ()
		{
			foreach (Coordinate c in apples)
				SetTile (TileType.Yellow, c.X, c.Y);
		}

		// Figure out which way the snake is going, see if he's run into anything (the
		// walls, himself, or an apple). If he's not going to die, we then add to the
		// front and subtract from the rear in order to simulate motion. If we want to
		// grow him, we don't subtract from the rear.
		private void UpdateSnake ()
		{
			bool growSnake = false;

			// grab the snake by the head
			Coordinate head = snake_trail[0];
			Coordinate newHead = new Coordinate (1, 1);

			mDirection = mNextDirection;

			switch (mDirection) {
				case Direction.East:
					newHead = new Coordinate (head.X + 1, head.Y);
					break;
				case Direction.West:
					newHead = new Coordinate (head.X - 1, head.Y);
					break;
				case Direction.North:
					newHead = new Coordinate (head.X, head.Y - 1);
					break;
				case Direction.South:
					newHead = new Coordinate (head.X, head.Y + 1);
					break;
			}

			// Collision detection
			// For now we have a 1-square wall around the entire arena
			if ((newHead.X < 1) || (newHead.Y < 1) || (newHead.X > x_tile_count - 2)
				|| (newHead.Y > y_tile_count - 2)) {
				SetMode (GameMode.Lost);

				return;
			}

			// Look for collisions with itself
			foreach (Coordinate snake in snake_trail) {
				if (snake.Equals (newHead)) {
					SetMode (GameMode.Lost);
					return;
				}
			}

			// Look for apples
			foreach (Coordinate apple in apples) {
				if (apple.Equals (newHead)) {
					apples.Remove (apple);
					AddRandomApple ();

					mScore++;
					Log.Info ("tag", mMoveDelay.ToString ());
					mMoveDelay = (int)(mMoveDelay * 0.9);
					Log.Info ("tag", mMoveDelay.ToString ());

					growSnake = true;

					break;
				}
			}

			// Push a new head onto the List
			snake_trail.Insert (0, newHead);

			// Unless we want the snake to grow, remove the last tail piece
			if (!growSnake)
				snake_trail.RemoveAt (snake_trail.Count - 1);

			int index = 0;

			// Update tiles to match new snake
			foreach (Coordinate c in snake_trail) {
				if (index == 0)
					SetTile (TileType.Green, c.X, c.Y);
				else
					SetTile (TileType.Red, c.X, c.Y);

				index++;
			}
		}
		#endregion

		#region Save/Load State
		// Save game state so that the user does not lose anything
		// if the game process is killed while we are in the 
		// background.
		public Bundle SaveState ()
		{
			Bundle map = new Bundle ();

			map.PutIntArray ("mAppleList", Coordinate.ListToArray (apples));
			map.PutInt ("mDirection", (int)mDirection);
			map.PutInt ("mNextDirection", (int)mNextDirection);
			map.PutInt ("mMoveDelay", mMoveDelay);
			map.PutLong ("mScore", mScore);
			map.PutIntArray ("mSnakeTrail", Coordinate.ListToArray (snake_trail));

			return map;
		}

		// Restore game state if our process is being relaunched
		public void RestoreState (Bundle icicle)
		{
			SetMode (GameMode.Paused);

			apples = Coordinate.ArrayToList (icicle.GetIntArray ("mAppleList"));
			mDirection = (Direction)icicle.GetInt ("mDirection");
			mNextDirection = (Direction)icicle.GetInt ("mNextDirection");
			mMoveDelay = icicle.GetInt ("mMoveDelay");
			mScore = icicle.GetLong ("mScore");
			snake_trail = Coordinate.ArrayToList (icicle.GetIntArray ("mSnakeTrail"));
		}
		#endregion
	}
}
