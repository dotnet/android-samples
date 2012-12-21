using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

//[assembly:MetaData ("com.google.android.maps.v2.API_KEY", Value = "AIzaSyAyDUmQWH0tn3m0qSfTEag4QXYlYtTduzk")]
[assembly:UsesPermission (Android.Manifest.Permission.WriteExternalStorage)]
[assembly:UsesPermission (Android.Manifest.Permission.AccessCoarseLocation)]
[assembly:UsesPermission (Android.Manifest.Permission.AccessFineLocation)]
[assembly:UsesFeature (GLESVersion = 0x00020000, Required = true)]
