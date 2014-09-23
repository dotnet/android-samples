using System;
using CommonSampleLibrary;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FloatingActionButtonBasic
{
	public class FloatingActionButton : FrameLayout,ICheckable
	{
		// An array of states
		private int[] CHECKED_STATE_SET = {Android.Resource.Attribute.StateChecked};

		private const string TAG = "FloatingActionButton";

		// A bool that tells if the FAB is checked or not
		protected bool check ;

		// The View that is revealed
		protected View revealView;

		// The coordinates of a touch action
		protected Point touchPoint;

		// A GestureDetector to detect touch actions
		private GestureDetector gestureDetector;

		// A listener to communicate that the FAB has changed states
		private IOnCheckedChangeListener onCheckedChangeListener;

		public bool Checked {
			get{ return check; }
			set{ check = value; }
		}

		public FloatingActionButton(IntPtr a, Android.Runtime.JniHandleOwnership b) : base(a,b)
		{
		}

		public FloatingActionButton (Context context) : this(context,null,0,0)
		{
		}

		public FloatingActionButton(Context context, IAttributeSet attrs) : this(context,attrs,0,0)
		{
		}

		public FloatingActionButton(Context context, IAttributeSet attrs, int defStyleAttr) : this(context,attrs,defStyleAttr,0)
		{
		}

		public FloatingActionButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,attrs,defStyleAttr,defStyleRes)
		{
			// When a view is clickable it will change its state to "pressed" on every click.
			Clickable = true;

			// Create a GestureDetector to detect single taps
			gestureDetector = new GestureDetector (context, new MySimpleOnGestureListener (this));

			//A new View is created
			revealView = new View (context);
			AddView (revealView, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		}

		private class MySimpleOnGestureListener : GestureDetector.SimpleOnGestureListener
		{
			FloatingActionButton b;
			public MySimpleOnGestureListener(FloatingActionButton bu)
			{
				b = bu;
			}

			public override bool OnSingleTapConfirmed (MotionEvent e)
			{
				b.touchPoint = new Point ((int)e.GetX (), (int)e.GetY ());
				CommonSampleLibrary.Log.Debug (TAG, "Single tap captured.");
				b.Toggle ();
				return true;
			}
		}

		// Sets the checked/unchecked state of the FAB.
		public void SetChecked(bool check)
		{
			// If trying to change to the current state, ignore
			if(check== this.check)
				return;
			this.check = check;

			// Create and start the ValueAnimator that shows the new state
			ValueAnimator anim = CreateAnimator ();
			anim.SetDuration( Resources.GetInteger (Android.Resource.Integer.ConfigShortAnimTime));
			anim.Start ();

			// Set the new background color of the View to be revealed
			revealView.SetBackgroundColor (check ? Resources.GetColor (Resource.Color.fab_color_2)
				: Resources.GetColor (Resource.Color.fab_color_1));

			//Show the View to be revealed. Note that the animation has started already.
			revealView.Visibility = ViewStates.Visible;

			if (onCheckedChangeListener != null)
				onCheckedChangeListener.OnCheckedChanged (this, check);
		}

		public void SetOnCheckedChangeListener(IOnCheckedChangeListener listener)
		{
			onCheckedChangeListener = listener;
			Clickable = listener != null;
		}

		// Interface of a callback to be invoked when the checked state of a compound button changes.
		public interface IOnCheckedChangeListener
		{
			void OnCheckedChanged (FloatingActionButton fabView, bool isChecked);
		}

		protected ValueAnimator CreateAnimator() 
		{
			// Calculate the longest distance from the hot spot to the edge of the circle.
			int endRadius = Width / 2 + ((int)Math.Sqrt (Math.Pow (Width / 2 - touchPoint.Y, 2)
				+ Math.Pow (Width / 2 - touchPoint.X,2)));

			// Make sure the touch point is defined or set it to the middle of the view.
			if (touchPoint == null)
				touchPoint = new Point (Width / 2, Height / 2);

			ValueAnimator anim = ViewAnimationUtils.CreateCircularReveal (revealView, touchPoint.X, touchPoint.Y, 0, endRadius);
			anim.AddListener (new MyAnimatorListenerAdapter (this));
			return anim;

		}

		private class MyAnimatorListenerAdapter: AnimatorListenerAdapter
		{
			FloatingActionButton b;
			public MyAnimatorListenerAdapter(FloatingActionButton bu)
			{
				b = bu;
			}
			public override void OnAnimationEnd (Animator animation)
			{
				b.RefreshDrawableState ();

				b.revealView.Visibility = ViewStates.Gone;
				// Reset the touch point as the next call to setChecked might not come
				// from a tap.
				b.touchPoint = null;
			}
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			if (gestureDetector.OnTouchEvent (e))
				return true;

			return base.OnTouchEvent (e);
		}

		public bool IsChecked()
		{
			return check;
		}

		public void Toggle()
		{
			SetChecked (!check);
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);

			var outline = new Outline ();
			outline.SetOval (0, 0, w, h);
			SetOutline (outline);
			ClipToOutline = true;
		}

		protected override int[] OnCreateDrawableState (int extraSpace)
		{
			int[] drawableState = base.OnCreateDrawableState (extraSpace + 1);
			if (IsChecked ())
				MergeDrawableStates (drawableState, CHECKED_STATE_SET);

			return drawableState;
		}


	}
}

