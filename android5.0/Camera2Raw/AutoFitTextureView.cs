
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
using Java.Lang;

namespace Camera2Raw
{
	//[Register("com.xamarin.camera2raw.AutoFitTextureView")]
	/// <summary>
	/// A {@link TextureView} that can be adjusted to a specified aspect ratio.
	/// </summary>
	public class AutoFitTextureView : TextureView
	{
		private int mRatioWidth = 0;
		private int mRatioHeight = 0;

		public AutoFitTextureView (Context context) : this (context, null)
		{
		}

		public AutoFitTextureView (Context context, IAttributeSet attrs) :
			this (context, attrs, 0)
		{
		}

		public AutoFitTextureView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
		}

		/// <summary>
		/// Sets the aspect ratio for this view. The size of the view will be measured based on the ratio
		/// calculated from the parameters. Note that the actual sizes of parameters don't matter, that
		/// is, calling setAspectRatio(2, 3) and setAspectRatio(4, 6) make the same result.
		/// </summary>
		/// <param name="width">Relative horizontal size.</param>
		/// <param name="height">Relative vertical size.</param>
		public void SetAspectRatio (int width, int height)
		{
			if (width < 0 || height < 0)
				throw new IllegalArgumentException ("Size cannot be negative.");
			
			if (mRatioWidth == width && mRatioHeight == height)
				return;
			
			mRatioWidth = width;
			mRatioHeight = height;
			RequestLayout();
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);

			int width = MeasureSpec.GetSize (widthMeasureSpec);
			int height = MeasureSpec.GetSize (heightMeasureSpec);

			if (mRatioWidth == 0 || mRatioHeight == 0) {
				SetMeasuredDimension (width, height);
			} else {
				if (width < height * mRatioWidth / mRatioHeight) {
					SetMeasuredDimension (width, width * mRatioHeight / mRatioWidth);
				} else {
					SetMeasuredDimension (height * mRatioWidth / mRatioHeight, height);
				}
			}
		}
	}
}