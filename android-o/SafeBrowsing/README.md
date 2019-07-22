---
name: Xamarin.Android - SafeBrowsing
description: "SafeBrowsing is a simple project which demonstrates the safe browsing functionality of an Android Web View. This is a new feature in #androidoreo"
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: android-o-safebrowsing
---
# SafeBrowsing

SafeBrowsing is a simple project which demonstrates the safe browsing functionality of an Android Web View. This is a new feature added in Android 8.1 (API 27).

The recommended way to enable this feature is to add the following tag to your AndroidManifest file:
```xml
 <meta-data android:name="android.webkit.WebView.EnableSafeBrowsing"
            android:value="true" />
```

The StartSafeBrowsing method initializes safe browsing if the meta-data tag mentioned above is set. Safe browsing is not fully supported on all devices.

## Authors

Malkin Dmytro