/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.UrbanAirship.RichPush;
using Java.Text;
using Android.Graphics;
using Android.Appwidget;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * The widget provider for the rich push inbox
 */
	[BroadcastReceiver]
	[IntentFilter (new string [] {"android.appwidget.action.APPWIDGET_UPDATE"})]
	[MetaData ("android.appwidget.provider", Resource = "@xml/widgetinfo")]
	public class RichPushWidgetProvider : AppWidgetProvider {
		public const string OPEN_MESSAGE_ACTION = "com.xamarin.samples.urbanairship.richpush.widget.OPEN_MESSAGE";
		public const string REFRESH_ACTION = "com.xamarin.samples.urbanairship.richpush.widget.REFRESH";

		private static HandlerThread workerThread;
		private static Handler workerQueue;

		public RichPushWidgetProvider() {
			// Start the worker thread
			workerThread = new HandlerThread("RichPushSampleInbox-Provider");
			workerThread.Start();
			workerQueue = new Handler (workerThread.Looper);
		}

		override
			public void OnReceive(Context context, Intent intent) {
			String action = intent.Action;

			if (action == REFRESH_ACTION) {
				ScheduleUpdate(context);
			}

			base.OnReceive(context, intent);
		}

		override
			public void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds) {
			// Update each of the widgets with the remote adapter
			foreach (int id in appWidgetIds) {
				RemoteViews layout = null;

				// API 16 and above supports reconfigurable layouts
				if ((int) Build.VERSION.SdkInt >= 16) {
					Bundle options = appWidgetManager.GetAppWidgetOptions(id);
					layout = RemoteViewsFactory.CreateLayout(context, id, options);
				} else {
					layout = RemoteViewsFactory.CreateLayout(context, id);
				}

				appWidgetManager.UpdateAppWidget(id, layout);
			}
			base.OnUpdate(context, appWidgetManager, appWidgetIds);
		}

		override
			public void OnAppWidgetOptionsChanged(Context context,
			                                      AppWidgetManager appWidgetManager,
			                                      int appWidgetId,
			                                      Bundle newOptions) {

			RemoteViews layout = RemoteViewsFactory.CreateLayout(context, appWidgetId, newOptions);
			appWidgetManager.UpdateAppWidget(appWidgetId, layout);
		}

		/**
     * Adds a runnable to update the widgets in the worker queue
     * @param context used for creating layouts
     */
		private void ScheduleUpdate(Context context) {
			workerQueue.RemoveMessages(0);
			workerQueue.Post(() => {
					AppWidgetManager mgr = AppWidgetManager.GetInstance(context);
					ComponentName cn = new ComponentName(context, Java.Lang.Class.FromType (typeof (RichPushWidgetProvider)));
					OnUpdate(context, mgr, mgr.GetAppWidgetIds(cn));

					if ((int) Build.VERSION.SdkInt >= 11) {
						mgr.NotifyAppWidgetViewDataChanged(mgr.GetAppWidgetIds(cn), Resource.Id.message_list);
					}
			});
		}
	}
}
