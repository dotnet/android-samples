v4 Support Library Rev 20.0.0
=============================

This library is designed to be used with Android 1.6 (API level 4) and higher. It includes the largest set of APIs compared to the other libraries, including support for application components, user interface features, accessibility, data handling, network connectivity, and programming utilities. Here are a few of the key classes included in the v4 library:

**App Components**

- Fragment - Adds support encapsulation of user interface and functionality with Fragments, enabling applications provide layouts that adjust between small and large-screen devices.
- NotificationCompat - Adds support for rich notification features.
- LocalBroadcastManager - Allows applications to easily register for and receive intents within a single application without broadcasting them globally.

**User Interface**

- ViewPager - Adds a ViewGroup that manages the layout for the child views, which the user can swipe between.
- PagerTitleStrip - Adds a non-interactive title strip, that can be added as a child of ViewPager.
- PagerTabStrip - Adds a navigation widget for switching between paged views, that can also be used with ViewPager.
- DrawerLayout - Adds support for creating a Navigation Drawer that can be pulled in from the edge of a window.
- SlidingPaneLayout - Adds widget for creating linked summary and detail views that appropriately adapt to various screen sizes.

**Accessibility**

- ExploreByTouchHelper - Adds a helper class for implementing accessibility support for custom views.
- AccessibilityEventCompat - Adds support for AccessibilityEvent. For more information about implementing accessibility, see Accessibility.
- AccessibilityNodeInfoCompat - Adds support for AccessibilityNodeInfo.
- AccessibilityNodeProviderCompat - Adds support for AccessibilityNodeProvider.
- AccessibilityDelegateCompat - Adds support for View.AccessibilityDelegate.

**Content**

- Loader - Adds support for asynchronous loading of data. The library also provides concrete implementations of this class, including CursorLoader and AsyncTaskLoader.
- FileProvider - Adds support for sharing of private files between applications.
	
There are many other APIs included in this library. For complete, detailed information about the v4 Support Library APIs, [see the android.support.v4](http://developer.android.com/reference/android/support/v4/app/package-summary.html).

*Portions of this page are modifications based on [work][3] created and [shared by the Android Open Source Project][1] and used according to terms described in the [Creative Commons 2.5 Attribution License][2].*

[1]: http://code.google.com/policies.html
[2]: http://creativecommons.org/licenses/by/2.5/
[3]: http://developer.android.com/tools/support-library/features.html
