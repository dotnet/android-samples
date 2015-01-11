using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

[assembly: AssemblyTitle("FilePickerExample")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FilePickerExample")]
[assembly: AssemblyCopyright("Copyright ©  2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("a557ce8c-9dbe-4b93-8fc4-95ffc126cf14")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


#if DEBUG
[assembly:Application(Debuggable = true, Label="@string/app_name", Icon="@drawable/ic_launcher")]
#else
[assembly:Application(Debuggable = false, Label="@string/app_name", Icon="@drawable/ic_launcher")]
#endif

#if __ANDROID_16__
// This is a new permission for API Level 16
[assembly:UsesPermission(Android.Manifest.Permission.ReadExternalStorage )]
#endif