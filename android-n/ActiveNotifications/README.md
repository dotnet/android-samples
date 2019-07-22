---
name: Xamarin.Android - Active Notifications Sample
description: "Demonstrates how to use the NotficationManager API to tell you how many notifications your application is currently showing... #androidnougat"
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: android-n-activenotifications
---
# Active Notifications Sample

This sample demonstrates how to use the NotficationManager API to tell you how many notifications your application is currently showing.

## Instructions

* Touch the 'Add a Notification' button to display a notification.
* Scroll the notification bar down to clear notifications.

## Build Requirements
Using this sample requires the Android M Developer Preview and the Xamarin.Android M Web Preview

## Notes
This sample will crash when attempting to simultaniously clear larger sets of notifications. See
https://github.com/googlesamples/android-ActiveNotifications/issues/1 for more info.

![Active Notifications Sample application screenshot](Screenshots/active-notifications.png "Active Notifications Sample application screenshot")

## Authors
Copyright (c) 2014 The Android Open Source Project, Inc.
Ported from [Android ActiveNotification Sample](https://github.com/googlesamples/android-ActiveNotifications)
Ported to Xamarin.Android by Peter Collins