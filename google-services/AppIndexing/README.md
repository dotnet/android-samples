App Indexing Sample
=================

This sample demonstrates how to make your app viewable in Google Search using Google App Indexing.

Instructions
------------

* Run app from VS or XS on devicer or emulator
* Close app from the device or emulator
* Open terminal and run ```adb shell am start -a android.intent.action.VIEW -d "http://www.example.com/articles/test" com.xamarin.appindexing```

Authors
-------
Copyright (c) 2016 Google, Inc.
Ported from [Google App Indexing Quickstart Sample](https://github.com/firebase/quickstart-android/blob/master/app-indexing)
Ported to Xamarin.Android by Aaron Sky and Gonzalo Martin
