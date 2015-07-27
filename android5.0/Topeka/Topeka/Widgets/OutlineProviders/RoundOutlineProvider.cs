using System;

using Android.Views;
using Android.Graphics;

namespace Topeka.Widgets.OutlineProviders
{
	public class RoundOutlineProvider : ViewOutlineProvider
	{
		int size;

		public RoundOutlineProvider(int size)
		{
			if (size < 0)
				throw new InvalidOperationException ("size needs to be > 0. Actually was " + size);
			this.size = size;
		}

		public override void GetOutline (View view, Outline outline)
		{
			outline.SetOval (0, 0, size, size);
		}
	}
}

