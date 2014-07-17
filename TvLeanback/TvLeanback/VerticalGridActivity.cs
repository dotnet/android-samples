using System;
using Android.App;
using Android.OS;

namespace TvLeanback
{
	[Activity (Label = "VerticalGridActivity", Exported = true, ParentActivity = typeof(MainActivity))]
	public class VerticalGridActivity: Activity {
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.vertical_grid);
			Window.SetBackgroundDrawableResource (Resource.Drawable.grid_bg);
		}
	}
}

