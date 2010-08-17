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

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using Java.Lang; // for CharSequence

using System;
using System.Threading;

using Thread = System.Threading.Thread;

/**
 * View that draws, takes keystrokes, etc. for a simple LunarLander game.
 * 
 * Has a mode which RUNNING, PAUSED, etc. Has a x, y, dx, dy, ... capturing the
 * current ship physics. All x/y etc. are measured with (0,0) at the lower left.
 * updatePhysics() advances the physics based on realtime. draw() renders the
 * ship, and does an invalidate() to prompt another draw() as soon as possible
 * by the system.
 */

namespace Mono.Samples.LunarLander {
	class LunarView : SurfaceView, ISurfaceHolderCallback
	{
		/** Handle to the application context, used to e.g. fetch Drawables. */
		private Context mContext;

		/** Pointer to the text view to display "Paused.." etc. */
		private TextView mStatusText;

		/** The thread that actually draws the animation */
		private LunarThread thread;

		public LunarView (IntPtr handle) : base (handle) 
		{
			// register our interest in hearing about changes to our surface
			ISurfaceHolder holder = Holder;
			Holder.AddCallback (this);

			// create thread only; it's started in surfaceCreated()
			thread = new LunarThread(this, holder, Context, new StartThreadCallback (this));

			Focusable = true; // make sure we get key events
		}

		/**
		 * Fetches the animation thread corresponding to this LunarView.
		 * 
		 * @return the animation thread
		 */
		public LunarThread Thread {
			get { return thread; }
		}

		/**
		 * Standard override to get key-press events.
		 */
		public override bool OnKeyDown (int keyCode, KeyEvent msg)
		{
			return thread.DoKeyDown(keyCode, msg);
		}

		/**
		 * Standard override for key-up. We actually care about these, so we can
		 * turn off the engine or stop rotating.
		 */
		public override bool OnKeyUp(int keyCode, KeyEvent msg)
		{
			return thread.DoKeyUp(keyCode, msg);
		}

		/**
		 * Standard window-focus override. Notice focus lost so we can pause on
		 * focus lost. e.g. user switches to take a call.
		 */
		public override void OnWindowFocusChanged(bool hasWindowFocus)
		{
			if (!hasWindowFocus) thread.Pause();
		}

		/**
		 * Installs a pointer to the text view used for messages.
		 */
		public void SetTextView(TextView textView)
		{
			mStatusText = textView;
		}

		/* Callback invoked when the surface dimensions change. */
		public void SurfaceChanged(ISurfaceHolder holder, int format, int width, int height)
		{
			thread.SetSurfaceSize(width, height);
		}

		/*
		 * Callback invoked when the Surface has been created and is ready to be
		 * used.
		 */
		public void SurfaceCreated(ISurfaceHolder holder)
		{
			// start the thread here so that we don't busy-wait in run()
			// waiting for the surface to be created
			thread.SetRunning(true);
			thread.Start();
		}

		/*
		 * Callback invoked when the Surface has been destroyed and must no longer
		 * be touched. WARNING: after this method returns, the Surface/Canvas must
		 * never be touched again!
		 */
		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			// we have to tell thread to shut down & wait for it to finish, or else
			// it might touch the Surface after we return and explode
			bool retry = true;
			thread.SetRunning(false);
			while (retry) {
				try {
					thread.Join();
					retry = false;
				} catch (ThreadInterruptedException e) {
				}
			}
		}

		class StartThreadCallback : Handler
		{
			LunarView view;
			public StartThreadCallback (LunarView view)
			{
				this.view = view;
			}

			public override void HandleMessage(Message m)
			{
				view.mStatusText.Visibility = m.Data.GetInt("viz");
				view.mStatusText.Text = m.Data.GetString("text");
			}
		}

		public class LunarThread
		{
			/*
			 * Difficulty setting constants
			 */
			public const int DIFFICULTY_EASY = 0;
			public const int DIFFICULTY_HARD = 1;
			public const int DIFFICULTY_MEDIUM = 2;
			/*
			 * Physics constants
			 */
			public const int PHYS_DOWN_ACCEL_SEC = 35;
			public const int PHYS_FIRE_ACCEL_SEC = 80;
			public const int PHYS_FUEL_INIT = 60;
			public const int PHYS_FUEL_MAX = 100;
			public const int PHYS_FUEL_SEC = 10;
			public const int PHYS_SLEW_SEC = 120; // degrees/second rotate
			public const int PHYS_SPEED_HYPERSPACE = 180;
			public const int PHYS_SPEED_INIT = 30;
			public const int PHYS_SPEED_MAX = 120;
			/*
			 * State-tracking constants
			 */
			public const int STATE_LOSE = 1;
			public const int STATE_PAUSE = 2;
			public const int STATE_READY = 3;
			public const int STATE_RUNNING = 4;
			public const int STATE_WIN = 5;

			/*
			 * Goal condition constants
			 */
			public const int TARGET_ANGLE = 18; // > this angle means crash
			public const int TARGET_BOTTOM_PADDING = 17; // px below gear
			public const int TARGET_PAD_HEIGHT = 8; // how high above ground
			public const int TARGET_SPEED = 28; // > this speed means crash
			public const double TARGET_WIDTH = 1.6; // width of target
			/*
			 * UI constants (i.e. the speed & fuel bars)
			 */
			public const int UI_BAR = 100; // width of the bar(s)
			public const int UI_BAR_HEIGHT = 10; // height of the bar(s)
			private const string KEY_DIFFICULTY = "mDifficulty";
			private const string KEY_DX = "mDX";

			private const string KEY_DY = "mDY";
			private const string KEY_FUEL = "mFuel";
			private const string KEY_GOAL_ANGLE = "mGoalAngle";
			private const string KEY_GOAL_SPEED = "mGoalSpeed";
			private const string KEY_GOAL_WIDTH = "mGoalWidth";

			private const string KEY_GOAL_X = "mGoalX";
			private const string KEY_HEADING = "mHeading";
			private const string KEY_LANDER_HEIGHT = "mLanderHeight";
			private const string KEY_LANDER_WIDTH = "mLanderWidth";
			private const string KEY_WINS = "mWinsInARow";

			private const string KEY_X = "mX";
			private const string KEY_Y = "mY";

			/*
			 * Member (state) fields
			 */
			/** The drawable to use as the background of the animation canvas */
			private Bitmap mBackgroundImage;

			/**
			 * Current height of the surface/canvas.
			 * 
			 * @see #setSurfaceSize
			 */
			private int mCanvasHeight = 1;

			/**
			 * Current width of the surface/canvas.
			 * 
			 * @see #setSurfaceSize
			 */
			private int mCanvasWidth = 1;

			/** What to draw for the Lander when it has crashed */
			private Drawable mCrashedImage;

			/**
			 * Current difficulty -- amount of fuel, allowed angle, etc. Default is
			 * MEDIUM.
			 */
			private int mDifficulty;

			/** Velocity dx. */
			private double mDX;

			/** Velocity dy. */
			private double mDY;

			/** Is the engine burning? */
			private bool mEngineFiring;

			/** What to draw for the Lander when the engine is firing */
			private Drawable mFiringImage;

			/** Fuel remaining */
			private double mFuel;

			/** Allowed angle. */
			private int mGoalAngle;

			/** Allowed speed. */
			private int mGoalSpeed;

			/** Width of the landing pad. */
			private int mGoalWidth;

			/** X of the landing pad. */
			private int mGoalX;

			/** Message handler used by thread to interact with TextView */
			private Handler mHandler;

			/**
			 * Lander heading in degrees, with 0 up, 90 right. Kept in the range
			 * 0..360.
			 */
			private double mHeading;

			/** Pixel height of lander image. */
			private int mLanderHeight;

			/** What to draw for the Lander in its normal state */
			private Drawable mLanderImage;

			/** Pixel width of lander image. */
			private int mLanderWidth;

			/** Used to figure out elapsed time between frames */
			private long mLastTime;

			/** Paint to draw the lines on screen. */
			private Paint mLinePaint;

			/** "Bad" speed-too-high variant of the line color. */
			private Paint mLinePaintBad;

			/** The state of the game. One of READY, RUNNING, PAUSE, LOSE, or WIN */
			private int mMode;

			/** Currently rotating, -1 left, 0 none, 1 right. */
			private int mRotating;

			/** Indicate whether the surface has been created & is ready to draw */
			private bool mRun = false;

			/** Scratch rect object. */
			private RectF mScratchRect;

			/** Handle to the surface manager object we interact with */
			private ISurfaceHolder mSurfaceHolder;

			/** Number of wins in a row. */
			private int mWinsInARow;

			/** X of lander center. */
			private double mX;

			/** Y of lander center. */
			private double mY;

			Thread thread;
			LunarView view;

			public LunarThread (LunarView view, ISurfaceHolder surfaceHolder, Context context, Handler handler)
			{
				this.view = view;
				thread = new Thread (Run);

				// get handles to some important objects
				mSurfaceHolder = surfaceHolder;
				mHandler = handler;
				view.mContext = context;

				Resources res = context.Resources;
				// cache handles to our key sprites & other drawables
				mLanderImage = context.Resources.GetDrawable (
						R.drawable.lander_plain);
				mFiringImage = context.Resources.GetDrawable(
						R.drawable.lander_firing);
				mCrashedImage = context.Resources.GetDrawable(
						R.drawable.lander_crashed);

				// load background image as a Bitmap instead of a Drawable b/c
				// we don't need to transform it and it's faster to draw this way
				mBackgroundImage = BitmapFactory.DecodeResource (res,
						R.drawable.earthrise);

				// Use the regular lander image as the model size for all sprites
				mLanderWidth = mLanderImage.IntrinsicWidth;
				mLanderHeight = mLanderImage.IntrinsicHeight;

				// Initialize paints for speedometer
				mLinePaint = new Paint ();
				mLinePaint.AntiAlias = true;
				mLinePaint.SetARGB (255, 0, 255, 0);

				mLinePaintBad = new Paint ();
				mLinePaintBad.AntiAlias = true;
				mLinePaintBad.SetARGB (255, 120, 180, 0);

				mScratchRect = new RectF (0, 0, 0, 0);

				mWinsInARow = 0;
				mDifficulty = DIFFICULTY_MEDIUM;

				// initial show-up of lander (not yet playing)
				mX = mLanderWidth;
				mY = mLanderHeight * 2;
				mFuel = PHYS_FUEL_INIT;
				mDX = 0;
				mDY = 0;
				mHeading = 0;
				mEngineFiring = true;
			}

			public void Start ()
			{
				thread.Start ();
			}

			public void Join ()
			{
				thread.Join ();
			}

			/**
			 * Starts the game, setting parameters for the current difficulty.
			 */
			public void DoStart()
			{
				lock (mSurfaceHolder) {
					// First set the game for Medium difficulty
					mFuel = PHYS_FUEL_INIT;
					mEngineFiring = false;
					mGoalWidth = (int) (mLanderWidth * TARGET_WIDTH);
					mGoalSpeed = TARGET_SPEED;
					mGoalAngle = TARGET_ANGLE;
					int speedInit = PHYS_SPEED_INIT;

					// Adjust difficulty params for EASY/HARD
					if (mDifficulty == DIFFICULTY_EASY) {
						mFuel = mFuel * 3 / 2;
						mGoalWidth = mGoalWidth * 4 / 3;
						mGoalSpeed = mGoalSpeed * 3 / 2;
						mGoalAngle = mGoalAngle * 4 / 3;
						speedInit = speedInit * 3 / 4;
					} else if (mDifficulty == DIFFICULTY_HARD) {
						mFuel = mFuel * 7 / 8;
						mGoalWidth = mGoalWidth * 3 / 4;
						mGoalSpeed = mGoalSpeed * 7 / 8;
						speedInit = speedInit * 4 / 3;
					}

					// pick a convenient initial location for the lander sprite
					mX = mCanvasWidth / 2;
					mY = mCanvasHeight - mLanderHeight / 2;

					System.Random random = new System.Random ();

					// start with a little random motion
					mDY = random.Next() * -speedInit;
					mDX = random.Next() * 2 * speedInit - speedInit;
					mHeading = 0;

					// Figure initial spot for landing, not too near center
					while (true) {
						mGoalX = (int) (random.Next() * (mCanvasWidth - mGoalWidth));
						if (System.Math.Abs(mGoalX - (mX - mLanderWidth / 2)) > mCanvasHeight / 6)
							break;
					}

					mLastTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + 100;
					SetState (STATE_RUNNING);
				}
			}

			/**
			 * Pauses the physics update & animation.
			 */
			public void Pause ()
			{
				lock (mSurfaceHolder)
					if (mMode == STATE_RUNNING)
						SetState (STATE_PAUSE);
			}

			/**
			 * Restores game state from the indicated Bundle. Typically called when
			 * the Activity is being restored after having been previously
			 * destroyed.
			 * 
			 * @param savedState Bundle containing the game state
			 */
			public void RestoreState (Bundle savedState)
			{
				lock (mSurfaceHolder) {
					SetState (STATE_PAUSE);
					mRotating = 0;
					mEngineFiring = false;

					mDifficulty = savedState.GetInt(KEY_DIFFICULTY);
					mX = savedState.GetDouble(KEY_X);
					mY = savedState.GetDouble(KEY_Y);
					mDX = savedState.GetDouble(KEY_DX);
					mDY = savedState.GetDouble(KEY_DY);
					mHeading = savedState.GetDouble(KEY_HEADING);

					mLanderWidth = savedState.GetInt(KEY_LANDER_WIDTH);
					mLanderHeight = savedState.GetInt(KEY_LANDER_HEIGHT);
					mGoalX = savedState.GetInt(KEY_GOAL_X);
					mGoalSpeed = savedState.GetInt(KEY_GOAL_SPEED);
					mGoalAngle = savedState.GetInt(KEY_GOAL_ANGLE);
					mGoalWidth = savedState.GetInt(KEY_GOAL_WIDTH);
					mWinsInARow = savedState.GetInt(KEY_WINS);
					mFuel = savedState.GetDouble(KEY_FUEL);
				}
			}

			void Run ()
			{
				while (mRun) {
					Canvas c = null;
					try {
						c = mSurfaceHolder.LockCanvas (null);
						lock (mSurfaceHolder) {
							if (mMode == STATE_RUNNING) UpdatePhysics();
							DoDraw(c);
						}
					} finally {
						// do this in a finally so that if an exception is thrown
						// during the above, we don't leave the Surface in an
						// inconsistent state
						if (c != null) {
							mSurfaceHolder.UnlockCanvasAndPost(c);
						}
					}
				}
			}

			/**
			 * Dump game state to the provided Bundle. Typically called when the
			 * Activity is being suspended.
			 * 
			 * @return Bundle with this view's state
			 */
			public Bundle SaveState(Bundle map)
			{
				lock (mSurfaceHolder) {
					if (map != null) {
						map.PutInt(KEY_DIFFICULTY, mDifficulty);
						map.PutDouble(KEY_X, mX);
						map.PutDouble(KEY_Y, mY);
						map.PutDouble(KEY_DX, mDX);
						map.PutDouble(KEY_DY, mDY);
						map.PutDouble(KEY_HEADING, mHeading);
						map.PutInt(KEY_LANDER_WIDTH, mLanderWidth);
						map.PutInt(KEY_LANDER_HEIGHT, mLanderHeight);
						map.PutInt(KEY_GOAL_X, mGoalX);
						map.PutInt(KEY_GOAL_SPEED, mGoalSpeed);
						map.PutInt(KEY_GOAL_ANGLE, mGoalAngle);
						map.PutInt(KEY_GOAL_WIDTH, mGoalWidth);
						map.PutInt(KEY_WINS, mWinsInARow);
						map.PutDouble(KEY_FUEL, mFuel);
					}
				}
				return map;
			}

			/**
			 * Sets the current difficulty.
			 * 
			 * @param difficulty
			 */
			public void SetDifficulty (int difficulty)
			{
				lock (mSurfaceHolder) {
					mDifficulty = difficulty;
				}
			}

			/**
			 * Sets if the engine is currently firing.
			 */
			public void SetFiring (bool firing)
			{
				lock (mSurfaceHolder) {
					mEngineFiring = firing;
				}
			}

			/**
			 * Used to signal the thread whether it should be running or not.
			 * Passing true allows the thread to run; passing false will shut it
			 * down if it's already running. Calling start() after this was most
			 * recently called with false will result in an immediate shutdown.
			 * 
			 * @param b true to run, false to shut down
			 */
			public void SetRunning(bool b)
			{
				mRun = b;
			}

			/**
			 * Sets the game mode. That is, whether we are running, paused, in the
			 * failure state, in the victory state, etc.
			 * 
			 * @see #setState(int, CharSequence)
			 * @param mode one of the STATE_* constants
			 */
			public void SetState (int mode)
			{
				lock (mSurfaceHolder) {
					SetState(mode, null);
				}
			}

			/**
			 * Sets the game mode. That is, whether we are running, paused, in the
			 * failure state, in the victory state, etc.
			 * 
			 * @param mode one of the STATE_* constants
			 * @param message string to add to screen or null
			 */
			public void SetState(int mode, CharSequence message)
			{
				/*
				 * This method optionally can cause a text message to be displayed
				 * to the user when the mode changes. Since the View that actually
				 * renders that text is part of the main View hierarchy and not
				 * owned by this thread, we can't touch the state of that View.
				 * Instead we use a Message + Handler to relay commands to the main
				 * thread, which updates the user-text View.
				 */
				lock (mSurfaceHolder) {
					mMode = mode;

					if (mMode == STATE_RUNNING) {
						Message msg = mHandler.ObtainMessage();
						Bundle b = new Bundle();
						b.PutString("text", "");
						b.PutInt("viz", View.INVISIBLE);
						msg.Data = b;
						mHandler.SendMessage (msg);
					} else {
						mRotating = 0;
						mEngineFiring = false;
						Resources res = view.mContext.Resources;
						CharSequence str = "";
						if (mMode == STATE_READY)
							str = res.GetText(R.@string.mode_ready);
						else if (mMode == STATE_PAUSE)
							str = res.GetText(R.@string.mode_pause);
						else if (mMode == STATE_LOSE)
							str = res.GetText(R.@string.mode_lose);
						else if (mMode == STATE_WIN)
							str = res.GetString(R.@string.mode_win_prefix)
									+ mWinsInARow + " "
									+ res.GetString(R.@string.mode_win_suffix);

						if (message != null) {
							str = message + "\n" + str;
						}

						if (mMode == STATE_LOSE) mWinsInARow = 0;

						Message msg = mHandler.ObtainMessage();
						Bundle b = new Bundle();
						b.PutString("text", str.ToString());
						b.PutInt("viz", View.VISIBLE);
						msg.Data = b;
						mHandler.SendMessage(msg);
					}
				}
			}

			/* Callback invoked when the surface dimensions change. */
			public void SetSurfaceSize(int width, int height)
			{
				// synchronized to make sure these all change atomically
				lock (mSurfaceHolder) {
					mCanvasWidth = width;
					mCanvasHeight = height;

					// don't forget to resize the background image
					mBackgroundImage = Bitmap.CreateScaledBitmap(
							mBackgroundImage, width, height, true);
				}
			}

			/**
			 * Resumes from a pause.
			 */
			public void Unpause()
			{
				// Move the real time clock up to now
				lock (mSurfaceHolder) {
					mLastTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + 100;
				}
				SetState (STATE_RUNNING);
			}

			/**
			 * Handles a key-down event.
			 * 
			 * @param keyCode the key that was pressed
			 * @param msg the original event object
			 * @return true
			 */
			public bool DoKeyDown(int keyCode, KeyEvent msg)
			{
				lock (mSurfaceHolder) {
					bool okStart = false;
					if (keyCode == KeyEvent.KEYCODE_DPAD_UP) okStart = true;
					if (keyCode == KeyEvent.KEYCODE_DPAD_DOWN) okStart = true;
					if (keyCode == KeyEvent.KEYCODE_S) okStart = true;

					bool center = (keyCode == KeyEvent.KEYCODE_DPAD_UP);

					if (okStart && (mMode == STATE_READY || mMode == STATE_LOSE || mMode == STATE_WIN)) {
						// ready-to-start -> start
						DoStart();
						return true;
					} else if (mMode == STATE_PAUSE && okStart) {
						// paused -> running
						Unpause();
						return true;
					} else if (mMode == STATE_RUNNING) {
						// center/space -> fire
						if (keyCode == KeyEvent.KEYCODE_DPAD_CENTER
								|| keyCode == KeyEvent.KEYCODE_SPACE) {
							SetFiring(true);
							return true;
							// left/q -> left
						} else if (keyCode == KeyEvent.KEYCODE_DPAD_LEFT
								|| keyCode == KeyEvent.KEYCODE_Q) {
							mRotating = -1;
							return true;
							// right/w -> right
						} else if (keyCode == KeyEvent.KEYCODE_DPAD_RIGHT
								|| keyCode == KeyEvent.KEYCODE_W) {
							mRotating = 1;
							return true;
							// up -> pause
						} else if (keyCode == KeyEvent.KEYCODE_DPAD_UP) {
							Pause();
							return true;
						}
					}

					return false;
				}
			}

			/**
			 * Handles a key-up event.
			 * 
			 * @param keyCode the key that was pressed
			 * @param msg the original event object
			 * @return true if the key was handled and consumed, or else false
			 */
			public bool DoKeyUp(int keyCode, KeyEvent msg)
			{
				bool handled = false;

				lock (mSurfaceHolder) {
					if (mMode == STATE_RUNNING) {
						if (keyCode == KeyEvent.KEYCODE_DPAD_CENTER
								|| keyCode == KeyEvent.KEYCODE_SPACE) {
							SetFiring(false);
							handled = true;
						} else if (keyCode == KeyEvent.KEYCODE_DPAD_LEFT
								|| keyCode == KeyEvent.KEYCODE_Q
								|| keyCode == KeyEvent.KEYCODE_DPAD_RIGHT
								|| keyCode == KeyEvent.KEYCODE_W) {
							mRotating = 0;
							handled = true;
						}
					}
				}

				return handled;
			}

			/**
			 * Draws the ship, fuel/speed bars, and background to the provided
			 * Canvas.
			 */
			private void DoDraw(Canvas canvas)
			{
				// Draw the background image. Operations on the Canvas accumulate
				// so this is like clearing the screen.
				canvas.DrawBitmap(mBackgroundImage, 0, 0, null);

				int yTop = mCanvasHeight - ((int) mY + mLanderHeight / 2);
				int xLeft = (int) mX - mLanderWidth / 2;

				// Draw the fuel gauge
				int fuelWidth = (int) (UI_BAR * mFuel / PHYS_FUEL_MAX);
				mScratchRect.Set(4, 4, 4 + fuelWidth, 4 + UI_BAR_HEIGHT);
				canvas.DrawRect(mScratchRect, mLinePaint);

				// Draw the speed gauge, with a two-tone effect
				double speed = System.Math.Sqrt(mDX * mDX + mDY * mDY);
				int speedWidth = (int) (UI_BAR * speed / PHYS_SPEED_MAX);

				if (speed <= mGoalSpeed) {
					mScratchRect.Set(4 + UI_BAR + 4, 4,
							4 + UI_BAR + 4 + speedWidth, 4 + UI_BAR_HEIGHT);
					canvas.DrawRect(mScratchRect, mLinePaint);
				} else {
					// Draw the bad color in back, with the good color in front of
					// it
					mScratchRect.Set(4 + UI_BAR + 4, 4,
							4 + UI_BAR + 4 + speedWidth, 4 + UI_BAR_HEIGHT);
					canvas.DrawRect(mScratchRect, mLinePaintBad);
					int goalWidth = (UI_BAR * mGoalSpeed / PHYS_SPEED_MAX);
					mScratchRect.Set(4 + UI_BAR + 4, 4, 4 + UI_BAR + 4 + goalWidth,
							4 + UI_BAR_HEIGHT);
					canvas.DrawRect(mScratchRect, mLinePaint);
				}

				// Draw the landing pad
				canvas.DrawLine(mGoalX, 1 + mCanvasHeight - TARGET_PAD_HEIGHT,
						mGoalX + mGoalWidth, 1 + mCanvasHeight - TARGET_PAD_HEIGHT,
						mLinePaint);


				// Draw the ship with its current rotation
				canvas.Save();
				canvas.Rotate((float) mHeading, (float) mX, mCanvasHeight
						- (float) mY);
				if (mMode == STATE_LOSE) {
					mCrashedImage.SetBounds(xLeft, yTop, xLeft + mLanderWidth, yTop
							+ mLanderHeight);
					mCrashedImage.Draw(canvas);
				} else if (mEngineFiring) {
					mFiringImage.SetBounds(xLeft, yTop, xLeft + mLanderWidth, yTop
							+ mLanderHeight);
					mFiringImage.Draw(canvas);
				} else {
					mLanderImage.SetBounds(xLeft, yTop, xLeft + mLanderWidth, yTop
							+ mLanderHeight);
					mLanderImage.Draw(canvas);
				}
				canvas.Restore();
			}

			/**
			 * Figures the lander state (x, y, fuel, ...) based on the passage of
			 * realtime. Does not invalidate(). Called at the start of draw().
			 * Detects the end-of-game and sets the UI to the next state.
			 */
			private void UpdatePhysics()
			{
				long now = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

				// Do nothing if mLastTime is in the future.
				// This allows the game-start to delay the start of the physics
				// by 100ms or whatever.
				if (mLastTime > now) return;

				double elapsed = (now - mLastTime) / 1000.0;

				// mRotating -- update heading
				if (mRotating != 0) {
					mHeading += mRotating * (PHYS_SLEW_SEC * elapsed);

					// Bring things back into the range 0..360
					if (mHeading < 0)
						mHeading += 360;
					else if (mHeading >= 360) mHeading -= 360;
				}

				// Base accelerations -- 0 for x, gravity for y
				double ddx = 0.0;
				double ddy = -PHYS_DOWN_ACCEL_SEC * elapsed;

				if (mEngineFiring) {
					// taking 0 as up, 90 as to the right
					// cos(deg) is ddy component, sin(deg) is ddx component
					double elapsedFiring = elapsed;
					double fuelUsed = elapsedFiring * PHYS_FUEL_SEC;

					// tricky case where we run out of fuel partway through the
					// elapsed
					if (fuelUsed > mFuel) {
						elapsedFiring = mFuel / fuelUsed * elapsed;
						fuelUsed = mFuel;

						// Oddball case where we adjust the "control" from here
						mEngineFiring = false;
					}

					mFuel -= fuelUsed;

					// have this much acceleration from the engine
					double accel = PHYS_FIRE_ACCEL_SEC * elapsedFiring;

					double radians = 2 * System.Math.PI * mHeading / 360;
					ddx = System.Math.Sin(radians) * accel;
					ddy += System.Math.Cos(radians) * accel;
				}

				double dxOld = mDX;
				double dyOld = mDY;

				// figure speeds for the end of the period
				mDX += ddx;
				mDY += ddy;

				// figure position based on average speed during the period
				mX += elapsed * (mDX + dxOld) / 2;
				mY += elapsed * (mDY + dyOld) / 2;

				mLastTime = now;

				// Evaluate if we have landed ... stop the game
				double yLowerBound = TARGET_PAD_HEIGHT + mLanderHeight / 2
						- TARGET_BOTTOM_PADDING;
				if (mY <= yLowerBound) {
					mY = yLowerBound;

					int result = STATE_LOSE;
					CharSequence message = "";
					Resources res = view.mContext.Resources;
					double speed = System.Math.Sqrt(mDX * mDX + mDY * mDY);
					bool onGoal = (mGoalX <= mX - mLanderWidth / 2 && mX
							+ mLanderWidth / 2 <= mGoalX + mGoalWidth);

					// "Hyperspace" win -- upside down, going fast,
					// puts you back at the top.
					if (onGoal && System.Math.Abs(mHeading - 180) < mGoalAngle
							&& speed > PHYS_SPEED_HYPERSPACE) {
						result = STATE_WIN;
						mWinsInARow++;
						DoStart();

						return;
						// Oddball case: this case does a return, all other cases
						// fall through to setMode() below.
					} else if (!onGoal) {
						message = res.GetText(R.@string.message_off_pad);
					} else if (!(mHeading <= mGoalAngle || mHeading >= 360 - mGoalAngle)) {
						message = res.GetText(R.@string.message_bad_angle);
					} else if (speed > mGoalSpeed) {
						message = res.GetText(R.@string.message_too_fast);
					} else {
						result = STATE_WIN;
						mWinsInARow++;
					}

					SetState(result, message);
				}
			}
		}

	}



}
