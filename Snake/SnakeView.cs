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
using Java.Lang;

namespace Mono.Samples.Snake
{
	// SnakeView: implementation of a simple game of Snake
	public class SnakeView : TileView
	{
		private static String TAG = "SnakeView";

		/**
		 * Current mode of application: READY to run, RUNNING, or you have already
		 * lost. static ints are used instead of an enum for performance
		 * reasons.
		 */
		private int mMode = READY;
		public static int PAUSE = 0;
		public static int READY = 1;
		public static int RUNNING = 2;
		public static int LOSE = 3;

		/**
		 * Current direction the snake is headed.
		 */
		private int mDirection = NORTH;
		private int mNextDirection = NORTH;
		private const int NORTH = 1;
		private const int SOUTH = 2;
		private const int EAST = 3;
		private const int WEST = 4;

		/**
		 * Labels for the drawables that will be loaded into the TileView class
		 */
		private static int RED_STAR = 1;
		private static int YELLOW_STAR = 2;
		private static int GREEN_STAR = 3;

		/**
		 * mScore: used to track the number of apples captured mMoveDelay: number of
		 * milliseconds between snake movements. This will decrease as apples are
		 * captured.
		 */
		private long mScore = 0;
		private long mMoveDelay = 600;
		/**
		 * mLastMove: tracks the absolute time when the snake last moved, and is used
		 * to determine if a move should be made based on mMoveDelay.
		 */
		private long mLastMove;

		/**
		 * mStatusText: text shows to the user in some run states
		 */
		private TextView mStatusText;

		/**
		 * mSnakeTrail: a list of Coordinates that make up the snake's body
		 * mAppleList: the secret location of the juicy apples the snake craves.
		 */
		private List<Coordinate> mSnakeTrail = new List<Coordinate> ();
		private List<Coordinate> mAppleList = new List<Coordinate> ();

		/**
		 * Everyone needs a little randomness in their life
		 */
		private static Random RNG = new Random ();

		/**
		 * Create a simple handler that we can use to cause animation to happen.  We
		 * set ourselves as a target and we can use the sleep()
		 * function to cause an update/invalidate to occur at a later date.
		 */
		private RefreshHandler mRedrawHandler = new RefreshHandler ();

		class RefreshHandler : Handler
		{

			public override void HandleMessage (Message msg)
			{
				// TODO
				//SnakeView.this.update();
				//SnakeView.this.invalidate();
			}

			public void sleep (long delayMillis)
			{
				this.RemoveMessages (0);
				SendMessageDelayed (ObtainMessage (0), delayMillis);
			}
		};


		/**
		 * Constructs a SnakeView based on inflation from XML
		 * 
		 * @param context
		 * @param attrs
		 */
		public SnakeView (Context context, AttributeSet attrs) :
			base (context, attrs)
		{
			InitSnakeView ();
		}

		public SnakeView (Context context, AttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			InitSnakeView ();
		}

		private void InitSnakeView ()
		{
			Focusable = true;

			Resources r = this.Context.Resources;

			resetTiles (4);
			loadTile (RED_STAR, r.GetDrawable (R.drawable.redstar));
			loadTile (YELLOW_STAR, r.GetDrawable (R.drawable.yellowstar));
			loadTile (GREEN_STAR, r.GetDrawable (R.drawable.greenstar));
		}

		private void InitNewGame ()
		{
			mSnakeTrail.Clear ();
			mAppleList.Clear ();

			// For now we're just going to load up a short default eastbound snake
			// that's just turned north
			mSnakeTrail.Add (new Coordinate (7, 7));
			mSnakeTrail.Add (new Coordinate (6, 7));
			mSnakeTrail.Add (new Coordinate (5, 7));
			mSnakeTrail.Add (new Coordinate (4, 7));
			mSnakeTrail.Add (new Coordinate (3, 7));
			mSnakeTrail.Add (new Coordinate (2, 7));
			mNextDirection = NORTH;

			// Two apples to start with
			AddRandomApple ();
			AddRandomApple ();

			mMoveDelay = 600;
			mScore = 0;
		}

		/**
		 * Given a List of coordinates, we need to flatten them into an array of
		 * ints before we can stuff them into a map for flattening and storage.
		 * 
		 * @param cvec : a List of Coordinate objects
		 * @return : a simple array containing the x/y values of the coordinates
		 * as [x1,y1,x2,y2,x3,y3...]
		 */
		private int[] CoordListToArray (List<Coordinate> cvec)
		{
			int count = cvec.Count;
			int[] rawArray = new int[count * 2];
			for (int index = 0; index < count; index++) {
				Coordinate c = cvec[index];
				rawArray[2 * index] = c.x;
				rawArray[2 * index + 1] = c.y;
			}
			return rawArray;
		}

		/**
		 * Save game state so that the user does not lose anything
		 * if the game process is killed while we are in the 
		 * background.
		 * 
		 * @return a Bundle with this view's state
		 */
		public Bundle SaveState ()
		{
			Bundle map = new Bundle ();

			map.PutIntArray ("mAppleList", CoordListToArray (mAppleList));
			map.PutInt ("mDirection", mDirection);
			map.PutInt ("mNextDirection", mNextDirection);
			map.PutLong ("mMoveDelay", mMoveDelay);
			map.PutLong ("mScore", mScore);
			map.PutIntArray ("mSnakeTrail", CoordListToArray (mSnakeTrail));

			return map;
		}

		/**
		 * Given a flattened array of ordinate pairs, we reconstitute them into a
		 * List of Coordinate objects
		 * 
		 * @param rawArray : [x1,y1,x2,y2,...]
		 * @return a List of Coordinates
		 */
		private List<Coordinate> CoordArrayToList (int[] rawArray)
		{
			List<Coordinate> coordList = new List<Coordinate> ();

			int coordCount = rawArray.Length;
			for (int index = 0; index < coordCount; index += 2) {
				Coordinate c = new Coordinate (rawArray[index], rawArray[index + 1]);
				coordList.Add (c);
			}
			return coordList;
		}

		/**
		 * Restore game state if our process is being relaunched
		 * 
		 * @param icicle a Bundle containing the game state
		 */
		public void RestoreState (Bundle icicle)
		{
			setMode (PAUSE);

			mAppleList = CoordArrayToList (icicle.GetIntArray ("mAppleList"));
			mDirection = icicle.GetInt ("mDirection");
			mNextDirection = icicle.GetInt ("mNextDirection");
			mMoveDelay = icicle.GetLong ("mMoveDelay");
			mScore = icicle.GetLong ("mScore");
			mSnakeTrail = CoordArrayToList (icicle.GetIntArray ("mSnakeTrail"));
		}

		/*
		 * handles key events in the game. Update the direction our snake is traveling
		 * based on the DPAD. Ignore events that would cause the snake to immediately
		 * turn back on itself.
		 * 
		 * (non-Javadoc)
		 * 
		 * @see android.view.View#onKeyDown(int, android.os.KeyEvent)
		 */
		public override bool OnKeyDown (int keyCode, KeyEvent msg)
		{

			if (keyCode == KeyEvent.KeycodeDpadUp) {
				if (mMode == READY | mMode == LOSE) {
					/*
					 * At the beginning of the game, or the end of a previous one,
					 * we should start a new game.
					 */
					InitNewGame ();
					setMode (RUNNING);
					Update ();
					return (true);
				}

				if (mMode == PAUSE) {
					/*
					 * If the game is merely paused, we should just continue where
					 * we left off.
					 */
					setMode (RUNNING);
					Update ();
					return (true);
				}

				if (mDirection != SOUTH) {
					mNextDirection = NORTH;
				}
				return (true);
			}

			if (keyCode == KeyEvent.KeycodeDpadDown) {
				if (mDirection != NORTH) {
					mNextDirection = SOUTH;
				}
				return (true);
			}

			if (keyCode == KeyEvent.KeycodeDpadLeft) {
				if (mDirection != EAST) {
					mNextDirection = WEST;
				}
				return (true);
			}

			if (keyCode == KeyEvent.KeycodeDpadRight) {
				if (mDirection != WEST) {
					mNextDirection = EAST;
				}
				return (true);
			}

			return base.OnKeyDown (keyCode, msg);
		}

		/**
		 * Sets the TextView that will be used to give information (such as "Game
		 * Over" to the user.
		 * 
		 * @param newView
		 */
		public void setTextView (TextView newView)
		{
			mStatusText = newView;
		}

		/**
		 * Updates the current mode of the application (RUNNING or PAUSED or the like)
		 * as well as sets the visibility of textview for notification
		 * 
		 * @param newMode
		 */
		public void setMode (int newMode)
		{
			int oldMode = mMode;
			mMode = newMode;

			if (newMode == RUNNING & oldMode != RUNNING) {
				mStatusText.Visibility = View.Invisible;
				Update ();
				return;
			}

			Resources res = Context.Resources;
			CharSequence str = "";
			if (newMode == PAUSE) {
				str = res.GetText (R.@string.mode_pause);
			}
			if (newMode == READY) {
				str = res.GetText (R.@string.mode_ready);
			}
			if (newMode == LOSE) {
				str = res.GetString (R.@string.mode_lose_prefix) + mScore
				      + res.GetString (R.@string.mode_lose_suffix);
			}

			mStatusText.Text = str;
			mStatusText.Visibility = View.Visible;
		}

		/**
		 * Selects a random location within the garden that is not currently covered
		 * by the snake. Currently _could_ go into an infinite loop if the snake
		 * currently fills the garden, but we'll leave discovery of this prize to a
		 * truly excellent snake-player.
		 * 
		 */
		private void AddRandomApple ()
		{
			Coordinate newCoord = null;
			bool found = false;
			while (!found) {
				// Choose a new location for our apple
				int newX = 1 + RNG.Next (mXTileCount - 2);
				int newY = 1 + RNG.Next (mYTileCount - 2);
				newCoord = new Coordinate (newX, newY);

				// Make sure it's not already under the snake
				bool collision = false;
				int snakelength = mSnakeTrail.Count;
				for (int index = 0; index < snakelength; index++) {
					if (mSnakeTrail[index] == newCoord) {
						collision = true;
					}
				}
				// if we're here and there's been no collision, then we have
				// a good location for an apple. Otherwise, we'll circle back
				// and try again
				found = !collision;
			}
			if (newCoord == null) {
				Log.E (TAG, "Somehow ended up with a null newCoord!");
			}
			mAppleList.Add (newCoord);
		}


		/**
		 * Handles the basic update loop, checking to see if we are in the running
		 * state, determining if a move should be made, updating the snake's location.
		 */
		public void Update ()
		{
			if (mMode == RUNNING) {
				long now = DateTime.Now.Millisecond; // System.currentTimeMillis ();

				if (now - mLastMove > mMoveDelay) {
					clearTiles ();
					UpdateWalls ();
					UpdateSnake ();
					UpdateApples ();
					mLastMove = now;
				}
				mRedrawHandler.sleep (mMoveDelay);
			}

		}

		/**
		 * Draws some walls.
		 * 
		 */
		private void UpdateWalls ()
		{
			for (int x = 0; x < mXTileCount; x++) {
				setTile (GREEN_STAR, x, 0);
				setTile (GREEN_STAR, x, mYTileCount - 1);
			}
			for (int y = 1; y < mYTileCount - 1; y++) {
				setTile (GREEN_STAR, 0, y);
				setTile (GREEN_STAR, mXTileCount - 1, y);
			}
		}

		/**
		 * Draws some apples.
		 * 
		 */
		private void UpdateApples ()
		{
			foreach (Coordinate c in mAppleList) {
				setTile (YELLOW_STAR, c.x, c.y);
			}
		}

		/**
		 * Figure out which way the snake is going, see if he's run into anything (the
		 * walls, himself, or an apple). If he's not going to die, we then add to the
		 * front and subtract from the rear in order to simulate motion. If we want to
		 * grow him, we don't subtract from the rear.
		 * 
		 */
		private void UpdateSnake ()
		{
			bool growSnake = false;

			// grab the snake by the head
			Coordinate head = mSnakeTrail[0];
			Coordinate newHead = new Coordinate (1, 1);

			mDirection = mNextDirection;

			switch (mDirection) {
				case EAST: {
						newHead = new Coordinate (head.x + 1, head.y);
						break;
					}
				case WEST: {
						newHead = new Coordinate (head.x - 1, head.y);
						break;
					}
				case NORTH: {
						newHead = new Coordinate (head.x, head.y - 1);
						break;
					}
				case SOUTH: {
						newHead = new Coordinate (head.x, head.y + 1);
						break;
					}
			}

			// Collision detection
			// For now we have a 1-square wall around the entire arena
			if ((newHead.x < 1) || (newHead.y < 1) || (newHead.x > mXTileCount - 2)
				|| (newHead.y > mYTileCount - 2)) {
				setMode (LOSE);
				return;

			}

			// Look for collisions with itself
			int snakelength = mSnakeTrail.Count;
			for (int snakeindex = 0; snakeindex < snakelength; snakeindex++) {
				Coordinate c = mSnakeTrail[snakeindex];
				if (c == newHead) {
					setMode (LOSE);
					return;
				}
			}

			// Look for apples
			int applecount = mAppleList.Count;
			for (int appleindex = 0; appleindex < applecount; appleindex++) {
				Coordinate c = mAppleList[appleindex];
				if (c == newHead) {
					mAppleList.Remove (c);
					AddRandomApple ();

					mScore++;
					mMoveDelay *= (long)0.9;

					growSnake = true;
				}
			}

			// push a new head onto the List and pull off the tail
			mSnakeTrail.Insert (0, newHead);
			// except if we want the snake to grow
			if (!growSnake) {
				mSnakeTrail.RemoveAt (mSnakeTrail.Count - 1);
			}

			int index = 0;
			foreach (Coordinate c in mSnakeTrail) {
				if (index == 0) {
					setTile (YELLOW_STAR, c.x, c.y);
				} else {
					setTile (RED_STAR, c.x, c.y);
				}
				index++;
			}

		}
	}
}
