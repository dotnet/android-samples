---
name: Xamarin.Android - App Invite Sample
description: This sample demonstrates how to allow your users to invite people they know to use your app using Google App Invites. Instructions Tap the Invite...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: google-services-appinvite
---
# App Invite Sample

This sample demonstrates how to allow your users to invite people they know to use your app using Google App Invites.

## Instructions

* Tap the Invite Friends button and Log in to a valid Google developer account.
* Follow these steps to [enable Google services for your app.](https://developers.google.com/mobile/add?platform=android&cntapi=appinvite&cnturl=https:%2F%2Fdevelopers.google.com%2Fapp-invites%2Fandroid%2Fguides%2Fapp%3Fconfigured%3Dtrue%23add-config&cntlbl=Continue%20Adding%20App%20Invites)
* Make sure to provide the correct package name - `com.xamarin.appinvite` for the sample when configuring.
* Refer to the following [documentation](https://docs.xamarin.com/guides/android/deployment,_testing,_and_metrics/MD5_SHA1/offline.pdf) if you need help locating your keystore's SHA1 signature.
* Once the service is configured, you'll be able to send an invite to an email address or phone number.


## Build Requirements
Using this sample requires the Android SDK platform for Android 5.0 (API level 21).


![App Invite Sample application screenshot](Screenshots/app-invites-sample.png "App Invite Sample application screenshot")

## Authors
Copyright (c) 2015 Google, Inc.

Ported from [Google App Invites Quickstart Sample](https://github.com/googlesamples/google-services/tree/master/android/appinvites)

Ported to Xamarin.Android by Aaron Sky