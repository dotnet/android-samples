using System;
using Android.App;

// Note that you need to obtain API key and add it as meta-data element in AndroidManifest.xml.
[assembly:UsesPermission (Android.Manifest.Permission.Internet)]
[assembly:UsesPermission ("com.google.android.providers.gsf.permission.READ_GSERVICES")]
