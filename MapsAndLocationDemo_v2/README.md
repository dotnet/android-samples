Maps and Location Demo v2
=========================

This code shows how to use Google Maps v2 in an Android application and how to create the Java Binding Library project for most recent Google Play services client library.

This sample is relevant for users of Xamarin.Android 4.4 or Xamarin 4.6. If you are using Xamarin.Android 4.8 or higher, it is recommended that you use the Google Play Services component as demonstrated in the [MapsAndLocationDemo_v3](https://github.com/xamarin/monodroid-samples/tree/master/MapsAndLocationDemo_v3) sample.

According to the [Android Dashboard](http://developer.android.com/about/dashboards/index.html), nearly 77% of all Android devices are running Android 4.0 (API level 14) or higher so the focus of this sample is on API 14 and higher. Support for Android 2.2 (Froyo, API level 8) is only provided by revision 12 of Google Play Services. This means that the current revisions of Google Play Services client library do not support Android 2.2. This sample will only work with Android 4.0 or higher. 

The `Debug` build configuration contains the following projects, and targets API level 14 or higher. This build configuration will only compile the following projects:

* **LocationDemo** - this project shows how to use the LocationManager to figure out where the device is. This project does not require Google Play Services client library.
* **SimpleMapDemo** - this project demonstrates some of the simple features of Google Maps for Android v2. It does require the Google Play Services client library.
* **GooglePlayServices** - this is a Java Binding project for the Google Play Services.

This project does not include the Google Play services client library, which is a requirement for Google Maps v2. It is not possible for 3rd parties to distribute this API, so you must compile the binding project yourself.

**Note:** You must use Xamarin.Android 4.4 or higher to compile the Java binding project.

**Note:** Ensure that the package name of your application is all lower case. Android is very particular and the Google Maps API will not authenticate the API key property if the package name has mixed case.


Building the Google Play Client Services Library
------------------------------------------------

To access Google Maps v2, it is necessary to create a Xamarin.Android binding for the Google Play services client library. This example has a Xamarin.Android Binding Library project, but it does not have the Google Play Services client library. It is necessary to first compile the Google Play Client Services library. This section will outline how to do so .

### Prequisites

*Do not* skip these pre-requisites. They are **mandatory**.

These directions assume that you have [Apache Ant](http://ant.apache.org/) installed and in your `$PATH`. [Installing Apache Ant](http://ant.apache.org/manual/install.html) is beyond the scope of this document.

`android` is a command line utility that is required to prepare the Android Library project for compiliation. You must have the directory `$ANDROID_HOME/tools` has been added to your `$PATH`. 

**Note:** You *must* have these two command line utilities configured properly, or you will not be able to bind the Google Play Services in Xamarin.Android.

### Compiling the Google Play Services client library

To compile the Google Play Client Services library using Ant, follow these steps:

1. Use the Android SDK Manager to install Google Play Services.
2. Copy the directory located at `extras/google/google_play_services/libproject/google-play-services_lib` into the same directory as this README.
3. Build the project with `ant` like so:
    
        cd google-play-services_lib
        android update project -p .
        ant debug

4. Open the `MapsAndLocationDemo.sln`. Add the file `google-play-services_lib/project.properties` to the `GooglePlayServices` project as a ***linked*** file. It is crucial that <code>project.properties</code> be linked and not added. The following screenshot shows how to link <code>project.properties</code>:

![Link File](/images/link_file.png)

If this file is moved or copied to into the project, the build will fail and Xamarin.Android will display an error message similar to the following in the Build Output:

    Error   1   The "CreateLibraryResourceArchive" task failed unexpectedly.
      System.IO.PathTooLongException: The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
      at System.IO.PathHelper.GetFullPathName()
      at System.IO.Path.NormalizePath(String path, Boolean fullCheck, Int32 maxPathLength)
      at System.IO.Path.GetFullPathInternal(String path)
      at System.IO.File.InternalCopy(String sourceFileName, String destFileName, Boolean overwrite, Boolean checkHost)
      at System.IO.File.Copy(String sourceFileName, String destFileName, Boolean overwrite)
      at Xamarin.Android.Tasks.MonoAndroidHelper.CopyIfChanged(String source, String destination)


A bash script file has been provided which will automate this step for you. It assumes that you already have `ant` installed somewhere in your path and that the environment variable `$ANDROID_HOME` holds the path to your Android SDK.

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

In `AndroidManifest.xml`, add the following two elements as a child of the <application> element, by inserting it just before the closing tag </application> 

	<application android:label="@string/app_name">

		<!-- Put your Google Maps V2 API Key here. This key will not work for you.-->
		<!-- See https://developers.google.com/maps/documentation/android/start#obtaining_an_api_key -->
		<meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="<YOUR API KEY HERE>" />
		<meta-data android:name="com.google.android.gms.version" android:value="@integer/google_play_services_version" />

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

The [Google APIs Add-On](https://developers.google.com/android/add-ons/google-apis/) is only supported on emulator images that are running the most recent API level of Android. 

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
	
	
Backwards Compatibility with Froyo
----------------------------------

Google Play Services rev 12 was the last update to support on Android 2.2 (API level 8) or higher. The binding in this project will not work with these older API's. 

#Troubleshooting

##AAPT.EXE location incorrect

Google changed the location of certain tools in r22 of the Android SDK (release in mid-May, 2013), which may cause Xamarin.Android to report this error. The forum post [aapt.exe location incorrect](http://forums.xamarin.com/discussion/comment/15360/#Comment_15360) contains some helpful advice for dealing with this issue.

##"does not implement inherited abstract member" compile error

This error is typically caused by one of two things:

1. **Out of date version of Xamarin.Android** - As of June 13, 2013 please use Xamarin.Android 4.6.8 from the Stable update channel. Older versions of Xamarin.Android do not seem to work. The Beta and Alpha channels do not receive the same level of QA. Care and patience are required if you wish to use the Alpha or Beta builds for production applications.

2. **A stale `google-play-services_lib` directory** - Ensure that you have an up-to-date version of the Google Play Services library via the Android SDK manager, and ensure that the Android Library project that your project is using is also up to date. The forum post [Google Maps v2 and "does not implement inherited abstract member" messages](http://forums.xamarin.com/discussion/5030/google-maps-v2-and-does-not-implement-inherited-abstract-member-messages) will also be useful in troubleshooting this issue.
