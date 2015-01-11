Xamarin.Android Binding for ActionBarSherlock
=============================================

This package is a binding for ActionBarSherlock Android library by Jake Wharton.
ActionBarSherlock is explained immediately below, as an excerpt from
ActionBarSherlock's README.

ActionBarSherlock
=================

ActionBarSherlock is an standalone library designed to facilitate the use of
the action bar design pattern across all versions of Android through a single
API.

The library will automatically use the [native ActionBar][2] implementation on
Android 4.0 or later. For previous versions which do not include ActionBar, a
custom action bar implementation based on the sources of Ice Cream Sandwich
will automatically be wrapped around the layout. This allows you to easily
develop an application with an action bar for every version of Android from 2.x
and up.

About This Binding
==================

This binding source is available at [monodroid-samples on GitHub][1].

Java packages are renamed in order for .NET developers to become in .NET manner:
from "com.actionbar.sherlock" to "Xamarin.ActionBarSherlockBinding".

For complete mapping see [Metadata.xml][5] in the source code.


Screenshots
===========

Note that this component is primarily about action bars, so you would
like to take a closer look at the navigation bar part.

List navigation example:

![List Navigation][3]

Feature showcase (from "FeatureToggles" sample):

![Feature showcase (from "FeatureToggles" sample)][4]


[1]: https://github.com/xamarin/monodroid-samples/tree/master/ActionBarSherlock
[2]: http://developer.android.com/guide/topics/ui/actionbar.html
[3]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_ListNavigation.png
[4]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_FeatureToggles.png
[5]: https://github.com/xamarin/monodroid-samples/blob/master/ActionBarSherlock/ActionBarSherlock/Transforms/Metadata.xml
