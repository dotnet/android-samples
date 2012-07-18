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
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Util;
using Android.Views;

namespace JetBoy
{
	// JET info: the JetBoyThread receives all the events from the JET player
	// JET info: through the OnJetEventListener interface.
	public class JetBoyThread : Java.Lang.Thread, JetPlayer.IOnJetEventListener
	{
		const string TAG = "JetBoy";

		private Scoreboard scores;

		// How many frames per beat? The basic animation can be changed for
		// instance to 3/4 by changing this to 3.
		// untested is the impact on other parts of game logic for non 4/4 time.
		const int ANIMATION_FRAMES_PER_BEAT = 4;

		public bool mInitialized = false;

		// Queue for GameEvents
		protected Queue<GameEvent> mEventQueue = new Queue<GameEvent> ();

		// Context for processKey to maintain state across frames
		protected object mKeyContext = null;

		// the timer display in seconds
		public int mTimerLimit;

		// used for internal timing logic.
		public int TIMER_LIMIT = 72;

		// string value for timer display
		private String mTimerValue = "1:12";

		// which music bed is currently playing?
		public int mCurrentBed = 0;

		// a lazy graphic fudge for the initial title splash
		private Bitmap mTitleBG;

		private Bitmap mTitleBG2;

		// start, play, running, lose are the states we use
		public GameState mState;

		// has laser been fired and for how long?
		// user for fx logic on laser fire
		bool mLaserOn = false;

		long mLaserFireTime = 0;

		/** The drawable to use as the far background of the animation canvas */
		private Bitmap mBackgroundImageFar;

		/** The drawable to use as the close background of the animation canvas */
		private Bitmap mBackgroundImageNear;

		// JET info: event IDs within the JET file.
		// JET info: in this game 80 is used for sending asteroid across the screen
		// JET info: 82 is used as game time for 1/4 note beat.
		private byte NEW_ASTEROID_EVENT = 80;
		private byte TIMER_EVENT = 82;

		// used to track beat for synch of mute/unmute actions
		private int mBeatCount = 1;

		// our intrepid space boy
		private Bitmap[] mShipFlying = new Bitmap[4];

		// the twinkly bit
		private Bitmap[] mBeam = new Bitmap[4];

		// the things you are trying to hit
		private Bitmap[] mAsteroids = new Bitmap[12];

		// hit animation
		private Bitmap[] mExplosions = new Bitmap[4];

		private Bitmap mTimerShell;

		private Bitmap mLaserShot;

		// used to save the beat event system time.
		private long mLastBeatTime;

		private long mPassedTime;

		// how much do we move the asteroids per beat?
		private int mPixelMoveX = 25;

		// the asteroid send events are generated from the Jet File.
		// but which land they start in is random.
		private Random mRandom = new Random ();

		// JET info: the star of our show, a reference to the JetPlayer object.
		private JetPlayer mJet = null;

		private bool mJetPlaying = false;

		// Message handler used by thread to interact with TextView
		private Handler mHandler;

		// Handle to the surface manager object we interact with
		private ISurfaceHolder mSurfaceHolder;

		// Handle to the application context, used to e.g. fetch Drawables.
		private Context mContext;

		// Indicate whether the surface has been created & is ready to draw
		private bool mRun = false;

		// updates the screen clock. Also used for tempo timing.
		private Java.Util.Timer mTimer = null;

		private Java.Util.TimerTask mTimerTask = null;

		// one second - used to update timer
		private int mTaskIntervalInMillis = 1000;

		// Current size of the surface/canvas.
		private int mCanvasHeight = 1;
		private int mCanvasWidth = 1;

		// used to track the picture to draw for ship animation
		private int mShipIndex = 0;

		// stores all of the asteroid objects in order
		private List<Asteroid> asteroids;
		private List<Explosion> explosions;

		// right to left scroll tracker for near and far BG
		private int mBGFarMoveX = 0;
		private int mBGNearMoveX = 0;

		// how far up (close to top) jet boy can fly
		private int mJetBoyYMin = 40;
		private int mJetBoyX = 0;
		private int mJetBoyY = 0;

		// this is the pixel position of the laser beam guide.
		private int mAsteroidMoveLimitX = 110;

		// how far up asteroid can be painted
		private int mAsteroidMinY = 40;

		Resources mRes;

		// array to store the mute masks that are applied during game play to respond to
		// the player's hit streaks
		private bool[][] muteMask = new bool[9][];

		#region Constructor
		public JetBoyThread (ISurfaceHolder surfaceHolder, Context context, Scoreboard scores, Handler handler)
		{
			this.scores = scores;

			mSurfaceHolder = surfaceHolder;
			mHandler = handler;
			mContext = context;
			mRes = context.Resources;

			for (int x = 0; x < muteMask.Length; x++)
				muteMask[x] = new bool[32];

			// JET info: this are the mute arrays associated with the music beds in the
			// JET info: JET file
			for (int ii = 0; ii < 8; ii++)
				for (int xx = 0; xx < 32; xx++)
					muteMask[ii][xx] = true;

			muteMask[0][2] = false;
			muteMask[0][3] = false;
			muteMask[0][4] = false;
			muteMask[0][5] = false;

			muteMask[1][2] = false;
			muteMask[1][3] = false;
			muteMask[1][4] = false;
			muteMask[1][5] = false;
			muteMask[1][8] = false;
			muteMask[1][9] = false;

			muteMask[2][2] = false;
			muteMask[2][3] = false;
			muteMask[2][6] = false;
			muteMask[2][7] = false;
			muteMask[2][8] = false;
			muteMask[2][9] = false;

			muteMask[3][2] = false;
			muteMask[3][3] = false;
			muteMask[3][6] = false;
			muteMask[3][11] = false;
			muteMask[3][12] = false;

			muteMask[4][2] = false;
			muteMask[4][3] = false;
			muteMask[4][10] = false;
			muteMask[4][11] = false;
			muteMask[4][12] = false;
			muteMask[4][13] = false;

			muteMask[5][2] = false;
			muteMask[5][3] = false;
			muteMask[5][10] = false;
			muteMask[5][12] = false;
			muteMask[5][15] = false;
			muteMask[5][17] = false;

			muteMask[6][2] = false;
			muteMask[6][3] = false;
			muteMask[6][14] = false;
			muteMask[6][15] = false;
			muteMask[6][16] = false;
			muteMask[6][17] = false;

			muteMask[7][2] = false;
			muteMask[7][3] = false;
			muteMask[7][6] = false;
			muteMask[7][14] = false;
			muteMask[7][15] = false;
			muteMask[7][16] = false;
			muteMask[7][17] = false;
			muteMask[7][18] = false;

			// Set all tracks to play
			for (int xx = 0; xx < 32; xx++)
				muteMask[8][xx] = false;

			// Always set state to start, ensure we come in from
			// front door if app gets tucked into background
			mState = GameState.Start;

			SetInitialGameState ();

			// Load background image as a Bitmap instead of a Drawable b/c
			// we don't need to transform it and it's faster to draw this way
			mTitleBG = BitmapFactory.DecodeResource (mRes, Resource.Drawable.title_hori);

			// Two background since we want them moving at different speeds
			mBackgroundImageFar = BitmapFactory.DecodeResource (mRes, Resource.Drawable.background_a);
			mBackgroundImageNear = BitmapFactory.DecodeResource (mRes, Resource.Drawable.background_b);

			mLaserShot = BitmapFactory.DecodeResource (mRes, Resource.Drawable.laser);

			mShipFlying[0] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.ship2_1);
			mShipFlying[1] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.ship2_2);
			mShipFlying[2] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.ship2_3);
			mShipFlying[3] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.ship2_4);

			mBeam[0] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.intbeam_1);
			mBeam[1] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.intbeam_2);
			mBeam[2] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.intbeam_3);
			mBeam[3] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.intbeam_4);

			mTimerShell = BitmapFactory.DecodeResource (mRes, Resource.Drawable.int_timer);

			// I wanted them to rotate in a certain way
			// so I loaded them backwards from the way created.
			mAsteroids[11] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid01);
			mAsteroids[10] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid02);
			mAsteroids[9] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid03);
			mAsteroids[8] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid04);
			mAsteroids[7] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid05);
			mAsteroids[6] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid06);
			mAsteroids[5] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid07);
			mAsteroids[4] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid08);
			mAsteroids[3] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid09);
			mAsteroids[2] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid10);
			mAsteroids[1] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid11);
			mAsteroids[0] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid12);

			mExplosions[0] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid_explode1);
			mExplosions[1] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid_explode2);
			mExplosions[2] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid_explode3);
			mExplosions[3] = BitmapFactory.DecodeResource (mRes, Resource.Drawable.asteroid_explode4);
		}
		#endregion

		#region Initialization
		// Does the grunt work of setting up initial jet requirements
		private void InitializeJetPlayer ()
		{
			// JET info: let's create our JetPlayer instance using the factory.
			// JET info: if we already had one, the same singleton is returned.
			mJet = JetPlayer.GetJetPlayer ();

			mJetPlaying = false;

			// JET info: make sure we flush the queue,
			// JET info: otherwise left over events from previous gameplay can hang around.
			// JET info: ok, here we don't really need that but if you ever reuse a JetPlayer
			// JET info: instance, clear the queue before reusing it, this will also clear any
			// JET info: trigger clips that have been triggered but not played yet.
			mJet.ClearQueue ();

			// JET info: we are going to receive in this example all the JET callbacks
			// JET info: inthis animation thread object. 
			mJet.SetEventListener (this);

			Log.Debug (TAG, "opening jet file");

			// JET info: load the actual JET content the game will be playing,
			// JET info: it's stored as a raw resource in our APK, and is labeled "level1"
			// JET info: if our JET file was stored on the sdcard for instance, we would have used
			// JET info: mJet.loadJetFile("/sdcard/level1.jet");
			mJet.LoadJetFile (mContext.Resources.OpenRawResourceFd (Resource.Raw.level1));

			Log.Debug (TAG, "opening jet file DONE");

			mCurrentBed = 0;
			sbyte sSegmentID = 0;

			Log.Debug (TAG, " start queuing jet file");

			// JET info: now we're all set to prepare queuing the JET audio segments for the game.
			// JET info: in this example, the game uses segment 0 for the duration of the game play,
			// JET info: and plays segment 1 several times as the "outro" music, so we're going to
			// JET info: queue everything upfront, but with more complex JET compositions, we could
			// JET info: also queue the segments during the game play.

			// JET info: this is the main game play music
			// JET info: it is located at segment 0
			// JET info: it uses the first DLS lib in the .jet resource, which is at index 0
			// JET info: index -1 means no DLS
			mJet.QueueJetSegment (0, 0, 0, 0, 0, sSegmentID);

			// JET info: end game music, loop 4 times normal pitch
			mJet.QueueJetSegment (1, 0, 4, 0, 0, sSegmentID);

			// JET info: end game music loop 4 times up an octave
			mJet.QueueJetSegment (1, 0, 4, 1, 0, sSegmentID);

			// JET info: set the mute mask as designed for the beginning of the game, when the
			// JET info: the player hasn't scored yet.
			mJet.SetMuteArray (muteMask[0], true);

			Log.Debug (TAG, " start queuing jet file DONE");
		}

		private void SetInitialGameState ()
		{
			mTimerLimit = TIMER_LIMIT;

			mJetBoyY = mJetBoyYMin;

			// set up jet stuff
			InitializeJetPlayer ();

			mTimer = new Java.Util.Timer ();

			asteroids = new List<Asteroid> ();

			explosions = new List<Explosion> ();

			mInitialized = true;

			scores.HitStreak = 0;
			scores.HitTotal = 0;
		}
		#endregion

		#region Drawing
		private void DoDraw (Canvas canvas)
		{
			switch (mState) {
				case GameState.Start:
					DoDrawReady (canvas);
					break;
				case GameState.Play:
				case GameState.Lost:
					if (mTitleBG2 == null)
						mTitleBG2 = BitmapFactory.DecodeResource (mRes, Resource.Drawable.title_bg_hori);

					DoDrawPlay (canvas);

					break;
				case GameState.Running:
					DoDrawRunning (canvas);
					break;
			}
		}

		// Draws current state of the game Canvas.
		private void DoDrawRunning (Canvas canvas)
		{
			// Decrement the far background
			mBGFarMoveX = mBGFarMoveX - 1;

			// Decrement the near background
			mBGNearMoveX = mBGNearMoveX - 4;

			// Calculate the wrap factor for matching image draw
			int newFarX = mBackgroundImageFar.Width - (-mBGFarMoveX);

			// If we have scrolled all the way, reset to start
			if (newFarX <= 0) {
				mBGFarMoveX = 0;
				// Only need one draw
				canvas.DrawBitmap (mBackgroundImageFar, mBGFarMoveX, 0, null);

			} else {
				// Need to draw original and wrap
				canvas.DrawBitmap (mBackgroundImageFar, mBGFarMoveX, 0, null);
				canvas.DrawBitmap (mBackgroundImageFar, newFarX, 0, null);
			}

			// Same story different image...
			int newNearX = mBackgroundImageNear.Width - (-mBGNearMoveX);

			if (newNearX <= 0) {
				mBGNearMoveX = 0;
				canvas.DrawBitmap (mBackgroundImageNear, mBGNearMoveX, 0, null);
			} else {
				canvas.DrawBitmap (mBackgroundImageNear, mBGNearMoveX, 0, null);
				canvas.DrawBitmap (mBackgroundImageNear, newNearX, 0, null);
			}

			DoAsteroidAnimation (canvas);

			canvas.DrawBitmap (mBeam[mShipIndex], 51 + 20, 0, null);

			mShipIndex++;

			if (mShipIndex == 4)
				mShipIndex = 0;

			// Draw the space ship in the same lane as the next asteroid
			canvas.DrawBitmap (mShipFlying[mShipIndex], mJetBoyX, mJetBoyY, null);

			if (mLaserOn) {
				canvas.DrawBitmap (mLaserShot, mJetBoyX + mShipFlying[0].Width, mJetBoyY
					+ (mShipFlying[0].Height / 2), null);
			}

			// Tick tock
			canvas.DrawBitmap (mTimerShell, mCanvasWidth - mTimerShell.Width, 0, null);

		}

		private void DoAsteroidAnimation (Canvas canvas)
		{
			if ((asteroids == null | asteroids.Count == 0)
				&& (explosions != null && explosions.Count == 0))
				return;

			// Compute what percentage through a beat we are and adjust
			// animation and position based on that. This assumes 140bpm(428ms/beat).
			// This is just inter-beat interpolation, no game state is updated
			long frameDelta = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - mLastBeatTime;

			int animOffset = (int)(ANIMATION_FRAMES_PER_BEAT * frameDelta / 428);

			for (int i = (asteroids.Count - 1); i >= 0; i--) {
				Asteroid asteroid = asteroids[i];

				if (!asteroid.Missed)
					mJetBoyY = asteroid.DrawY;

				canvas.DrawBitmap (
					mAsteroids[(asteroid.AnimationIndex + animOffset) % mAsteroids.Length],
					asteroid.DrawX, asteroid.DrawY, null);
			}

			for (int i = (explosions.Count - 1); i >= 0; i--) {
				Explosion ex = explosions[i];

				canvas.DrawBitmap (mExplosions[(ex.AnimationIndex + animOffset) % mExplosions.Length],
					ex.DrawX, ex.DrawY, null);
			}
		}

		private void DoDrawReady (Canvas canvas)
		{
			canvas.DrawBitmap (mTitleBG, 0, 0, null);
		}

		private void DoDrawPlay (Canvas canvas)
		{
			canvas.DrawBitmap (mTitleBG2, 0, 0, null);
		}
		#endregion

		#region Game Logic
		public override void Run ()
		{
			while (mRun) {
				Canvas c = null;

				if (mState == GameState.Running) {
					// Process any input and apply it to the game state
					UpdateGameState ();

					if (!mJetPlaying) {
						mInitialized = false;

						Log.Debug (TAG, "------> STARTING JET PLAY");

						mJet.Play ();
						mJetPlaying = true;
					}

					mPassedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

					// Kick off the timer task for counter update
					// if not already initialized
					if (mTimerTask == null) {
						mTimerTask = new CountDownTimerTask (this);

						mTimer.Schedule (mTimerTask, mTaskIntervalInMillis);
					}
				} else if (mState == GameState.Play && !mInitialized) {
					SetInitialGameState ();
				} else if (mState == GameState.Lost) {
					mInitialized = false;
				}

				try {
					c = mSurfaceHolder.LockCanvas (null);

					lock (mSurfaceHolder)
					DoDraw (c);
				} finally {
					// do this in a finally so that if an exception is thrown
					// during the above, we don't leave the Surface in an
					// inconsistent state
					if (c != null)
						mSurfaceHolder.UnlockCanvasAndPost (c);
				}
			}
		}

		// This method handles updating the model of the game state. No
		// rendering is done here only processing of inputs and update of state.
		// This includes positons of all game objects (asteroids, player,
		// explosions), their state (animation frame, hit), creation of new
		// objects, etc.
		protected void UpdateGameState ()
		{
			// Process any game events and apply them
			while (true) {
				if (mEventQueue.Count == 0)
					break;

				GameEvent ev = mEventQueue.Dequeue ();

				if (ev == null)
					break;

				// Process keys tracking the input context
				// to pass in to later calls
				if (ev is KeyGameEvent) {
					// Process the key for effects other then asteroid hits
					mKeyContext = ProcessKeyEvent ((KeyGameEvent)ev, mKeyContext);

					// Update laser state. Having this here allows the laser to
					// be triggered right when the key is
					// pressed. If we comment this out the laser will only be
					// turned on when updateLaser is called
					// when processing a timer event below.
					UpdateLaser (mKeyContext);
				}
				
				// JET events trigger a state update
				if (ev is JetGameEvent) {
					JetGameEvent jetEvent = (JetGameEvent)ev;

					// Only update state on a timer event
					if (jetEvent.Value == TIMER_EVENT) {
						// Note the time of the last beat
						mLastBeatTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

						// Update laser state, turning it on if a key has been
						// pressed or off if it has been
						// on for too long.
						UpdateLaser (mKeyContext);

						// Update explosions before we update asteroids because
						// updateAsteroids may add
						// new explosions that we do not want updated until next
						// frame
						UpdateExplosions (mKeyContext);

						// Update asteroid positions, hit status and animations
						UpdateAsteroids (mKeyContext);
					}

					ProcessJetEvent (jetEvent.Player, jetEvent.Segment, jetEvent.Track,
						jetEvent.Channel, jetEvent.Controller, jetEvent.Value);
				}
			}
		}

		// This method handles the state updates that can be caused by key press
		// events. Key events may mean different things depending on what has
		// come before, to support this concept this method takes an opaque
		// context object as a parameter and returns an updated version. This
		// context should be set to null for the first event then should be set
		// to the last value returned for subsequent events.
		protected object ProcessKeyEvent (KeyGameEvent ev, object context)
		{
			if (ev.Up) {
				// If it is a key up on the fire key
				// make sure we mute the associated sound
				if (ev.KeyCode == Keycode.DpadCenter)
					return null;
			} else {
				// If it is a key down on the fire key start playing the sound and
				// update the context to indicate that a key has been
				// pressed and to ignore further presses
				if (ev.KeyCode == Keycode.DpadCenter && (context == null))
					return ev;
			}

			// Return the context unchanged
			return context;
		}

		// This method updates the laser status based on user input and shot
		// duration
		protected void UpdateLaser (object inputContext)
		{
			// Lookup the time of the fire event if there is one
			long keyTime = inputContext == null ? 0 : ((GameEvent)inputContext).EventTime;

			// If the laser has been on too long shut it down
			if (mLaserOn && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - mLaserFireTime > 400) {
				mLaserOn = false;
			}

			// Trying to tune the laser hit timing
			else if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - mLaserFireTime > 300) {
				// JET info: the laser sound is on track 23, we mute it (true) right away (false)
				mJet.SetMuteFlag (23, true, false);

			}

			// Now check to see if we should turn the laser on. We do this after
			// the above shutdown logic so it can be turned back on in the  same frame
			// it was turned off in. If we want to add a cooldown period this may change.
			if (!mLaserOn && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - keyTime <= 400) {
				mLaserOn = true;
				mLaserFireTime = keyTime;

				// JET info: unmute the laser track (false) right away (false)
				mJet.SetMuteFlag (23, false, false);
			}
		}

		// Update asteroid state including position and laser hit status.
		protected void UpdateAsteroids (object inputContext)
		{
			if (asteroids == null | asteroids.Count == 0)
				return;

			for (int i = (asteroids.Count - 1); i >= 0; i--) {
				Asteroid asteroid = asteroids[i];

				// If the asteroid is within laser range but not already missed
				// check if the key was pressed close enough to the beat to make a hit
				if (asteroid.DrawX <= mAsteroidMoveLimitX + 20 && !asteroid.Missed) {
					// If the laser was fired on the beat destroy the asteroid
					if (mLaserOn) {
						// Track hit streak for adjusting music
						scores.HitStreak++;
						scores.HitTotal++;

						// Replace the asteroid with an explosion
						Explosion ex = new Explosion ();

						ex.AnimationIndex = 0;
						ex.DrawX = asteroid.DrawX;
						ex.DrawY = asteroid.DrawY;

						explosions.Add (ex);

						mJet.SetMuteFlag (24, false, false);

						asteroids.RemoveAt (i);

						// This asteroid has been removed process the next one
						continue;
					} else {
						// Sorry, timing was not good enough, mark the asteroid
						// as missed so on next frame it cannot be hit even if it is still
						// within range
						asteroid.Missed = true;

						scores.HitStreak = scores.HitStreak - 1;

						if (scores.HitStreak < 0)
							scores.HitStreak = 0;
					}
				}

				// Update the asteroids position, even missed ones keep moving
				asteroid.DrawX -= mPixelMoveX;

				// Update asteroid animation frame
				asteroid.AnimationIndex = (asteroid.AnimationIndex + ANIMATION_FRAMES_PER_BEAT)
					% mAsteroids.Length;

				// if we have scrolled off the screen
				if (asteroid.DrawX < 0)
					asteroids.RemoveAt (i);
			}
		}

		// This method updates explosion animation and
		// removes them once they have completed.
		protected void UpdateExplosions (object inputContext)
		{
			if (explosions == null | explosions.Count == 0)
				return;

			for (int i = explosions.Count - 1; i >= 0; i--) {
				Explosion ex = explosions[i];

				ex.AnimationIndex += ANIMATION_FRAMES_PER_BEAT;

				// When the animation completes remove the explosion
				if (ex.AnimationIndex > 3) {
					mJet.SetMuteFlag (24, true, false);
					mJet.SetMuteFlag (23, true, false);

					explosions.RemoveAt (i);
				}
			}
		}

		// This method handles the state updates that can be caused by JET
		// events.
		protected void ProcessJetEvent (JetPlayer player, short segment, sbyte track, sbyte channel,
			sbyte controller, sbyte value)
		{
			// Check for an event that triggers a new asteroid
			if (value == NEW_ASTEROID_EVENT)
				DoAsteroidCreation ();

			mBeatCount++;

			if (mBeatCount > 4)
				mBeatCount = 1;

			// Scale the music based on progress

			// it was a game requirement to change the mute array on 1st beat of
			// the next measure when needed
			// and so we track beat count, after that we track hitStreak to
			// determine the music "intensity"
			// if the intensity has go gone up, call a corresponding trigger clip, otherwise just
			// execute the rest of the music bed change logic.
			if (mBeatCount == 1) {

				// do it back wards so you fall into the correct one
				if (scores.HitStreak > 28) {

					// did the bed change?
					if (mCurrentBed != 7) {
						// did it go up?
						if (mCurrentBed < 7)
							mJet.TriggerClip (7);

						mCurrentBed = 7;
						// JET info: change the mute mask to update the way the music plays based
						// JET info: on the player's skills.
						mJet.SetMuteArray (muteMask[7], false);

					}
				} else if (scores.HitStreak > 24) {
					if (mCurrentBed != 6) {
						if (mCurrentBed < 6) {
							// JET info: quite a few asteroids hit, trigger the clip with the guy's
							// JET info: voice that encourages the player.
							mJet.TriggerClip (6);
						}

						mCurrentBed = 6;
						mJet.SetMuteArray (muteMask[6], false);
					}
				} else if (scores.HitStreak > 20) {
					if (mCurrentBed != 5) {
						if (mCurrentBed < 5) {
							mJet.TriggerClip (5);
						}

						mCurrentBed = 5;
						mJet.SetMuteArray (muteMask[5], false);
					}
				} else if (scores.HitStreak > 16) {
					if (mCurrentBed != 4) {

						if (mCurrentBed < 4) {
							mJet.TriggerClip (4);
						}
						mCurrentBed = 4;
						mJet.SetMuteArray (muteMask[4], false);
					}
				} else if (scores.HitStreak > 12) {
					if (mCurrentBed != 3) {
						if (mCurrentBed < 3) {
							mJet.TriggerClip (3);
						}
						mCurrentBed = 3;
						mJet.SetMuteArray (muteMask[3], false);
					}
				} else if (scores.HitStreak > 8) {
					if (mCurrentBed != 2) {
						if (mCurrentBed < 2) {
							mJet.TriggerClip (2);
						}

						mCurrentBed = 2;
						mJet.SetMuteArray (muteMask[2], false);
					}
				} else if (scores.HitStreak > 4) {
					if (mCurrentBed != 1) {

						if (mCurrentBed < 1) {
							mJet.TriggerClip (1);
						}

						mJet.SetMuteArray (muteMask[1], false);

						mCurrentBed = 1;
					}
				}
			}
		}

		private void DoAsteroidCreation ()
		{
			Asteroid a = new Asteroid ();

			int drawIndex = mRandom.Next (0, 4);

			a.DrawY = mAsteroidMinY + (drawIndex * 63);
			a.DrawX = (mCanvasWidth - mAsteroids[0].Width);
			a.StartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			asteroids.Add (a);
		}

		// Used to signal the thread whether it should be running or not.
		// Passing true allows the thread to run; passing false will shut it
		// down if it's already running. Calling start() after this was most
		// recently called with false will result in an immediate shutdown.
		public void SetRunning (bool b)
		{
			mRun = b;

			if (mRun == false) {
				if (mTimerTask != null)
					mTimerTask.Cancel ();
			}
		}

		// Sets the game mode. That is, whether we are running, paused, in the
		// failure state, in the victory state, etc.
		public GameState GameState {
			get {
				lock (mSurfaceHolder)
					return mState;
			}
			set { SetGameState (value); }
		}

		/**
		 * Sets state based on input, optionally also passing in a text message.
		 * 
		 * @param state
		 * @param message
		 */
		public void SetGameState (GameState state)
		{
			lock (mSurfaceHolder) {
				// Change state if needed
				if (mState != state)
					mState = state;

				if (mState == GameState.Play) {
					Resources res = mContext.Resources;

					mBackgroundImageFar.Dispose ();

					// Don't forget to resize the background image
					using (var b = BitmapFactory.DecodeResource (res, Resource.Drawable.background_a))
						mBackgroundImageFar = Bitmap.CreateScaledBitmap (b, mCanvasWidth * 2, mCanvasHeight, true);

					mBackgroundImageNear.Dispose ();

					// Don't forget to resize the background image
					using (var b = BitmapFactory.DecodeResource (res, Resource.Drawable.background_b))
						mBackgroundImageNear = Bitmap.CreateScaledBitmap (b, mCanvasWidth * 2, mCanvasHeight, true);

				} else if (mState == GameState.Running) {
					// When we enter the running state we should clear any old
					// events in the queue
					mEventQueue.Clear ();

					// And reset the key state so we don't think a button is pressed when it isn't
					mKeyContext = null;
				}

			}
		}

		// Add key press input to the GameEvent queue
		public bool DoKeyDown (Keycode keyCode, KeyEvent msg)
		{
			mEventQueue.Enqueue (new KeyGameEvent (keyCode, false, msg));

			return true;
		}


		// Add key press input to the GameEvent queue
		public bool DoKeyUp (Keycode keyCode, KeyEvent msg)
		{
			mEventQueue.Enqueue (new KeyGameEvent (keyCode, true, msg));

			return true;
		}

		// Treat a click as pressing the dpad
		public void DoClick ()
		{
			mEventQueue.Enqueue (new KeyGameEvent (Keycode.DpadCenter, false, null));
			mEventQueue.Enqueue (new KeyGameEvent (Keycode.DpadCenter, true, null));
		}

		// Callback invoked when the surface dimensions change.
		public void SetSurfaceSize (int width, int height)
		{
			// synchronized to make sure these all change atomically
			lock (mSurfaceHolder) {
				mCanvasWidth = width;
				mCanvasHeight = height;

				// don't forget to resize the background image
				using (var b = mBackgroundImageFar)
					mBackgroundImageFar = Bitmap.CreateScaledBitmap (b, width * 2, height, true);

				// don't forget to resize the background image
				using (var b = mBackgroundImageNear)
					mBackgroundImageNear = Bitmap.CreateScaledBitmap (b, width * 2, height, true);
			}
		}

		// Pauses the physics update & animation.
		public void Pause ()
		{
			lock (mSurfaceHolder) {
				if (mState == GameState.Running)
					GameState = GameState.Pause;

				if (mTimerTask != null)
					mTimerTask.Cancel ();

				if (mJet != null)
					mJet.Pause ();
			}
		}

		// Does the work of updating timer
		internal void DoCountDown ()
		{
			mTimerLimit = mTimerLimit - 1;

			TimeSpan ts = new TimeSpan (0, 0, mTimerLimit);
			mTimerValue = string.Format ("{0}:{1:00}", ts.Minutes, ts.Seconds);

			Message msg = mHandler.ObtainMessage ();

			Bundle b = new Bundle ();
			b.PutString ("text", mTimerValue);

			// Time's up
			if (mTimerLimit == 0) {
				b.PutString ("STATE_LOSE", "" + (int)GameState.Lost);
				mTimerTask = null;

				mState = GameState.Lost;
			} else {
				mTimerTask = new CountDownTimerTask (this);
				mTimer.Schedule (mTimerTask, mTaskIntervalInMillis);
			}

			// This is how we send data back up to the main JetBoyView thread.
			// if you look in constructor of JetBoyView you will see code for
			// Handling of messages. This is borrowed directly from lunar lander.
			// Thanks again!
			msg.Data = b;
			mHandler.SendMessage (msg);
		}
		#endregion

		#region IOnJetEventListener Members
		public void OnJetEvent (JetPlayer player, short segment, sbyte track, sbyte channel, sbyte controller, sbyte value)
		{
			// Events fire outside the animation thread. This can cause timing issues.
			// put in queue for processing by animation thread.
			mEventQueue.Enqueue (new JetGameEvent (player, segment, track, channel, controller, value));
		}

		public void OnJetNumQueuedSegmentUpdate (JetPlayer player, int nbSegments)
		{
		}

		public void OnJetPauseUpdate (JetPlayer player, int paused)
		{
		}

		public void OnJetUserIdUpdate (JetPlayer player, int userId, int repeatCount)
		{
		}
		#endregion
	}
}
