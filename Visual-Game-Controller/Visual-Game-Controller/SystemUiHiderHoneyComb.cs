using System;
using Android.Annotation;
using Android.App;
using Android.OS;
using Android.Views;
using VisualGameController;

namespace VisualGameController
{
	public class SystemUiHiderHoneyComb : SystemUiHider
	{
		public int show_flags;
		private int hide_flags;
		public int test_flags;

		private MyOnSystemUiChangeListener system_visibility_change_listener;

		public bool visible = true;

		public SystemUiHiderHoneyComb (Activity activity, View anchor_view, int flags) : base(activity,anchor_view,flags)
		{
			system_visibility_change_listener = new MyOnSystemUiChangeListener(this);
			show_flags = (int)SystemUiFlags.Visible;
			hide_flags = (int)SystemUiFlags.LowProfile;
			test_flags = (int)SystemUiFlags.LowProfile;

			if ((flags & FLAG_FULLSCREEN) != 0) {

				show_flags |= (int)SystemUiFlags.LayoutFullscreen;
				hide_flags |= (int)SystemUiFlags.LayoutFullscreen | (int)SystemUiFlags.Fullscreen;
			}

			if ((flags & FLAG_HIDE_NAVIGATION) != 0) {

				show_flags |= (int)SystemUiFlags.LayoutHideNavigation;
				hide_flags |= (int)SystemUiFlags.LayoutHideNavigation | (int)SystemUiFlags.HideNavigation;
				test_flags |= (int)SystemUiFlags.HideNavigation;
			}
		}

		public override void Setup ()
		{
			anchor_view.SetOnSystemUiVisibilityChangeListener (system_visibility_change_listener);
		}

		public override bool IsVisible ()
		{
			return visible;
		}

		public override void Hide ()
		{
			anchor_view.SystemUiVisibility = (StatusBarVisibility)hide_flags;
		}

		public override void Show ()
		{
			anchor_view.SystemUiVisibility = (StatusBarVisibility)show_flags;
		}


	}

	public class MyOnSystemUiChangeListener : Java.Lang.Object,View.IOnSystemUiVisibilityChangeListener
	{
		SystemUiHiderHoneyComb hider;
		public MyOnSystemUiChangeListener(SystemUiHiderHoneyComb hider)
		{
			this.hider = hider;
		}
			
		public void OnSystemUiVisibilityChange(StatusBarVisibility vis) {
			if (((int)vis & hider.test_flags) != 0) {
				if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean) {
					hider.activity.ActionBar.Hide ();
					//hider.activity.Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
					hider.activity.Window.AddFlags (WindowManagerFlags.Fullscreen);
				}

				hider.on_visibility_change_listener.OnVisibilityChange (false);
				hider.visible = false;
			} else {
				hider.anchor_view.SystemUiVisibility = (StatusBarVisibility)hider.show_flags;
				if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean) {
					hider.activity.Window.ClearFlags (WindowManagerFlags.Fullscreen);
					hider.activity.ActionBar.Show ();
					//hider.activity.Window.SetFlags (0, WindowManagerFlags.Fullscreen);

				}

				hider.on_visibility_change_listener.OnVisibilityChange (true);
				hider.visible = true;
			}
		}
			
	}
		

}

