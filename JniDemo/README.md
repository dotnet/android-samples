Java Native Invoke Sample
=========================

This sample shows how to manually bind to a Java library so it can
be consumed by a Mono for Android application.

Note this binds to the Google Maps library as a sample, but Mono for Android
provides bindings to the Google Maps library for you.  If you just want
to use Maps, see the MapsDemo sample instead.

Requirements
------------

There are three requirements in order to build and run this sample:

 1. Mono for Android Preiew 13 or later.
 2. The Google APIs Android SDK add-on.
 3. A device with Google Maps support.
 4. A keystore file.
 5. Obtain an API Key for use with Google Maps.

Commands that need to be executed are indicated within backtics (`),
and $ANDROID_SDK_PATH is the directory that contains your Android SDK
installation.

Installing the Google APIs Android SDK add-on
---------------------------------------------

To install the Google APIs Android SDK add-on:

 1. Launch the Android SDK adn AVD manager:
        `$ANDROID_SDK_PATH/tools/android`

 2. Within the Android SDK and AVD manager, click Available packages
    in the left-hand pane.

 3. In the right-hand pane, navigate to the tree view node Third party
    Add-ons /Google Inc. add-ons (dl-ssl.google.com)

 4. Select the check-box for "Google APIs by Google Inc., Android API
    8, revision 2".

 5. Click the Install Selected button in the lower right corner.

 6. In the "Choose Packages to Install" dialog, select the Accept
    radio button, then click the Install button.

Creating a device with Google Maps support
------------------------------------------

To verify that your target device has Google Maps support, you can use
`$ANDROID_SDK_PATH/platform-tools/adb shell ls /system/framework/*map*`
to see if Google Maps support is present.  It should be present on
hardware devices, but may not be present within some emulators.

If you need an emulator with Google Maps support:

 1. Launch the Android SDK adn AVD manager:
        `$ANDROID_SDK_PATH/tools/android`

 2. Within the Android SDK and AVD manager, click Virtual devices
    in the left-hand pane.

 3. Click the New... button on the right-hande side.

 4. In the Create new Andorid Virtual Device (AVD) dialog, provide a
    name for the device (e.g. MAPS), and in the Target drop-down 
    select the Google APIs (Google Inc.) - API Level 8 entry.

 5. Click the Create AVD button.

You may now launch the emulator with: 

        `$ANDROID_SDK_PATH/tools/emulator -partition-size 512 -avd MAPS`

Keystore
--------

The Google Maps API key is based on the `keytool`-generated keystore.
Consequently, you must use the same certificate every time you sign the
application. Since the default Mono for Android build process always
creates a new debug keystore (and one only good for 6 months, and thus not
suitable for use on the Android Market), we need to alter the default
behavior.

This involves two steps:

 1. Creating the keystore to use.
 2. Altering the build process to use the new keystore.

To simplify matters, this project includes a `public.keystore` file
within the repository, but this is likely not a good idea for
production apps:

        http://developer.android.com/guide/publishing/app-signing.html#cert

The `public.keystore` file was generated using the command:

        `keytool -genkey -v -keystore public.keystore -alias public -keyalg RSA -keysize 2048 -validity 10000`

Once you have a keystore to use, you need to tell Mono for Android to
use your keystore to sign the final package instead of using the
default debug keystore. This requires modifying the .csproj in
accordance with:

        http://mono-android.net/Documentation/Build_Process#Signing

The required build system changes have already been done for `GoogleMaps.csproj`:

        <PropertyGroup>
            <AndroidKeyStore>True</AndroidKeyStore>
            <AndroidSigningKeyStore>public.keystore</AndroidSigningKeyStore>
            <AndroidSigningStorePass>public</AndroidSigningStorePass>
            <AndroidSigningKeyAlias>public</AndroidSigningKeyAlias>
            <AndroidSigningKeyPass>public</AndroidSigningKeyPass>
        </PropertyGroup>

Again, for actual non-sample apps, it may not be advisable to have the
private keystore within source control, as is done for this sample.

Google Maps API Key
-------------------

See the Google Maps sample/tutorial at
http://mobiforge.com/developing/story/using-google-maps-android
for more information about installing and using the API key.  You will
also need to edit the file Resources\Layout\Map.axml and edit the
/RelativeLayout/com.google.android.maps.MapView/@android:apiKey
attribute to contain your Google Maps API Key.

If you don't change this, you'll get a blank map, though the sample
will still serve to demonstrate how C# and Java interop can work.

How it works
------------

As Mono for Android 1.0 does not support binding arbitrary .jar
files (only the Android SDK android.jar is bound), alternative
mechanisms must instead used for interoperation between Java and
managed code.  Two primary mechanisms are:

 1. Android.Runtime.JNIEnv for creating instances of Java objects and
    invoking methods on them from C#.  This is very similar to
    System.Reflection in that everything is string based and thus
    untyped at compile time.

 2. The ability to include Java source code and .jar files into the
    resulting Android application.

Use of the Google Maps API requires two things:

 1. com.google.android.maps.MapActivity MUST be subclassed and the
    MapActivity.isRouteDisplayed() method overridden.

 2. The MapActivity subclass must be created BEFORE creating any
    MapView instances, AND the MapView instances can ONLY be added as
    children of the MapActivity.

Because we need to subclass an unbound Java type (1), we will need to
write custom Java code, thus necessitating Mechanism (2).

The Java source code is kept in MyMapActivity.java, which is included
in the project with a Build Action of AndroidJavaSource.

Furthermore, we edit Properties\AndroidManifest.xml so that it
contains three additional elements:

 1. /manifest/uses-permission must be added to include the
    android.permission.INTERNET permission.  This can be done within 
    the IDE, if desired.

 2. /manifest/application/uses-library must be added to specify that
    the com.google.android.maps library will be used.

 3. A /manifest/application/activity element must be created so that
    we can use Context.StartActivity() to create the custom activity.

This translates to having the following XML within
AndroidManifest.xml:

	<application android:label="Managed Maps">
		<uses-library android:name="com.google.android.maps" />
		<activity android:name="mono.samples.googlemaps.MyMapActivity" />
	</application>
	<uses-permission android:name="android.permission.INTERNET" />

MyMapActivity.java uses the Resources\Layout\Map.axml resource, which
contains a com.google.android.maps.MapView instance.
Resource.Layout.Map CANNOT be used from Activity1.cs, as MapView
requires that the constructing Activity be a MapActivity subclass.

Instead, Activity1.cs uses Java.Lang.Class.FindClass() to obtain a
Java.Lang.Class instance for the custom MyMapActivity type, then we
create an Intent to pass to Activity.StartActivity() to launch the
MapActivity subclass.

The <uses-library/> element is used by monodroid.exe to lookup the
appropriate Android SDK Add-on and include the .jar file in the Java
source compilation so that everything compiles properly.  No
additional .jar files need to be specified.
