using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Views;
using Android.Graphics;

namespace MonoIO.Utilities
{
	// An extension of ActivityHelper that provides Android 3.0-specific functionality for
	// Honeycomb tablets. It thus requires API level 11.
	public class ActivityHelperHoneycomb : ActivityHelper
	{
		private IMenu options_menu;

		public ActivityHelperHoneycomb (Activity activity) : base (activity)
		{
		}

		public override void OnPostCreate (Android.OS.Bundle savedInstanceState)
		{
			// Do nothing in onPostCreate. ActivityHelper creates the
			// old action bar, we don't need to for Honeycomb.
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			options_menu = menu;

			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// Handle the HOME / UP affordance. Since the app is only two levels deep
			// hierarchically, UP always just goes home.
			if (item.ItemId == Android.Resource.Id.Home) {
				GoHome ();
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		public override void SetupHomeActivity()
		{
			base.SetupHomeActivity();

			// NOTE: there needs to be a content view set before this is called, so this method
			// should be called in onPostCreate.
			
			if (UIUtils.IsTablet(activity))
				activity.ActionBar.SetDisplayOptions(0, (ActionBarDisplayOptions.ShowHome | ActionBarDisplayOptions.ShowTitle));
			else
				activity.ActionBar.SetDisplayOptions(ActionBarDisplayOptions.UseLogo, (ActionBarDisplayOptions.UseLogo | ActionBarDisplayOptions.ShowTitle));
		}

		public override void SetupSubActivity ()
		{
			base.SetupSubActivity ();

			// NOTE: there needs to be a content view set before this is called, so this method
			// should be called in onPostCreate.

			if (UIUtils.IsTablet (activity))
				activity.ActionBar.SetDisplayOptions ((ActionBarDisplayOptions.UseLogo | ActionBarDisplayOptions.HomeAsUp), (ActionBarDisplayOptions.UseLogo | ActionBarDisplayOptions.HomeAsUp));
			else
			    activity.ActionBar.SetDisplayOptions (0, (ActionBarDisplayOptions.UseLogo | ActionBarDisplayOptions.HomeAsUp));
		}

		// No-op on Honeycomb. The action bar title always remains the same.
		public override void SetActionBarTitle (Java.Lang.ICharSequence title)
		{
		}
		
		
		// No-op on Honeycomb. The action bar color always remains the same.
		public override void SetActionBarColor (Color color)
		{
			if (!UIUtils.IsTablet (activity)) {
				base.SetActionBarColor (color);
			}
		}

		public override void SetRefreshActionButtonCompatState(bool refreshing)
		{
			// On Honeycomb, we can set the state of the
			// refresh button by giving it a custom action view.
			if (options_menu == null)
				return;

			var refresh_item = options_menu.FindItem(Resource.Id.menu_refresh);

			if (refresh_item != null) {
				if (refreshing) {
					refresh_item.SetActionView(Resource.Layout.actionbar_indeterminate_progress);
				} else {
					refresh_item.SetActionView(null);
				}
			}
		}
	}
}