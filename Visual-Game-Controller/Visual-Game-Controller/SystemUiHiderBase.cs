using System;
using Android.App;
using Android.OS;
using Android.Views;


namespace VisualGameController
{
	public class SystemUiHiderBase : SystemUiHider
	{
		private bool visible = true;

		public SystemUiHiderBase(Activity activity, View anchor_view, int flags) : base(activity,anchor_view,flags) {
		}

		public override void Setup ()
		{
			if ((flags & FLAG_LAYOUT_IN_SCREEN_OLDER_DEVICES) == 0) {
				activity.Window.AddFlags (WindowManagerFlags.LayoutNoLimits);
			}
		}
		public override bool IsVisible ()
		{
			return visible;
		}

		//hide the navigation
		public override void Hide ()
		{
			if ((flags & FLAG_FULLSCREEN) != 0) {
				activity.Window.AddFlags (WindowManagerFlags.Fullscreen);
			}
			on_visibility_change_listener.OnVisibilityChange (false);
			visible = false;
		}

		//show the navigation
		public override void Show ()
		{
			if ((flags & FLAG_FULLSCREEN) != 0) {
				activity.Window.ClearFlags (WindowManagerFlags.Fullscreen);
			}
			on_visibility_change_listener.OnVisibilityChange (true);
			visible = true;
		}


	}
}

