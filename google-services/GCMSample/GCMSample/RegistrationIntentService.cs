using Android.App;
using Android.Preferences;
using Android.Gms.Gcm.Iid;
using Android;
using Android.Gms.Gcm;
using Android.Util;
using Android.Support.V4.Content;
using System;
using Android.Content;


namespace GCMSample
{
	[Service (Exported = false)]
	public class RegistrationIntentService : IntentService
	{
		const string TAG = "RegIntentService";
		static readonly string[] TOPICS = {"global"};

		public RegistrationIntentService() : base (TAG) {
		}

		protected override void OnHandleIntent (Intent intent)
		{
			var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

			try {
				lock (TAG) {
					var instanceID = InstanceID.GetInstance(this);
					var token = instanceID.GetToken(GetString(Resource.String.gcm_defaultSenderId),
						GoogleCloudMessaging.InstanceIdScope, null);
					Log.Info(TAG, "GCM Registration Token: " + token);

					SendRegistrationToServer(token);

					SubscribeTopics(token);

					sharedPreferences.Edit().PutBoolean(QuickstartPreferences.SENT_TOKEN_TO_SERVER, true).Apply();
				}
			} catch (Exception e) {
				Log.Debug(TAG, "Failed to complete token refresh", e);
				// If an exception happens while fetching the new token or updating our registration data
				// on a third-party server, this ensures that we'll attempt the update at a later time.
				sharedPreferences.Edit().PutBoolean(QuickstartPreferences.SENT_TOKEN_TO_SERVER, false).Apply();
			}
			// Notify UI that registration has completed, so the progress indicator can be hidden.
			var registrationComplete = new Intent(QuickstartPreferences.REGISTRATION_COMPLETE);
			LocalBroadcastManager.GetInstance(this).SendBroadcast(registrationComplete);
		}

		void SendRegistrationToServer(string token) {
			// Add custom implementation, as needed.
		}

		void SubscribeTopics(string token) {
			foreach (var topic in TOPICS) {
				var pubSub = GcmPubSub.GetInstance(this);
				pubSub.Subscribe(token, "/topics/" + topic, null);
			}
		}
	}
}

