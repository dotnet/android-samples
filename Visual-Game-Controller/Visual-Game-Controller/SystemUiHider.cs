using System;
using Android.App;
using Android.OS;
using Android.Views;

namespace VisualGameController
{
	public abstract class SystemUiHider
	{
		public const int FLAG_LAYOUT_IN_SCREEN_OLDER_DEVICES=0x1;

		public const int FLAG_FULLSCREEN=0x2;

		public const int FLAG_HIDE_NAVIGATION=FLAG_FULLSCREEN|0x4;

		public Activity activity;

		public View anchor_view;

		protected int flags;

		public static DummyListener dummy_listener = new DummyListener();

		public OnVisibilityChangeListner on_visibility_change_listener = dummy_listener;

		public static SystemUiHider GetInstance(Activity activity, View anchorView, int flags) {
			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb) {
				return new SystemUiHiderHoneyComb (activity, anchorView, flags);
			} else {
				return new SystemUiHiderBase (activity,anchorView,flags);
			}
		}

		protected SystemUiHider(Activity activity, View anchorView, int flags) {
			this.activity = activity;
			anchor_view = anchorView;
			this.flags = flags;
		}

		public abstract void Setup();

		public abstract bool IsVisible();

		public abstract void Hide ();

		public abstract void Show();

		public void Toggle() {
			if (IsVisible ()) {
				Hide ();
			} else {
				Show ();
			}
		}

		public void SetOnVisibilityChangeListener(OnVisibilityChangeListner listener) {
			if (listener == null) {
				listener = dummy_listener;
			}
			on_visibility_change_listener = listener;
		}

		public interface OnVisibilityChangeListner {
			void OnVisibilityChange(bool visible);
		}

		public class DummyListener : OnVisibilityChangeListner {
			public void OnVisibilityChange(bool visible)
			{
			}
		}
	}
}

