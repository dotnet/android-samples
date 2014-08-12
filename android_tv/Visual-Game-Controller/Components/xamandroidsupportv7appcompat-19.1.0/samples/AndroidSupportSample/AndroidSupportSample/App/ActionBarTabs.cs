using System;
using Android.Support.V7.App;
using Android.OS;
using Android.Views;
using Java.Interop;

namespace AndroidSupportSample
{
	[Android.App.Activity (Label = "@string/action_bar_tabs", Theme = "@style/Theme.AppCompat")]			
	public class ActionBarTabs : ActionBarActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.action_bar_tabs);
		}

		[Export ("onAddTab")]
		public void OnAddTab (View v)
		{
			var bar = SupportActionBar;
			int tabCount = bar.TabCount;
			string text = "Tab " + tabCount;

			bar.AddTab (bar.NewTab ()
				.SetText (text)
				.SetTabListener (new TabListener (this, new TabContentFragment (text))));
		}

		[Export ("onRemoveTab")]
		public void OnRemoveTab (View v)
		{
			var bar = SupportActionBar;
			if (bar.TabCount > 0) {
				bar.RemoveTabAt (bar.TabCount - 1);
			}
		}

		[Export ("onToggleTabs")]
		public void OnToggleTabs (View v)
		{
			var bar = SupportActionBar;

			if (bar.NavigationMode == Android.Support.V7.App.ActionBar.NavigationModeTabs) {
				bar.NavigationMode = Android.Support.V7.App.ActionBar.NavigationModeStandard;
				bar.SetDisplayOptions (Android.Support.V7.App.ActionBar.DisplayShowTitle, Android.Support.V7.App.ActionBar.DisplayShowTitle);
			} else {
				bar.NavigationMode = Android.Support.V7.App.ActionBar.NavigationModeTabs;
				bar.SetDisplayOptions (0, Android.Support.V7.App.ActionBar.DisplayShowTitle);
			}
		}

		[Export ("onRemoveAllTabs")]
		public void OnRemoveAllTabs (View v)
		{
			SupportActionBar.RemoveAllTabs ();
		}

		private class TabListener : Java.Lang.Object, Android.Support.V7.App.ActionBar.ITabListener
		{
			private TabContentFragment mFragment;
			ActionBarTabs mActionBarTabs;

			public TabListener (ActionBarTabs actionBarTabs, TabContentFragment fragment)
			{
				mFragment = fragment;
				mActionBarTabs = actionBarTabs;
			}

			public void OnTabReselected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
			{
				Android.Widget.Toast.MakeText (mActionBarTabs, "Reselected!", Android.Widget.ToastLength.Short).Show ();
			}

			public void OnTabSelected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
			{
				ft.Add (Resource.Id.fragment_content, mFragment, mFragment.Text);
			}

			public void OnTabUnselected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
			{
				ft.Remove (mFragment);
			}
		}

		class TabContentFragment : Android.Support.V4.App.Fragment
		{
			private string mText;

			public TabContentFragment (String text)
			{
				mText = text;
			}

			public String Text {
				get { return mText; }
			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				View fragView = inflater.Inflate (Resource.Layout.action_bar_tab_content, container, false);

				var text = (Android.Widget.TextView)fragView.FindViewById <Android.Widget.TextView> (Resource.Id.text);
				text.Text = mText;

				return fragView;
			}
		}
	}
}

