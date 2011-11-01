MapsDemo
========

This demo shows how to use Managed Google Maps API.

It uses Google Maps API which is part of Android SDK add-ons.

Starting from Mono for Android 1.9.2, we provide Mono.Android.GoogleMaps.dll which
is a managed binding to the Java API so that you don't have to go through painful
hack around the Java API anymore).

Google requires per-app API key.  You can obtain a maps API key from here:
http://code.google.com/android/maps-api-signup.html

Once you obtained the key, you have to alter two parts of the sample sources to fully
run the demo app:

	- in Resource/Layout/Main.axml, replace "apiKey" attribute value with your API key.
	- in MapsDemo/MapsViewCompassDemo.cs, replace the second constructor parameter
	  in OnCreate() method.

