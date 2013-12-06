With Google Play services, your app can take advantage of the latest, Google-powered features such as Maps, Google+, and more, with automatic platform updates distributed as an APK through the Google Play store. This makes it faster for your users to receive updates and easier for you to integrate the newest that Google has to offer.

#### Google Technology
* Google Play services provides you with easy access to Google services and is tightly integrated with the Android OS. Easy-to-use client libraries are provided for each service that let you implement the functionality you want easier and faster.

#### Standard Authorization
* All products in Google Play services share a common authorization API that leverages the existing Google accounts on the device. You and your users have a consistent and safe way to grant and receive OAuth2 access tokens to Google services.

#### Automatic Updates
* Devices running Android 2.2 and newer and that have the Google Play Store app automatically receive updates to Google Play services. Enhance your app with the most recent version of Google Play services without worrying about your users' Android version.


Showing a Map
=============

This snippet shows how easy is to add a Map fragment to you app. **Additional setup is required please see** `GettingStarted` for more information.

### MainActivity.cs

```csharp
using Android.Gms.Maps;
// ...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	// add map fragment to frame layout
	var mapFragment = new MapFragment ();
	FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
	fragmentTx.Add (Resource.Id.linearLayout1, mapFragment);
	fragmentTx.Commit ();

	// Set our view from the "main" layout resource
	SetContentView (Resource.Layout.Main);
}
```

How It Works
============

### The Google Play services client library

The client library contains the interfaces to the individual Google services and allows you to obtain authorization from users to gain access to these services with their credentials. It also contains APIs that allow you to resolve any issues at runtime, such as a missing, disabled, or out-of-date Google Play services APK. The client library has a light footprint, so it won't have an adverse impact on your app's file size.

If you want to access added features or products, you can upgrade to a new version of the client library as they are released. However, upgrading is not necessary if you don't care about new features or bug fixes. We anticipate more Google services to be continuously added, so be on the lookout for these updates.

### The Google Play services APK
The Google Play services APK contains the individual Google services and runs as a background service in the Android OS. You interact with the background service through the client library and the service carries out the actions on your behalf. An easy-to-use authorization flow is also provided to gain access to the each Google service, which provides consistency for both you and your users.

The Google Play services APK is delivered through the Google Play Store, so updates to the services are not dependent on carrier or OEM system image updates. In general, devices running Android 2.2 (Froyo) or later and have the Google Play Store app installed receive updates within a few days. This allows you to use the newest APIs in Google Play services and reach most of the devices in the Android ecosystem (devices older than Android 2.2 or devices without the Google Play Store app are not supported).

![The Google Play services APK on user devices receives regular updates for new APIs, features, and bug fixes.][1]

### The benefits for your app
Google Play services gives you the freedom to use the newest APIs for popular Google services without worrying about device support. Updates to Google Play services are distributed automatically by the Google Play Store and new versions of the client library are delivered through the Android SDK Manager. This makes it easy for you to focus on what's important: your users' experience.

[1]: http://developer.android.com/images/play-services-diagram.png