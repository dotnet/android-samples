/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using System;
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
	/**
 * Utility class to help refresh the rich push inbox widget
 *
 */
	public class RichPushWidgetUtils {

		/**
     * Sends a request to the rich push message to refresh
     * @param context Application context
     */
		public static void RefreshWidget(Context context) {
			RefreshWidget(context, 0);
		}

		/**
     * Sends a request to the rich push message to refresh with a delay
     * @param context Application context
     * @param delayInMs Delay to wait in milliseconds before sending the request
     */
		public static void RefreshWidget(Context context, long delayInMs) {
			Intent refreshIntent = new Intent(context, typeof (RichPushWidgetProvider));
			refreshIntent.SetAction (RichPushWidgetProvider.REFRESH_ACTION);

			if (delayInMs > 0) {
				PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, refreshIntent, 0);
				AlarmManager am = (AlarmManager) context.GetSystemService(Context.AlarmService);
				am.Set (AlarmType.RtcWakeup, (long) new TimeSpan (DateTime.Now.Ticks).TotalMilliseconds + delayInMs, pendingIntent);
			} else {
				context.SendBroadcast(refreshIntent);
			}
		}
	}
}
