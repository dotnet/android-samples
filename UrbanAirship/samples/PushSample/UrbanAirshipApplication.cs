using System;
using Android.App;
using Android.OS;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Push;
using Xamarin.UrbanAirship.RichPush;
using Android.Runtime;

namespace PushSample
{
	// Since a simple port didn't initialize things well, we needed to find some hack to make it just run.
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

		public const string MESSAGE_ID_RECEIVED_KEY = "com.xamarin.samples.urbanairship.pushsample.MESSAGE_ID_RECEIVED";
		public const string HOME_ACTIVITY = "Home";
		public const string INBOX_ACTIVITY = "Inbox";
		public static readonly string[] navList = new string[] {
			HOME_ACTIVITY, INBOX_ACTIVITY
		};

		public override void OnCreate ()
		{
			UAirship.TakeOff (this);
			//PushManager.Shared ().IntentReceiver = Java.Lang.Class.FromType (typeof(IntentReceiver));
			RichPushManager.SetJavascriptInterface (Java.Lang.Class.FromType (typeof(RichPushMessageJavaScript)), "urbanairship");

			/*
			// If running on Jelly Bean or higher, then use the inbox style notification builder
			if (Build.VERSION.SdkInt >= 16) {
				PushManager.Shared().NotificationBuilder = new RichNotificationBuilder ();
			}
			*/
		}
	}
}

