/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	/**
 	* This is an example of implementing an application service that runs locally
 	* in the same process as the application.  The {@link LocalServiceActivities.Controller}
 	* and {@link LocalServiceActivities.Binding} classes show how to interact with the
 	* service.
 	*
 	* <p>Notice the use of the {@link NotificationManager} when interesting things
 	* happen in the service.  This is generally how background services should
 	* interact with the user, rather than doing something more disruptive such as
 	* calling startActivity().
 	*/
	[Service]
	public class LocalService : Service
	{
		private NotificationManager nm;
		private IBinder binder;

		// Unique Identification Number for the Notification.
		// We use it on Notification start, and to cancel it.
		private int NOTIFICATION = Resource.String.local_service_started;

		public LocalService ()
		{
			binder = new LocalBinder (this);
		}

		/**
     	* Class for clients to access.  Because we know this service always
     	* runs in the same process as its clients, we don't need to deal with
     	* IPC.
    	*/
		public class LocalBinder : Binder {
			LocalService self;

			public LocalBinder (LocalService self)
			{
				this.self = self;
			}

			public LocalService Service {
				get {return self;}
			}
		}

		public override void OnCreate ()
		{
			nm = (NotificationManager) GetSystemService (NotificationService);

			// Display a notification about us starting.  We put an icon in the status bar.
			ShowNotification ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Info ("LocalService", "Received start id " + startId + ": " + intent);

			// We want this service to continue running until it is explicitly
			// stopped, so return sticky.
			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			// Cancel the persistent notification.
			nm.Cancel (NOTIFICATION);

			// Tell the user we stopped.
			Toast.MakeText (this, Resource.String.local_service_stopped, ToastLength.Short).Show ();
		}

		public override IBinder OnBind (Intent intent)
		{
			return binder;
		}

		/**
     	* Show a notification while this service is running.
     	*/
		void ShowNotification ()
		{
			// In this sample, we'll use the same text for the ticker and the expanded notification
			var text = GetText (Resource.String.local_service_started);

			// Set the icon, scrolling text and timestamp
			var sinceEpoch = DateTime.UtcNow - new DateTime (1970, 1, 1);
			var msSinceEpoch = (long) sinceEpoch.TotalMilliseconds;
			var notification = new Notification (Resource.Drawable.stat_sample, text, msSinceEpoch);

			// The PendingIntent to launch our activity if the user selects this notification
			PendingIntent contentIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof (LocalServiceActivities.Controller)), 0);

			// Set the info for the views that show in the notification panel.
			notification.SetLatestEventInfo (this, GetText (Resource.String.local_service_label), text, contentIntent);

			// Send the notification.
			nm.Notify (NOTIFICATION, notification);
		}
	}
}
