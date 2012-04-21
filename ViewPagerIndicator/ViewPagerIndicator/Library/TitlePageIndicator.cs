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
	public enum IndicatorStyle
	{
		None = 0, 
		Triangle = 1, 
		Underline = 2
	}
	
	public class TitlePageIndicator : View, PageIndicator
	{
		/**
	     * Percentage indicating what percentage of the screen width away from
	     * center should the underline be fully faded. A value of 0.25 means that
	     * halfway between the center of the screen and an edge.
	     */
		const float SELECTION_FADE_PERCENTAGE = 0.25f;
		
		/**
	     * Percentage indicating what percentage of the screen width away from
	     * center should the selected text bold turn off. A value of 0.05 means
	     * that 10% between the center and an edge.
	     */
		const float BOLD_FADE_PERCENTAGE = 0.05f;
		
		/**
	     * Interface for a callback when the center item has been clicked.
	     */
		public interface OnCenterItemClickListener
		{
			/**
	         * Callback when the center item has been clicked.
	         *
	         * @param position Position of the current center item.
	         */
			void OnCenterItemClick (int position);
		}
		
		private ViewPager mViewPager;
		private ViewPager.IOnPageChangeListener mListener;
		private TitleProvider mTitleProvider;
		private int mCurrentPage;
		private int mCurrentOffset;
		private int mScrollState;
		private Paint mPaintText = new Paint ();
		private bool mBoldText;
		private Color mColorText;
		private Color mColorSelected;
		private Path mPath;
		private Paint mPaintFooterLine = new Paint ();
		private IndicatorStyle mFooterIndicatorStyle;
		private Paint mPaintFooterIndicator = new Paint ();
		private float mFooterIndicatorHeight;
		private float mFooterIndicatorUnderlinePadding;
		private float mFooterPadding;
		private float mTitlePadding;
		private float mTopPadding;
		/** Left and right side padding for not active view titles. */
		private float mClipPadding;
		private float mFooterLineHeight;
		private const int INVALID_POINTER = -1;
		private int mTouchSlop;
		private float mLastMotionX = -1;
		private int mActivePointerId = INVALID_POINTER;
		private bool mIsDragging;
		private OnCenterItemClickListener mCenterItemClickListener;
		
		public TitlePageIndicator (Context context) : this(context, null)
		{
		}
		
		public TitlePageIndicator (Context context, IAttributeSet attrs) : this (context, attrs, Resource.Attribute.vpiTitlePageIndicatorStyle)
		{
		}
		
		public TitlePageIndicator (Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			//Load defaults from resources
			var res = Resources;
			int defaultFooterColor = res.GetColor (Resource.Color.default_title_indicator_footer_color);
			float defaultFooterLineHeight = res.GetDimension (Resource.Dimension.default_title_indicator_footer_line_height);
			int defaultFooterIndicatorStyle = res.GetInteger (Resource.Integer.default_title_indicator_footer_indicator_style);
			float defaultFooterIndicatorHeight = res.GetDimension (Resource.Dimension.default_title_indicator_footer_indicator_height);
			float defaultFooterIndicatorUnderlinePadding = res.GetDimension (Resource.Dimension.default_title_indicator_footer_indicator_underline_padding);
			float defaultFooterPadding = res.GetDimension (Resource.Dimension.default_title_indicator_footer_padding);
			int defaultSelectedColor = res.GetColor (Resource.Color.default_title_indicator_selected_color);
			bool defaultSelectedBold = res.GetBoolean (Resource.Boolean.default_title_indicator_selected_bold);
			int defaultTextColor = res.GetColor (Resource.Color.default_title_indicator_text_color);
			float defaultTextSize = res.GetDimension (Resource.Dimension.default_title_indicator_text_size);
			float defaultTitlePadding = res.GetDimension (Resource.Dimension.default_title_indicator_title_padding);
			float defaultClipPadding = res.GetDimension (Resource.Dimension.default_title_indicator_clip_padding);
			float defaultTopPadding = res.GetDimension (Resource.Dimension.default_title_indicator_top_padding);
	
			//Retrieve styles attributes
			var a = context.ObtainStyledAttributes (attrs, Resource.Styleable.TitlePageIndicator, defStyle, Resource.Style.Widget_TitlePageIndicator);
	
			//Retrieve the colors to be used for this view and apply them.
			mFooterLineHeight = a.GetDimension (Resource.Styleable.TitlePageIndicator_footerLineHeight, defaultFooterLineHeight);
			mFooterIndicatorStyle = (IndicatorStyle)a.GetInteger (Resource.Styleable.TitlePageIndicator_footerIndicatorStyle, defaultFooterIndicatorStyle);
			mFooterIndicatorHeight = a.GetDimension (Resource.Styleable.TitlePageIndicator_footerIndicatorHeight, defaultFooterIndicatorHeight);
			mFooterIndicatorUnderlinePadding = a.GetDimension (Resource.Styleable.TitlePageIndicator_footerIndicatorUnderlinePadding, defaultFooterIndicatorUnderlinePadding);
			mFooterPadding = a.GetDimension (Resource.Styleable.TitlePageIndicator_footerPadding, defaultFooterPadding);
			mTopPadding = a.GetDimension (Resource.Styleable.TitlePageIndicator_topPadding, defaultTopPadding);
			mTitlePadding = a.GetDimension (Resource.Styleable.TitlePageIndicator_titlePadding, defaultTitlePadding);
			mClipPadding = a.GetDimension (Resource.Styleable.TitlePageIndicator_clipPadding, defaultClipPadding);
			mColorSelected = a.GetColor (Resource.Styleable.TitlePageIndicator_selectedColor, defaultSelectedColor);
			mColorText = a.GetColor (Resource.Styleable.TitlePageIndicator_textColor, defaultTextColor);
			mBoldText = a.GetBoolean (Resource.Styleable.TitlePageIndicator_selectedBold, defaultSelectedBold);
	
			float textSize = a.GetDimension (Resource.Styleable.TitlePageIndicator_textSize, defaultTextSize);
			Color footerColor = a.GetColor (Resource.Styleable.TitlePageIndicator_footerColor, defaultFooterColor);
			mPaintText.TextSize = textSize;
			mPaintText.AntiAlias = true;
			mPaintFooterLine.SetStyle (Android.Graphics.Paint.Style.FillAndStroke);
			mPaintFooterLine.StrokeWidth = mFooterLineHeight;
			mPaintFooterLine.Color = footerColor;
			mPaintFooterIndicator.SetStyle (Android.Graphics.Paint.Style.FillAndStroke);
			mPaintFooterIndicator.Color = footerColor;
	
			a.Recycle ();
	
			var configuration = ViewConfiguration.Get (context);
			mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop (configuration);
				
		}
		
		public int GetFooterColor ()
		{
			return mPaintFooterLine.Color;
		}

		public void SetFooterColor (Color footerColor)
		{
			mPaintFooterLine.Color = footerColor;
			mPaintFooterIndicator.Color = footerColor;
			Invalidate ();
		}
		
		public float GetFooterLineHeight ()
		{
			return mFooterLineHeight;
		}
	
		public void SetFooterLineHeight (float footerLineHeight)
		{
			mFooterLineHeight = footerLineHeight;
			mPaintFooterLine.StrokeWidth = mFooterLineHeight;
			Invalidate ();
		}
	
		public float GetFooterIndicatorHeight ()
		{
			return mFooterIndicatorHeight;
		}
	
		public void SetFooterIndicatorHeight (float footerTriangleHeight)
		{
			mFooterIndicatorHeight = footerTriangleHeight;
			Invalidate ();
		}
	
		public float GetFooterIndicatorPadding ()
		{
			return mFooterPadding;
		}
	
		public void SetFooterIndicatorPadding (float footerIndicatorPadding)
		{
			mFooterPadding = footerIndicatorPadding;
			Invalidate ();
		}
	
		public IndicatorStyle GetFooterIndicatorStyle ()
		{
			return mFooterIndicatorStyle;
		}
	
		public void SetFooterIndicatorStyle (IndicatorStyle indicatorStyle)
		{
			mFooterIndicatorStyle = indicatorStyle;
			Invalidate ();
		}
	
		public Color GetSelectedColor ()
		{
			return mColorSelected;
		}
	
		public void SetSelectedColor (Color selectedColor)
		{
			mColorSelected = selectedColor;
			Invalidate ();
		}
	
		public bool IsSelectedBold ()
		{
			return mBoldText;
		}
	
		public void SetSelectedBold (bool selectedBold)
		{
			mBoldText = selectedBold;
			Invalidate ();
		}
		
		public int GetTextColor ()
		{
			return mColorText;
		}
	
		public void SetTextColor (Color textColor)
		{
			mPaintText.Color = textColor;
			mColorText = textColor;
			Invalidate ();
		}
	
		public float GetTextSize ()
		{
			return mPaintText.TextSize;
		}
	
		public void SetTextSize (float textSize)
		{
			mPaintText.TextSize = textSize;
			Invalidate ();
		}
	
		public float GetTitlePadding ()
		{
			return this.mTitlePadding;
		}
	
		public void SetTitlePadding (float titlePadding)
		{
			mTitlePadding = titlePadding;
			Invalidate ();
		}
	
		public float GetTopPadding ()
		{
			return this.mTopPadding;
		}
	
		public void SetTopPadding (float topPadding)
		{
			mTopPadding = topPadding;
			Invalidate ();
		}
	
		public float GetClipPadding ()
		{
			return this.mClipPadding;
		}
	
		public void SetClipPadding (float clipPadding)
		{
			mClipPadding = clipPadding;
			Invalidate ();
		}
	
		public void SetTypeface (Typeface typeface)
		{
			mPaintText.SetTypeface (typeface);
			Invalidate ();
		}
	
		public Typeface GetTypeface ()
		{
			return mPaintText.Typeface;
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
	
			//Calculate views bounds
			var bounds = CalculateAllBounds (mPaintText);
			int boundsSize = bounds.Count;
	
			//Make sure we're on a page that still exists
			if (mCurrentPage >= boundsSize) {
				SetCurrentItem (boundsSize - 1);
				return;
			}
	
			int countMinusOne = count - 1;
			float halfWidth = Width / 2f;
			int left = Left;
			float leftClip = left + mClipPadding;
			int width = Width;
			int height = Height;
			int right = left + width;
			float rightClip = right - mClipPadding;
	
			int page = mCurrentPage;
			float offsetPercent;
			if (mCurrentOffset <= halfWidth) {
				offsetPercent = 1.0f * mCurrentOffset / width;
			} else {
				page += 1;
				offsetPercent = 1.0f * (width - mCurrentOffset) / width;
			}
			bool currentSelected = (offsetPercent <= SELECTION_FADE_PERCENTAGE);
			bool currentBold = (offsetPercent <= BOLD_FADE_PERCENTAGE);
			float selectedPercent = (SELECTION_FADE_PERCENTAGE - offsetPercent) / SELECTION_FADE_PERCENTAGE;
	
			//Verify if the current view must be clipped to the screen
			RectF curPageBound = bounds [mCurrentPage];
			float curPageWidth = curPageBound.Right - curPageBound.Left;
			if (curPageBound.Left < leftClip) {
				//Try to clip to the screen (left side)
				ClipViewOnTheLeft (curPageBound, curPageWidth, left);
			}
			if (curPageBound.Right > rightClip) {
				//Try to clip to the screen (right side)
				ClipViewOnTheRight (curPageBound, curPageWidth, right);
			}
	
			//Left views starting from the current position
			if (mCurrentPage > 0) {
				for (int i = mCurrentPage - 1; i >= 0; i--) {
					RectF bound = bounds [i];
					//Is left side is outside the screen
					if (bound.Left < leftClip) {
						float w = bound.Right - bound.Left;
						//Try to clip to the screen (left side)
						ClipViewOnTheLeft (bound, w, left);
						//Except if there's an intersection with the right view
						RectF rightBound = bounds [i + 1];
						//Intersection
						if (bound.Right + mTitlePadding > rightBound.Left) {
							bound.Left = rightBound.Left - w - mTitlePadding;
							bound.Right = bound.Left + w;
						}
					}
				}
			}
			//Right views starting from the current position
			if (mCurrentPage < countMinusOne) {
				for (int i = mCurrentPage + 1; i < count; i++) {
					RectF bound = bounds [i];
					//If right side is outside the screen
					if (bound.Right > rightClip) {
						float w = bound.Right - bound.Left;
						//Try to clip to the screen (right side)
						ClipViewOnTheRight (bound, w, right);
						//Except if there's an intersection with the left view
						RectF leftBound = bounds [i - 1];
						//Intersection
						if (bound.Left - mTitlePadding < leftBound.Right) {
							bound.Left = leftBound.Right + mTitlePadding;
							bound.Right = bound.Left + w;
						}
					}
				}
			}
	
			//Now draw views
			//int colorTextAlpha = mColorText >>> 24;
			int colorTextAlpha = mColorText >> 24;
			for (int i = 0; i < count; i++) {
				//Get the title
				RectF bound = bounds [i];
				//Only if one side is visible
				if ((bound.Left > left && bound.Left < right) || (bound.Right > left && bound.Right < right)) {
					bool currentPage = (i == page);
					//Only set bold if we are within bounds
					mPaintText.FakeBoldText = (currentPage && currentBold && mBoldText);
	
					//Draw text as unselected
					mPaintText.Color = (mColorText);
					if (currentPage && currentSelected) {
						//Fade out/in unselected text as the selected text fades in/out
						mPaintText.Alpha = (colorTextAlpha - (int)(colorTextAlpha * selectedPercent));
					}
					canvas.DrawText (mTitleProvider.GetTitle (i), bound.Left, bound.Bottom + mTopPadding, mPaintText);
	
					//If we are within the selected bounds draw the selected text
					if (currentPage && currentSelected) {
						mPaintText.Color = mColorSelected;
						mPaintText.Alpha = ((int)((mColorSelected >> 24) * selectedPercent));
						canvas.DrawText (mTitleProvider.GetTitle (i), bound.Left, bound.Bottom + mTopPadding, mPaintText);
					}
				}
			}
	
			//Draw the footer line
			mPath = new Path ();
			mPath.MoveTo (0, height - mFooterLineHeight / 2f);
			mPath.LineTo (width, height - mFooterLineHeight / 2f);
			mPath.Close ();
			canvas.DrawPath (mPath, mPaintFooterLine);
	
			switch (mFooterIndicatorStyle) {
			case IndicatorStyle.Triangle:
				mPath = new Path ();
				mPath.MoveTo (halfWidth, height - mFooterLineHeight - mFooterIndicatorHeight);
				mPath.LineTo (halfWidth + mFooterIndicatorHeight, height - mFooterLineHeight);
				mPath.LineTo (halfWidth - mFooterIndicatorHeight, height - mFooterLineHeight);
				mPath.Close ();
				canvas.DrawPath (mPath, mPaintFooterIndicator);
				break;
	
			case IndicatorStyle.Underline:
				if (!currentSelected || page >= boundsSize) {
					break;
				}
	
				RectF underlineBounds = bounds [page];
				mPath = new Path ();
				mPath.MoveTo (underlineBounds.Left - mFooterIndicatorUnderlinePadding, height - mFooterLineHeight);
				mPath.LineTo (underlineBounds.Right + mFooterIndicatorUnderlinePadding, height - mFooterLineHeight);
				mPath.LineTo (underlineBounds.Right + mFooterIndicatorUnderlinePadding, height - mFooterLineHeight - mFooterIndicatorHeight);
				mPath.LineTo (underlineBounds.Left - mFooterIndicatorUnderlinePadding, height - mFooterLineHeight - mFooterIndicatorHeight);
				mPath.Close ();
	
				mPaintFooterIndicator.Alpha = ((int)(0xFF * selectedPercent));
				canvas.DrawPath (mPath, mPaintFooterIndicator);
				mPaintFooterIndicator.Alpha = (0xFF);
				break;
			}
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
					float leftThird = halfWidth - sixthWidth;
					float rightThird = halfWidth + sixthWidth;
					float eventX = ev.GetX ();

					if (eventX < leftThird) {
						if (mCurrentPage > 0) {
							mViewPager.CurrentItem = mCurrentPage - 1;
							return true;
						}
					} else if (eventX > rightThird) {
						if (mCurrentPage < count - 1) {
							mViewPager.CurrentItem = mCurrentPage + 1;
							return true;
						}
					} else {
						//Middle third
						if (mCenterItemClickListener != null) {
							mCenterItemClickListener.OnCenterItemClick (mCurrentPage);
						}
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
		
		/**
	     * Set bounds for the right textView including clip padding.
	     *
	     * @param curViewBound
	     *            current bounds.
	     * @param curViewWidth
	     *            width of the view.
	     */
		private void ClipViewOnTheRight (RectF curViewBound, float curViewWidth, int right)
		{
			curViewBound.Right = right - mClipPadding;
			curViewBound.Left = curViewBound.Right - curViewWidth;
		}
	
		/**
	     * Set bounds for the left textView including clip padding.
	     *
	     * @param curViewBound
	     *            current bounds.
	     * @param curViewWidth
	     *            width of the view.
	     */
		private void ClipViewOnTheLeft (RectF curViewBound, float curViewWidth, int left)
		{
			curViewBound.Left = left + mClipPadding;
			curViewBound.Right = mClipPadding + curViewWidth;
		}
		
		/**
	     * Calculate views bounds and scroll them according to the current index
	     *
	     * @param paint
	     * @param currentIndex
	     * @return
	     */
		private List<RectF> CalculateAllBounds (Paint paint)
		{
			var list = new List<RectF> ();
			//For each views (If no values then add a fake one)
			int count = mViewPager.Adapter.Count;
			int width = Width;
			int halfWidth = width / 2;

			for (int i = 0; i < count; i++) {
				RectF bounds = CalcBounds (i, paint);
				float w = (bounds.Right - bounds.Left);
				float h = (bounds.Bottom - bounds.Top);
				bounds.Left = (halfWidth) - (w / 2) - mCurrentOffset + ((i - mCurrentPage) * width);
				bounds.Right = bounds.Left + w;
				bounds.Top = 0;
				bounds.Bottom = h;
				list.Add (bounds);
			}
	
			return list;
		}
		
		/**
	     * Calculate the bounds for a view's title
	     *
	     * @param index
	     * @param paint
	     * @return
	     */
		private RectF CalcBounds (int index, Paint paint)
		{
			//Calculate the text bounds
			RectF bounds = new RectF ();
			bounds.Right = paint.MeasureText (mTitleProvider.GetTitle (index));
			bounds.Bottom = paint.Descent () - paint.Ascent ();
			return bounds;
		}
		
		public void SetViewPager (ViewPager view)
		{
			var adapter = view.Adapter;
			if (adapter == null) {
				throw new IllegalStateException ("ViewPager does not have adapter instance.");
			}
			if (!(adapter is TitleProvider)) {
				throw new IllegalStateException ("ViewPager adapter must implement TitleProvider to be used with TitlePageIndicator.");
			}
			mViewPager = view;
			mViewPager.SetOnPageChangeListener (this);
			mTitleProvider = (TitleProvider)adapter;
			Invalidate ();
		}
		
		public void SetViewPager (ViewPager view, int initialPosition)
		{
			SetViewPager (view);
			SetCurrentItem (initialPosition);
		}
		
		public void NotifyDataSetChanged ()
		{
			Invalidate ();
		}
		
		/**
	     * Set a callback listener for the center item click.
	     *
	     * @param listener Callback instance.
	     */
		public void SetOnCenterItemClickListener (OnCenterItemClickListener listener)
		{
			mCenterItemClickListener = listener;
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
			Invalidate ();
	
			if (mListener != null) {
				mListener.OnPageScrolled (position, positionOffset, positionOffsetPixels);
			}
		}
		
		public void OnPageSelected (int position)
		{
			if (mScrollState == ViewPager.ScrollStateIdle) {
				mCurrentPage = position;
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
			//Measure our width in whatever mode specified
			int measuredWidth = MeasureSpec.GetSize (widthMeasureSpec);
	
			//Determine our height
			float height = 0;
			var heightSpecMode = MeasureSpec.GetMode (heightMeasureSpec);
			if (heightSpecMode == MeasureSpecMode.Exactly) {
				//We were told how big to be
				height = MeasureSpec.GetSize (heightMeasureSpec);
			} else {
				//Calculate the text bounds
				RectF bounds = new RectF ();
				bounds.Bottom = mPaintText.Descent () - mPaintText.Ascent ();
				height = bounds.Bottom - bounds.Top + mFooterLineHeight + mFooterPadding + mTopPadding;
				if (mFooterIndicatorStyle != IndicatorStyle.None) {
					height += mFooterIndicatorHeight;
				}
			}
			int measuredHeight = (int)height;
	
			SetMeasuredDimension (measuredWidth, measuredHeight);
		}
		
		protected override void OnRestoreInstanceState (IParcelable state)
		{
			try {
				SavedState savedState = (SavedState)state;
				base.OnRestoreInstanceState (savedState.SuperState);
				mCurrentPage = savedState.CurrentPage; 
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
	
			public SavedState (Parcel parcel) : base(parcel)
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

