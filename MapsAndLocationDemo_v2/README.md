Maps and Location Demo v2
=========================

This is the sample code for the [Maps and Location article](http://docs.xamarin.com/android/tutorials/Maps_and_Location), which has been updated to [Google Maps for Android v2](https://developers.google.com/maps/documentation/android/). 

This code shows how to use Google Maps v2 in an Android application and how to create the Java Binding Library project for Google Play services client library.

This project does not include the Google Play services client library, which is a requirement for Google Maps v2. It is not possible for 3rd parties to distribute this API, so you must compile the binding project yourself.

**Note:** You must use Mono for Android 4.4.x or higher to compile the Java Binding project. It is recommended that you use the most recent stable build of Mono For Android, which is 4.4.54 as of January 3, 2013.

**Note:** You must have the Android Support Packages installed.

To include the Google Play services client library: 

1. Use the Android SDK Manager to install Google Play Services.
2. Copy the directory located at `extras/google/google_play_services/libproject/google-play-services_lib` into the same directory as this README.
3. Build the project with `ant` like so:
    
    cd google-play-services_lib
    android project update -p .
    ant debug

4. Open the `MapsAndLocationDemo.sln`. Add the file `google-play-services_lib/project.properties` to the `GooglePlayServices` project as a *linked* file.

A bash script file has been provided which will automate this step for you. It assumes that you already have `ant` installed somewhere in your path and that the environment variable `$ANDROID_HOME` holds the path to your Android SDK.

Google Maps v2 API Key
----------------------

You must [obtain a new API Key](https://developers.google.com/maps/documentation/android/start#the_google_maps_api_key) for Google Maps v2, API keys from Google Maps v1 will not work. 

The location of the debug.keystore file that Mono for Android uses depends on your platform:

- **Windows Vista / Windows 7 / Windows 8**: `C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore`
- **OSX** : `/Users/[USERNAME]/.local/share/Xamarin/Mono for Android/debug.keystore`
