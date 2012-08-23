WebView JavaScript Interface
============================

This demonstrates C# - to - JavaScript interoperability in WebView (through
Java interface).

For the API details, see:
http://docs.mono-android.net/?link=M%3aAndroid.Webkit.WebView.AddJavascriptInterface%28Java.Lang.Object%2cSystem.String%29

This sample requires Mono for Android 4.1 or later, since it makes use of
Java.Interop.ExportAttribute.


Testing Note
============

_DO NOT RUN_ this sample on an API 10 (Android v2.3.3) emulator; it _will_ break

    W/dalvikvm(  390): JNI WARNING: jarray 0x40549fe8 points to non-array object (Ljava/lang/String;)
    I/dalvikvm(  390): "WebViewCoreThread" prio=5 tid=11 NATIVE
    I/dalvikvm(  390):   | group="main" sCount=0 dsCount=0 obj=0x40522a58 self=0x310c18
    I/dalvikvm(  390):   | sysTid=411 nice=0 sched=0/0 cgrp=default handle=3639344
    I/dalvikvm(  390):   | schedstat=( 420074041 479837048 123 )
    I/dalvikvm(  390):   at android.webkit.WebViewCore.nativeTouchUp(Native Method)
    I/dalvikvm(  390):   at android.webkit.WebViewCore.nativeTouchUp(Native Method)
    I/dalvikvm(  390):   at android.webkit.WebViewCore.access$3300(WebViewCore.java:53)
    I/dalvikvm(  390):   at android.webkit.WebViewCore$EventHub$1.handleMessage(WebViewCore.java:1158)
    I/dalvikvm(  390):   at android.os.Handler.dispatchMessage(Handler.java:99)
    I/dalvikvm(  390):   at android.os.Looper.loop(Looper.java:123)
    I/dalvikvm(  390):   at android.webkit.WebViewCore$WebCoreThread.run(WebViewCore.java:629)
    I/dalvikvm(  390):   at java.lang.Thread.run(Thread.java:1019)
    I/dalvikvm(  390): 
    E/dalvikvm(  390): VM aborting
    W/        (  390): Thread 0x0 may have been prematurely finalized
    D/Zygote  (   33): Process 390 terminated by signal (11)

Why? Android bug:

 * http://code.google.com/p/android/issues/detail?id=12987
 * http://stackoverflow.com/questions/5253916/why-does-the-webviewdemo-die
