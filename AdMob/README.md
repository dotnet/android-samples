Admob Sdk Binding Sample
========================

This is a Java binding project sample for Google AdMob SDK.

AdMob SDK is included in Android SDK extras package.

To get this sample working, you have to make some manual changes:

- You need AdMob Publisher ID and you have to edit "ads:adUnitId" attribute
  in AdmobTest/Resources/layout/Main.axml. Replace "your_ad_unit_id" with it.
- Add Admob SDK .jar file to the project, in both AdMob and AdMobTest
  projects (by default each of them have "missing" references).
- If you are going to run it on a device instead of an emulator, you have
  to edit AdMobTest/MainActivity.cs to call adRequest.AddTestDevice() with
  the expected device identifier. Without that, adb log will tell you the
  ID.


