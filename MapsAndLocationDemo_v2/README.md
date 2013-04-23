Maps and Location Demo v2
=========================

This code shows how to use Google Maps v2 in an Android application and how to create the Java Binding Library project for Google Play services client library.

This project does not include the Google Play services client library, which is a requirement for Google Maps v2. It is not possible for 3rd parties to distribute this API, so you must compile the binding project yourself.

According to the [Android Dashboard](http://developer.android.com/about/dashboards/index.html), nearly 54% of all Android devices are running Android 4.0 (API level 14) or higher. This project has been updated to target Android 4.0 or higher. See the section below on backwards compatibility with the older API's.

**Note:** You must use Mono for Android 4.4.x or higher to compile the Java Binding project. It is recommended that you use the most recent stable build of Mono For Android, which is 4.6.4 as of April 23, 2013.

**Note:** Ensure that the package name of your application is all lower case. Android is very particular and the Google Maps API will not authenticate the API key property if the package name has mixed case.


Building the Client Library
---------------------------
To access Google Maps v2, it is necessary to create a Mono for Android binding for the Google Play services client library. This example has a binding library already, but there are some manual steps that must be taken so that the project will compile:

1. Use the Android SDK Manager to install Google Play Services.
2. Copy the directory located at `extras/google/google_play_services/libproject/google-play-services_lib` into the same directory as this README.
3. Build the project with `ant` like so:
    
        cd google-play-services_lib
        android update project -p .
        ant debug

4. Open the `MapsAndLocationDemo.sln`. Add the file `google-play-services_lib/project.properties` to the `GooglePlayServices` project as a *linked* file.

A bash script file has been provided which will automate this step for you. It assumes that you already have `ant` installed somewhere in your path and that the environment variable `$ANDROID_HOME` holds the path to your Android SDK.

Google Maps v2 API Key
----------------------

You must [obtain a new API Key](https://developers.google.com/maps/documentation/android/start#the_google_maps_api_key) for Google Maps v2, API keys from Google Maps v1 will not work. 

The location of the debug.keystore file that Mono for Android uses depends on your platform:

- **Windows Vista / Windows 7 / Windows 8**: `C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore`
- **OSX** : `/Users/[USERNAME]/.local/share/Xamarin/Mono for Android/debug.keystore`

To obtain the SHA1 fingerprint of the debug keystore, you can use the `keytool` command that is a part of the JDK. This is an example of using `keytool` at the command-line:

    $ keytool -V -list -keystore debug.keystore -alias androiddebugkey -storepass android -keypass android

A Note About Google Play Services
---------------------------------

[Google Play Services](https://play.google.com/store/apps/details?id=com.google.android.gms) must be installed on the device before Google Maps for Android v2 will work. *The emulator will not have Google Play Services installed*. Installing Google Play Services is beyond the scope of this example. 

If the device does not have Google Play Services installed, you will see a stack trace similar to the following:

	com.htc.autotest.dlib.RecordEngine in loader dalvik.system.DexClassLoader@4052ca48Loaded assembly: Mono.Android.Support.v4.dll [External]
	
	Google Play services is missing.
	Google Play services is missing.
	Google Play services is missing.
	Shutting down VM
	FATAL EXCEPTION: main
	java.lang.NoClassDefFoundError: com.google.android.gms.R$string
		at com.google.android.gms.common.GooglePlayServicesUtil.b(Unknown Source)
		at com.google.android.gms.internal.d.a(Unknown Source)
		at com.google.android.gms.internal.d.onCreateView(Unknown Source)
		at com.google.android.gms.maps.SupportMapFragment.onCreateView(Unknown Source)
		at android.support.v4.app.Fragment.performCreateView(Fragment.java:1460)
		at android.support.v4.app.FragmentManagerImpl.moveToState(FragmentManager.java:911)
		at android.support.v4.app.FragmentManagerImpl.moveToState(FragmentManager.java:1088)
		at android.support.v4.app.BackStackRecord.run(BackStackRecord.java:682)
		at android.support.v4.app.FragmentManagerImpl.execPendingActions(FragmentManager.java:1444)
		at android.support.v4.app.FragmentActivity.onStart(FragmentActivity.java:551)
		at android.app.Instrumentation.callActivityOnStart(Instrumentation.java:1201)
		at android.app.Activity.performStart(Activity.java:3908)
		at android.app.ActivityThread.performLaunchActivity(ActivityThread.java:1794)
		at android.app.ActivityThread.handleLaunchActivity(ActivityThread.java:1842)
		at android.app.ActivityThread.access$1500(ActivityThread.java:132)
		at android.app.ActivityThread$H.handleMessage(ActivityThread.java:1038)
		at android.os.Handler.dispatchMessage(Handler.java:99)
		at android.os.Looper.loop(Looper.java:143)
		at android.app.ActivityThread.main(ActivityThread.java:4263)
		at java.lang.reflect.Method.invokeNative(Native Method)
		at java.lang.reflect.Method.invoke(Method.java:507)
		at com.android.internal.os.ZygoteInit$MethodAndArgsCaller.run(ZygoteInit.java:839)
		at com.android.internal.os.ZygoteInit.main(ZygoteInit.java:597)
		at dalvik.system.NativeStart.main(Native Method)
	Sending signal. PID: 19208 SIG: 9
	
	
Backwards Compatibility with GingerBread / Froyo
------------------------------------------------

Google Play Services is supported on Android 2.2 (API level 8) or higher. This binding will work on these older API's, but some changes must be made to the binding library project and the sample project first:

* In the GooglePlayServices binding library, change the target framework to **Android 2.2**.
* In the GooglePlayServices binding library, add a reference to **Mono.Android.Support.v4**.
* In the MapsAndLocationDemo project, change the target framework to **Android 2.2**.
* In the MapsAndLocationDemo, add a reference to **Mono.Android.Support.v4**.
* In the MapsAndLocationDemo fix the compile errors: 
	* Change <code>FragmentManager</code> to <code>SupportFragmentManager</code>
	* Change <code>Activity</code> to <code>FragmentActivity</code>
	* Change <code>MapFragment</code> to <code>SupportMapFragment</code>
	* Change <code>using Android.App;</code> to <code>using Android.Support.V4.App;</code>

At this point the binding will target Android 2.2 and will run on older devices.
	    