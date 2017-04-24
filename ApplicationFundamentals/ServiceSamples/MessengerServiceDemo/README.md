
# Xamarin.Android Messenger and Service Sample

This solution is an example of IPC communication between an Android service and an Activity (using a Messenger and a Handler). It demonstates how perform one-way and two-way IPC calls between an Activty and a Service running in it's own process.

Ensure that the **MessengerService** project is installed on the device _before_ the **MeessengerClient** project. If the **MessengerClient** is inadvertently installed first, it will be necessary to uninstall it, install the **MessengerService** app, and then reinstall the **MessengerClient** project.

![](./Screenshots/service-messenger-activity.png)

## Authors

Tom Opgenorth (toopge@microsoft.com)
