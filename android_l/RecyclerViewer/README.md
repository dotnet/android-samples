RecyclerViewer
==============

This sample app accompanies the articles
[CardView](http://developer.xamarin.com/guides/android/platform_features/android_l/cardview) and
[RecyclerView](http://developer.xamarin.com/guides/android/platform_features/android_l/recyclerview).
You can use this sample to learn how to use the new `CardView` and `RecyclerView` widgets
introduced with Android 5.0 Lollipop.

This app is a simple "Photo Album Viewer" that lets the user scroll up 
and down to view a collection of photos. Each photo, which consists of 
an image with caption text, is displayed as a row item in the 
`RecyclerView`. The <span class="uiitem">Random Pick</span> button 
randomly swaps a photo in the collection with the first photo to 
demonstrate how `RecyclerView` is updated when the data set changes. 
When the user taps a photo, a toast appears to display the number of 
the photo within the collection &ndash; this demonstrates how item view 
click handlers work. 

![](Screenshots/example-screens.png)

Each image/caption (photo) row item is displayed in a `CardView` 
layout, and the app uses a single `RecyclerView` layout to hold and 
display over thirty unique row items. An *adapter* is used to connect 
the `RecyclerView` with a simple "Photo Album" database, a 
*view-holder* is used to cache view references, and a linear *layout 
manager* positions the `CardView` row items within the `RecyclerView`. 


Requirements
------------

To build and run this sample, you must add the 
**Xamarin.Android.Support.v7.CardView** and 
**Xamarin.Android.Support.v7.RecyclerView** packages to the 
**RecyclerViewer** project as described next. 

To add these packages in Xamarin Studio:

1. Open the project, right-click **Packages** and 
   select **Add Packages**. 
2. In the **Add Packages** dialog, search for **CardView**.
3. Select **Xamarin Support Library v7 CardView**,
   then click **Add Package**.
4. In the **Add Packages** dialog, search for
   **RecyclerView**.
5. Select **Xamarin Support Library v7 RecyclerView**,
   then click **Add Package**.

To add these packages in Microsoft Visual Studio:

1. Open the project, right-click the **References**
   node (in the **Solution Explorer** pane) and select 
   **Manage NuGet Packages...**.
2. When the **Manage NuGet Packages** dialog is displayed,
   enter **CardView** in the search box.
3. When **Xamarin Support Library v7 CardView**
   appears, click **Install**.
4. In the **Manage NuGet Packages** dialog,
   enter **RecyclerView** in the search box.
5. When **Xamarin Support Library v7 RecyclerView**
   appears, click **Install**.

Author
------ 

Mark McLemore
