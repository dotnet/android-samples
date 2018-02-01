using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace AutofillFramework
{
    public class ScrollableCustomVirtualView : CustomVirtualView, GestureDetector.IOnGestureListener
    {
        private static string TAG = "ScrollableCustomView";

        private GestureDetector mGestureDetector;

        protected ScrollableCustomVirtualView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference,
            transfer)
        {
        }

        public ScrollableCustomVirtualView(Context context) : this(context, null)
        {
        }

        public ScrollableCustomVirtualView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public ScrollableCustomVirtualView(Context context, IAttributeSet attrs, int defStyleAttr) : this(context,
            attrs, defStyleAttr, 0)
        {
        }

        public ScrollableCustomVirtualView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :
            base(context, attrs, defStyleAttr, defStyleRes)
        {
            mGestureDetector = new GestureDetector(context, this);
        }

        /**
         * Resets the UI to the intial state.
         */
        public void ResetPositions()
        {
            ResetCoordinates();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return mGestureDetector.OnTouchEvent(e);
        }

        public bool OnDown(MotionEvent e)
        {
            OnMotion((int) e.GetY());
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (VERBOSE) Log.Verbose(TAG, "onScroll(): " + distanceX + " - " + distanceY);
            if (mFocusedLine != null)
            {
                mAutofillManager.NotifyViewExited(this, mFocusedLine.mFieldTextItem.id);
            }
            mTopMargin -= (int) distanceY;
            mLeftMargin -= (int) distanceX;
            Invalidate();
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return true;
        }
    }
}