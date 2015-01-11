using System;
using Android.Views;
using Android.Animation;
using Android.Graphics;
using Android.Views.Animations;
using Android.Content;
using Java.Interop;

namespace BatchStepSensor.CardStream
{
	public class DefaultCardStreamAnimator : CardStreamAnimator
	{
		public override ObjectAnimator GetDisappearingAnimator (Android.Content.Context context)
		{
			ObjectAnimator animator = ObjectAnimator.OfPropertyValuesHolder (
				                          PropertyValuesHolder.OfFloat ("alpha", 1f, 0f),
				                          PropertyValuesHolder.OfFloat ("scaleX", 1f, 0f),
				                          PropertyValuesHolder.OfFloat ("scaleY", 1f, 0f),
				                          PropertyValuesHolder.OfFloat ("rotation", 0f, 270f));
			animator.SetDuration ((long)(200 * mSpeedFactor));
			return animator;
		}

		public override ObjectAnimator GetAppearingAnimator (Android.Content.Context context)
		{
			Point outPoint = new Point ();
			IWindowManager wm = context.GetSystemService (Context.WindowService).JavaCast<IWindowManager>();
			wm.DefaultDisplay.GetSize (outPoint);

			ObjectAnimator animator = ObjectAnimator.OfPropertyValuesHolder (
				                          PropertyValuesHolder.OfFloat ("alpha", 0f, 1f),
				                          PropertyValuesHolder.OfFloat ("translationY", outPoint.Y / 2f, 0f),
				                          PropertyValuesHolder.OfFloat ("rotation", -45f, 0f));

			animator.SetDuration ((long)(200 * mSpeedFactor));
			return animator;
		}

		public override ObjectAnimator GetInitialAnimator (Android.Content.Context context)
		{
			ObjectAnimator animator = ObjectAnimator.OfPropertyValuesHolder (
				                          PropertyValuesHolder.OfFloat ("alpha", 0.5f, 1f),
				                          PropertyValuesHolder.OfFloat ("rotation", 60f, 0f));
			animator.SetDuration ((long)(200 * mSpeedFactor));
			return animator;
		}

		public override ObjectAnimator GetSwipeInAnimator (View view, float deltaX, float deltaY)
		{
			float deltaXAbs = Math.Abs (deltaX);
			float fractionCovered = 1f - (deltaXAbs / view.Width);
			long duration = Math.Abs ((int)((1 - fractionCovered) * 200 * mSpeedFactor));

			// Animate position and alpha of swiped item

			ObjectAnimator animator = ObjectAnimator.OfPropertyValuesHolder (view,
				                          PropertyValuesHolder.OfFloat ("alpha", 1f),
				                          PropertyValuesHolder.OfFloat ("translationX", 0f),
				                          PropertyValuesHolder.OfFloat ("rotationY", 0f));

			animator.SetDuration (duration).SetInterpolator (new BounceInterpolator ());

			return animator;
		}

		public override ObjectAnimator GetSwipeOutAnimator (View view, float deltaX, float deltaY)
		{
			float endX, endRotationY;

			float deltaXAbs = Math.Abs (deltaX);

			float fractionCovered = 1f - (deltaXAbs / view.Width);
			long duration = Math.Abs ((int)((1 - fractionCovered) * 200 * mSpeedFactor));

			endX = deltaX < 0 ? -view.Width : view.Width;
			if (deltaX > 0)
				endRotationY = -15f;
			else
				endRotationY = 15f;

			// Animate position and alpha of swiped item
			return (ObjectAnimator)ObjectAnimator.OfPropertyValuesHolder (view,
				PropertyValuesHolder.OfFloat ("alpha", 0f),
				PropertyValuesHolder.OfFloat ("translationX", endX),
				PropertyValuesHolder.OfFloat ("rotationY", endRotationY)).SetDuration (duration);
		}
	}
}

