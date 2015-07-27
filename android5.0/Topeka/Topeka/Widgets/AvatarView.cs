using System;

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

using Topeka.Widgets.OutlineProviders;

namespace Topeka
{
	public class AvatarView : ImageView, ICheckable
	{
		bool isChecked;

		public bool Checked {
			get {
				return isChecked;
			}
			set {
				isChecked = value;
				Invalidate ();
			}
		}

		public AvatarView (Context context) : this (context, null)
		{
		}

		public AvatarView (Context context, IAttributeSet attrs) : this (context, attrs, 0)
		{
		}

		public AvatarView (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			ClipToOutline = true;
		}

		public void Toggle ()
		{
			isChecked = !isChecked;
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
			if (isChecked) {
				var border = Resources.GetDrawable (Resource.Drawable.selector_avatar, null);
				border.SetBounds (0, 0, Width, Height);
				border.Draw (canvas);
			}
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			if (w > 0 && h > 0)
				OutlineProvider = new RoundOutlineProvider (Math.Min (w, h));
		}
	}
}

