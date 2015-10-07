using System;
using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Android.Gms.Common.Data;
using Java.Util.Concurrent;
using Android.Gms.Common;
using System.Collections;
using Android.Util;
using Java.Interop;
using System.Text;
using Android.Content;
using Android.App;

namespace Wearable
{
	/// <summary>
	/// Listens to DataItems and Messages from the local node
	/// </summary>
	[Service(), IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" }) ]
	public class DataLayerListenerService : WearableListenerService
	{
		const string Tag = "DataLayerListenerServic";

		public const string StartActivityPath = "/start-activity";
		public const string DataItemReceivedPath = "/data-item-received";
		public const string CountPath = "/count";
		public const string ImagePath = "/image";
		public const string ImageKey = "photo";
		const string ContKey = "count";
		const int MaxLogTagLength = 23;
		GoogleApiClient googleApiClient;

		public override void OnCreate ()
		{
			base.OnCreate ();
			googleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.Build ();
			googleApiClient.Connect ();
		}

		public override async void OnDataChanged (DataEventBuffer dataEvents)
		{
			LOGD (Tag, "OnDataChanged: " + dataEvents);
			IList events = FreezableUtils.FreezeIterable (dataEvents);
			dataEvents.Close ();
			if (!googleApiClient.IsConnected) {
				ConnectionResult connectionResult = googleApiClient.BlockingConnect (30, TimeUnit.Seconds);
				if (!connectionResult.IsSuccess) {
					Log.Error (Tag, "DataLayerListenerService failed to connect to GoogleApiClient");
					return;
				}
			}

			// Loop through the events and send a message back to the node that created the data item
			foreach (var ev in events) {
				var e = ((Java.Lang.Object)ev).JavaCast<IDataEvent> ();
				var uri = e.DataItem.Uri;
				if (CountPath.Equals (CountPath)) {
					// Get the node ID of the node that created the date item from the host portion of the Uri
					string nodeId = uri.Host;
					// Set the data of the message to the bytes of the Uri
					byte[] payload = Encoding.UTF8.GetBytes (uri.ToString ());

					// Send the rpc
					await WearableClass.MessageApi.SendMessageAsync (googleApiClient, nodeId, DataItemReceivedPath, payload);
				}
			}
		}

		public override void OnMessageReceived (IMessageEvent messageEvent)
		{
			LOGD (Tag, "OnMessageReceived: " + messageEvent);

			// Check to see if the message is to start an activity
			if (messageEvent.Path.Equals (StartActivityPath)) {
				Intent startIntent = new Intent (this, typeof(MainActivity));
				startIntent.AddFlags (ActivityFlags.NewTask);
				StartActivity (startIntent);
			}
		}

		public override void OnPeerConnected (INode peer)
		{
			LOGD (Tag, "OnPeerConnected: " + peer);
		}

		public override void OnPeerDisconnected (INode peer)
		{
			LOGD (Tag, "OnPeerDisconnected: " + peer);
		}

		public static void LOGD(string tag, string message) 
		{
			if (Log.IsLoggable(tag, LogPriority.Debug)) {
				Log.Debug(tag, message);
			}
		}
	}
}

