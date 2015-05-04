Android Wear
============================

This folder contains Android Wear samples.

Many Wear samples contain both a handheld (phone) and Wear project.

The wear app in these samples is embedded within the handheld app, so by running the handheld app in release mode, the wear app will be automatically installed onto the watch paired to the handheld device. Sometimes it takes a minute or so to install so it won’t respond to the handheld app until then. In debug mode, the wear app will need to be installed in the same way that you would any other app.
  
When using an emulator to run the Wear project, in order to pair it to a connected device,  you must run:
```
adb -d forward tcp:5601 tcp:5601
```
For further information, refer to [Android Developer](http://developer.android.com/training/wearables/apps/creating.html#SetupEmulator).