using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;

namespace WatchViewStubSample
{
	[Activity (Label = "WatchViewStub", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, Android.Support.Wearable.Views.WatchViewStub.IOnLayoutInflatedListener
	{
		RelativeLayout rectBackground, roundBackground;

		GestureDetectorCompat gestureDetector;
		DismissOverlayView dismissOverlayView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_activity);

			WatchViewStub stub = (WatchViewStub)FindViewById (Resource.Id.stub);
			stub.SetOnLayoutInflatedListener (this);

			dismissOverlayView = (DismissOverlayView)FindViewById (Resource.Id.dismiss_overlay);
			gestureDetector = new GestureDetectorCompat (this, new LongPressListener (this));
		}

		public void OnLayoutInflated (WatchViewStub stub)
		{
			rectBackground = (RelativeLayout)FindViewById (Resource.Id.rect_layout);
			roundBackground = (RelativeLayout)FindViewById (Resource.Id.round_layout);
		}

		/// <summary>
		/// Animates the layout when clicked. The animation depends on whether the device is round or rectangular.
		/// </summary>
		/// <param name="view">View.</param>
		[Export("onLayoutClicked")]
		public void OnLayoutClicked(View view)
		{
			if (rectBackground != null) {
				ScaleAnimation scaleAnimation = new ScaleAnimation (1, 0.7f, 1, 0.7f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
				scaleAnimation.Duration = 300;
				scaleAnimation.RepeatCount = 1;
				scaleAnimation.RepeatMode = RepeatMode.Reverse;
				rectBackground.StartAnimation (scaleAnimation);
			}
			if (roundBackground != null) {
				roundBackground.Animate ().RotationBy (360).SetDuration (300).Start ();
			}
		}

		public override bool DispatchTouchEvent (MotionEvent ev)
		{
			return gestureDetector.OnTouchEvent (ev) || base.DispatchTouchEvent (ev);
		}

		private class LongPressListener : GestureDetector.SimpleOnGestureListener {
			MainActivity activity;
			public LongPressListener(MainActivity activity)
			{
				this.activity = activity;
			}
			public override void OnLongPress (MotionEvent e)
			{
				base.OnLongPress (e);
				if (activity != null) {
					activity.dismissOverlayView.Show ();
				}
			}
		}
	}


}


