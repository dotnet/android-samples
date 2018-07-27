Android P Mini Demo 
===================

**AndroidPMiniDemo** is a sample app that accompanies the forthcoming article,
[Android P Preview](https://docs.microsoft.com/en-us/xamarin/android/platform/android-p).

This sample demonstrates the new [DisplayCutout](https://developer.android.com/reference/android/view/DisplayCutout.html)
and image notification features in Android P. 

The display cutout demo provides three buttons to select a display
cutout mode:

-   **Short Edges** &ndash; Clicking this button causes the app to go
    full screen and set the
    [LayoutInDisplayCutoutModeShortEdges](https://developer.android.com/reference/android/view/WindowManager.LayoutParams.html#LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES)
    window attribute. This causes the app's white background to
    extend into the display cutout area.

-   **Never** &ndash; Clicking this button causes the app to go full screen
    and set the
    [LayoutInDisplayCutoutModeNever](https://developer.android.com/reference/android/view/WindowManager.LayoutParams.html#LAYOUT_IN_DISPLAY_CUTOUT_MODE_NEVER) window attribute. This prevents the app's white background from extending into
the display cutout area.

-   **Reset** &ndash; Changes the app back to non-fullscreen mode.

The notification demo generates a notification by using the new
[Person](https://developer.android.com/reference/android/app/Person.html)
class and includes an image in the notification by using the new
 [Notification.MessagingStyle.Message.setData](https://developer.android.com/reference/android/app/Notification.MessagingStyle.Message.html#setData%28java.lang.String,%20android.net.Uri) method.

Clicking the **Send Image Notification** button causes the app to send an
example text notification followed several seconds later by a notification
that contains an image.


Author
------

Copyright 2018 Microsoft

Created by Mark Mclemore
