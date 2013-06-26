/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Push;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	public class RichPushApplication : Application
	{
		public RichPushApplication ()
		{
		}

		public RichPushApplication (System.IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		public const string MESSAGE_ID_RECEIVED_KEY = "com.xamarin.samples.urbanairship.richpush.MESSAGE_ID_RECEIVED";
		public const string HOME_ACTIVITY = "Home";
		public const string INBOX_ACTIVITY = "Inbox";
		public static readonly String[] navList = new String[] {
			new String (HOME_ACTIVITY), new String (INBOX_ACTIVITY)
		};

		public override void OnCreate ()
		{
			UAirship.TakeOff (this);
			PushManager.Shared ().IntentReceiver = Class.FromType (typeof(PushReceiver));
			RichPushManager.SetJavascriptInterface (Class.FromType (typeof(RichPushMessageJavaScript)), "urbanairship");

			// If running on Jelly Bean or higher, then use the inbox style notification builder
			if ((int) Build.VERSION.SdkInt >= (int) BuildVersionCodes.JellyBean) {
				PushManager.Shared ().NotificationBuilder = new RichNotificationBuilder ();
			}
		}
	}
}