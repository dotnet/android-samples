---
name: Xamarin.Android - WatchFace
description: "Demonstrates how to use CanvasWatchFaceService and CanvasWatchFaceService.Engine to implement a custom Android Wear watch face"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - androidwear
urlFragment: wear-watchface
---
# WatchFace

**WatchFace** is a sample app that accompanies the article,
[Creating a Watch Face](https://docs.microsoft.com/xamarin/android/wear/platform/creating-a-watchface).

This sample demonstrates how to use `CanvasWatchFaceService` and
`CanvasWatchFaceService.Engine` to implement a custom Android Wear
watch face. This analog-style watch face sports an hour hand, a minute
hand, and a seconds hand; it also handles changes between ambient mode
and interactive mode. A time zone receiver listens for time zone
changes and updates the time as needed.

The watch face service is packaged with a very simple app that is used
only as a packaging vehicle for getting the watch face service into the
Wear device (the app doesn't interact with the watch face).

To run the watch face:

1. Build and deploy the solution to the Wear device.

2. Swipe right until you the default watch face appears.

3. Press down for a second to enter the watch face picker
    (alternately, you can enter **Setup** and tap **Change watch face**).

4. Swipe until you see the **Xamarin Sample** watch face.

5. Tap to select the **Xamarin Sample** watch face.

Note that this app depends on the
[Xamarin Android Wear Support Libraries](https://www.nuget.org/packages/Xamarin.Android.Wear).

This sample was ported from the Java **WatchFace** sample described in the Android Developer
[Drawing Watch Faces](https://developer.android.com/training/wearables/watch-faces/drawing.html)
topic.
