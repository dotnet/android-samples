namespace TouchWalkthrough
{
	using System;

	using Android.App;
	using Android.OS;
	using Android.Views;
	using Android.Widget;

	[Activity(Label = "@string/activity_touch_sample")]
	public class TouchActivity : Activity
	{
		private TextView _touchInfoTextView;
		private ImageView _touchMeImageView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.touch_layout);
			_touchInfoTextView = FindViewById<TextView> (Resource.Id.touchInfoTextView);
			_touchMeImageView = FindViewById<ImageView> (Resource.Id.touchImageView);

			_touchMeImageView.Touch += TouchMeImageViewOnTouch;
		}

		private void TouchMeImageViewOnTouch (object sender, View.TouchEventArgs touchEventArgs)
		{
			string message;
			switch (touchEventArgs.Event.Action & MotionEventActions.Mask) {
			case MotionEventActions.Down:
			case MotionEventActions.Move:
                    // Handle both the Down and Move actions.
				message = "Touch Begins";
				break;

			case MotionEventActions.Up:
				message = "Touch Ends";
				break;
                
			default:
				message = string.Empty;
				break;
			}

			_touchInfoTextView.Text = message;
		}
	}
}
