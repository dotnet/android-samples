---
name: Xamarin.Android - OSMDroid Bindings for Xamarin.Android
description: This is an example of how to create bindings for a .jar file using a Java Binding Project in Xamarin.Android. This example creates a binding for...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-android
urlFragment: osmdroidbindingexample
---
# OSMDroid Bindings for Xamarin.Android

This is an example of how to create bindings for a .jar file using a Java Binding Project in Xamarin.Android.  This example creates a binding for [osmdroid](http://code.google.com/p/osmdroid/) - an alternative to Google Maps based on the [OpenStreetMap](http://www.openstreetmap.org).

There are two projects:

* `OsmDroid.csproj` is the solution that holds the Java Binding Project for the `osmdroid.jar`. The is project is kept in the directory `OsmDroid`.
* `OsmDroidTest.csproj` is a Xamarin.Android application that gives a quick example of using the new binding. The code for this project is kept in the directory `OsmDroidTest`. Because of a [bug](https://bugzilla.xamarin.com/show_bug.cgi?id=6695) in Xamarin.Android, the binding `OsmDroid` is built as a DLL and then copied to the `lib` folder. This DLL is then referenced by `OsmDroidTest`, and not the project file `OsmDroidBinding.csproj`.  

The lib folder holds the two reference `.jar` files that are necessary to bind OSMDroid.
