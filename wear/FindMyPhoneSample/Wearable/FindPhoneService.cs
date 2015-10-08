using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Runtime;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;

using Java.Util.Concurrent;

namespace FindMyPhoneSample
{
	[Service]
	public class FindPhoneService : IntentService,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		const string TAG = "ExampleFindPhoneApp";
		const string FIELD_ALARM_ON = "alarm_on";
		const string PATH_SOUND_ALARM = "/sound_alarm";

		public const string ACTION_TOGGLE_ALARM = "action_toggle_alarm";
		public const string ACTION_CANCEL_ALARM = "action_alarm_off";

		const int CONNECTION_TIME_OUT_MS = 100;
		private GoogleApiClient google_api_client;
	

		public override void OnCreate()
		{
			base.OnCreate ();
			google_api_client = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnHandleIntent(Intent intent)
		{
			google_api_client.BlockingConnect (CONNECTION_TIME_OUT_MS, TimeUnit.Milliseconds);

			if (Log.IsLoggable (TAG, LogPriority.Verbose)) 
				Log.Verbose (TAG, "FindPhoneService.OnHandleEvent");

			if (google_api_client.IsConnected) {
				bool alarmOn = false;
				if (intent.Action == ACTION_TOGGLE_ALARM) {
					var result = WearableClass.DataApi.GetDataItems (google_api_client).Await ().JavaCast<DataItemBuffer>();
					if (result.Status.IsSuccess) {
						if (result.Count == 1) {
							alarmOn = DataMap.FromByteArray ((result.Get(0).JavaCast<IDataItem>()).GetData ()).GetBoolean (FIELD_ALARM_ON, false);
						} else {
							Log.Error (TAG, "Unexpected number of DataItems found.\n" +
								"\tExpected: 1\n" +
								"\tActual: " + result.Count);
						}
					} else if (Log.IsLoggable (TAG, LogPriority.Debug)) {
						Log.Debug (TAG, "OnHandleIntent: failed to get current alarm state");
					}

					result.Close();
					alarmOn = !alarmOn;
					string notificationText = alarmOn ? GetString(Resource.String.turn_alarm_on) : GetString(Resource.String.turn_alarm_off);
					MainActivity.UpdateNotification (this, notificationText);
				}

				var putDataMapRequest = PutDataMapRequest.Create (PATH_SOUND_ALARM);
				putDataMapRequest.DataMap.PutBoolean (FIELD_ALARM_ON, alarmOn);
				WearableClass.DataApi.PutDataItem (google_api_client, putDataMapRequest.AsPutDataRequest ()).Await ();
			} else {
				Log.Error (TAG, "Failed to toggle alarm on phone - Client disconnected from Google Play Services");
			}
			google_api_client.Disconnect ();
		}

		public void OnConnected(Bundle connectionHint)
		{
		}
		public void OnConnectionSuspended(int cause)
		{
		}
		public void OnConnectionFailed(ConnectionResult result) 
		{
		}
			
	}
}

