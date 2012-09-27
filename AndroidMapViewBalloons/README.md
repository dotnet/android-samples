android-mapviewballoons
=======================

This project contains the binding for the Android Library Project [android-mapviewballons](https://github.com/jgilfelt/android-mapviewballoons). 

The directory `android-mapviewballoons` holds the Java source code. 

The directory `mfa-mapviewballoons` holds the actual binding. Due to licensing restrictions, you must add in the Google API for Maps to the binding project yourself.  Copy `maps.jar` for API Level 8 into the folder `mfa-mapviewballoons/Jars` and change the build action of the file to `ReferenceJar`. ***Until this file is copied in, the Java Binding Library project will not compile***.

The folder `mfa-mapviewballoons-example` holds a sample Mono for Android application demonstrating the use of the binding. It will be necessary to update the file `mfa-mapviewballoons-example/Resources/Layout/main.axml` with a [Google Maps API Key](https://developers.google.com/android/maps-api-signup).

