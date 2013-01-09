Mono San Angeles sample port
============================

This is a port of Android NDK sample (sanangeles).
http://developer.android.com/tools/sdk/ndk/index.html#Samples

The project contains pre-compiled libsanangeles.so under libs directory
(armeabi, armeabi-v7a and x86 are supported).

This library sample is easily ported because there is no dependency on
any jobject instances or JNIEnv instance in C code.

