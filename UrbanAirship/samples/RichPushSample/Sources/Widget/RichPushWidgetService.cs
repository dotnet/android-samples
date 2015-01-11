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
 * This is the service that provides the factory to be bound to the collection service.
 */
	[Service (Exported = false, Permission = Android.Manifest.Permission.BindRemoteviews)]
	public class RichPushWidgetService : RemoteViewsService
	{
		override
			public IRemoteViewsFactory OnGetViewFactory (Intent intent)
		{
			return new StackRemoteViewsFactory (this.ApplicationContext, intent);
		}
	}

	/**
 * This is the factory that will provide data to the collection widget.
 */
	class StackRemoteViewsFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
	{
		private Context context;

		public StackRemoteViewsFactory (Context context, Intent intent)
		{
			this.context = context;
		}

		public void OnCreate ()
		{

		}

		public void OnDestroy ()
		{

		}

		public int Count { get {
				return RichPushManager.Shared ().RichPushUser.Inbox.Count;
			} }

		public RemoteViews GetViewAt (int position)
		{

			IList<RichPushMessage> messages = RichPushManager.Shared ().RichPushUser.Inbox.Messages;

			if (position > messages.Count) {
				return null;
			}

			// Get the data for this position from the content provider
			RichPushMessage message = messages [position];

			// Return a proper item
			String formatStr = context.Resources.GetString (Resource.String.item_format_string);
			int itemId = Resource.Layout.widget_item;
			RemoteViews rv = new RemoteViews (context.PackageName, itemId);
			rv.SetTextViewText (Resource.Id.widget_item_text, Java.Lang.String.Format (formatStr, message.Title));

			int iconDrawable = message.IsRead ? Resource.Drawable.mark_read : Resource.Drawable.mark_unread;
			rv.SetImageViewResource (Resource.Id.widget_item_icon, iconDrawable);

			SimpleDateFormat dateFormat = new SimpleDateFormat ("yyyy-MM-dd HH:mm");
			rv.SetTextViewText (Resource.Id.date_sent, dateFormat.Format (message.SentDate));

			// Add the message id to the intent
			Intent fillInIntent = new Intent ();
			Bundle extras = new Bundle ();
			extras.PutString (RichPushApplication.MESSAGE_ID_RECEIVED_KEY, message.MessageId);
			fillInIntent.PutExtras (extras);
			rv.SetOnClickFillInIntent (Resource.Id.widget_item, fillInIntent);

			return rv;
		}

		public RemoteViews LoadingView {
			get {
				// We aren't going to return a default loading view in this sample
				return null;
			}
		}

		public int ViewTypeCount {
			get {
				// Technically, we have two types of views (the dark and light background views)
				return 2;
			}
		}

		public long GetItemId (int position)
		{
			return position;
		}

		public bool HasStableIds {
			get { return false; }
		}

		public void OnDataSetChanged ()
		{

		}
	}
}