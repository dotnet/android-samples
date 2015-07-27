using System;

using Android.Content;
using Android.Util;
using Android.Widget;

using Topeka.Widgets.OutlineProviders;

namespace Topeka.Widgets.Fab
{
	public class FloatingActionButton : ImageView
	{
		public FloatingActionButton (Context context) : this (context, null)
		{
		}

		public FloatingActionButton (Context context, IAttributeSet attrs) : this (context, attrs, 0)
		{
		}

		public FloatingActionButton (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			Focusable = true;
			Clickable = true;
			ClipToOutline = true;
			SetScaleType (ScaleType.CenterInside);
			SetBackgroundResource (Resource.Drawable.fab_background);
			Elevation = Resources.GetDimension (Resource.Dimension.elevation_fab);
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			if (w > 0 && h > 0)
				OutlineProvider = new RoundOutlineProvider (Math.Min (w, h));
		}
	}
}

