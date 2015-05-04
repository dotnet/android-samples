using Android.Widget;
using Android.Content;
using System;
using Android.Runtime;

namespace AndroidLSamples.Utils
{

	/// <summary>
	/// This is a magical adjustable Image View that derives from MvxImageView
	/// This allows you to specify specific height ratio vs width
	/// So if you do local:ratio=".5" then the height will be half the width
	/// It default to 1 so it is a perfect square.
	/// Also you can blur the image if you enable it as well.
	/// </summary>
	public class HeightAdjustableImageView : ImageView
	{
		private float widthRatio = 1.0f;
		private Context context;

		public HeightAdjustableImageView(Context context, Android.Util.IAttributeSet attrs)
			: base(context, attrs)
		{
			Initialize(context, attrs);
		}

		public HeightAdjustableImageView(Context context)
			: base(context)
		{
			this.Initialize(context, null);
		}

		protected HeightAdjustableImageView(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}


		private void Initialize(Context context, Android.Util.IAttributeSet attrs)
		{
			this.context = context;
			try
			{
				var values = context.ObtainStyledAttributes(attrs, Resource.Styleable.HeightAdjustableImageView);

				widthRatio = values.GetFloat(Resource.Styleable.HeightAdjustableImageView_ratio, 1.0f);
				values.Recycle();
				this.Invalidate();
			}
			catch (Exception ex )
			{
			}

		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			this.SetMeasuredDimension(this.MeasuredWidth, (int)(this.MeasuredWidth * widthRatio));
		}
	
	}
}

