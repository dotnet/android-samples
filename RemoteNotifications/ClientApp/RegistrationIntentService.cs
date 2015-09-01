using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;

namespace ClientApp
{
    // This intent service receives the registration token from GCM:
    [Service (Exported = false)]
    class RegistrationIntentService : IntentService
    {
        // Notification topics that I subscribe to:
        static readonly string[] Topics = { "global" };

        // Create the IntentService, name the worker thread for debugging purposes:
        public RegistrationIntentService() : base ("RegistrationIntentService")
        { }

        // OnHandleIntent is invoked on a worker thread:
        protected override void OnHandleIntent (Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");

                // Ensure that the request is atomic:
                lock (this)
                {
                    // Request a registration token:
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken("YOUR_SERVER_ID",
                        GoogleCloudMessaging.InstanceIdScope, null);

                    // Log the registration token that was returned from GCM:
                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);

                    // Send to the app server (if it requires it):
                    SendRegistrationToAppServer(token);

                    // Subscribe to receive notifications:
                    SubscribeToTopics(token, Topics);
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            // Add custom implementation here as needed.
        }

        // Subscribe to topics to receive notifications from the app server:
        void SubscribeToTopics (string token, string[] topics)
        {
            foreach (var topic in topics)
            {
                var pubSub = GcmPubSub.GetInstance(this);
                pubSub.Subscribe(token, "/topics/" + topic, null);
            }
        }
    }
}
