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
using Android.Graphics.Drawables;
using Android.Util;
using Java.Lang;
using Android.Support.V4.View;
using Math = Java.Lang.Math;

namespace MonoIO.UI.Widget
{
	public class Workspace : ViewGroup
	{
		private string TAG = "Workspace";
	
	    private const int INVALID_SCREEN = -1;
	
	    /**
	     * The velocity at which a fling gesture will cause us to snap to the next screen
	     */
	    private int SNAP_VELOCITY = 500;
	
	    /**
	     * The user needs to drag at least this much for it to be considered a fling gesture. This
	     * reduces the chance of a random twitch sending the user to the next screen.
	     */
	    // TODO: refactor
	    private int MIN_LENGTH_FOR_FLING = 100;
	
	    private int mDefaultScreen;
	
	    private bool mFirstLayout = true;
	    private bool mHasLaidOut = false;
	
	    private int mCurrentScreen;
	    private int mNextScreen = INVALID_SCREEN;
	    private Scroller mScroller;
	    private VelocityTracker mVelocityTracker;
	
	    /**
	     * X position of the active pointer when it was first pressed down.
	     */
	    private float mDownMotionX;
	
	    /**
	     * Y position of the active pointer when it was first pressed down.
	     */
	    private float mDownMotionY;
	
	    /**
	     * This view's X scroll offset when the active pointer was first pressed down.
	     */
	    private int mDownScrollX;
	
	    private const int TOUCH_STATE_REST = 0;
	    private const int TOUCH_STATE_SCROLLING = 1;
	
	    private int mTouchState = TOUCH_STATE_REST;
	
	    private View.IOnLongClickListener mLongClickListener;
	
	    private bool mAllowLongPress = true;
	
	    private int mTouchSlop;
	    private int mPagingTouchSlop;
	    private int mMaximumVelocity;
	
	    private const int INVALID_POINTER = -1;
	
	    private int mActivePointerId = INVALID_POINTER;
	
	    private Drawable mSeparatorDrawable;
	
	    private OnScreenChangeListener mOnScreenChangeListener;
	    private OnScrollListener mOnScrollListener;
	
	    private bool mLocked;
	
	    private int mDeferredScreenChange = -1;
	    private bool mDeferredScreenChangeFast = false;
	    private bool mDeferredNotify = false;
	
	    private bool mIgnoreChildFocusRequests;
	
	    private bool mIsVerbose = false;
	
	    public interface OnScreenChangeListener 
		{
	        void OnScreenChanged(View newScreen, int newScreenIndex);
	        void OnScreenChanging(View newScreen, int newScreenIndex);
	    }
	
	    public interface OnScrollListener 
		{
	        void OnScroll(float screenFraction);
	    }
		
		public Workspace(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			mDefaultScreen = 0;
	        mLocked = false;
	
			HapticFeedbackEnabled = false;
			InitWorkspace();
			
	        mIsVerbose = Log.IsLoggable(TAG, LogPriority.Verbose);
			
		}
		
		/**
	     * Initializes various states for this workspace.
	     */
	    private void InitWorkspace() 
		{
	        mScroller = new Scroller(Context);
	        mCurrentScreen = mDefaultScreen;
	
	        var configuration = ViewConfiguration.Get(Context);
	        mTouchSlop = configuration.ScaledTouchSlop;
	        mMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
	
	        mPagingTouchSlop = ReflectionUtils.CallWithDefault<int>(configuration, "ScaledPagingTouchStop", mTouchSlop * 2);
	    }
		
		/**
	     * Returns the index of the currently displayed screen.
	     */
	    int GetCurrentScreen() {
	        return mCurrentScreen;
	    }
	
	    /**
	     * Returns the number of screens currently contained in this Workspace.
	     */
	    int GetScreenCount() {
	        int childCount = ChildCount;
	        if (mSeparatorDrawable != null) {
	            return (childCount + 1) / 2;
	        }
	        return childCount;
	    }
	
	    View GetScreenAt(int index) {
	        if (mSeparatorDrawable == null) {
	            return GetChildAt(index);
	        }
	        return GetChildAt(index * 2);
	    }
	
	    int GetScrollWidth() {
	        int w = Width;
	        if (mSeparatorDrawable != null) {
	            w += mSeparatorDrawable.IntrinsicWidth;
	        }
	        return w;
	    }
	
	    void HandleScreenChangeCompletion(int currentScreen) {
	        mCurrentScreen = currentScreen;
	        View screen = GetScreenAt(mCurrentScreen);
	        //screen.requestFocus();
	        try {
	            ReflectionUtils.TryInvoke(screen, "DispatchDisplayHint", new Type[]{}, ViewStates.Visible);
	            Invalidate();
	        } catch (NullPointerException e) {
	            Log.Error(TAG, "Caught NullPointerException", e);
	        }
	        NotifyScreenChangeListener(mCurrentScreen, true);
	    }
	
	    void NotifyScreenChangeListener(int whichScreen, bool changeComplete) {
	        if (mOnScreenChangeListener != null) {
	            if (changeComplete)
	                mOnScreenChangeListener.OnScreenChanged(GetScreenAt(whichScreen), whichScreen);
	            else
	                mOnScreenChangeListener.OnScreenChanging(GetScreenAt(whichScreen), whichScreen);
	        }
	        if (mOnScrollListener != null) {
	            mOnScrollListener.OnScroll(GetCurrentScreenFraction());
	        }
	    }
		
		public override void SetOnLongClickListener (IOnLongClickListener listener)
		{
			mLongClickListener = listener;
	        int count = GetScreenCount();
	        for (int i = 0; i < count; i++) {
	            GetScreenAt(i).SetOnLongClickListener(listener);
	        }
		}
		
		public override void ComputeScroll ()
		{
			if (mScroller.ComputeScrollOffset()) {
	            ScrollTo(mScroller.CurrX, mScroller.CurrY);
	            if (mOnScrollListener != null) {
	                mOnScrollListener.OnScroll(GetCurrentScreenFraction());
	            }
				PostInvalidate();
	        } else if (mNextScreen != INVALID_SCREEN) {
	            // The scroller has finished.
	            HandleScreenChangeCompletion(Java.Lang.Math.Max(0, Java.Lang.Math.Min(mNextScreen, GetScreenCount() - 1)));
	            mNextScreen = INVALID_SCREEN;
	        }
		}
		
		protected override void DispatchDraw (Android.Graphics.Canvas canvas)
		{
			bool restore = false;
	        int restoreCount = 0;
	
	        // ViewGroup.dispatchDraw() supports many features we don't need:
	        // clip to padding, layout animation, animation listener, disappearing
	        // children, etc. The following implementation attempts to fast-track
	        // the drawing dispatch by drawing only what we know needs to be drawn.
	
	        bool fastDraw = mTouchState != TOUCH_STATE_SCROLLING && mNextScreen == INVALID_SCREEN;
	        // If we are not scrolling or flinging, draw only the current screen
	        if (fastDraw) {
	            if (GetScreenAt(mCurrentScreen) != null) {
	                DrawChild(canvas, GetScreenAt(mCurrentScreen), DrawingTime);
	            }
	        } else {
	            long drawingTime = DrawingTime;
	            // If we are flinging, draw only the current screen and the target screen
	            if (mNextScreen >= 0 && mNextScreen < GetScreenCount() && Java.Lang.Math.Abs(mCurrentScreen - mNextScreen) == 1) {
	                DrawChild(canvas, GetScreenAt(mCurrentScreen), drawingTime);
	                DrawChild(canvas, GetScreenAt(mNextScreen), drawingTime);
	            } else {
	                // If we are scrolling, draw all of our children
	                int count = ChildCount;
	                for (int i = 0; i < count; i++) {
	                    DrawChild(canvas, GetChildAt(i), drawingTime);
	                }
	            }
	        }
	
	        if (restore) {
	            canvas.RestoreToCount(restoreCount);
	        }
		}
		
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			
			// The children are given the same width and height as the workspace
	        int count = ChildCount;
	        for (int i = 0; i < count; i++) {
	            if (mSeparatorDrawable != null && (i & 1) == 1) {
	                // separator
	                GetChildAt(i).Measure(mSeparatorDrawable.IntrinsicWidth, heightMeasureSpec);
	            } else {
	                GetChildAt(i).Measure(widthMeasureSpec, heightMeasureSpec);
	            }
	        }
	
	        if (mFirstLayout) {
	            HorizontalScrollBarEnabled = false;
	            int width = MeasureSpec.GetSize(widthMeasureSpec);
	            if (mSeparatorDrawable != null) {
	                width += mSeparatorDrawable.IntrinsicWidth;
	            }
	            ScrollTo(mCurrentScreen * width, 0);
	            HorizontalScrollBarEnabled = true;
	            mFirstLayout = false;
	        }
		}
		
		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			int childLeft = 0;
	
	        int count = ChildCount;
	        for (int i = 0; i < count; i++) {
	            View child = GetChildAt(i);
	            if (child.Visibility != ViewStates.Gone) {
	                int childWidth = child.MeasuredWidth;
	                child.Layout(childLeft, 0, childLeft + childWidth, child.MeasuredHeight);
	                childLeft += childWidth;
	            }
	        }
	
	        mHasLaidOut = true;
	        if (mDeferredScreenChange >= 0) {
	            SnapToScreen(mDeferredScreenChange, mDeferredScreenChangeFast, mDeferredNotify);
	            mDeferredScreenChange = -1;
	            mDeferredScreenChangeFast = false;
	        }
		}
		
		public override bool RequestChildRectangleOnScreen (View child, Android.Graphics.Rect rectangle, bool immediate)
		{
			int screen = IndexOfChild(child);
	        if (mIgnoreChildFocusRequests && !mScroller.IsFinished) {
	            Log.Warn(TAG, "Ignoring child focus request: request " + mCurrentScreen + " -> " + screen);
	            return false;
	        }
	        if (screen != mCurrentScreen || !mScroller.IsFinished) {
	            SnapToScreen(screen);
	            return true;
	        }
	        return false;
		}
		
		protected override bool OnRequestFocusInDescendants (int direction, Android.Graphics.Rect previouslyFocusedRect)
		{
			int focusableScreen;
	        if (mNextScreen != INVALID_SCREEN) {
	            focusableScreen = mNextScreen;
	        } else {
	            focusableScreen = mCurrentScreen;
	        }
	        View v = GetScreenAt(focusableScreen);
	        if (v != null) {
	            return v.RequestFocus((FocusSearchDirection) direction, previouslyFocusedRect);
	        }
	        return false;
		}
		
		public override bool DispatchUnhandledMove (View focused, FocusSearchDirection direction)
		{
			if (direction == FocusSearchDirection.Left) {
	            if (GetCurrentScreen() > 0) {
	                SnapToScreen(GetCurrentScreen() - 1);
	                return true;
	            }
	        } else if (direction == FocusSearchDirection.Right) {
	            if (GetCurrentScreen() < GetScreenCount() - 1) {
	                SnapToScreen(GetCurrentScreen() + 1);
	                return true;
	            }
	        }
	        return base.DispatchUnhandledMove(focused, direction);
		}
		
		public override void AddFocusables (IList<View> views, FocusSearchDirection direction, FocusablesFlags focusableMode)
		{
			View focusableSourceScreen = null;
	        if (mCurrentScreen >= 0 && mCurrentScreen < GetScreenCount()) {
	            focusableSourceScreen = GetScreenAt(mCurrentScreen);
	        }
	        if (direction == FocusSearchDirection.Left) {
	            if (mCurrentScreen > 0) {
	                focusableSourceScreen = GetScreenAt(mCurrentScreen - 1);
	            }
	        } else if (direction == FocusSearchDirection.Right) {
	            if (mCurrentScreen < GetScreenCount() - 1) {
	                focusableSourceScreen = GetScreenAt(mCurrentScreen + 1);
	            }
	        }
	
	        if (focusableSourceScreen != null) {
	            focusableSourceScreen.AddFocusables(views, direction, focusableMode);
	        }
		}
		
		/**
	     * If one of our descendant views decides that it could be focused now, only pass that along if
	     * it's on the current screen.
	     *
	     * This happens when live folders requery, and if they're off screen, they end up calling
	     * requestFocus, which pulls it on screen.
	     */
		public override void FocusableViewAvailable (View focused)
		{		   
			View current = GetScreenAt(mCurrentScreen);
			View v = focused;
			IViewParent parent;
			while (true) {
			    if (v == current) {
			        base.FocusableViewAvailable(focused);
			        return;
			    }
			    if (v == this) {
			        return;
			    }
			    parent = v.Parent;
			    if (parent is View) {
			        v = (View) v.Parent;
			    } else {
			        return;
			    }
			}
		}
		
		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			/*
	         * This method JUST determines whether we want to intercept the motion.
	         * If we return true, onTouchEvent will be called and we do the actual
	         * scrolling there.
	         */
	
	        // Begin tracking velocity even before we have intercepted touch events.
	        if (mVelocityTracker == null) {
	            mVelocityTracker = VelocityTracker.Obtain();
	        }
	        mVelocityTracker.AddMovement(ev);
	
	        /*
	         * Shortcut the most recurring case: the user is in the dragging
	         * state and he is moving his finger.  We want to intercept this
	         * motion.
	         */
	        var action = ev.Action;
	        if (mIsVerbose) {
	            Log.Verbose(TAG, "onInterceptTouchEvent: " + (ev.Action & MotionEventActions.Mask));
	        }
	        if (((action & MotionEventActions.Mask) == MotionEventActions.Move)
	                && (mTouchState == TOUCH_STATE_SCROLLING)) {
	            if (mIsVerbose) {
	                Log.Verbose(TAG, "Intercepting touch events");
	            }
	            return true;
	        }
	
	        switch (action & MotionEventActions.Mask) {
	            case MotionEventActions.Move: {
	                if (mLocked) {
	                    // we're locked on the current screen, don't allow moving
	                    break;
	                }
	
	                /*
	                 * Locally do absolute value. mDownMotionX is set to the y value
	                 * of the down event.
	                 */
	                int pointerIndex = MotionEventCompat.FindPointerIndex(ev, mActivePointerId);
	                float x = MotionEventCompat.GetX(ev, pointerIndex);
	                float y = MotionEventCompat.GetY(ev, pointerIndex);
	                int xDiff = (int) Java.Lang.Math.Abs(x - mDownMotionX);
	                int yDiff = (int) Java.Lang.Math.Abs(y - mDownMotionY);
	
	                bool xPaged = xDiff > mPagingTouchSlop;
	                bool xMoved = xDiff > mTouchSlop;
	                bool yMoved = yDiff > mTouchSlop;
	
	                if (xMoved || yMoved) {
	                    if (xPaged) {
	                        // Scroll if the user moved far enough along the X axis
	                        mTouchState = TOUCH_STATE_SCROLLING;
	                    }
	                    // Either way, cancel any pending longpress
	                    if (mAllowLongPress) {
	                        mAllowLongPress = false;
	                        // Try canceling the long press. It could also have been scheduled
	                        // by a distant descendant, so use the mAllowLongPress flag to block
	                        // everything
	                        View currentScreen = GetScreenAt(mCurrentScreen);
	                        if (currentScreen != null) {
	                            currentScreen.CancelLongPress();
	                        }
	                    }
	                }
	                break;
	            }
	
	            case MotionEventActions.Down: {
	                float x = ev.GetX();
	                float y = ev.GetY();
	                // Remember location of down touch
	                mDownMotionX = x;
	                mDownMotionY = y;
	                mDownScrollX = ScrollX;
	                mActivePointerId = MotionEventCompat.GetPointerId(ev, 0);
	                mAllowLongPress = true;
	
	                /*
	                 * If being flinged and user touches the screen, initiate drag;
	                 * otherwise don't.  mScroller.isFinished should be false when
	                 * being flinged.
	                 */
	                mTouchState = mScroller.IsFinished ? TOUCH_STATE_REST : TOUCH_STATE_SCROLLING;
	                break;
	            }
	
	            case MotionEventActions.Cancel:
	            case MotionEventActions.Up:
	                // Release the drag
	                mTouchState = TOUCH_STATE_REST;
	                mAllowLongPress = false;
	                mActivePointerId = INVALID_POINTER;
	                if (mVelocityTracker == null) {
	                    mVelocityTracker.Recycle();
	                    mVelocityTracker = null;
	                }
	                break;
	
				 
				
	            case MotionEventActions.PointerUp:
	                OnSecondaryPointerUp(ev);
	                break;
	        }
			
			/*
	         * The only time we want to intercept motion events is if we are in the
	         * drag mode.
	         */
	        bool intercept = mTouchState != TOUCH_STATE_REST;
	        if (mIsVerbose) {
	            Log.Verbose(TAG, "Intercepting touch events: " + intercept);
	        }
	        return intercept;
		}
		
		void OnSecondaryPointerUp(MotionEvent ev) {
	        var pointerIndex = ((int)ev.Action & MotionEventCompat.ActionPointerIndexMask) >> MotionEventCompat.ActionPointerIndexShift;
			
	        var pointerId = MotionEventCompat.GetPointerId(ev, pointerIndex);
	        if (pointerId == mActivePointerId) {
	            // This was our active pointer going up. Choose a new
	            // active pointer and adjust accordingly.
	            // TODO: Make this decision more intelligent.
	            int newPointerIndex = pointerIndex == 0 ? 1 : 0;
	            mDownMotionX = MotionEventCompat.GetX(ev, newPointerIndex);
	            mDownMotionX = MotionEventCompat.GetY(ev, newPointerIndex);
	            mDownScrollX = ScrollX;
	            mActivePointerId = MotionEventCompat.GetPointerId(ev, newPointerIndex);
	            if (mVelocityTracker != null) {
	                mVelocityTracker.Clear();
	            }
	        }
	    }
		
		public override void RequestChildFocus (View child, View focused)
		{
			base.RequestChildFocus (child, focused);
			int screen = IndexOfChild(child);
	        if (mSeparatorDrawable != null) {
	            screen /= 2;
	        }
	        if (screen >= 0 && !IsInTouchMode) {
	            SnapToScreen(screen);
	        }
		}
		
		public override bool OnTouchEvent (MotionEvent ev)
		{
			if (mIsVerbose) {
	            Log.Verbose(TAG, "onTouchEvent: " + ((int)ev.Action & MotionEventCompat.ActionMask));
	        }
	
	        if (mVelocityTracker == null) {
	            mVelocityTracker = VelocityTracker.Obtain();
	        }
	        mVelocityTracker.AddMovement(ev);
	
	        var action = ev.Action;
	
	        switch ((int)action & MotionEventCompat.ActionMask) {
	            case (int)MotionEventActions.Down:
	                // If being flinged and user touches, stop the fling. isFinished
	                // will be false if being flinged.
	                if (!mScroller.IsFinished) {
	                    mScroller.AbortAnimation();
	                }
	
	                // Remember where the motion event started
	                mDownMotionX = ev.GetX();
	                mDownMotionY = ev.GetY();
	                mDownScrollX = ScrollX;
	                mActivePointerId = MotionEventCompat.GetPointerId(ev, 0);
	                break;
	
	            case (int)MotionEventActions.Move:
	                if (mIsVerbose) {
	                    Log.Verbose(TAG, "mTouchState=" + mTouchState);
	                }
	
	                if (mTouchState == TOUCH_STATE_SCROLLING) {
	                    // Scroll to follow the motion event
	                    int pointerIndex = MotionEventCompat.FindPointerIndex(ev, mActivePointerId);
	                    float x = MotionEventCompat.GetX(ev, pointerIndex);
	
	                    View lastChild = GetChildAt(ChildCount - 1);
	                    int maxScrollX = lastChild.Right - Width;
	                    ScrollTo(Math.Max(0, Math.Min(maxScrollX,
	                            (int)(mDownScrollX + mDownMotionX - x
	                            ))), 0);
	                    if (mOnScrollListener != null) {
	                        mOnScrollListener.OnScroll(GetCurrentScreenFraction());
	                    }
	
	                } else if (mTouchState == TOUCH_STATE_REST) {
	                    if (mLocked) {
	                        // we're locked on the current screen, don't allow moving
	                        break;
	                    }
	
	                    /*
	                     * Locally do absolute value. mLastMotionX is set to the y value
	                     * of the down event.
	                     */
	                    int pointerIndex = MotionEventCompat.FindPointerIndex(ev, mActivePointerId);
	                    float x = MotionEventCompat.GetX(ev, pointerIndex);
	                    float y = MotionEventCompat.GetY(ev, pointerIndex);
	                    int xDiff = (int) Math.Abs(x - mDownMotionX);
	                    int yDiff = (int) Math.Abs(y - mDownMotionY);
	
	                    bool xPaged = xDiff > mPagingTouchSlop;
	                    bool xMoved = xDiff > mTouchSlop;
	                    bool yMoved = yDiff > mTouchSlop;
	
	                    if (xMoved || yMoved) {
	                        if (xPaged) {
	                            // Scroll if the user moved far enough along the X axis
	                            mTouchState = TOUCH_STATE_SCROLLING;
	                        }
	                        // Either way, cancel any pending longpress
	                        if (mAllowLongPress) {
	                            mAllowLongPress = false;
	                            // Try canceling the long press. It could also have been scheduled
	                            // by a distant descendant, so use the mAllowLongPress flag to block
	                            // everything
	                            View currentScreen = GetScreenAt(mCurrentScreen);
	                            if (currentScreen != null) {
	                                currentScreen.CancelLongPress();
	                            }
	                        }
	                    }
	                }
	                break;
	
	            case (int)MotionEventActions.Up:
	                if (mTouchState == TOUCH_STATE_SCROLLING) {
	                    int activePointerId = mActivePointerId;
	                    int pointerIndex = MotionEventCompat.FindPointerIndex(ev, activePointerId);
	                    float x = MotionEventCompat.GetX(ev, pointerIndex);
	                    VelocityTracker velocityTracker = mVelocityTracker;
	                    velocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
	                    //TODO(minsdk8): int velocityX = (int) MotionEventUtils.getXVelocity(velocityTracker, activePointerId);
	                    int velocityX = (int) velocityTracker.XVelocity;
	                    bool isFling = Math.Abs(mDownMotionX - x) > MIN_LENGTH_FOR_FLING;
	
	                    float scrolledPos = GetCurrentScreenFraction();
	                    int whichScreen = Math.Round(scrolledPos);
	
	                    if (isFling && mIsVerbose) {
	                        Log.Verbose(TAG, "isFling, whichScreen=" + whichScreen
	                                + " scrolledPos=" + scrolledPos
	                                + " mCurrentScreen=" + mCurrentScreen
	                                + " velocityX=" + velocityX);
	                    }
	                    if (isFling && velocityX > SNAP_VELOCITY && mCurrentScreen > 0) {
	                        // Fling hard enough to move left
	                        // Don't fling across more than one screen at a time.
	                        int bound = scrolledPos <= whichScreen ?
	                                mCurrentScreen - 1 : mCurrentScreen;
	                        SnapToScreen(Math.Min(whichScreen, bound));
	                    } else if (isFling && velocityX < -SNAP_VELOCITY &&
	                            mCurrentScreen < ChildCount - 1) {
	                        // Fling hard enough to move right
	                        // Don't fling across more than one screen at a time.
	                        int bound = scrolledPos >= whichScreen ?
	                                mCurrentScreen + 1 : mCurrentScreen;
	                        SnapToScreen(Math.Max(whichScreen, bound));
	                    } else {
	                        SnapToDestination();
	                    }
	                } else {
	                    PerformClick();
	                }
	                mTouchState = TOUCH_STATE_REST;
	                mActivePointerId = INVALID_POINTER;
					// Can't do this -> // Intentially fall through to cancel
					mTouchState = TOUCH_STATE_REST;
	                mActivePointerId = INVALID_POINTER;
	                if (mVelocityTracker != null) {
	                    mVelocityTracker.Recycle();
	                    mVelocityTracker = null;
	                }
					break;
	
	            case (int)MotionEventActions.Cancel:
	                mTouchState = TOUCH_STATE_REST;
	                mActivePointerId = INVALID_POINTER;
	                if (mVelocityTracker != null) {
	                    mVelocityTracker.Recycle();
	                    mVelocityTracker = null;
	                }
	                break;
	
				
	            case (int)MotionEventCompat.ActionPointerUp:
	                OnSecondaryPointerUp(ev);
	                break;
	        }
	
	        return true;
		}
		
		
		/**
	     * Returns the current scroll position as a float value from 0 to the number of screens.
	     * Fractional values indicate that the user is mid-scroll or mid-fling, and whole values
	     * indicate that the Workspace is currently snapped to a screen.
	     */
	    float GetCurrentScreenFraction() {
	        if (!mHasLaidOut) {
	            return mCurrentScreen;
	        }
			
	        int scrollX = ScrollX;
	        int screenWidth = Width;
	        return (float) scrollX / screenWidth;
		}
		
		void SnapToDestination() {
	        int screenWidth = GetScrollWidth();
	        int whichScreen = (ScrollX + (screenWidth / 2)) / screenWidth;
	
	        SnapToScreen(whichScreen);
	    }

		
		void SnapToScreen(int whichScreen) {
	        SnapToScreen(whichScreen, false, true);
	    }
	
	    void SnapToScreen(int whichScreen, bool fast, bool notify) {
	        if (!mHasLaidOut) { // Can't handle scrolling until we are laid out.
	            mDeferredScreenChange = whichScreen;
	            mDeferredScreenChangeFast = fast;
	            mDeferredNotify = notify;
	            return;
	        }
	
	        if (mIsVerbose) {
	            Log.Verbose(TAG, "Snapping to screen: " + whichScreen);
	        }
	
	        whichScreen = Java.Lang.Math.Max(0, Java.Lang.Math.Min(whichScreen, GetScreenCount() - 1));
	
	        int screenDelta = Java.Lang.Math.Abs(whichScreen - mCurrentScreen);
	
	        bool screenChanging =
	                (mNextScreen != INVALID_SCREEN && mNextScreen != whichScreen) ||
	                        (mCurrentScreen != whichScreen);
	
	        mNextScreen = whichScreen;
	
	        View focusedChild = FocusedChild;
	        bool setTabFocus = false;
	        if (focusedChild != null && screenDelta != 0 && focusedChild == GetScreenAt(mCurrentScreen)) {
	            // clearing the focus of the child will cause focus to jump to the tabs,
	            // which will in turn cause snapToScreen to be called again with a different
	            // value. To prevent this, we temporarily disable the OnTabClickListener
	            //if (mTabRow != null) {
	            //    mTabRow.setOnTabClickListener(null);
	            //}
	            //focusedChild.clearFocus();
	            //setTabRow(mTabRow); // restore the listener
	            //setTabFocus = true;
	        }
	
	        int newX = whichScreen * GetScrollWidth();
	        int sX = ScrollX;
	        int delta = newX - sX;
	        int duration = screenDelta * 300;
	        AwakenScrollBars(duration);
	        if (duration == 0) {
	            duration = Java.Lang.Math.Abs(delta);
	        }
	        if (fast) {
	            duration = 0;
	        }
	
	        if (mNextScreen != mCurrentScreen) {
	            // make the current listview hide its filter popup
	            View screenAt = GetScreenAt(mCurrentScreen);
	            if (screenAt != null) {
	                ReflectionUtils.TryInvoke(screenAt, "DispatchDisplayHint", new Type[]{}, ViewStates.Invisible);
	            } else {
	                Log.Error(TAG, "Screen at index was null. mCurrentScreen = " + mCurrentScreen);
	                return;
	            }
	
	            // showing the filter popup for the next listview needs to be delayed
	            // until we've fully moved to that listview, since otherwise the
	            // popup will appear at the wrong place on the screen
	            //removeCallbacks(mFilterWindowEnabler);
	            //postDelayed(mFilterWindowEnabler, duration + 10);
	
	            // NOTE: moved to computeScroll and handleScreenChangeCompletion()
	        }
	
	        if (!mScroller.IsFinished) {
	            mScroller.AbortAnimation();
	        }
	        mScroller.StartScroll(sX, 0, delta, 0, duration);
	        if (screenChanging && notify) {
	            NotifyScreenChangeListener(mNextScreen, false);
	        }
	        Invalidate();
	    }
		
		/**
	     * @return True is long presses are still allowed for the current touch
	     */
	    bool AllowLongPress() {
	        return mAllowLongPress;
	    }
		
		/**
	     * Register a callback to be invoked when the screen is changed, either programmatically or via
	     * user interaction.  Will automatically trigger a callback.
	     *
	     * @param screenChangeListener The callback.
	     */
	    public void SetOnScreenChangeListener(OnScreenChangeListener screenChangeListener) {
	        SetOnScreenChangeListener(screenChangeListener, true);
	    }
	
	    /**
	     * Register a callback to be invoked when the screen is changed, either programmatically or via
	     * user interaction.
	     *
	     * @param screenChangeListener The callback.
	     * @param notifyImmediately Whether to trigger a notification immediately
	     */
	    public void SetOnScreenChangeListener(OnScreenChangeListener screenChangeListener, bool notifyImmediately) {
	        mOnScreenChangeListener = screenChangeListener;
	        if (mOnScreenChangeListener != null && notifyImmediately) {
	            mOnScreenChangeListener.OnScreenChanged(GetScreenAt(mCurrentScreen), mCurrentScreen);
	        }
	    }
	
	    /**
	     * Register a callback to be invoked when this Workspace is mid-scroll or mid-fling, either
	     * due to user interaction or programmatic changes in the current screen index.
	     *
	     * @param scrollListener The callback.
	     * @param notifyImmediately Whether to trigger a notification immediately
	     */
	    public void SetOnScrollListener(OnScrollListener scrollListener, bool notifyImmediately) {
	        mOnScrollListener = scrollListener;
	        if (mOnScrollListener != null && notifyImmediately) {
	            mOnScrollListener.OnScroll(GetCurrentScreenFraction());
	        }
	    }
	
	    /**
	     * Scrolls to the given screen.
	     */
	    public void SetCurrentScreen(int screenIndex) {
	        SnapToScreen(Math.Max(0, Math.Min(GetScreenCount() - 1, screenIndex)));
	    }
	
	    /**
	     * Scrolls to the given screen fast (no matter how large the scroll distance is)
	     *
	     * @param screenIndex
	     */
	    public void SetCurrentScreenNow(int screenIndex) {
	        SetCurrentScreenNow(screenIndex, true);
	    }
	
	    public void SetCurrentScreenNow(int screenIndex, bool notify) {
	        SnapToScreen(Math.Max(0, Math.Min(GetScreenCount() - 1, screenIndex)), true, notify);
	    }
	
	    /**
	     * Scrolls to the screen adjacent to the current screen on the left, if it exists. This method
	     * is a no-op if the Workspace is currently locked.
	     */
	    public void ScrollLeft() {
	        if (mLocked) {
	            return;
	        }
	        if (mScroller.IsFinished) {
	            if (mCurrentScreen > 0) {
	                SnapToScreen(mCurrentScreen - 1);
	            }
	        } else {
	            if (mNextScreen > 0) {
	                SnapToScreen(mNextScreen - 1);
	            }
	        }
	    }
	
	    /**
	     * Scrolls to the screen adjacent to the current screen on the right, if it exists. This method
	     * is a no-op if the Workspace is currently locked.
	     */
	    public void ScrollRight() {
	        if (mLocked) {
	            return;
	        }
	        if (mScroller.IsFinished) {
	            if (mCurrentScreen < ChildCount - 1) {
	                SnapToScreen(mCurrentScreen + 1);
	            }
	        } else {
	            if (mNextScreen < ChildCount - 1) {
	                SnapToScreen(mNextScreen + 1);
	            }
	        }
	    }
	
	    /**
	     * If set, invocations of requestChildRectangleOnScreen() will be ignored.
	     */
	    public void SetIgnoreChildFocusRequests(bool mIgnoreChildFocusRequests) {
	        this.mIgnoreChildFocusRequests = mIgnoreChildFocusRequests;
	    }
	
	    public void MarkViewSelected(View v) {
	        mCurrentScreen = IndexOfChild(v);
	    }
	
	    /**
	     * Locks the current screen, preventing users from changing screens by swiping.
	     */
	    public void LockCurrentScreen() {
	        mLocked = true;
	    }
	
	    /**
	     * Unlocks the current screen, if it was previously locked. See also {@link
	     * Workspace#lockCurrentScreen()}.
	     */
	    public void UnlockCurrentScreen() {
	        mLocked = false;
	    }
	
	    /**
	     * Sets the resource ID of the separator drawable to use between adjacent screens.
	     */
	    public void SetSeparator(int resId) {
	        if (mSeparatorDrawable != null && resId == 0) {
	            // remove existing separators
	            mSeparatorDrawable = null;
	            int num = ChildCount;
	            for (int i = num - 2; i > 0; i -= 2) {
	                RemoveViewAt(i);
	            }
	            RequestLayout();
	        } else if (resId != 0) {
	            // add or update separators
	            if (mSeparatorDrawable == null) {
	                // add
	                int numsep = ChildCount;
	                int insertIndex = 1;
	                mSeparatorDrawable = Resources.GetDrawable(resId);
	                for (int i = 1; i < numsep; i++) {
	                    View v = new View(Context);
	                    v.SetBackgroundDrawable(mSeparatorDrawable);
	                    LayoutParams lp = new LayoutParams(LayoutParams.WrapContent,LayoutParams.FillParent);
	                    v.LayoutParameters = lp;
	                    AddView(v, insertIndex);
	                    insertIndex += 2;
	                }
	                RequestLayout();
	            } else {
	                // update
	                mSeparatorDrawable = Resources.GetDrawable(resId);
	                int num = ChildCount;
	                for (int i = num - 2; i > 0; i -= 2) {
	                    GetChildAt(i).SetBackgroundDrawable(mSeparatorDrawable);
	                }
	                RequestLayout();
	            }
	        }
	    }
	}
}

