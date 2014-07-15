v7 Support AppCompat Library
=========================

There are several libraries designed to be used with Android 2.1 (API level 7) and higher. 
These libraries provide specific feature sets and can be included in your application independently 
from each other.  This library adds support for the [Action Bar][4] user interface [design pattern][5].

Here are a few of the key classes included in the v7 appcompat library:

 - ActionBar - Provides an implementation of the action bar user interface pattern. For more information on using the Action Bar, see the Action Bar developer guide.
 - ActionBarActivity - Adds an application activity class that must be used as a base class for activities that uses the Support Library action bar implementation.
 - ShareActionProvider - Adds support for a standardized sharing action (such as email or posting to social applications) that can be included in an action bar.

Using ActionBar
------

```csharp
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
		Android.Widget.Toast.MakeText (this, 
			"Selected Item: " + 
			item.TitleFormatted, 
			Android.Widget.ToastLength.Short).Show();
			
		return true;
	}
}
```


*Portions of this page are modifications based on [work][3] created and [shared by the Android Open Source Project][1] and used according to terms described in the [Creative Commons 2.5 Attribution License][2].*

[1]: http://code.google.com/policies.html
[2]: http://creativecommons.org/licenses/by/2.5/
[3]: http://developer.android.com/tools/support-library/features.html
[4]: http://developer.android.com/guide/topics/ui/actionbar.html
[5]: http://developer.android.com/design/patterns/actionbar.html

