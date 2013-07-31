Maps and Location Demo v2
=========================

This code shows how to use Google Maps v2 in an Android application. According to the [Android Dashboard](http://developer.android.com/about/dashboards/index.html), nearly 61% of all Android devices are running Android 4.0 (API level 14) or higher so the focus of this sample is on API 14 and higher. 

The `Debug` build configuration contains the following projects, and targets API level 14 or higher. This build configuration will only compile the following projects:

* **LocationDemo** - this project shows how to use the LocationManager to figure out where the device is. This project does not require Google Play Services client library.
* **SimpleMapDemo** - this project demonstrates some of the simple features of Google Maps for Android v2. It does require the Google Play Services client library.

In the `Debug_Froyo` build configuration, and it targets API level 8 and higher. This build configuration will only compile the following project:

* **SimpleMapDemo_Froyo** - this project demonstrates some of the simple features of Google Maps for Android v2. It does require the Google Play Services client library. 

**Note:** You must use Xamarin.Android 4.8 or higher for this sample. It is recommended that you use the most recent stable build of Xamarin.Android, which is 4.8 as of July 30, 2013. If you have an older version of Xamarin.Android then you must create your own Java Binding project for Google Play Services.

**Note:** Ensure that the package name of your application is all lower case. Android is very particular and the Google Maps API will not authenticate the API key property if the package name has mixed case.

## Prerequisites

These sample use the Google Play Services component that is available in the Xamarin Component Store.  There are two versions of this component: [one for Froyo](https://components.xamarin.com/view/googleplayservicesfroyo/) and [one for Ice Cream Sandwich](https://components.xamarin.com/view/googleplayservices/). You will need to install the Google Play Services client library and the components before these samples with work.

You must also have the Google Play Client Services library installed. You can install this by using the Android SDK Manager. This library is available under the *Extras* folder:

![Link File](/images/android_sdk_manager.png)

Google Maps v2 API Key
----------------------

You must [obtain a new API Key](https://developers.google.com/maps/documentation/android/start#the_google_maps_api_key) for Google Maps v2, API keys from Google Maps v1 will not work. 

The location of the debug.keystore file that Xamarin.Android uses depends on your platform:

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

* Install the [Google Play Services (Froyo) component](https://components.xamarin.com/view/googleplayservicesfroyo/).
* In the MapsAndLocationDemo project, change the target framework to **Android 2.2**.
* In the MapsAndLocationDemo, add a reference to **Mono.Android.Support.V4**.
* In the MapsAndLocationDemo fix the compile errors: 
	* Change <code>FragmentManager</code> to <code>SupportFragmentManager</code>
	* Change <code>Activity</code> to <code>FragmentActivity</code>
	* Change <code>MapFragment</code> to <code>SupportMapFragment</code>
	* Change <code>using Android.App;</code> to <code>using Android.Support.V4.App;</code>

At this point the projects will run on Android 2.2 and Android 2.3 devices. 

#Troubleshooting

##AAPT.EXE location incorrect

Google changed the location of certain tools in r22 of the Android SDK (release in mid-May, 2013), which may cause Xamarin.Android to report this error. The forum post [aapt.exe location incorrect](http://forums.xamarin.com/discussion/comment/15360/#Comment_15360) contains some helpful advice for dealing with this issue.

##"does not implement inherited abstract member" compile error

This error is typically caused by one of two things:

1. **Out of date version of Xamarin.Android** - As of June 13, 2013 please use Xamarin.Android 4.6.8 from the Stable update channel. Older versions of Xamarin.Android do not seem to work. The Beta and Alpha channels do not receive the same level of QA. Care and patience are required if you wish to use the Alpha or Beta builds for production applications.

2. **A stale `google-play-services_lib` directory** - Ensure that you have an up-to-date version of the Google Play Services library via the Android SDK manager, and ensure that the Android Library project that your project is using is also up to date. The forum post [Google Maps v2 and "does not implement inherited abstract member" messages](http://forums.xamarin.com/discussion/5030/google-maps-v2-and-does-not-implement-inherited-abstract-member-messages) will also be useful in troubleshooting this issue.
