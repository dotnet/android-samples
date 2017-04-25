Xamarin.Android Service Samples
===============================

This directory holds the sample projects from the [Creating Services](https://developer.xamarin.com/guides/android/application_fundamentals/services/) guides for Xamarin.Android. There are multiple solutions in the subdirectories of this project. Each solution is meant to be a stand alone solution that focus on a specific topic.

* **BoundServicesDemo** &ndash; Example of [creating a bound service](https://developer.xamarin.com/guides/android/application_fundamentals/services/creating-a-service/bound-services/) with Xamarin.Android.

    ![](./BoundServiceDemo/Screenshots/bound-service.png)

* **ForegroundServiceDemo** &ndash; A sample demonstrating a [foreground service](https://developer.xamarin.com/guides/android/application_fundamentals/services/foreground-services/).

    ![](./ForegroundServiceDemo/Screenshots/foreground-service.png)

* **StartedServicesDemo** &ndash; An example of [creating a started service](https://developer.xamarin.com/guides/android/application_fundamentals/services/creating-a-service/started-services/) in Xamarin.Android.

    ![](./StartedServicesDemo/Screenshots/started-service.png)

* **MessengerService** &ndash; This solution is an example of IPC communication between an Android service and an Activity (using a Messenger and a Handler). It demonstates how perform one-way and two-way IPC calls between an Activty and a Service running in it's own process.

    Ensure that the **MessengerService** project is installed on the device _before_ the **MeessengerClient** project. If the **MessengerClient** is inadvertently installed first, it will be necessary to uninstall it, install the **MessengerService** app, and then reinstall the **MessengerClient** project.

    ![](./MessengerServiceDemo/Screenshots/service-messenger-activity.png)

Authors
-------

Tom Opgenorth (toopge@microsoft.com)
