/*
Copyright 2009-2011 Urban Airship Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation
and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE URBAN AIRSHIP INC ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
EVENT SHALL URBAN AIRSHIP INC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

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
using Xamarin.UrbanAirship.Push;
using Xamarin.UrbanAirship;

namespace PushSample
{
	[BroadcastReceiver]

	public class IntentReceiver : BroadcastReceiver {

		const string logTag = "PushSample";

		public static string APID_UPDATED_ACTION_SUFFIX = ".apid.updated";

		public override void OnReceive(Context context, Intent intent) {
			Log.Info(logTag, "Received intent: " + intent);
			String action = intent.Action;

			if (action == PushManager.ActionPushReceived) {

				int id = intent.GetIntExtra(PushManager.ExtraNotificationId, 0);

				Log.Info(logTag, "Received push notification. Alert: "
				      + intent.GetStringExtra(PushManager.ExtraAlert)
				      + " [NotificationID="+id+"]");

				LogPushExtras(intent);

			} else if (action == PushManager.ActionNotificationOpened) {

				Log.Info(logTag, "User clicked notification. Message: " + intent.GetStringExtra(PushManager.ExtraAlert));

				LogPushExtras(intent);

				Intent launch = new Intent (Intent.ActionMain);
				launch.SetClass(UAirship.Shared().ApplicationContext, typeof (MainActivity));
				launch.SetFlags (ActivityFlags.NewTask);

				UAirship.Shared().ApplicationContext.StartActivity(launch);

			} else if (action == PushManager.ActionRegistrationFinished) {
				Log.Info(logTag, "Registration complete. APID:" + intent.GetStringExtra(PushManager.ExtraApid)
				      + ". Valid: " + intent.GetBooleanExtra(PushManager.ExtraRegistrationValid, false));

				// Notify any app-specific listeners
				Intent launch = new Intent(UAirship.PackageName + APID_UPDATED_ACTION_SUFFIX);
				UAirship.Shared().ApplicationContext.SendBroadcast(launch);

			} else if (action == GCMMessageHandler.ActionGcmDeletedMessages) {
				Log.Info(logTag, "The GCM service deleted "+intent.GetStringExtra(GCMMessageHandler.ExtraGcmTotalDeleted)+" messages.");
			}

		}

		/**
     * Log the values sent in the payload's "extra" dictionary.
     * 
     * @param intent A PushManager.ACTION_NOTIFICATION_OPENED or ACTION_PUSH_RECEIVED intent.
     */
		private void LogPushExtras(Intent intent) {
			var keys = intent.Extras.KeySet();
			foreach (String key in keys) {

				//ignore standard C2DM extra keys
				IList<String> ignoredKeys = new String [] {
					"collapse_key",//c2dm collapse key
					"from",//c2dm sender
					PushManager.ExtraNotificationId,//int id of generated notification (ACTION_PUSH_RECEIVED only)
					PushManager.ExtraPushId,//internal UA push id
					PushManager.ExtraAlert};//ignore alert
				if (ignoredKeys.Contains(key)) {
					continue;
				}
				Log.Info(logTag, "Push Notification Extra: ["+key+" : " + intent.GetStringExtra(key) + "]");
			}
		}
	}
}
