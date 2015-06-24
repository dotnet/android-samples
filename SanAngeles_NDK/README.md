Mono San Angeles sample port
============================

This is a port of Android NDK sample (sanangeles).
http://developer.android.com/tools/sdk/ndk/index.html#Samples


This library sample is easily ported because there is no dependency on
any jobject instances or JNIEnv instance in C code.

There are **TWO** versions available:


1) C++ for Visual Studio 2015 (SanAngeles_NativeDebug)
---

Complete C++ source is built as part of the solution, which means you can debug the C++ code while running the Xamarin.Android project using **Visual Studio 2015**.



2) Pre-compiled (SanAngeles_NDK)
-----

The project contains pre-compiled libsanangeles.so under libs directory
(armeabi, armeabi-v7a and x86 are supported). You'll only debug the Xamarin.Android C# code with this example.


