Admob Sdk Binding Sample
========================

This is a Java binding project sample for Google AdMob SDK.

AdMob SDK is included in Android SDK extras package.

To get this sample working, you have to make some manual changes:

- either create a symbolic link to {android-sdk}/extras/google/admob_ads_sdk
  in this directory, or simply copy the directory here. Some project items
  (e.g. the library jar) depends on it.
  - You may also have to update the reference to jar file as Google may
    update the AdMob SDK version, changing the referenced jar filename.
- You need AdMob Publisher ID and you have to edit "ads:adUnitId" attribute
  in AdmobTest/Resources/layout/Main.axml. Replace "your_ad_unit_id" with it.
- If you are going to run it on a device instead of an emulator, you have
  to edit AdMobTest/MainActivity.cs to call adRequest.AddTestDevice() with
  the expected device identifier. Without that, adb log will tell you the
  ID.

