Android 5.0 RecyclerViewer
==========================

This sample app accompanies the article 
[RecyclerView and CardView](http://developer.xamarin.com/guides/android/platform_features/android_l/recyclerview_and_cardview).
You can use this sample to learn how to use the new `RecyclerView` widget
introduced in Android 5.0 Lollipop.

This app is a simple "Photo Album" that lets the user scroll up and 
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

If you are using a pre-release version of Xamarin Studio, you must add 
the *Xamarin.Android.Support.v7.CardView* and 
*Xamarin.Android.Support.v7.RecyclerView* packages to the **RecyclerViewer** 
project as described next. 

To install the *Xamarin.Android.Support.v7.CardView* package:

1.  Right-click the **RecyclerViewer** project, click **Add**, **Add Packages**.

2.  In **Add Packages**, enable **Show pre-release packages**, then
    enter **CardView** in the search box. 

3.  When the **Xamarin Support Library CardView** package appears, 
    click **Add Package**.

To install the *Xamarin.Android.Support.v7.RecyclerView* package:

1.  Right-click the **RecyclerViewer** project, click **Add**, **Add Packages**.

2.  In **Add Packages**, enable **Show pre-release packages**, then
    enter **RecyclerView** in the search box. 

3.  When the **Xamarin Support Library RecyclerView** package appears, 
    click **Add Package**.

Author
------ 

Mark McLemore
