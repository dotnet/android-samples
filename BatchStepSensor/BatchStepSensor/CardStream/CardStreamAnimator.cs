using System;
using Android.Animation;
using Android.Content;
using Android.Views;

namespace BatchStepSensor.CardStream
{

	/// <summary>
	/// An abstract class which defines animators for CardStreamLinearLayout
	/// </summary>
	public abstract class CardStreamAnimator : Java.Lang.Object
	{
		protected float mSpeedFactor = 1f;

		/// <summary>
		/// Gets or sets the speed factor of animations. Higher value means longer duration & slow animation.
		/// </summary>
		/// <value>speed type 1: SLOW, 2: NORMAL, 3: FAST</value>
		public float SpeedFactor
		{
			set {
				mSpeedFactor = value;
			}
			get {
				return mSpeedFactor;
			}
		}

		/// <summary>
		/// Define initial animation of each child which fires when the rotates the screen.
		/// </summary>
		/// <returns>TObjectAnimation for initial animation</returns>
		/// <param name="context"></param>
		public abstract ObjectAnimator GetInitialAnimator(Context context);

		/// <summary>
		/// Define disappearing animation of a child which is fired when a view is removed programmatically
		/// </summary>
		/// <returns>ObjectAnimator for disappearing animation</returns>
		/// <param name="context"></param>
		public abstract ObjectAnimator GetDisappearingAnimator(Context context);

		/// <summary>
		/// Define appearing animation of a child which is fired when a view is added programmatically
		/// </summary>
		/// <returns>ObjectAnimation for appearing animation</returns>
		/// <param name="context">Context.</param>
		public abstract ObjectAnimator GetAppearingAnimator (Context context);

		/// <summary>
		/// Define swipe-in (back to the original position) animation of a child which is fired when a view is not moved enough to be removed.
		/// </summary>
		/// <returns>ObjectAnimator for swipe-in animation</returns>
		/// <param name="view"></param>
		/// <param name="deltaX"></param>
		/// <param name="deltaY"></param>
		public abstract ObjectAnimator GetSwipeInAnimator(View view, float deltaX, float deltaY);

		/// <summary>
		/// Define swipe-out animation of a child which is fired when a view is removed by a user swipe action
		/// </summary>
		/// <returns>ObjectAnimator for swipe-out animation</returns>
		/// <param name="view">View.</param>
		/// <param name="deltaX">Delta x.</param>
		/// <param name="deltaY">Delta y.</param>
		public abstract ObjectAnimator GetSwipeOutAnimator(View view, float deltaX, float deltaY);

		public class EmptyAnimator : CardStreamAnimator
		{
			#region implemented abstract members of CardStreamAnimator

			public override ObjectAnimator GetInitialAnimator (Context context)
			{
				return null;
			}

			public override ObjectAnimator GetDisappearingAnimator (Context context)
			{
				return null;
			}

			public override ObjectAnimator GetAppearingAnimator (Context context)
			{
				return null;
			}

			public override ObjectAnimator GetSwipeInAnimator (View view, float deltaX, float deltaY)
			{
				return null;
			}

			public override ObjectAnimator GetSwipeOutAnimator (View view, float deltaX, float deltaY)
			{
				return null;
			}

			#endregion


		}
	}
}

