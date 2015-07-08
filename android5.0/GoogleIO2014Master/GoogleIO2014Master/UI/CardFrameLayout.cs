
using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android;
using Android.Views;
using Android.Graphics;

namespace GoogleIO2014Master.UI
{
	public class CardFrameLayout : FrameLayout
	{
		float radius;
		int vw;
		int vh;

		public CardFrameLayout (Context context)
			: this (context, null, 0)
		{
		}

		public CardFrameLayout(Context context, IAttributeSet attrs)
			:this(context, attrs, 0)
		{
		}

		public CardFrameLayout(Context context, IAttributeSet attrs, int defStyle)
			:base(context, attrs, defStyle)
		{
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);

			radius = Resources.GetDimensionPixelSize (Resource.Dimension.card_corner_radius);
			vw = w;
			vh = h;
			var vop = new CardProvider (this);
			OutlineProvider = vop;
			ClipToOutline = true;
		}

		private class CardProvider : ViewOutlineProvider
		{
			CardFrameLayout c;
			public CardProvider(CardFrameLayout c)
			{
				this.c=c;
			}
			public override void GetOutline (View view, Outline outline)
			{
				outline.SetRoundRect (0, 0, c.vw, c.vh, c.radius);
			}
		}
	}
}

