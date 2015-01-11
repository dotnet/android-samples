using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace BatchStepSensor.CardStream
{

	/// <summary>
	/// Custom Button with a special 'pressed' effect for touch events
	/// </summary>
	public class CardLayout : RelativeLayout
	{
		private bool mSwiping = false;
		private float mDownX = 0f;
		private float mDownY = 0f;
		private float mTouchSlop = 0f;

		public CardLayout(Context context)
			:base(context)
		{
			init ();
		}

		public CardLayout(Context context, IAttributeSet attrs)
			:base(context, attrs)
		{
			init ();
		}
		public CardLayout(Context context, IAttributeSet attrs, int defStyle)
			:base(context, attrs, defStyle)
		{
			init ();
		}

		private void init()
		{
			Focusable = true;
			DescendantFocusability = DescendantFocusability.AfterDescendants;
			SetWillNotDraw (false);
			Clickable = true;

			mTouchSlop = ViewConfiguration.Get (Context).ScaledTouchSlop * 2f;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Cancel:
				break;
			case MotionEventActions.Up:
				mSwiping = false;
				break;
			}
			return base.OnTouchEvent (e);
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			switch (ev.Action) {
			case MotionEventActions.Move:
				if (!mSwiping) {
					mSwiping = Math.Abs (mDownX - ev.GetX ()) > mTouchSlop;
				}
				break;
			case MotionEventActions.Down:
				mDownX = ev.GetX ();
				mDownY = ev.GetY ();
				mSwiping = false;
				break;
			case MotionEventActions.Cancel:
				break;
			case MotionEventActions.Up:
				mSwiping = false;
				break;
			}
			return mSwiping;
		}
	}
}

