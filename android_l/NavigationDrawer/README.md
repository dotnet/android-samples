NavigationDrawer
================
  This example illustrates a common usage of the DrawerLayout widget
  in the Android support library.
  When a navigation (left) drawer is present, the host activity should detect presses of
  the action bar's Up affordance as a signal to open and close the navigation drawer. The
  ActionBarDrawerToggle facilitates this behavior.
  Items within the drawer should fall into one of two categories:  
 *  **View switches**. A view switch follows the same basic policies as
  list or tab navigation in that a view switch does not create navigation history.
  This pattern should only be used at the root activity of a task, leaving some form
  of Up navigation active for activities further down the navigation hierarchy.
 * **Selective Up**. The drawer allows the user to choose an alternate
  parent for Up navigation. This allows a user to jump across an app's navigation
  hierarchy at will. The application should treat this as it treats Up navigation from
  a different task, replacing the current task stack using TaskStackBuilder or similar.
  This is the only form of navigation drawer that should be used outside of the root
  activity of a task.

Right side drawers should be used for actions, not navigation. This follows the pattern
  established by the Action Bar that navigation should be to the left and actions to the right.
  An action should be an operation performed on the current contents of the window,
  for example enabling or disabling a data overlay on top of the current content.

Instructions
------------
* Select 'Navigation Drawer Example' from the main screen
* Bring up the Navigation Drawer by hitting the three horizontal bars or by swipping in from the left side of the device
* Select a planet to display

Build Requirements
-----------------
* Xamarin Studio 5.3+
* Xamarin Android 4.17+

Author
------
Copyright (c) 2005-2008, The Android Open Source Project  
Ported to Xamarin.Android by Ben O'Halloran
