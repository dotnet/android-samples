
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

namespace Camera2VideoSample
{
	public class AutoFitTextureView : TextureView
	{
		private int ratioWidth = 0;
		private int ratioHeight = 0;

		public AutoFitTextureView (Context context) : this(context,null)
		{
		}

		public AutoFitTextureView (Context context, IAttributeSet attrs) :
		this(context, attrs,0)
		{
		}

		public AutoFitTextureView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{

		}

		public void SetAspectRatio(int width,int height) {
			if (width < 0 || height < 0)
				throw new Exception ("Size cannot be negative.");
			ratioWidth = width;
			ratioHeight = height;
			RequestLayout ();
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			int width = MeasureSpec.GetSize (widthMeasureSpec);
			int height = MeasureSpec.GetSize (heightMeasureSpec);
			if (0 == ratioWidth || 0 == ratioHeight)
				SetMeasuredDimension (width, height);
			else {
				if (width < height * ratioWidth / ratioHeight) {
					SetMeasuredDimension (width, width * ratioHeight / ratioWidth);
				} else {
					SetMeasuredDimension (height * ratioWidth / ratioHeight, height);
				}
			}
		}
	}
}

