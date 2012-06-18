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
using Android.Util;
using MonoIO.Utilities;
using Android.Text.Format;

namespace MonoIO
{
	/**
	 * Custom layout that contains and organizes a {@link TimeRulerView} and several
	 * instances of {@link BlockView}. Also positions current "now" divider using
	 * {@link R.id#blocks_now} view when applicable.
	 */
	public class BlocksLayout : ViewGroup
	{
		private int mColumns = 3;
	
	    private TimeRulerView mRulerView;
	    private View mNowView;
	
	    public BlocksLayout(Context context) : this(context, null)
		{
	    }
	
	    public BlocksLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
	    }
	
	    public BlocksLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.BlocksLayout, defStyle, 0);
	
	        mColumns = a.GetInt(Resource.Styleable.TimeRulerView_headerWidth, mColumns);
	
	        a.Recycle();
	    }
		
		private void EnsureChildren() {
	        mRulerView = FindViewById<TimeRulerView>(Resource.Id.blocks_ruler);
	        if (mRulerView == null) {
	            throw new Exception("Must include a Resource.Id.blocks_ruler view.");
	        }
	        mRulerView.DrawingCacheEnabled = true;
	
	        mNowView = FindViewById(Resource.Id.blocks_now);
	        if (mNowView == null) {
	            throw new Exception("Must include a Resource.Id.blocks_now view.");
	        }
	        mNowView.DrawingCacheEnabled = true;
	    }
		
		/**
	     * Remove any {@link BlockView} instances, leaving only
	     * {@link TimeRulerView} remaining.
	     */
	    public void RemoveAllBlocks() {
	        EnsureChildren();
	        RemoveAllViews();
	        AddView(mRulerView);
	        AddView(mNowView);
	    }
		
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			EnsureChildren();

	        mRulerView.Measure(widthMeasureSpec, heightMeasureSpec);
	        mNowView.Measure(widthMeasureSpec, heightMeasureSpec);
	
	        int width = mRulerView.	MeasuredWidth;
	        int height = mRulerView.MeasuredHeight;
	
	        SetMeasuredDimension(ResolveSize(width, widthMeasureSpec), ResolveSize(height, heightMeasureSpec));
		}
		
		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			EnsureChildren();

	        TimeRulerView rulerView = mRulerView;
	        int headerWidth = rulerView.GetHeaderWidth();
	        int columnWidth = (Width - headerWidth) / mColumns;
	
	        rulerView.Layout(0, 0, Width, Height);
	
	        int count = ChildCount;
	        for (int i = 0; i < count; i++) {
	            View child = GetChildAt(i);
	            if (child.Visibility == ViewStates.Gone) continue;
	
	            if (child is BlockView) {
	                BlockView blockView = (BlockView) child;
	                int top = rulerView.GetTimeVerticalOffset(blockView.GetStartTime());
	                int bottom = rulerView.GetTimeVerticalOffset(blockView.GetEndTime());
	                int left = headerWidth + (blockView.GetColumn() * columnWidth);
	                int right = left + columnWidth;
	                child.Layout(left, top, right, bottom);
	            }
	        }
	
	        // Align now view to match current time
	        View nowView = mNowView;
	        long now = UIUtils.GetCurrentTime(Context);
	
	        int top2 = rulerView.GetTimeVerticalOffset(now);
	        int bottom2 = top2 + nowView.MeasuredHeight;
	        int left2 = 0;
	        int right2 = Width;
	
	        nowView.Layout(left2, top2, right2, bottom2);
		}
	}
}

