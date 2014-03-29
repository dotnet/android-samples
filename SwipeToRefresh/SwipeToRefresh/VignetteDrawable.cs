using System;

using Android.Graphics;
using Android.Graphics.Drawables;

namespace SwipeToRefresh
{
	public class VignetteDrawable : Drawable
	{
		bool useGradientOverlay;
		float mCornerRadius;
		RectF mRect = new RectF ();
		BitmapShader bitmapShader;
		Paint paint;
		Paint strokePaint;
		int margin;

		public VignetteDrawable (Bitmap bitmap, float cornerRadius = 5, int margin = 3, bool withEffect = true)
		{
			useGradientOverlay = withEffect;
			mCornerRadius = cornerRadius;

			bitmapShader = new BitmapShader (bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

			paint = new Paint () {
				AntiAlias = true,
			};
			paint.SetShader (bitmapShader);
			strokePaint = new Paint () {
				Color = Color.Rgb (200, 200, 200),
				StrokeWidth = 2,
			};
			strokePaint.SetStyle (Paint.Style.FillAndStroke);

			this.margin = margin;
		}

		protected override void OnBoundsChange (Rect bounds)
		{
			base.OnBoundsChange (bounds);
			mRect.Set (margin, margin, bounds.Width () - margin, bounds.Height () - margin);

			if (useGradientOverlay) {
				var colors = new int[] { 0, 0, 0x20000000 };
				var pos = new float[] { 0.0f, 0.7f, 1.0f };
				RadialGradient vignette = new RadialGradient (mRect.CenterX (),
				                                              mRect.CenterY () * 1.0f / 0.7f,
				                                              mRect.CenterX () * 1.3f,
				                                              colors,
				                                              pos, Shader.TileMode.Clamp);

				Matrix oval = new Matrix ();
				oval.SetScale (1.0f, 0.7f);
				vignette.SetLocalMatrix (oval);

				paint.SetShader (new ComposeShader (bitmapShader, vignette, PorterDuff.Mode.SrcOver));
			}
		}

		public override void Draw (Canvas canvas)
		{
			canvas.DrawRoundRect (mRect, mCornerRadius, mCornerRadius, strokePaint);
			canvas.DrawRoundRect (mRect, mCornerRadius, mCornerRadius, paint);
		}

		public override int Opacity {
			get {
				return (int)Format.Translucent;
			}
		}

		public override void SetAlpha (int alpha)
		{
			paint.Alpha = alpha;
		}

		public override void SetColorFilter (ColorFilter cf)
		{
			paint.SetColorFilter (cf);
		}
	}
}

