The Android Wear Developer Preview provides tools and APIs that allow you to enhance your app notifications, giving an optimized user experience on Android Wear.

With the Android Wear Developer Preview, you can:

 - Run the Android Wear platform in the Android emulator.
 - Connect your Android device to the emulator and view notifications from the device as cards on Android Wear.
 - Try new APIs in the preview support library that enhance your app's notifications with features such as voice replies and notification pages.
 
Android Wear builds on the notification subsystem introduced in Android 4.3 (Jelly Bean) and its Android Support v4 counterpart (NotificationCompat API). It will thus integrate out of the box with your existing app notifications.

This component allows your Xamarin.Android application to use the extra notification features introduced by Wear such as paged navigation, voice control input or notification stacking. Simply wrap your existing `NotificationCompat.Builder` instances using the new `WearableNotifications.Builder` class to start using the API subset.

You can read the following [Android developer article][5] to learn how the existing notification API translates into the Android Wear world.

The Android Wear Developer Preview component is a Xamarin binding project for the [Android Wear][4] Sender SDK.
  
<blockquote><strong>Caution</strong>: The current Android Wear Developer Preview is intended for <strong>development and testing purposes only</strong>, not for production apps. Google may change this Developer Preview significantly prior to the official release of the Android Wear SDK. You may not publicly distribute or ship any application using this Developer Preview, as this Developer Preview will no longer be supported after the official SDK is released (which will cause applications based only on the Developer Preview to break).</blockquote>

*Portions of this page are modifications based on [work][3] created and [shared by the Android Open Source Project][1] and used according to terms described in the [Creative Commons 2.5 Attribution License][2].*

[1]: http://code.google.com/policies.html
[2]: http://creativecommons.org/licenses/by/2.5/
[3]: http://developer.android.com/wear/preview/start.html
[4]: http://developer.android.com/wear/index.html
[5]: http://developer.android.com/wear/notifications/creating.html

