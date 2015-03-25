using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace MonoDroid.ApiDemo
{
	/**
 	* This activity demonstrates how to use the system UI flags to
 	* implement an immersive game.
 	*/
	[Activity (Label = "Views/System UI Visibility/Game", 
		Name = "monodroid.apidemo.GameActivity",
		Theme = "@android:style/Theme.Holo.NoActionBar")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class GameActivity : Activity
	{
		/**
     	* Implementation of a view for the game, filling the entire screen.
     	*/
		[Register ("monodroid.apidemo.GameActivity_Content")]
		public class Content : TouchPaint.PaintView, View.IOnSystemUiVisibilityChangeListener, View.IOnClickListener
		{
			Button mPlayButton;
			bool mPaused;
			StatusBarVisibility mLastSystemUiVis;
			bool mUpdateSystemUi;

			Action mFader;

			public void Run ()
			{
				Fade ();
				if (mUpdateSystemUi) {
					UpdateNavVisibility ();
				}
				if (!mPaused) {
					Handler.PostDelayed (mFader, 1000 / 30);
				}
			}

			public Content (Context context, IAttributeSet attrs) : base (context, attrs)
			{
				SetOnSystemUiVisibilityChangeListener (this);
			}

			public void Init (Activity activity, Button playButton)
			{
				// This called by the containing activity to supply the surrounding
				// state of the game that it will interact with.
				mPlayButton = playButton;
				mPlayButton.SetOnClickListener (this);
				SetGamePaused (true);

				mFader = () => Run ();
			}

			public void OnSystemUiVisibilityChange (StatusBarVisibility visibility)
			{
				// Detect when we go out of nav-hidden mode, to reset back to having
				// it hidden; our game wants those elements to stay hidden as long
				// as it is being played and stay shown when paused.
				StatusBarVisibility diff = mLastSystemUiVis ^ visibility;
				mLastSystemUiVis = visibility;
				if (!mPaused && ((int)diff & (int)SystemUiFlags.HideNavigation) != 0
				    && ((int)visibility & (int)SystemUiFlags.HideNavigation) == 0) {
					// We are running and the system UI navigation has become
					// shown...  we want it to remain hidden, so update our system
					// UI state at the next game loop.
					mUpdateSystemUi = true;
				}
			}

			protected override void OnWindowVisibilityChanged (ViewStates visibility)
			{
				base.OnWindowVisibilityChanged (visibility);

				// When we become visible or invisible, play is paused.
				SetGamePaused (true);
			}

			public override void OnWindowFocusChanged (bool hasWindowFocus)
			{
				base.OnWindowFocusChanged (hasWindowFocus);

				// When we become visible or invisible, play is paused.
				// Optional: pause game when window loses focus.  This will cause it to
				// pause, for example, when the notification shade is pulled down.
				if (!hasWindowFocus) {
					SetGamePaused (true);
				}
			}

			public void OnClick (View v)
			{
				if (v == mPlayButton) {
					// Clicking on the play/pause button toggles its state.
					SetGamePaused (!mPaused);
				}
			}

			public void SetGamePaused (bool paused)
			{
				mPaused = paused;
				mPlayButton.SetText (paused ? Resource.String.play : Resource.String.pause);
				KeepScreenOn = !paused;
				UpdateNavVisibility ();
				if (Handler != null) {
					Handler.RemoveCallbacks (mFader);
					if (!paused) {
						mFader ();
						Text ("Draw!");
					}
				}
			}

			void UpdateNavVisibility ()
			{
				var newVis = SystemUiFlags.LayoutFullscreen
					| SystemUiFlags.LayoutHideNavigation
					| SystemUiFlags.LayoutStable;
				if (!mPaused) {
					newVis |= SystemUiFlags.LowProfile | SystemUiFlags.Fullscreen
						| SystemUiFlags.HideNavigation  | SystemUiFlags.ImmersiveSticky;
				}

				// Set the new desired visibility.
				SystemUiVisibility = (StatusBarVisibility)newVis;
				mUpdateSystemUi = false;
			}
		}
			
		Content mContent;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.game);
			mContent = FindViewById <Content> (Resource.Id.content);
			mContent.Init (this, FindViewById <Button> (Resource.Id.play));
		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Unpause game when its activity is resumed.
			mContent.SetGamePaused (false);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Pause game when its activity is paused.
			mContent.SetGamePaused (true);
		}
	}
}

