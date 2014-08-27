
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
using Android.Support.V4.Widget;

namespace ElevationDrag
{
	public class DragFrameLayout : FrameLayout
	{

		/**
	     * The list of {@link View}s that will be draggable.
	     */
		internal IList<View> mDragViews;

		/**
	     * The {@link DragFrameLayoutController} that will be notify on drag.
	     */
		internal DragFrameLayoutController DragFrameLayoutController;

		private ViewDragHelper mDragHelper;

		public DragFrameLayout (Context context) :
			base (context)
		{
			Initialize ();
		}

		public DragFrameLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public DragFrameLayout (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			mDragViews = new List<View> ();

			/**
	         * Create the {@link ViewDragHelper} and set its callback.
	         */
			mDragHelper = ViewDragHelper.Create (this, 1.0f, new Callbacks (this));
		}


		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			var action = ev.Action;
			if (action == MotionEventActions.Cancel || action == MotionEventActions.Up) {
				mDragHelper.Cancel ();
				return false;
			}
			return mDragHelper.ShouldInterceptTouchEvent (ev);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			mDragHelper.ProcessTouchEvent (e);
			return true;
		}

		public void AddDragView (View dragView)
		{
			mDragViews.Add (dragView);
		}
	}

	internal class Callbacks : ViewDragHelper.Callback
	{
		private	DragFrameLayout owner;

		public Callbacks (DragFrameLayout owner)
		{
			this.owner = owner;
		}

		public override bool TryCaptureView (View child, int pointerId)
		{
			return owner.mDragViews.Contains (child);
		}

		public override void OnViewPositionChanged (View changedView, int left, int top, int dx, int dy)
		{
			base.OnViewPositionChanged (changedView, left, top, dx, dy);
		}

		public override int ClampViewPositionHorizontal (View child, int left, int dx)
		{
			return left;
		}

		public override int ClampViewPositionVertical (View child, int top, int dy)
		{
			return top;
		}

		public override void OnViewCaptured (View capturedChild, int activePointerId)
		{
			base.OnViewCaptured (capturedChild, activePointerId);
			if (owner.DragFrameLayoutController != null)
				owner.DragFrameLayoutController.OnDragDrop (false);
		}
	}

	public class DragFrameLayoutController
	{
		private Action<bool> action;
		public DragFrameLayoutController(Action<bool> action){
			this.action = action;
		}
		public virtual void OnDragDrop (bool captured){
			action.Invoke (captured);
		}
	}
}

