ActionBarSherlock is a Java Android library that brings action bar feature,
which became available in Android 4.0 API, to Android 2.x too.

This Java Android library is brought to Xamarin.Android by making use of
Jar Binding project. (In fact ActionBarSherlock is rather an "Android library"
which requires some special care to deal with Android resources, but you
don't have to care about that unless you are making some changes in
ActionBarSherlock dll.)

The API is almost the same as the standard Android API (Android.App.ActionBar)
in API Level 14 or later.

Basic Project Setup
===================

ActionBarSherlock binding is a simple single binding dll. It contains
ActionBarSherlock Java library and required resources internally, and
Xamarin.Android automagically includes them into your application apk.

Create a new Android Application project, and add a reference to
ActionBarSherlock.dll. With the component support in either of the IDEs
(Xamarin Studio or Visual Studio), the dll is automatically added when
you add reference to the component.

The next step is mandatory: you have to set TargetFramework as Android 4.0
or later: it is to resolve some Android resources which are available
only in that API Level.

![Configure Target Framework][1]

Also it is important to set "Minimum Android Version" in the project
property to lower API Level (ActionBarSherlock works on API Level 2.x).
To set this property, you need to create "AndroidManifest.xml". It can be
created via the project options ("Android Application" tab).

![Configure Android Application (AndroidManifest.xml)][2]

(In theory you can skip this process, but that means your application won't
run on Android 2.x, which makes it pointless to use this library.)

Simple ActionBar Example
===================

Now you are ready to write C# code.

First, you might want to import these ActionBarSherlock
namespaces e.g.:

    using Xamarin.ActionbarSherlockBinding.App;

You can use SherlockActivity as your activity:

    public class FeatureToggles : SherlockActivity

Another important step is to apply Sherlock Theme. You would usually do it in
SherlockActivity.OnCreate():

    protected override void OnCreate (Bundle savedInstanceState)
    {
        SetTheme (Resource.Style.Theme_Sherlock);
        base.OnCreate (savedInstanceState);
    }

![The Default App template with Sherlock theme applied][3]

This brings the basic ActionBar on top of the screen (Otherwise the button
showed up at the top of the screen). Now you can add various components
of ActionBarSherlock onto this space.


Learn by Features Showcase
==========================

ActionBarSherlock provides many features and they are demonstrated in the
(port of) demo sample appllication ([ActionBarSherlockTest in our github repo][4]).

The features include:

- Options menu as List navigation. The menu items are shown on the action bar.
  You override OnCreateOptionsMenu(IMenu menu) method and call menu.Add() to add items.

![List navigation][5]

- Tab navigation with optionally collapsible title bar.

![Tab navigation][6]

- Flexible actions. ShowAsAction could indicate e.g. 1) to show up only if
  there is enough room, 2) to show collapsible action menu (when activated,
  other menu items go away), 3) normal display etc.

![Collapsible Action Items - default][7] ![Collapsible Action Items - collapsed search state][8]

- Custom UI components on the action bar.
- Progress controls (circle and line).

See the sample project sources to see how to write actual code.

[1]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_TargetFramework.png
[2]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_AndroidManifest.png
[3]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_SimpleTheme.png
[4]: https://github.com/xamarin/monodroid-samples/tree/master/ActionBarSherlock
[5]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_ListNavigation.png
[6]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_TabNavigation.png
[7]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_CollapsibleActionItems1.png
[8]: https://github.com/xamarin/monodroid-samples/raw/master/ActionBarSherlock/components/sshot_CollapsibleActionItems2.png
