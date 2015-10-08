using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Util;

using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;

using Java.Util.Concurrent;
using Java.Interop;

namespace Wearable
{
	[Service]
	public class DeleteQuestionService : IntentService, GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener
	{
		const string TAG = "DeleteQuestionReciever";

		private GoogleApiClient google_api_client;

		public override void OnCreate ()
		{
			base.OnCreate ();
			google_api_client = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnHandleIntent (Intent intent)
		{
			google_api_client.BlockingConnect (Constants.CONNECT_TIMEOUT_MS, TimeUnit.Milliseconds);
			Android.Net.Uri dataItemUri = intent.Data;
			if (!google_api_client.IsConnected) {
				Log.Error (TAG, "Failed to update data item " + dataItemUri 
					+ " because client is disconnected from Google Play Services");
				return;
			}
			var dataItemResult = WearableClass.DataApi.GetDataItem (
				google_api_client, dataItemUri).Await ().JavaCast<IDataApiDataItemResult>();
			var putDataMapRequest = PutDataMapRequest
				.CreateFromDataMapItem (DataMapItem.FromDataItem (dataItemResult.DataItem));
			var dataMap = putDataMapRequest.DataMap;
			dataMap.PutBoolean (Constants.QUESTION_WAS_DELETED, true);
			var request = putDataMapRequest.AsPutDataRequest ();
			WearableClass.DataApi.PutDataItem (google_api_client, request).Await ();
			google_api_client.Disconnect ();
		}

		public void OnConnected(Bundle bundle)
		{
		}

		public void OnConnectionSuspended(int i)
		{
		}

		public void OnConnectionFailed(ConnectionResult connectionResult)
		{
		}
	}
}
