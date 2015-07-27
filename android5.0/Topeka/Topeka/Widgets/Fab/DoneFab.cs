using Android.Content;
using Android.Util;

namespace Topeka.Widgets.Fab
{
	public class DoneFab : FloatingActionButton
	{
		public DoneFab (Context context) : this (context, null)
		{
		}

		public DoneFab (Context context, IAttributeSet attrs) : this (context, attrs, 0)
		{
		}

		public DoneFab (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			SetImageResource (Resource.Drawable.ic_tick);
		}
	}
}

