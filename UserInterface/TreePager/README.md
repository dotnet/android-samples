---
name: Xamarin.Android - TreePager
description: TreePager is a sample app that accompanies the article, ViewPager. This sample demonstrates how to use ViewPager and PagerTabStrip together to...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: userinterface-treepager
---
# TreePager 

**TreePager** is a sample app that accompanies the article, 
[ViewPager](http://developer.xamarin.com/guides/android/user_interface/viewpager/).

This sample demonstrates how to use `ViewPager` and `PagerTabStrip` 
together to implement a simple image gallery of deciduous and evergreen 
trees. The user swipes left and right through a tree catalog. On each 
page of the catalog, the name of the tree is listed in the 
`PagerTabStrip` and an image of the tree is displayed in an 
`ImageView`. Because of the relative simplicity of this app, the added 
complexity of using Fragments is unnecessary. 

The heavy lifting of this app takes place in **TreePagerAdapter.cs**, 
which adapts a tree catalog (implemented in **TreeCatalog.cs**) to a 
`ViewPager` that is located and initialized in **MainActivity.cs**. The 
`ViewPager` layout includes a `PagerTabStrip` in **Main.axml**. Note 
that this app depends on 
[Android Support Library v4](https://components.xamarin.com/gettingstarted/xamandroidsupportv4-18). 


![TreePager  application screenshot](Screenshots/1-larch.png "TreePager  application screenshot")

## Author

Copyright 2016 Xamarin

Created by Mark Mclemore
