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
using Android.Graphics;

namespace MonoIO
{
	/**
	 * {@link TouchDelegate} that gates {@link MotionEvent} instances by comparing
	 * then against fractional dimensions of the source view.
	 * <p>
	 * This is particularly useful when you want to define a rectangle in terms of
	 * the source dimensions, but when those dimensions might change due to pending
	 * or future layout passes.
	 * <p>
	 * One example is catching touches that occur in the top-right quadrant of
	 * {@code sourceParent}, and relaying them to {@code targetChild}. This could be
	 * done with: <code>
	 * FractionalTouchDelegate.setupDelegate(sourceParent, targetChild, new RectF(0.5f, 0f, 1f, 0.5f));
	 * </code>
	 */
	public class FractionalTouchDelegate : TouchDelegate
	{
	    private View mSource;
	    private View mTarget;
	
	    private RectF mSourceFraction;
	
	    private Rect mScrap = new Rect();
	
	    /** Cached full dimensions of {@link #mSource}. */
	    private Rect mSourceFull = new Rect();
	    /** Cached projection of {@link #mSourceFraction} onto {@link #mSource}. */
	    private Rect mSourcePartial = new Rect();
	
	    private bool mDelegateTargeted;
	
	    public FractionalTouchDelegate(View source, View target, RectF sourceFraction) : base(new Rect(0, 0, 0, 0), target)
		{
	        mSource = source;
	        mTarget = target;
	        mSourceFraction = sourceFraction;
	    }
	
	    /**
	     * Helper to create and setup a {@link FractionalTouchDelegate} between the
	     * given {@link View}.
	     *
	     * @param source Larger source {@link View}, usually a parent, that will be
	     *            assigned {@link View#setTouchDelegate(TouchDelegate)}.
	     * @param target Smaller target {@link View} which will receive
	     *            {@link MotionEvent} that land in requested fractional area.
	     * @param sourceFraction Fractional area projected onto source {@link View}
	     *            which determines when {@link MotionEvent} will be passed to
	     *            target {@link View}.
	     */
	    public static void SetupDelegate(View source, View target, RectF sourceFraction) {
	        source.TouchDelegate = new FractionalTouchDelegate(source, target, sourceFraction);
	    }
	
	    /**
	     * Consider updating {@link #mSourcePartial} when {@link #mSource}
	     * dimensions have changed.
	     */
	    private void UpdateSourcePartial() {
	        mSource.GetHitRect(mScrap);
	        if (!mScrap.Equals(mSourceFull)) {
	            // Copy over and calculate fractional rectangle
	            mSourceFull.Set(mScrap);
	
	            int width = mSourceFull.Width();
	            int height = mSourceFull.Height();
	
	            mSourcePartial.Left = (int) (mSourceFraction.Left * width);
	            mSourcePartial.Top = (int) (mSourceFraction.Top * height);
	            mSourcePartial.Right = (int) (mSourceFraction.Right * width);
	            mSourcePartial.Bottom = (int) (mSourceFraction.Bottom * height);
	        }
	    }
		
		public override bool OnTouchEvent (MotionEvent e)
		{
	
	        UpdateSourcePartial();
	
	        // The logic below is mostly copied from the parent class, since we
	        // can't update private mBounds variable.
	
	        // http://android.git.kernel.org/?p=platform/frameworks/base.git;a=blob;
	        // f=core/java/android/view/TouchDelegate.java;hb=eclair#l98
	
	        Rect sourcePartial = mSourcePartial;
	        View target = mTarget;
	
	        int x = (int)e.GetX();
	        int y = (int)e.GetY();
	
	        bool sendToDelegate = false;
	        bool hit = true;
	        bool handled = false;
	
	        switch (e.Action) {
	        case MotionEventActions.Down:
	            if (sourcePartial.Contains(x, y)) {
	                mDelegateTargeted = true;
	                sendToDelegate = true;
	            }
	            break;
	        case MotionEventActions.Up:
	        case MotionEventActions.Move:
	            sendToDelegate = mDelegateTargeted;
	            if (sendToDelegate) {
	                if (!sourcePartial.Contains(x, y)) {
	                    hit = false;
	                }
	            }
	            break;
	        case MotionEventActions.Cancel:
	            sendToDelegate = mDelegateTargeted;
	            mDelegateTargeted = false;
	            break;
	        }
	
	        if (sendToDelegate) {
	            if (hit) {
	                e.SetLocation(target.Width / 2, target.Height / 2);
	            } else {
	                e.SetLocation(-1, -1);
	            }
	            handled = target.DispatchTouchEvent(e);
	        }
	        return handled;
		}
	}
}

