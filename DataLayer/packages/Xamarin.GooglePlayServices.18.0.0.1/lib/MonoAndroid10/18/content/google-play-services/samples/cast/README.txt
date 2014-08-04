These sample apps demonstrate how you can integrate the
Google Cast SDK in your app.

To get started with Google Cast, visit:
https://developer.google.com/cast

To setup your system for Android app development, visit:
https://developers.google.com/cast/docs/android_sender

This project has a dependency on the following android support libraries
  * android-support-v7-mediarouter
  * android-support-v7-appcompat

---------------------------------------------
How to Set Up Your Environment with Eclipse?
---------------------------------------------
1. Install the following extra packages from the Android SDK Manager:
  * Android Support Library
  * Google Play services

2. Import the existing Android v7 support library projects
  * File --> Import --> General --> Existing Projects into Workspace. Click Next
  * Go to the directory of the Android support libraries, and select the v7 appcompat and
    v7 mediarouter projects
    1. $(ANDROID_SDK_ROOT)/extras/android/support/v7/appcompat
    2. $(ANDROID_SDK_ROOT)/extras/android/support/v7/mediarouter
  * Add android-support-v7-appcompat as a library dependency for android-support-v7-mediarouter
    * android-support-v7-mediarouter project properties --> Android --> Add
    * Select android-support-v7-appcompat --> Ok

3. Import Google Play Services Library and the Google Cast sample apps
  * File --> Import --> Android -> Existing Android Code Into Workspace. Click Next
  * Go to directory of the Google Play Services library and import
    google-play-services_lib project
    i.e. $(ANDROID_SDK_ROOT)/extras/google/google_play_services/libproject/google-play-services_lib
  * Finish
  * File --> Import --> Android -> Existing Android Code Into Workspace. Click Next
  * Go to the directory of the Google Cast sample apps and import the sample apps
    i.e. $(ANDROID_SDK_ROOT)/extras/google/google_play_services/samples/cast/democastplayer
         $(ANDROID_SDK_ROOT)/extras/google/google_play_services/samples/cast/tictactoe
  * Finish

5. Build and run the sample apps
