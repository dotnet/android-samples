using System;
using Android.OS;
using Android.Support.V7.App;


namespace AndroidSupportSample
{
	[Android.App.Activity ()] // Properties set in AndroidManifest.xml directly
	public class ActionBarDisplayOptions : ActionBarActivity, Android.Views.View.IOnClickListener, ActionBar.ITabListener
	{

		private Android.Views.View mCustomView;
		private Android.Support.V7.App.ActionBar.LayoutParams mCustomViewLayoutParams;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.action_bar_display_options);

			FindViewById(Resource.Id.toggle_home_as_up).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_show_home).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_use_logo).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_show_title).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_show_custom).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_navigation).SetOnClickListener(this);
			FindViewById(Resource.Id.cycle_custom_gravity).SetOnClickListener(this);
			FindViewById(Resource.Id.toggle_visibility).SetOnClickListener(this);

			// Configure several action bar elements that will be toggled by display options.
			mCustomView = LayoutInflater.Inflate(Resource.Layout.action_bar_display_options_custom, null);
			mCustomViewLayoutParams = new Android.Support.V7.App.ActionBar.LayoutParams(
				Android.Support.V7.App.ActionBar.LayoutParams.WrapContent, 
				Android.Support.V7.App.ActionBar.LayoutParams.WrapContent);

			ActionBar bar = SupportActionBar;
			bar.SetCustomView(mCustomView, mCustomViewLayoutParams);
			bar.AddTab(bar.NewTab().SetText("Tab 1").SetTabListener(this));
			bar.AddTab(bar.NewTab().SetText("Tab 2").SetTabListener(this));
			bar.AddTab(bar.NewTab().SetText("Tab 3").SetTabListener(this));
		}

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.display_options_actions, menu);
			return true;
		}

		public override bool OnSupportNavigateUp ()
		{
			Finish();
			return true;
		}

		public void OnClick (Android.Views.View v)
		{
			ActionBar bar = SupportActionBar;
			int flags = 0;
			switch (v.Id) {
			case Resource.Id.toggle_home_as_up:
				flags = Android.Support.V7.App.ActionBar.DisplayHomeAsUp;
				break;
			case Resource.Id.toggle_show_home:
				flags = Android.Support.V7.App.ActionBar.DisplayShowHome;
				break;
			case Resource.Id.toggle_use_logo:
				flags = Android.Support.V7.App.ActionBar.DisplayUseLogo;
				break;
			case Resource.Id.toggle_show_title:
				flags = Android.Support.V7.App.ActionBar.DisplayShowTitle;
				break;
			case Resource.Id.toggle_show_custom:
				flags = Android.Support.V7.App.ActionBar.DisplayShowCustom;
				break;
			case Resource.Id.toggle_navigation:
				bar.NavigationMode = 
					bar.NavigationMode == Android.Support.V7.App.ActionBar.NavigationModeStandard
					? Android.Support.V7.App.ActionBar.NavigationModeTabs
					: Android.Support.V7.App.ActionBar.NavigationModeStandard;
				return;
			case Resource.Id.cycle_custom_gravity: {
					ActionBar.LayoutParams lp = mCustomViewLayoutParams;
					Android.Views.GravityFlags newGravity = 0;
					switch ((Android.Views.GravityFlags)(lp.Gravity & (int)Android.Views.GravityFlags.HorizontalGravityMask)) {
					case Android.Views.GravityFlags.Left:
						newGravity = Android.Views.GravityFlags.CenterHorizontal;
						break;
					case Android.Views.GravityFlags.CenterHorizontal:
						newGravity = Android.Views.GravityFlags.Right;
						break;
					case Android.Views.GravityFlags.Right:
						newGravity = Android.Views.GravityFlags.Left;
						break;
					}
					lp.Gravity = lp.Gravity & (int)~Android.Views.GravityFlags.HorizontalGravityMask | (int)newGravity;
					bar.SetCustomView(mCustomView, lp);
					return;
				}
			case Resource.Id.toggle_visibility:
				if (bar.IsShowing) {
					bar.Hide();
				} else {
					bar.Show();
				}
				return;
			}

			int change = bar.DisplayOptions ^ flags;
			bar.SetDisplayOptions(change, flags);
		}

		public void OnTabReselected (Android.Support.V7.App.ActionBar.Tab p0, Android.Support.V4.App.FragmentTransaction p1)
		{
		}

		public void OnTabSelected (Android.Support.V7.App.ActionBar.Tab p0, Android.Support.V4.App.FragmentTransaction p1)
		{
		}

		public void OnTabUnselected (Android.Support.V7.App.ActionBar.Tab p0, Android.Support.V4.App.FragmentTransaction p1)
		{
		}
	}
}

