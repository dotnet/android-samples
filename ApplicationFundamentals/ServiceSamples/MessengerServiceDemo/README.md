---
name: Xamarin.Android - Xamarin.Android Messenger and Service Sample
description: This solution is an example of IPC communication between an Android service and an Activity (using a Messenger and a Handler). It demonstates how...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-android
urlFragment: applicationfundamentals-servicesamples-messengerservicedemo
---
# Xamarin.Android Messenger and Service Sample

This solution is an example of IPC communication between an Android service and an Activity (using a Messenger and a Handler). It demonstates how to perform one-way and two-way IPC calls between an Activity and a Service running in it's own process.

Ensure that the **MessengerService** project is installed on the device _before_ the **MessengerClient** project. If the **MessengerClient** is inadvertently installed first, it will be necessary to uninstall it, install the **MessengerService** app, and then reinstall the **MessengerClient** project.

![](./Screenshots/service-messenger-activity.png)

**Note**: Currently it is not possible to implement an `IsolatedProcess` in Xamarin.Android. Please see [Bugzilla 51940 -  Services with isolated processes and custom Application class fail to resolve overloads properly](https://bugzilla.xamarin.com/show_bug.cgi?id=51940) for more details.


![Xamarin.Android Messenger and Service Sample application screenshot](Screenshots/service-messenger-activity.png "Xamarin.Android Messenger and Service Sample application screenshot")

## Authors

Tom Opgenorth (toopge@microsoft.com)
