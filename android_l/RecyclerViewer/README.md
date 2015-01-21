Android 5.0 RecyclerViewer
==========================

This sample app accompanies the articles
[CardView](http://developer.xamarin.com/guides/android/platform_features/android_l/cardview) and
[RecyclerView](http://developer.xamarin.com/guides/android/platform_features/android_l/recyclerview).
You can use this sample to learn how to use the new `RecyclerView` widget
introduced in Android 5.0 Lollipop.

This app is a simple "Photo Album Viewer" that lets the user scroll up and 
down to view a collection of photos. Each photo has associated title 
text that describes the photo. Each photo/title combination is 
displayed using a `CardView`, and the app uses a single `RecyclerView` 
instance to hold and display over thirty unique `CardView` instances. A 
basic *adapter* is used to connect the "Photo Album" data with the 
`RecyclerView`. 

![](Screenshots/example-screens.png)


Requirements
------------

To build and run this sample, you must first enable Android 5.0 support as 
described in 
[Setting Up an Android 5.0 Project](http://developer.xamarin.com/guides/android/platform_features/android_l/introduction_to_android_l#settingup).

You must also add the **Xamarin.Android.Support.v7.CardView** and 
**Xamarin.Android.Support.v7.RecyclerView** packages to the 
**RecyclerViewer** project as described next. 

To add these packages in Xamarin Studio:

1. Open the project, right-click <span
   class="uiitem">Packages</span> and select <span class="uiitem">Add
   Packages</span>.
2. In the <span class="uiitem">Add Packages</span> dialog, search for
   <span class="uiitem">CardView</span>.
3. Select <span class="uiitem">Xamarin Support Library v7 CardView</span>,
   then click <span class="uiitem">Add Package</span>.
4. In the <span class="uiitem">Add Packages</span> dialog, search for
   <span class="uiitem">RecyclerView</span>.
5. Select <span class="uiitem">Xamarin Support Library v7 RecyclerView</span>,
   then click <span class="uiitem">Add Package</span>.

To add these packages in Microsoft Visual Studio:

1. Open the project, right-click the <span class="uiitem">References</span>
   node (in the <span class="uiitem">Solution Explorer</span>
   pane) and select <span class="uiitem">Manage NuGet Packages...</span>.
2. When the <span class="uiitem">Manage NuGet Packages</span> dialog is displayed,
   enter <span class="uiitem">CardView</span> in the search box.
3. When <span class="uiitem">Xamarin Support Library v7 CardView</span>
   appears, click <span class="uiitem">Install</span>.
4. In the <span class="uiitem">Manage NuGet Packages</span> dialog,
   enter <span class="uiitem">RecyclerView</span> in the search box.
5. When <span class="uiitem">Xamarin Support Library v7 RecyclerView</span>
   appears, click <span class="uiitem">Install</span>.

Author
------ 

Mark McLemore
