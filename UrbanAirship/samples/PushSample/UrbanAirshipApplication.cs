using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Locations;
using Xamarin.UrbanAirship.Push;
using Xamarin.UrbanAirship.RichPush;

namespace PushSample
{
	[Application]
	public class UrbanAirshipApplication : Application
	{
		public UrbanAirshipApplication ()
		{
		}

		public UrbanAirshipApplication (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		public override void OnCreate ()
		{
			var options = AirshipConfigOptions.LoadDefaultOptions (this);

			// Optionally, customize your config
			options.InProduction = false;
			options.DevelopmentAppKey = "";
			options.DevelopmentAppSecret = "";

			UAirship.TakeOff (this, options);

			//use CustomPushNotificationBuilder to specify a custom layout
			CustomPushNotificationBuilder nb = new CustomPushNotificationBuilder ();

			nb.StatusBarIconDrawableId = Resource.Id.icon;//custom status bar icon

			nb.Layout = Resource.Layout.notification;
			nb.LayoutIconDrawableId = Resource.Id.icon;//custom layout icon
			nb.LayoutIconId = Resource.Id.icon;
			nb.LayoutSubjectId = Resource.Id.subject;
			nb.LayoutMessageId = Resource.Id.message;

			PushManager.Shared ().NotificationBuilder = nb;
			PushManager.Shared ().IntentReceiver = Class.FromType (typeof(IntentReceiver));
			UALocationManager.Shared ().IntentReceiver = Class.FromType (typeof(IntentReceiver));
		}
	}
}

