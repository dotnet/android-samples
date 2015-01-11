
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Animation;
using Android.Content.Res;
using System.Threading;

namespace BatchStepSensor.CardStream
{
	public class CardStreamLinearLayout : LinearLayout
	{
		public const int ANIMATION_SPEED_SLOW = 1001;
		public const int ANIMATION_SPEED_NORMAL = 1002;
		public const int ANIMATION_SPEED_FAST = 1003;

		private const String TAG = "CardStreamLinearLayout";
		private readonly List<View> mFixedViewList = new List<View>();
		private readonly Rect mChildRect = new Rect();
		private CardStreamAnimator mAnimators;
		private OnDismissListener mDismissListener = null;
		private bool mLayouted = false;
		private bool mSwiping = false;
		private String mFirstVisibleCardTag = null;
		private bool mShowInitialAnimation = false;

		/// <summary>
		/// Handle touch events to fade / move dragged items as they are swiped out
		/// </summary>
		private OnTouchListener mTouchListener;
		private class OnTouchListener : Java.Lang.Object, View.IOnTouchListener
		{
			private float mDownX, mDownY;
			CardStreamLinearLayout layout;
			public OnTouchListener(CardStreamLinearLayout Layout)
			{
				layout = Layout;
			}
			public bool OnTouch (View v, MotionEvent e)
			{
				switch (e.Action) {
				case MotionEventActions.Down:
					mDownX = e.GetX ();
					mDownY = e.GetY ();
					break;
				case MotionEventActions.Cancel:
					layout.ResetAnimatedView (v);
					layout.mSwiping = false;
					mDownX = 0;
					mDownY = 0;
					break;
				case MotionEventActions.Move:
					{
						float x = e.GetX () + v.TranslationX;
						float y = e.GetY () + v.TranslationY;

						mDownX = mDownX == 0 ? x : mDownX;
						mDownY = mDownY == 0 ? y : mDownY;

						float deltaX = x - mDownX;
						float deltaY = y - mDownY;

						if (!layout.mSwiping && layout.IsSwiping (deltaX, deltaY)) {
							layout.mSwiping = true;
							v.Parent.RequestDisallowInterceptTouchEvent (true);
						} else {
							layout.SwipeView (v, deltaX, deltaY);
						}
					}
					break;
				case MotionEventActions.Up:
					{
						// User let go - figure out whether to animate the view out or back into place
						if (layout.mSwiping) {
							float x = e.GetX () + v.TranslationX;
							float y = e.GetY () + v.TranslationY;

							float deltaX = x - mDownX;
							float deltaY = y - mDownY;
							float deltaXAbs = Math.Abs(deltaX);

							bool remove = deltaXAbs > v.Width / 4 && !layout.IsFixedView(v);
							if (remove)
							{
								layout.HandleViewSwipingOut(v, deltaX, deltaY);
							}
							else 
							{
								layout.HandleViewSwipingIn(v, deltaX, deltaY);
							}
						}
						mDownX = 0;
						mDownY = 0;
						layout.mSwiping = false;
					}
					break;
					default:
					return false;
				}
				return false;
			}
		}

		private int mSwipeSlop = -1;

		private TransitionListener mTransitionListener;

		private OnHierarchyChangeListener mOnHieratchyChangeListener;

		private int mLastDownX;

		public CardStreamLinearLayout (Context context) :
			base (context)
		{
			Initialize (null, 0);
		}

		public CardStreamLinearLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize (attrs, 0);
		}

		public CardStreamLinearLayout (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize (attrs, defStyle);
		}

		/// <summary>
		/// Add a card view with canDismiss flag
		/// </summary>
		/// <param name="cardView">A card view</param>
		/// <param name="canDismiss">IFlag to indicated whether or not this card is dismissible</param>
		public void AddCard (View cardView, Boolean canDismiss)
		{
			if (cardView.Parent == null) {
				InitCard (cardView, canDismiss);

				ViewGroup.LayoutParams param = cardView.LayoutParameters;
				if (param == null)
					param = GenerateDefaultLayoutParams ();

				base.AddView (cardView, -1, param);
			}
		}

		public override void AddView(View child, int index, ViewGroup.LayoutParams param)
		{
			if (child.Parent == null) {
				InitCard (child, true);
				base.AddView (child, index, param);
			}
		}



		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			Log.Debug (TAG, "OnLayout: " + changed);

			if (changed && !mLayouted) {
				mLayouted = true;

				ObjectAnimator animator;
				LayoutTransition layoutTransition = new LayoutTransition ();

				animator = mAnimators.GetDisappearingAnimator (Context);
				layoutTransition.SetAnimator (LayoutTransitionType.Disappearing, animator);

				animator = mAnimators.GetAppearingAnimator (Context);
				layoutTransition.SetAnimator (LayoutTransitionType.Appearing, animator);

				layoutTransition.AddTransitionListener (mTransitionListener);

				if (animator != null)
					layoutTransition.SetDuration (animator.Duration);

				LayoutTransition = layoutTransition;

				if (mShowInitialAnimation)
					RunInitialAnimations ();

				if (mFirstVisibleCardTag != null) {
					ScrollToCard (mFirstVisibleCardTag);
					mFirstVisibleCardTag = null;
				}
			}
		}

		/// <summary>
		/// Check whether a user moved enough distance to start a swipe action or not.
		/// </summary>
		/// <returns><c>true</c> if the user is swiping</returns>
		/// <param name="deltaX"></param>
		/// <param name="deltaY"></param>
		protected bool IsSwiping (float deltaX, float deltaY)
		{
			if (mSwipeSlop < 0) {
				// get swiping slop from ViewConfiguration
				mSwipeSlop = ViewConfiguration.Get (Context).ScaledTouchSlop;
			}

			bool swiping = false;
			float absDeltaX = Math.Abs (deltaX);

			if (absDeltaX > mSwipeSlop)
				return true;

			return swiping;
		}

		/// <summary>
		/// Swipe a view by moving distance
		/// </summary>
		/// <param name="child">A target view</param>
		/// <param name="deltaX">Moving distance along the x-axis</param>
		/// <param name="deltaY">Moving distance along the y-axis</param>
		protected void SwipeView (View child, float deltaX,float deltaY)
		{
			if (IsFixedView (child)) {
				deltaX = deltaX / 4;
			}

			float deltaXAbs = Math.Abs (deltaX);
			float fractionCovered = deltaXAbs / (float)child.Width;

			child.TranslationX = deltaX;
			child.Alpha = 1 - fractionCovered;

			if (deltaX > 0)
				child.RotationY = -15f * fractionCovered;
			else
				child.RotationY = -15f * fractionCovered;
		}

		protected void NotifyOnDismissEvent (View child)
		{
			if (child == null || mDismissListener == null)
				return;
			mDismissListener.OnDismiss ((String)child.Tag);
		}

		/// <summary>
		/// Gets or sets the tag of the first visible child in this layout
		/// </summary>
		/// <value>Tag of a card which should already be added to this list</value>
		public String FirstVisibleCardTag
		{
			get {
				int count = ChildCount;

				if (count == 0)
					return null;

				for (int i = 0; i < count; i++) {
					// Check the position of each view.
					View child = GetChildAt (i);
					if (child.GetGlobalVisibleRect (mChildRect) == true)
						return (String)child.Tag;
				}
				return null;
			}
			set {
				if (value == null)
					return; // do nothing

				if (mLayouted) {
					ScrollToCard (value);
				} else {
					// Keep the tag for next use.
					mFirstVisibleCardTag = value;
				}
			}
		}

		public void SetCardStreamAnimator (CardStreamAnimator animators)
		{
			if (animators == null)
				mAnimators = new CardStreamAnimator.EmptyAnimator ();
			else
				mAnimators = animators;

			LayoutTransition layoutTransition = LayoutTransition;

			if (layoutTransition != null) {
				layoutTransition.SetAnimator (LayoutTransitionType.Appearing, mAnimators.GetAppearingAnimator (Context));
				layoutTransition.SetAnimator (LayoutTransitionType.Disappearing, mAnimators.GetDisappearingAnimator (Context));
			}
		}

		/// <summary>
		/// Set an OnDismissListener which is called when the user dismisses a card.
		/// </summary>
		/// <param name="listener">Listener.</param>
		public void SetOnDismissListener (OnDismissListener listener)
		{
			mDismissListener = listener;
		}

		/// <summary>
		/// If this flag is set, after finishing the initial OnLayout event, an initial animation which is defined in DefaultCardStreamAnimator is launched
		/// </summary>
		public void TriggerShowInitialAnimation() 
		{ 
			mShowInitialAnimation = true; 
		}

		void Initialize (IAttributeSet attrs, int defStyle)
		{
			mTouchListener = new OnTouchListener (this);
			mOnHieratchyChangeListener = new OnHierarchyChangeListener()
			{
				OnChildViewAddedAction = (View parent, View child) =>
				{
					Log.Debug(TAG, "Child is added: " + child);
					IViewParent scrollView = parent.Parent;
					if (scrollView != null && scrollView is ScrollView)
					{
						((ScrollView)scrollView).FullScroll(FocusSearchDirection.Down);
					}
					if (this.LayoutTransition != null)
					{
						View view = child.FindViewById(Resource.Id.card_actionarea);
						if (view != null)
							view.Alpha = 0;
					}
				},
				OnChildViewRemovedAction = (View parent, View child) =>
				{
					Log.Debug(TAG, "Child is removed: " + child);
					mFixedViewList.Remove(child);
				}
			};

			mTransitionListener = new TransitionListener()
			{
				StartTransitionAction = (LayoutTransition transition, ViewGroup container, View view, LayoutTransitionType transitionType) =>
				{
					Log.Debug(TAG, "Start LayoutTransition animation: " + transitionType);
				},
				EndTransitionAction = (LayoutTransition transition, ViewGroup container, View view, LayoutTransitionType transitionType) =>
				{
					Log.Debug(TAG, "End LayoutTransition animation: " + transitionType);
					if (transitionType == LayoutTransitionType.Appearing)
					{
						View area = view.FindViewById(Resource.Id.card_actionarea);
						if (area != null)
						{
							RunShowActionAreaAnimation(container, area);
						}
					}
				}
			};

			float speedFactor = 1f;

			if (attrs != null) {

				var v = Resource.Styleable.CardStream;
				TypedArray a = Context.ObtainStyledAttributes (attrs, Resource.Styleable.CardStream, defStyle, 0);

				if (a != null) {
					int speedType = a.GetInt (Resource.Styleable.CardStream_animationDuration, 1001);
					switch (speedType) {
					case ANIMATION_SPEED_FAST:
						speedFactor = 0.5f;
						break;
					case ANIMATION_SPEED_NORMAL:
						speedFactor = 1f;
						break;
					case ANIMATION_SPEED_SLOW:
						speedFactor = 2f;
						break;
					}

					string animatorName = a.GetString (Resource.Styleable.CardStream_animators);

					try {
						if (animatorName != null)
							mAnimators = Class.ClassLoader.LoadClass(animatorName).NewInstance() as CardStreamAnimator;
					}
					catch (Exception e) {
						Log.Error (TAG, "Failed to load animator: " + animatorName, e);
					}
					finally {
						if (mAnimators == null)
							mAnimators = new DefaultCardStreamAnimator ();
					}
					a.Recycle ();
				}
			}

			mAnimators.SpeedFactor = speedFactor;
			mSwipeSlop = ViewConfiguration.Get (Context).ScaledTouchSlop;
			SetOnHierarchyChangeListener (mOnHieratchyChangeListener);
		}

		private void InitCard(View cardView, bool canDismiss)
		{
			ResetAnimatedView (cardView);
			cardView.SetOnTouchListener (mTouchListener);
			if (!canDismiss)
				mFixedViewList.Add (cardView);
		}

		private bool IsFixedView(View v) { return mFixedViewList.Contains(v); }

		private void ResetAnimatedView(View child)
		{
			child.Alpha = 1;
			child.TranslationX = 0;
			child.TranslationY = 0;
			child.Rotation = 0;
			child.RotationX = 0;
			child.RotationY = 0;
			child.ScaleX = 1;
			child.ScaleY = 1;
		}

		private void RunInitialAnimations()
		{
			if (mAnimators == null)
				return;

			int count = ChildCount;

			for (int i = 0; i < count; i++) {
				View child = GetChildAt(i);
				ObjectAnimator animator = mAnimators.GetInitialAnimator(Context);
				if (animator != null)
				{
					animator.SetTarget(child);
					animator.Start();
				}
			}
		}

		private void RunShowActionAreaAnimation(View parent, View area)
		{
			area.PivotY = 0;
			area.PivotX = parent.Width / 2f;

			area.Alpha = 0.5f;
			area.RotationX = -90;
			area.Animate ().RotationX (0f).Alpha (1f).SetDuration (400);
		}

		private void HandleViewSwipingOut (View child, float deltaX, float deltaY)
		{
			ObjectAnimator animator = mAnimators.GetSwipeOutAnimator (child, deltaX, deltaY);
			if (animator != null) {
				animator.AddListener (new AnimatorListener () {
					OnAnimationEndAction = (Animator animation) => {
						RemoveView (child);
						NotifyOnDismissEvent (child);
					}
				});
			} else {
				RemoveView (child);
				NotifyOnDismissEvent (child);
			}

			if (animator != null) {
				animator.SetTarget (child);
				animator.Start ();
			}
		}

		private void HandleViewSwipingIn (View child, float deltaX, float deltaY)
		{
			ObjectAnimator animator = mAnimators.GetSwipeInAnimator (child, deltaX, deltaY);
			if (animator != null) {
				animator.AddListener (new AnimatorListener () {
					OnAnimationEndAction = (Animator animation) => {
						child.TranslationX = 0f;
						child.TranslationY = 0f;
					}
				});
			} else {
				child.TranslationX = 0f;
				child.TranslationY = 0f;
			}

			if (animator != null) {
				animator.SetTarget (child);
				animator.Start ();
			}
		}

		private void ScrollToCard(string tag)
		{
			int count = ChildCount;
			for (int i = 0; i < count; i++) {
				View child = GetChildAt (i);

				if (tag.Equals (child.Tag)) {
					IViewParent parent = Parent;
					if (parent != null && parent is ScrollView) {
						((ScrollView)Parent).SmoothScrollTo (0, child.Top - PaddingTop - child.PaddingTop);
					}
					return;
				}
			}
		}

		public interface OnDismissListener
		{
			void OnDismiss(String tag);
		}
	}
}

