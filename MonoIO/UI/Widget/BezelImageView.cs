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
using Android.Graphics.Drawables;
using Android.Util;

namespace MonoIO
{
	public class BezelImageView : ImageView
	{
		private static string TAG = "BezelImageView";

	    private Paint mMaskedPaint;
	    private Paint mCopyPaint;
	
	    private Rect mBounds;
	    private RectF mBoundsF;
	
	    private Drawable mBorderDrawable;
	    private Drawable mMaskDrawable;
		
		public BezelImageView(Context context) : this(context, null)
		{
	    }
	
	    public BezelImageView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
	    }
	
	    public BezelImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			// Attribute initialization
	        var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.BezelImageView, defStyle, 0);
	
	        mMaskDrawable = a.GetDrawable(Resource.Styleable.BezelImageView_maskDrawable);
	        if (mMaskDrawable == null) {
	            mMaskDrawable = Resources.GetDrawable(Resource.Drawable.bezel_mask);
	        }
	        mMaskDrawable.Callback = this;
	
	        mBorderDrawable = a.GetDrawable(Resource.Styleable.BezelImageView_borderDrawable);
	        if (mBorderDrawable == null) {
	            mBorderDrawable = Resources.GetDrawable(Resource.Drawable.bezel_border);
	        }
	        mBorderDrawable.Callback = this;
	
	        a.Recycle();
	
	        // Other initialization
	        mMaskedPaint = new Paint();
	        mMaskedPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcAtop));
	
	        mCopyPaint = new Paint();
		}
		
		protected override bool SetFrame (int l, int t, int r, int b)
		{
			bool changed = base.SetFrame(l, t, r, b);
	        mBounds = new Rect(0, 0, r - l, b - t);
	        mBoundsF = new RectF(mBounds);
	        mBorderDrawable.Bounds = mBounds;
	        mMaskDrawable.Bounds = mBounds;
	        return changed;
		}
		
		protected override void OnDraw (Canvas canvas)
		{
			int sc = canvas.SaveLayer(mBoundsF, mCopyPaint, SaveFlags.HasAlphaLayer | SaveFlags.FullColorLayer);
	        mMaskDrawable.Draw(canvas);
	        canvas.SaveLayer(mBoundsF, mMaskedPaint, 0);
	        base.OnDraw(canvas);
	        canvas.RestoreToCount(sc);
	        mBorderDrawable.Draw(canvas);
		}
		
		protected override void DrawableStateChanged ()
		{
			base.DrawableStateChanged ();
			if (mBorderDrawable.IsStateful) {
	            mBorderDrawable.SetState(GetDrawableState());
	        }
	        if (mMaskDrawable.IsStateful) {
	            mMaskDrawable.SetState(GetDrawableState());
	        }
	
	        // TODO: is this the right place to invalidate?
	        Invalidate();
		}
	}
}

