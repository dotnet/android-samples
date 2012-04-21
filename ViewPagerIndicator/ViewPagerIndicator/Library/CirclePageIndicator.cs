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
using Android.Support.V4.View;
using Android.Util;
using Java.Lang;
using Java.Interop;

namespace ViewPagerIndicator
{
	public class CirclePageIndicator : View, PageIndicator
	{
		const int HORIZONTAL = 0;
		const int VERTICAL = 1;
		private float mRadius;
		private Paint mPaintPageFill;
		private Paint mPaintStroke;
		private Paint mPaintFill;
		private ViewPager mViewPager;
		private ViewPager.IOnPageChangeListener mListener;
		private int mCurrentPage;
		private int mSnapPage;
		private int mCurrentOffset;
		private int mScrollState;
		private int mPageSize;
		private int mOrientation;
		private bool mCentered;
		private bool mSnap;
		private const int INVALID_POINTER = -1;
		private int mTouchSlop;
		private float mLastMotionX = -1;
		private int mActivePointerId = INVALID_POINTER;
		private bool mIsDragging;
		
		public CirclePageIndicator (Context context) : this(context, null)
		{
		}
		
		public CirclePageIndicator (Context context, IAttributeSet attrs) : this (context, attrs, Resource.Attribute.vpiCirclePageIndicatorStyle)
		{
		}
		
		public CirclePageIndicator (Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			//Load defaults from resources
			var res = Resources;
			int defaultPageColor = res.GetColor (Resource.Color.default_circle_indicator_page_color);
			int defaultFillColor = res.GetColor (Resource.Color.default_circle_indicator_fill_color);
			int defaultOrientation = res.GetInteger (Resource.Integer.default_circle_indicator_orientation);
			int defaultStrokeColor = res.GetColor (Resource.Color.default_circle_indicator_stroke_color);
			float defaultStrokeWidth = res.GetDimension (Resource.Dimension.default_circle_indicator_stroke_width);
			float defaultRadius = res.GetDimension (Resource.Dimension.default_circle_indicator_radius);
			bool defaultCentered = res.GetBoolean (Resource.Boolean.default_circle_indicator_centered);
			bool defaultSnap = res.GetBoolean (Resource.Boolean.default_circle_indicator_snap);
			
			//Retrieve styles attributes
			var a = context.ObtainStyledAttributes (attrs, Resource.Styleable.CirclePageIndicator, defStyle, Resource.Style.Widget_CirclePageIndicator);
		
			mCentered = a.GetBoolean (Resource.Styleable.CirclePageIndicator_centered, defaultCentered);
			mOrientation = a.GetInt (Resource.Styleable.CirclePageIndicator_orientation, defaultOrientation);
			mPaintPageFill = new Paint (PaintFlags.AntiAlias);
			mPaintPageFill.SetStyle (Paint.Style.Fill);
			mPaintPageFill.Color = a.GetColor (Resource.Styleable.CirclePageIndicator_pageColor, defaultPageColor);
			mPaintStroke = new Paint (PaintFlags.AntiAlias);
			mPaintStroke.SetStyle (Paint.Style.Stroke);
			mPaintStroke.Color = a.GetColor (Resource.Styleable.CirclePageIndicator_strokeColor, defaultStrokeColor);
			mPaintStroke.StrokeWidth = a.GetDimension (Resource.Styleable.CirclePageIndicator_strokeWidth, defaultStrokeWidth);
			mPaintFill = new Paint (PaintFlags.AntiAlias);
			mPaintFill.SetStyle (Paint.Style.Fill);
			mPaintFill.Color = a.GetColor (Resource.Styleable.CirclePageIndicator_fillColor, defaultFillColor);
			mRadius = a.GetDimension (Resource.Styleable.CirclePageIndicator_radius, defaultRadius);
			mSnap = a.GetBoolean (Resource.Styleable.CirclePageIndicator_snap, defaultSnap);
			
			a.Recycle ();
			
			var configuration = ViewConfiguration.Get (context);
			mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop (configuration);
			
		}

		public void SetCentered (bool centered)
		{
			mCentered = centered;
			Invalidate ();
		}
	
		public bool IsCentered ()
		{
			return mCentered;
		}
	
		public void SetPageColor (Color pageColor)
		{
			mPaintPageFill.Color = pageColor;
			Invalidate ();
		}
	
		public int GetPageColor ()
		{
			return mPaintPageFill.Color;
		}
	
		public void SetFillColor (Color fillColor)
		{
			mPaintFill.Color = fillColor;
			Invalidate ();
		}
	
		public int GetFillColor ()
		{
			return mPaintFill.Color;
		}
		
		public void setOrientation (int orientation)
		{
			switch (orientation) {
			case HORIZONTAL:
			case VERTICAL:
				mOrientation = orientation;
				UpdatePageSize ();
				RequestLayout ();
				break;
	
			default:
				throw new IllegalArgumentException ("Orientation must be either HORIZONTAL or VERTICAL.");
			}
		}
	
		public int GetOrientation ()
		{
			return mOrientation;
		}
	
		public void SetStrokeColor (Color strokeColor)
		{
			mPaintStroke.Color = strokeColor;
			Invalidate ();
		}
	
		public int GetStrokeColor ()
		{
			return mPaintStroke.Color;
		}
	
		public void SetStrokeWidth (float strokeWidth)
		{
			mPaintStroke.StrokeWidth = strokeWidth;
			Invalidate ();
		}
	
		public float GetStrokeWidth ()
		{
			return mPaintStroke.StrokeWidth;
		}
	
		public void SetRadius (float radius)
		{
			mRadius = radius;
			Invalidate ();
		}
	
		public float GetRadius ()
		{
			return mRadius;
		}

		public void SetSnap (bool snap)
		{
			mSnap = snap;
			Invalidate ();
		}
	
		public bool IsSnap ()
		{
			return mSnap;
		}
		
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
			
			if (mViewPager == null) {
				return;
			}
			int count = mViewPager.Adapter.Count;
			if (count == 0) {
				return;
			}
	
			if (mCurrentPage >= count) {
				SetCurrentItem (count - 1);
				return;
			}
	
			int longSize;
			int longPaddingBefore;
			int longPaddingAfter;
			int shortPaddingBefore;
			if (mOrientation == HORIZONTAL) {
				longSize = Width;
				longPaddingBefore = PaddingLeft;
				longPaddingAfter = PaddingRight;
				shortPaddingBefore = PaddingTop;
			} else {
				longSize = Height;
				longPaddingBefore = PaddingTop;
				longPaddingAfter = PaddingBottom;
				shortPaddingBefore = PaddingLeft;
			}
	
			float threeRadius = mRadius * 3;
			float shortOffset = shortPaddingBefore + mRadius;
			float longOffset = longPaddingBefore + mRadius;
			if (mCentered) {
				longOffset += ((longSize - longPaddingBefore - longPaddingAfter) / 2.0f) - ((count * threeRadius) / 2.0f);
			}
			
			float dX;
			float dY;
	
			float pageFillRadius = mRadius;
			if (mPaintStroke.StrokeWidth > 0) {
				pageFillRadius -= mPaintStroke.StrokeWidth / 2.0f;
			}
	
			//Draw stroked circles
			for (int iLoop = 0; iLoop < count; iLoop++) {
				float drawLong = longOffset + (iLoop * threeRadius);
				if (mOrientation == HORIZONTAL) {
					dX = drawLong;
					dY = shortOffset;
				} else {
					dX = shortOffset;
					dY = drawLong;
				}
				// Only paint fill if not completely transparent
				if (mPaintPageFill.Alpha > 0) {
					canvas.DrawCircle (dX, dY, pageFillRadius, mPaintPageFill);
				}
	
				// Only paint stroke if a stroke width was non-zero
				if (pageFillRadius != mRadius) {
					canvas.DrawCircle (dX, dY, mRadius, mPaintStroke);
				}
			}
	
			//Draw the filled circle according to the current scroll
			float cx = (mSnap ? mSnapPage : mCurrentPage) * threeRadius;
			if (!mSnap && (mPageSize != 0)) {
				cx += (mCurrentOffset * 1.0f / mPageSize) * threeRadius;
			}
			if (mOrientation == HORIZONTAL) {
				dX = longOffset + cx;
				dY = shortOffset;
			} else {
				dX = shortOffset;
				dY = longOffset + cx;
			}
			canvas.DrawCircle (dX, dY, mRadius, mPaintFill);
		}
		
		public override bool OnTouchEvent (MotionEvent ev)
		{
	        
			if (base.OnTouchEvent (ev)) {
				return true;
			}
			if ((mViewPager == null) || (mViewPager.Adapter.Count == 0)) {
				return false;
			}
	
			var action = ev.Action;
	
			switch ((int)action & MotionEventCompat.ActionMask) {
			case (int) MotionEventActions.Down:
				mActivePointerId = MotionEventCompat.GetPointerId (ev, 0);
				mLastMotionX = ev.GetX ();
				break;
	
			case (int)MotionEventActions.Move: {
					int activePointerIndex = MotionEventCompat.FindPointerIndex (ev, mActivePointerId);
					float x = MotionEventCompat.GetX (ev, activePointerIndex);
					float deltaX = x - mLastMotionX;
	
					if (!mIsDragging) {
						if (Java.Lang.Math.Abs (deltaX) > mTouchSlop) {
							mIsDragging = true;
						}
					}
	
					if (mIsDragging) {
						if (!mViewPager.IsFakeDragging) {
							mViewPager.BeginFakeDrag ();
						}
	
						mLastMotionX = x;
	
						mViewPager.FakeDragBy (deltaX);
					}
	
					break;
				}
	
			case (int)MotionEventActions.Cancel:
			case (int)MotionEventActions.Up:
				if (!mIsDragging) {
					int count = mViewPager.Adapter.Count;
					int width = Width;
					float halfWidth = width / 2f;
					float sixthWidth = width / 6f;
	
					if ((mCurrentPage > 0) && (ev.GetX () < halfWidth - sixthWidth)) {
						mViewPager.CurrentItem = mCurrentPage - 1;
						return true;
					} else if ((mCurrentPage < count - 1) && (ev.GetX () > halfWidth + sixthWidth)) {
						mViewPager.CurrentItem = mCurrentPage + 1;
						return true;
					}
				}
	
				mIsDragging = false;
				mActivePointerId = INVALID_POINTER;
				if (mViewPager.IsFakeDragging)
					mViewPager.EndFakeDrag ();
				break;
	
			case MotionEventCompat.ActionPointerDown: {
					int index = MotionEventCompat.GetActionIndex (ev);
					float x = MotionEventCompat.GetX (ev, index);
					mLastMotionX = x;
					mActivePointerId = MotionEventCompat.GetPointerId (ev, index);
					break;
				}
	
			case MotionEventCompat.ActionPointerUp:
				int pointerIndex = MotionEventCompat.GetActionIndex (ev);
				int pointerId = MotionEventCompat.GetPointerId (ev, pointerIndex);
				if (pointerId == mActivePointerId) {
					int newPointerIndex = pointerIndex == 0 ? 1 : 0;
					mActivePointerId = MotionEventCompat.GetPointerId (ev, newPointerIndex);
				}
				mLastMotionX = MotionEventCompat.GetX (ev, MotionEventCompat.FindPointerIndex (ev, mActivePointerId));
				break;
			}
	
			return true;
		}
		
		public void SetViewPager (ViewPager view)
		{
			if (view.Adapter == null) {
				throw new IllegalStateException ("ViewPager does not have adapter instance.");
			}
			mViewPager = view;
			mViewPager.SetOnPageChangeListener (this);
			UpdatePageSize ();
			Invalidate ();
		}
		
		private void UpdatePageSize ()
		{
			if (mViewPager != null) {
				mPageSize = (mOrientation == HORIZONTAL) ? mViewPager.Width : mViewPager.Height;
			}
		}
		
		public void SetViewPager (ViewPager view, int initialPosition)
		{
			SetViewPager (view);
			SetCurrentItem (initialPosition);
		}
		
		public void SetCurrentItem (int item)
		{
			if (mViewPager == null) {
				throw new IllegalStateException ("ViewPager has not been bound.");
			}
			mViewPager.CurrentItem = item;
			mCurrentPage = item;
			Invalidate ();
		}
		
		public void NotifyDataSetChanged ()
		{
			Invalidate ();
		}
		
		public void OnPageScrollStateChanged (int state)
		{
			mScrollState = state;
	
			if (mListener != null) {
				mListener.OnPageScrollStateChanged (state);
			}
		}
		
		public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
		{
			mCurrentPage = position;
			mCurrentOffset = positionOffsetPixels;
			UpdatePageSize ();
			Invalidate ();
	
			if (mListener != null) {
				mListener.OnPageScrolled (position, positionOffset, positionOffsetPixels);
			}
		}
		
		public void OnPageSelected (int position)
		{
			if (mSnap || mScrollState == ViewPager.ScrollStateIdle) {
				mCurrentPage = position;
				mSnapPage = position;
				Invalidate ();
			}
	
			if (mListener != null) {
				mListener.OnPageSelected (position);
			}
		}
		
		public void SetOnPageChangeListener (ViewPager.IOnPageChangeListener listener)
		{
			mListener = listener;
		}
		
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			if (mOrientation == HORIZONTAL) {
				SetMeasuredDimension (MeasureLong (widthMeasureSpec), MeasureShort (heightMeasureSpec));
			} else {
				SetMeasuredDimension (MeasureShort (widthMeasureSpec), MeasureLong (heightMeasureSpec));
			}
		}
		
		/**
	     * Determines the width of this view
	     *
	     * @param measureSpec
	     *            A measureSpec packed into an int
	     * @return The width of the view, honoring constraints from measureSpec
	     */
		private int MeasureLong (int measureSpec)
		{
			int result = 0;
			var specMode = MeasureSpec.GetMode (measureSpec);
			var specSize = MeasureSpec.GetSize (measureSpec);
	
			if ((specMode == MeasureSpecMode.Exactly) || (mViewPager == null)) {
				//We were told how big to be
				result = specSize;
			} else {
				//Calculate the width according the views count
				int count = mViewPager.Adapter.Count;
				result = (int)(PaddingLeft + PaddingRight
	                    + (count * 2 * mRadius) + (count - 1) * mRadius + 1);
				//Respect AT_MOST value if that was what is called for by measureSpec
				if (specMode == MeasureSpecMode.AtMost) {
					result = Java.Lang.Math.Min (result, specSize);
				}
			}
			return result;
		}
		
		/**
	     * Determines the height of this view
	     *
	     * @param measureSpec
	     *            A measureSpec packed into an int
	     * @return The height of the view, honoring constraints from measureSpec
	     */
		private int MeasureShort (int measureSpec)
		{
			int result = 0;
			var specMode = MeasureSpec.GetMode (measureSpec);
			var specSize = MeasureSpec.GetSize (measureSpec);
	
			if (specMode == MeasureSpecMode.Exactly) {
				//We were told how big to be
				result = specSize;
			} else {
				//Measure the height
				result = (int)(2 * mRadius + PaddingTop + PaddingBottom + 1);
				//Respect AT_MOST value if that was what is called for by measureSpec
				if (specMode == MeasureSpecMode.AtMost) {
					result = Java.Lang.Math.Min (result, specSize);
				}
			}
			return result;
		}
		
		protected override void OnRestoreInstanceState (IParcelable state)
		{
			
			try {
				SavedState savedState = (SavedState)state;
				base.OnRestoreInstanceState (savedState.SuperState);
				mCurrentPage = savedState.CurrentPage;
				mSnapPage = savedState.CurrentPage;
			} catch {
				base.OnRestoreInstanceState (state);
				// Ignore, this needs to support IParcelable...
			}
			RequestLayout ();
		}
		
		protected override IParcelable OnSaveInstanceState ()
		{
			var superState = base.OnSaveInstanceState ();
			var savedState = new SavedState (superState);
			savedState.CurrentPage = mCurrentPage;
			return savedState;
		}
		
		public class SavedState : BaseSavedState
		{
			public int CurrentPage { get; set; }
	
			public SavedState (IParcelable superState) : base(superState)
			{
			}
	
			private SavedState (Parcel parcel) : base(parcel)
			{
				CurrentPage = parcel.ReadInt ();
			}
			
			public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
			{
				base.WriteToParcel (dest, flags);
				dest.WriteInt (CurrentPage);
			}
			
			[ExportField ("CREATOR")]
			static SavedStateCreator InitializeCreator ()
			{
				return new SavedStateCreator ();
			}
			
			class SavedStateCreator : Java.Lang.Object, IParcelableCreator
			{
				public Java.Lang.Object CreateFromParcel (Parcel source)
				{
					return new SavedState (source);
				}
		
				public Java.Lang.Object[] NewArray (int size)
				{
					return new SavedState[size];
				}
			}
		}
	}
}

