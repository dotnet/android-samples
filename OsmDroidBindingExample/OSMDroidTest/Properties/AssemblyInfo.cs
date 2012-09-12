using System.Reflection;
using System.Runtime.CompilerServices;

using Android;
using Android.App;

[assembly: AssemblyTitle("OSMDroidTest")]
[assembly: AssemblyDescription("A simple program to test the bindings for OSM Droid.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0")]
// OSM Droid requires these permissions:


[assembly:UsesPermission(Manifest.Permission.AccessFineLocation)]
[assembly:UsesPermission(Manifest.Permission.AccessCoarseLocation)]
[assembly:UsesPermission(Manifest.Permission.AccessWifiState)]
[assembly:UsesPermission(Manifest.Permission.AccessNetworkState)]
[assembly:UsesPermission(Manifest.Permission.Internet)]
[assembly:UsesPermission(Manifest.Permission.WriteExternalStorage)]