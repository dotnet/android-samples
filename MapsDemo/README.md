MapsDemo
========

This demo shows how to use Managed Google Maps API.

It uses Google Maps API which is part of Android SDK add-ons.

Since Mono for Android 1.9.2, we provide Mono.Android.GoogleMaps.dll which
is a managed binding to the Java API.

There are 2 things you need to use Google Maps on Android device:
- Java Google Maps Library
- Google Maps API Key

Java Google Maps Library
------------------------

To get the Java Google Maps library, open the Android SDK Manager, and download
the "Google APIs by Google Inc".  Because this library is per-api level, you 
will need to download it for each API level you want to target.

Google Maps API Key
-------------------

Google Maps requires a per-app API key.  You can obtain a maps API key from here:
http://docs.xamarin.com/android/advanced_topics/Obtaining_a_Google_Maps_API_Key

Once you obtained the key, you have to alter two parts of the sample sources to fully
run the demo app:

	- in Resource/Layout/Main.axml, replace "apiKey" attribute value with your API key.
	- in MapsDemo/MapsViewCompassDemo.cs, replace the second constructor parameter
	  in OnCreate() method.

If you see an empty beige grid, that means the MapsView is working, but you do not
have a valid Maps API key.