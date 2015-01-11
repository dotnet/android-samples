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

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * Sample implementation of the InboxFragment
 *
 */
	public class RichPushSampleInboxFragment : InboxFragment
	{

		private static readonly SimpleDateFormat UA_DATE_FORMATTER = new SimpleDateFormat ("yyyy-MM-dd HH:mm");

		override
			public int RowLayoutId { get {
				return Resource.Layout.inbox_message;
			} }

		override
			public int EmptyListStringId {
			get {
				return Resource.String.no_messages;
			}
		}

		class MyViewBinder : RichPushMessageAdapter.IViewBinder
		{
			public MyViewBinder (Action<View, RichPushMessage> bindView)
			{
				this.bindView = bindView;
			}

			Action<View, RichPushMessage> bindView;

			public void BindView (View view, RichPushMessage message)
			{
				bindView (view, message);
			}
		}

		override
			protected RichPushMessageAdapter.IViewBinder CreateMessageBinder ()
		{
			return new MyViewBinder ((view, message) => {
				View unreadIndicator = view.FindViewById (Resource.Id.unread_indicator);
				TextView title = (TextView)view.FindViewById (Resource.Id.title);
				TextView timeStamp = (TextView)view.FindViewById (Resource.Id.date_sent);
				CheckBox checkBox = (CheckBox)view.FindViewById (Resource.Id.message_checkbox);

				if (message.IsRead) {
					unreadIndicator.SetBackgroundColor (Color.Black);
					unreadIndicator.ContentDescription = "Message is read";
				} else {
					unreadIndicator.SetBackgroundColor (Color.Yellow);
					unreadIndicator.ContentDescription = "Message is unread";
				}

				title.Text = (message.Title);
				timeStamp.Text = UA_DATE_FORMATTER.Format (message.SentDate);

				checkBox.Checked = (IsMessageSelected (message.MessageId));

				checkBox.Click += delegate {
					OnMessageSelected (message.MessageId, checkBox.Checked);
				};
				view.Focusable = (false);
				view.FocusableInTouchMode = (false);
			});
		}
	}
}