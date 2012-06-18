using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Java.Lang;
using Android.Widget;
using Android.Graphics;

namespace MonoIO.Utilities
{
	public class ActivityHelper
	{
		protected Activity activity;

		public ActivityHelper (Activity activity)
		{
			this.activity = activity;
		}

		public virtual void OnPostCreate (Bundle savedInstanceState)
		{
			// Create the action bar
			SimpleMenu menu = new SimpleMenu (activity);
			activity.OnCreatePanelMenu ((int)WindowFeatures.OptionsPanel, menu);
			
			// TODO: call onPreparePanelMenu here as well
			for (int i = 0; i < menu.Size (); i++) {
				IMenuItem item = menu.GetItem (i);
				AddActionButtonCompatFromMenuItem (item);
			}
		}

		public virtual bool OnCreateOptionsMenu (IMenu menu)
		{
			activity.MenuInflater.Inflate (Resource.Menu.default_menu_items, menu);
			return false;
		}

		public virtual bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_search:
				GoSearch ();
				return true;
			}

			return false;
		}

		public bool OnKeyDown (Android.Views.Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Menu)
				return true;

			return false;
		}

		public bool OnKeyLongPress (Android.Views.Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back) {
				GoHome ();
				return true;
			}

			return false;
		}

		// Method, to be called in <code>onPostCreate</code>, that sets
		// up this activity as the home activity for the app.
		public virtual void SetupHomeActivity ()
		{
		}

		// Method, to be called in <code>onPostCreate</code>, that
		// sets up this activity as a sub-activity in the app.
		public virtual void SetupSubActivity ()
		{
		}

		// Invoke "home" action, returning to HomeActivity.
		public void GoHome ()
		{
			if (activity is HomeActivity)
				return;

			var intent = new Intent (activity, typeof(HomeActivity));
			intent.SetFlags (ActivityFlags.ClearTop);

			activity.StartActivity (intent);

			if (!UIUtils.IsHoneycomb)
				activity.OverridePendingTransition (Resource.Animation.home_enter, Resource.Animation.home_exit);
		}

		public void GoSearch ()
		{
			activity.StartSearch (null, false, Bundle.Empty, false);
		}

		// Sets up the action bar with the given title and accent color. If title is null, then
		// the app logo will be shown instead of a title. Otherwise, a home button and title are
		// visible. If color is null, then the default colorstrip is visible.
		public void SetupActionBar (ICharSequence title, Color color)
		{
			var action_bar = GetActionBarCompat ();

			if (action_bar == null)
				return;

			var spring_layout_params = new LinearLayout.LayoutParams (0, ViewGroup.LayoutParams.FillParent);
			spring_layout_params.Weight = 1;

			if (title != null) {
				// Add Home button
				AddActionButtonCompat (Resource.Drawable.ic_title_home, Resource.String.description_home, delegate {
					GoHome (); }, true);

				// Add title text
				var title_text = new TextView (activity, null, Resource.Attribute.actionbarCompatTextStyle);
				title_text.LayoutParameters = spring_layout_params;
				title_text.TextFormatted = title;

				action_bar.AddView (title_text);
			} else {
				// Add logo
				var logo = new ImageButton (activity, null, Resource.Attribute.actionbarCompatLogoStyle);
				logo.Click += delegate {
					GoHome (); };

				action_bar.AddView (logo);

				// Add spring (dummy view to align future children to the right)
				View spring = new View (activity);
				spring.LayoutParameters = spring_layout_params;

				action_bar.AddView (spring);
			}

			SetActionBarColor (color);
		}

		// Sets the action bar color to the given color.
		public virtual void SetActionBarColor (Color color)
		{
			if (color == 0)
				return;

			var color_strip = activity.FindViewById<View> (Resource.Id.colorstrip);

			if (color_strip == null)
				return;

			color_strip.SetBackgroundColor (color);
		}

		// Sets the action bar title to the given string.
		public virtual void SetActionBarTitle (ICharSequence title)
		{
			var action_bar = GetActionBarCompat ();

			if (action_bar == null)
				return;

			var title_text = action_bar.FindViewById<TextView> (Resource.Id.actionbar_compat_text);

			if (title_text != null)
				title_text.TextFormatted = title;
		}

		// Returns the ViewGroup for the action bar on phones (compatibility action bar).
		// Can return null, and will return null on Honeycomb.
		public ViewGroup GetActionBarCompat ()
		{
			return activity.FindViewById<ViewGroup> (Resource.Id.actionbar_compat);
		}

		private View AddActionButtonCompat (int iconResId, int textResId, EventHandler clickAction, bool separatorAfter)
		{
			var action_bar = GetActionBarCompat ();

			if (action_bar == null)
				return null;

			// Create the separator
			var separator = new ImageView (activity, null, Resource.Attribute.actionbarCompatSeparatorStyle);
			separator.LayoutParameters = new ViewGroup.LayoutParams (2, ViewGroup.LayoutParams.FillParent);

			// Create the button
			var action_button = new ImageButton (activity, null, Resource.Attribute.actionbarCompatButtonStyle);
			action_button.LayoutParameters = new ViewGroup.LayoutParams ((int)activity.Resources.GetDimension (Resource.Dimension.actionbar_compat_height), ViewGroup.LayoutParams.FillParent);

			action_button.SetImageResource (iconResId);
			action_button.SetScaleType (ImageView.ScaleType.Center);
			action_button.ContentDescription = activity.Resources.GetString (textResId);
			action_button.Click += clickAction;

			// Add separator and button to the action bar in the desired order
			if (!separatorAfter)
				action_bar.AddView (separator);

			action_bar.AddView (action_button);

			if (separatorAfter)
				action_bar.AddView (separator);

			return action_button;
		}

		// Adds an action button to the compatibility action bar, using menu information from a
		// MenuItem. If the menu item ID is menu_refresh, the menu item's state
		// can be changed to show a loading spinner using
		// ActivityHelper#setRefreshActionButtonCompatState(boolean).
		private View AddActionButtonCompatFromMenuItem (IMenuItem item)
		{
			var action_bar = GetActionBarCompat ();

			if (action_bar == null)
				return null;

			// Create the separator
			var separator = new ImageView (activity, null, Resource.Attribute.actionbarCompatSeparatorStyle);
			separator.LayoutParameters = new ViewGroup.LayoutParams (2, ViewGroup.LayoutParams.FillParent);

			// Create the button
			var action_button = new ImageButton (activity, null, Resource.Attribute.actionbarCompatButtonStyle);
			action_button.LayoutParameters = new ViewGroup.LayoutParams ((int)activity.Resources.GetDimension (Resource.Dimension.actionbar_compat_height), ViewGroup.LayoutParams.FillParent);

			action_button.SetImageDrawable (item.Icon);
			action_button.SetScaleType (ImageView.ScaleType.Center);
			action_button.ContentDescriptionFormatted = item.TitleFormatted;
			action_button.Click += delegate {
				activity.OnMenuItemSelected ((int)WindowFeatures.OptionsPanel, item); };

			action_bar.AddView (separator);
			action_bar.AddView (action_button);

			if (item.ItemId == Resource.Id.menu_refresh) {
				// Refresh buttons should be stateful, and allow
				//  for indeterminate progress indicators,so add those.
				var width = activity.Resources.GetDimensionPixelSize (Resource.Dimension.actionbar_compat_height);
				width = width / 3;

				var indicator = new ProgressBar (activity, null, Resource.Attribute.actionbarCompatProgressIndicatorStyle);

				var layout = new LinearLayout.LayoutParams (width, width);
				layout.SetMargins (width, width, (width * 3) - 2 * width, 0);

				indicator.LayoutParameters = layout;
				indicator.Visibility = ViewStates.Gone;
				indicator.Id = Resource.Id.menu_refresh_progress;

				action_bar.AddView (indicator);
			}

			return action_button;
		}

		public virtual void SetRefreshActionButtonCompatState (bool refreshing)
		{
			var refresh_button = activity.FindViewById<View> (Resource.Id.menu_refresh);
			var refresh_indicator = activity.FindViewById<View> (Resource.Id.menu_refresh_progress);

			if (refresh_button != null)
				refresh_button.Visibility = refreshing ? ViewStates.Gone : ViewStates.Visible;

			if (refresh_indicator != null)
				refresh_indicator.Visibility = refreshing ? ViewStates.Visible : ViewStates.Gone;
		}
	}
}
