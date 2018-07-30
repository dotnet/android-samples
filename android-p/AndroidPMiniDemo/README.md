Android P Mini Demo 
===================

**AndroidPMiniDemo** is a sample app that accompanies the article,
[Android P Preview](https://docs.microsoft.com/en-us/xamarin/android/platform/android-p).

This sample demonstrates the new display cutout and image notification features 
in Android P. 

## Display Cutout Demo

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

If you are using the Android emulator, you must first enable cutout
mode from within the emulator while it is running an Android P virtual
device image:

1.  Go to **System Settings > System > Advanced > Developer Options**

2.  Under **Developer Options**, scroll down to **Drawing** and click
    **Simulate a display with a cutout**.

3.  Select **Tall display cutout** to simulate a typical phone.


## Notification Demo

The notification demo generates a notification by using the new
[Person](https://developer.android.com/reference/android/app/Person.html)
class, and it includes an image in the notification by using the new
 [Notification.MessagingStyle.Message.SetData](https://developer.android.com/reference/android/app/Notification.MessagingStyle.Message.html#setData%28java.lang.String,%20android.net.Uri) method.

Clicking the **Send Image Notification** button causes the app to send an
example text notification followed (several seconds later) by a notification
that contains an image.


## Requirements

-   **Visual Studio** &ndash; If you are using Windows, version 15.8
    Preview 5 or later of Visual Studio is required.  If you are using
    a Mac, the current Beta version of Visual Studio for Mac or later
    is required.

-   **Xamarin.Android** &ndash; Xamarin.Android 9.0.0.17 or later must
    be installed with Visual Studio.

-   **Android SDK** &ndash; Android SDK API 28 or later must be
    installed via the Android SDK Manager.


Author
------

Copyright 2018 Microsoft

Created by Mark Mclemore
