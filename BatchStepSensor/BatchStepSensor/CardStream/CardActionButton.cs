
using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Views;
using Android.Views.Animations;

namespace BatchStepSensor.CardStream
{
	/// <summary>
	/// Custom Button with a special 'pressed' effect for touch events.
	/// </summary>
	public class CardActionButton : Button
	{
		public CardActionButton(Context context)
			:base(context)
		{

		}

		public CardActionButton(Context context, IAttributeSet attrs)
			:base(context, attrs)
		{

		}

		public CardActionButton(Context context, IAttributeSet attrs, int defStyle)
			:base(context, attrs, defStyle)
		{

		}

		public override bool OnTouchEvent (Android.Views.MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				Pressed = true;
				Animate ().ScaleX (0.98f).ScaleY (0.98f).Alpha (0.8f).SetDuration (100).SetInterpolator (new DecelerateInterpolator ());
				break;
			case MotionEventActions.Up:
				Animate ().ScaleX (1.0f).ScaleY (1.0f).Alpha (1.0f).SetDuration (50).SetInterpolator (new BounceInterpolator ());
				break;
			case MotionEventActions.Cancel:
				Animate().ScaleX(1.0f).ScaleY(1.0f).Alpha(1.0f).SetDuration(50).
				SetInterpolator(new BounceInterpolator());
				break;
			}
			return base.OnTouchEvent (e);
		}
	}
}

