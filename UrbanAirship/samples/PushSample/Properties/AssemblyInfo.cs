using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("LocationPushSample")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("atsushi")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("1.0.0")]

// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

[assembly:Android.App.UsesPermission ("com.google.android.c2dm.permission.RECEIVE")]
[assembly:Android.App.Permission (Name = "com.xamarin.samples.urbanairship.pushsample.permission.C2D_MESSAGE", ProtectionLevel = Android.Content.PM.Protection.Signature)]
[assembly:Android.App.UsesPermission ("com.xamarin.samples.urbanairship.pushsample.permission.C2D_MESSAGE")]
[assembly:Android.App.UsesPermission (Android.Manifest.Permission.AccessFineLocation)]

