using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly:UsesPermission (Android.Manifest.Permission.WriteExternalStorage)]
[assembly:UsesPermission (Android.Manifest.Permission.AccessCoarseLocation)]
[assembly:UsesPermission (Android.Manifest.Permission.AccessFineLocation)]
[assembly:UsesFeature (GLESVersion = 0x00020000, Required = true)]
