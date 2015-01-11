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
 * Factory class to create remote views for the widget layouts
 *
 */
	//@SuppressLint("NewApi")
	class RemoteViewsFactory {


		/**
     * Creates a layout depending on the app widgets options
     * 
     * @param context Application context
     * @param appWidgetId Id of the widget
     * @param options Widgets options
     * @return RemoteViews for the layout
     */
		internal static RemoteViews CreateLayout(Context context, int appWidgetId, Bundle options) {
			bool isLargeLayout =  options.GetInt(AppWidgetManager.OptionAppwidgetMinHeight) >= 100;

			return isLargeLayout ? RemoteViewsFactory.CreateLargeLayout(context, appWidgetId) :
				RemoteViewsFactory.CreateSmallLayout(context, appWidgetId);
		}

		/**
     * Creates a layout depending on the current sdk version
     * 
     * 
     * @param context Application context
     * @param appWidgetId Id of the widget
     * @param options Widgets options
     * @return RemoteViews for the layout
     */
		internal static RemoteViews CreateLayout(Context context, int appWidgetId) {
			// Only in api >= 11 (Honeycomb) can we support the large layout because we depend on
			// the remote view service.
			return ((int) Build.VERSION.SdkInt >= 11) ?  RemoteViewsFactory.CreateLargeLayout(context, appWidgetId) :
				RemoteViewsFactory.CreateSmallLayout(context, appWidgetId);
		}

		/**
     * Creates a large layout for the app widget
     * 
     * This layout is only supported in SDK >= 11 (Honeycomb)
     * 
     * @param context Application context
     * @param appWidgetId id of the widget
     * @return RemoteViews for the large layout
     */
		private static RemoteViews CreateLargeLayout(Context context, int appWidgetId) {
			RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_layout);

			// Specify the service to provide data for the collection widget.  Note that we need to
			// embed the appWidgetId via the data otherwise it will be ignored.
			Intent intent = new Intent(context, typeof (RichPushWidgetService));
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
			remoteViews.SetRemoteAdapter(appWidgetId, Resource.Id.message_list, intent);

			// Set the empty view to be displayed if the collection is empty.  It must be a sibling
			// view of the collection view.
			remoteViews.SetEmptyView(Resource.Id.message_list, Resource.Id.empty_view);

			// Bind a click listener template for the contents of the message list
			remoteViews.SetPendingIntentTemplate(Resource.Id.message_list, CreateMessageTemplateIntent(context, appWidgetId));

			// Add a click pending intent to launch the inbox
			remoteViews.SetOnClickPendingIntent(Resource.Id.widget_header, CreateInboxActivityPendingIntent(context));

			return remoteViews;
		}

		/**
     * Creates a small layout for the app widget
     * 
     * 
     * @param context Application context
     * @param appWidgetId id of the widget
     * @return RemoteViews for the small layout
     */
		private static RemoteViews CreateSmallLayout(Context context, int appWidgetId) {
			RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_layout_small);

			// Update the header for the current unread message count
			int count = RichPushManager.Shared().RichPushUser.Inbox.UnreadCount;
			String inboxName = context.GetString(Resource.String.inbox_name);
			String header = context.GetString(Resource.String.header_format_string, count, inboxName);

			remoteViews.SetTextViewText(Resource.Id.widget_header_text, header);

			// Add a click pending intent to launch the inbox
			remoteViews.SetOnClickPendingIntent(Resource.Id.widget_header, CreateInboxActivityPendingIntent(context));

			return remoteViews;
		}

		/**
     * Creates an pending activity intent to launch the inbox
     * @param context Application context
     * @return Pending inbox activity intent
     */
		private static PendingIntent CreateInboxActivityPendingIntent(Context context) {
			Intent intent = new Intent(context, typeof (InboxActivity));
			intent.SetFlags( ActivityFlags.ClearTop | ActivityFlags.NewTask);
			return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
		}

		/**
     * Creates a pending broadcast intent as a template
     * for each message in the app widget
     * 
     * @param context Application context
     * @param appWidgetId Id of the widget
     * @return Pending broadcast intent
     */
		private static PendingIntent CreateMessageTemplateIntent(Context context, int appWidgetId) {
			Intent intent = new Intent(context, typeof (InboxActivity));
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

			intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
			return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
		}
	}

}
