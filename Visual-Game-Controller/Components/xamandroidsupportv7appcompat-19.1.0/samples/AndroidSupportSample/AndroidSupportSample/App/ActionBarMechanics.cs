using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;


namespace AndroidSupportSample
{
	[Activity (Label = "@string/action_bar_mechanics", Theme = "@style/Theme.AppCompat")]			
	public class ActionBarMechanics : ActionBarActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// The Action Bar is a window feature. The feature must be requested
			// before setting a content view. Normally this is set automatically
			// by your Activity's theme in your manifest. The provided system
			// theme Theme.WithActionBar enables this for you. Use it as you would
			// use Theme.NoTitleBar. You can add an Action Bar to your own themes
			// by adding the element <item name="android:windowActionBar">true</item>
			// to your style definition.
			SupportRequestWindowFeature(WindowCompat.FeatureActionBar);
		}

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			// Menu items default to never show in the action bar. On most devices this means
			// they will show in the standard options menu panel when the menu button is pressed.
			// On xlarge-screen devices a "More" button will appear in the far right of the
			// Action Bar that will display remaining items in a cascading menu.

			menu.Add(new Java.Lang.String ("Normal item"));

			var actionItem = menu.Add(new Java.Lang.String ("Action Button"));

			// Items that show as actions should favor the "if room" setting, which will
			// prevent too many buttons from crowding the bar. Extra items will show in the
			// overflow area.
			MenuItemCompat.SetShowAsAction(actionItem, MenuItemCompat.ShowAsActionIfRoom);

			// Items that show as actions are strongly encouraged to use an icon.
			// These icons are shown without a text description, and therefore should
			// be sufficiently descriptive on their own.
			actionItem.SetIcon(Android.Resource.Drawable.IcMenuShare);
			return true;
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			Android.Widget.Toast.MakeText (this, "Selected Item: " + item.TitleFormatted, Android.Widget.ToastLength.Short).Show();
			return true;
		}
	}
}

