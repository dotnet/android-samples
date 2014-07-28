###Prerequisites###

You must [sign up][4] with a Gmail or other Google account in order to download the preview support library and receive access to the Android Wear Preview app on Google Play Store.

For instructions on installing the _Android Wear system image_ and setting up the _Android Wear Emulator_ and the _Android Wear Preview App_, see the [Get Started][5] page for Android Wear.

###Showing a two page notification###
	

Creating a default notification as the first page:

``` csharp
var builder = new NotificationCompat.Builder (this)
	.SetSmallIcon (Resource.Drawable.ic_logo)
	.SetLargeIcon (largeIcon)
	.SetContentText ("Short Service announcement")
	.SetContentTitle ("Page 1")
	.SetContentIntent (pendingIntent);
```

Using BigText style for the second notification page:

``` csharp
var secondPageStyle = new NotificationCompat.BigTextStyle();
secondPageStyle.BigText("Xamarin.Android goes on your Android Wear too");

var secondNotification = new NotificationCompat.Builder (this)
	.SetSmallIcon (Resource.Drawable.ic_logo)
	.SetLargeIcon (largeIcon)
	.SetContentTitle ("Page 2")
	.SetStyle (secondPageStyle)
	.Build ();
```

Bridging the two pages together using some of the new API introduced with Android Wear:

``` csharp
var twoPageNotification = new WearableNotifications.Builder(builder)
	.AddPage(secondNotification)
	.Build();

NotificationManagerCompat.From (this)
	.Notify ("multi-page-notification", 1000, twoPageNotification);
```

[4]: http://developer.android.com/wear/preview/signup.html
[5]: http://developer.android.com/wear/preview/start.html

