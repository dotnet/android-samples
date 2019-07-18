---
name: Xamarin.Android - AutofillFramework Sample
description: This sample demonstrates the use of the Autofill Framework. It includes implementations of client Activities with views that should be autofilled,...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-android
urlFragment: android-o-autofillframework
---
# AutofillFramework Sample

This sample demonstrates the use of the Autofill Framework. It includes implementations of client Activities with views that should be autofilled, and a Service that can provide autofill data to client Activities.

## Instructions

To set the device's default Autofill service to the one in the sample, edit Settings > System > Languages & Input > Advanced > Auto-fill service and select the sample app. To edit the service's settings, tap the settings icon next to the Auto-fill service list item or open the Autofill Settings launcher icon.. Here, you can set whether you want to enable authentication on the entire autofill Response or just on individual autofill datasets. You should also set the master password to “unlock” authenticated autofill data with.


## Build Requirements
Using this sample requires the Android 8.0 (API 26) and the Xamarin.Android 8.0 or higher.


![AutofillFramework Sample application screenshot](Screenshots/ask_for_save.png "AutofillFramework Sample application screenshot")

## Authors
Copyright (c) 2017 The Android Open Source Project, Inc.  
Ported from [Android AutofillFramework Sample](https://github.com/googlesamples/android-AutofillFramework/).  
Ported to Xamarin.Android by Malkin Dmytro.