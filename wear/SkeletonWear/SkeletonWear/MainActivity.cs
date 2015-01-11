using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;

using Export = Java.Interop.ExportAttribute;

namespace SkeletonWear
{
	[Activity (Label = "SkeletonWear", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	[IntentFilter (new string[]{ "skeletonWear.GridExampleActivity" }, 
		Categories = new string[]{ "android.intent.category.DEFAULT" })]
	public class MainActivity : Activity, DelayedConfirmationView.IDelayedConfirmationListener
	{
		private static string TAG = "MainActivity";

		private static int NOTIFICATION_ID = 1;
		private static int NOTIFICATION_REQUEST_CODE = 1;
		private static int NUM_SECONDS = 5;

		private GestureDetectorCompat mGestureDetector;
		internal DismissOverlayView mDismissOverlayView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_activity);

			mDismissOverlayView = FindViewById<DismissOverlayView> (Resource.Id.dismiss_overlay);
			mDismissOverlayView.SetIntroText (Resource.String.intro_text);
			mDismissOverlayView.ShowIntroIfNecessary ();
			mGestureDetector = new GestureDetectorCompat (this, new LongPressListener (this));
		}

		public override bool DispatchTouchEvent (MotionEvent ev)
		{
			return mGestureDetector.OnTouchEvent (ev) || base.DispatchTouchEvent (ev);
		}

		/**
		* Handles the button to launch a notification.
		*/
		[Export ("showNotification")]
		public void ShowNotification (View view)
		{
			var notification = new NotificationCompat.Builder (this)
				.SetContentTitle (GetString (Resource.String.notification_title))
				.SetContentText (GetString (Resource.String.notification_title))
				.SetSmallIcon (Resource.Drawable.ic_launcher)
				.AddAction (Resource.Drawable.ic_launcher,
				                   GetText (Resource.String.action_launch_activity),
				                   PendingIntent.GetActivity (this, NOTIFICATION_REQUEST_CODE,
					                   new Intent (this, typeof(GridExampleActivity)),
					                   PendingIntentFlags.UpdateCurrent))
				.Build ();
			NotificationManagerCompat.From (this).Notify (NOTIFICATION_ID, notification);
			Finish ();
		}

		/**
	 * Handles the button press to finish this activity and take the user back to the Home.
	 */
		[Export ("onFinishActivity")]
		public void OnFinishActivity (View v)
		{
			SetResult (Result.Ok);
			Finish ();
		}

		/**
	 * Handles the button to start a DelayedConfirmationView timer.
	 */
		[Export ("onStartTimer")]
		public void OnStartTimer (View v)
		{
			var delayedConfirmationView = FindViewById<DelayedConfirmationView> (Resource.Id.timer);
			delayedConfirmationView.SetTotalTimeMs (NUM_SECONDS * 1000);
			delayedConfirmationView.SetListener (this);
			delayedConfirmationView.Start ();
			Scroll (FocusSearchDirection.Down);
		}

		public void OnTimerSelected (View v)
		{
			Log.Debug (TAG, "OnTimerSelected called");
			Scroll (FocusSearchDirection.Up);
		}

		public void OnTimerFinished (View v)
		{
			Log.Debug (TAG, "OnTimerFinished called");
			Scroll (FocusSearchDirection.Up);
		}

		private void Scroll (FocusSearchDirection scrollDirection)
		{
			var scrollView = FindViewById<ScrollView> (Resource.Id.scroll);
			scrollView.Post (() => {
				scrollView.FullScroll (scrollDirection);
			});
		}
	}

	internal class LongPressListener : GestureDetector.SimpleOnGestureListener
	{
		MainActivity owner;

		public LongPressListener (MainActivity owner)
		{
			this.owner = owner;
		}

		public override void OnLongPress (MotionEvent e)
		{
			owner.mDismissOverlayView.Show ();
		}
	}
}


