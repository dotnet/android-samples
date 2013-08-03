Maps and Location Demo v3
=========================

This code shows how to use Google Maps v2 in an Android application. According to the [Android Dashboard](http://developer.android.com/about/dashboards/index.html), nearly 61% of all Android devices are running Android 4.0 (API level 14) or higher so the focus of this sample is on API 14 and higher. 

This sample is relevant for users of Xamarin.Android 4.8. If you are using an Xamarin.Android 4.4 or 4.6, you will have to create a Java Binding project as demonstrated in the [MapsAndLocationDemo_v2](https://github.com/xamarin/monodroid-samples/tree/master/MapsAndLocationDemo_v2) sample.

The `Debug` build configuration contains the following projects, and targets API level 14 or higher. This build configuration will only compile the following projects:

* **LocationDemo** - this project shows how to use the LocationManager to figure out where the device is. This project does not require Google Play Services client library.
* **SimpleMapDemo** - this project demonstrates some of the simple features of Google Maps for Android v2. It does require the Google Play Services client library.

In the `Debug_Froyo` build configuration, and it targets API level 8 and higher. This build configuration will only compile the following project:

* **SimpleMapDemo_Froyo** - this project demonstrates some of the simple features of Google Maps for Android v2. It does require the Google Play Services client library. 

**Note:** You must use Xamarin.Android 4.8 or higher for this sample. If you have an older version of Xamarin.Android then you must create your own Java Binding project for Google Play Services. For an example of how to do this, please consult the [MapsAndLocationDemo_V2](https://github.com/xamarin/monodroid-samples/tree/master/MapsAndLocationDemo_v2) sample.

**Note:** Ensure that the package name of your application is all lower case. Android is very particular and the Google Maps API will not authenticate the API key property if the package name has mixed case.

## Prerequisites

These sample use the Google Play Services component that is available in the Xamarin Component Store. You must have Xamarin.Android 4.8 in order to use this component.

There are two versions of this component: [one for Froyo](https://components.xamarin.com/view/googleplayservicesfroyo/) and [one for Ice Cream Sandwich](https://components.xamarin.com/view/googleplayservices/). You will need to install the Google Play Services client library and the components before these samples with work.

You must also have the Google Play Client Services library installed. You can install this by using the Android SDK Manager. This library is available under the *Extras* folder:

![Link File](/images/android_sdk_manager.png)

Google Maps v2 API Key
----------------------

You must [obtain a new API Key](https://developers.google.com/maps/documentation/android/start#the_google_maps_api_key) for Google Maps v2. API keys from Google Maps v1 will not work. 

The location of the debug.keystore file that Xamarin.Android uses depends on your platform:

- **Windows Vista / Windows 7 / Windows 8**: `C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore`
- **OSX** : `/Users/[USERNAME]/.local/share/Xamarin/Mono for Android/debug.keystore`

To obtain the SHA1 fingerprint of the debug keystore, you can use the `keytool` command that is a part of the JDK. This is an example of using `keytool` at the command-line:

    $ keytool -V -list -keystore debug.keystore -alias androiddebugkey -storepass android -keypass android

Adding the API Key to your application
--------------------------------------

It goes in your application's manifest, contained in the file Properties/AndroidManifest.xml. From there, the Maps API reads the key value and passes it to the Google Maps server, which then confirms that you have access to Google Maps data. 

In AndroidManifest.xml, add the following element as a child of the <application> element, by inserting it just before the closing tag </application> 

    <application android:label="@string/app_name">

		<!-- Put your Google Maps V2 API Key here. This key will not work for you.-->

		<!-- See https://developers.google.com/maps/documentation/android/start#obtaining_an_api_key -->

		<meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="SAzaSyC1O8yQaNtuur4t5y6u7ZBPnYdVDgYKHtfA8" />

	</application>

Specifying additional permissions
---------------------------------

Besides permissions required by other parts of your application, you must add the following permissions to AndroidManifest.xml in order to use the Google Maps Android API: 

	<!-- We need to be able to download map tiles and access Google Play Services-->

	<uses-permission android:name="android.permission.INTERNET" />

	<!-- Allow the application to access Google web-based services. -->

	<uses-permission android:name="com.google.android.providers.gsf.permission.READ_GSERVICES" />

	<!-- Google Maps for Android v2 will cache map tiles on external storage -->

	<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

	<!-- Google Maps for Android v2 needs this permission so that it may check the connection state as it must download data -->

	<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

	<!-- These are optional, but recommended. They will allow Maps to use the My Location provider. -->

	<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />

	<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
  
Verifying Google Play Services installation on your device or emulator
----------------------------------------------------------------------

[Google Play Services](https://play.google.com/store/apps/details?id=com.google.android.gms) must be installed on a device or emulator before Google Maps for Android v2 will work.

Emulators using the [Google APIs Add-On](https://developers.google.com/android/add-ons/google-apis/) with API 17 and higher have Google Play Services included in the Google APIs Add On.

Emulators not using the Google APIs Add-On images, *will not have Google Play Services installed*. The appropriate APKs may be manually installed into the emulator image, but installing Google Play Services is beyond the scope of this example. 

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
