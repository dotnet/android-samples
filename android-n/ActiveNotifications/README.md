---
name: Xamarin.Android - Active Notifications Sample
description: "Demonstrates how to use the NotficationManager API to tell you how many notifications your application is showing... (Android Nougat)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - androidnougat
urlFragment: android-n-activenotifications
---
# Active Notifications Sample

This sample demonstrates how to use the NotficationManager API to tell you how many notifications your application is currently showing.

![Android app with notifications](Screenshots/active-notifications.png)

## Instructions

* Touch the 'Add a Notification' button to display a notification.
* Scroll the notification bar down to clear notifications.

## Build Requirements

Using this sample requires the Android M Developer Preview and the Xamarin.Android M Web Preview

## Notes

This sample will crash when attempting to simultaneously clear larger sets of notifications. See
https://github.com/googlesamples/android-ActiveNotifications/issues/1 for more info.

## License

Copyright (c) 2014 The Android Open Source Project, Inc.
Ported from [Android ActiveNotification Sample](https://github.com/googlesamples/android-ActiveNotifications)
