---
name: Xamarin.Android - Sanity Tests
description: SanityTests is a hodgepodge of things to test various interactions, such as SQLite use, JNI use, P/Invoke use, SSL, compression, and all manner of...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-android
urlFragment: sanitytests
---
# Sanity Tests

SanityTests is a hodgepodge of things to test various interactions,
such as SQLite use, JNI use, P/Invoke use, SSL, compression, and all
manner of other things.

This sample is *not* a "HOWTO" guide for how to 'properly' (sanely) do
something; it's our internal "test suite" (ha!) "thrown over the wall"
as it answers frequently recurring questions on the mailing list,
such as how to use Android.Runtime.JNIEnv, Java integration, and more.

Ideally such things will be split out into separate, easily digestible
samples in the future (presumably as part of ApiDemo), but in the
interest of expediency...

Note: Building this project requires that the Android NDK be installed.
The Android NDK is currently installed as part of the current installer,
but previous installs may lack it or require additional configuration
within your IDE to provide the path to the Android NDK.
