using System;
using Android;
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
	public class UpdateQuestionService : IntentService, GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener
	{
		public const string EXTRA_QUESTION_CORRECT = "extra_question_correct";
		public const string EXTRA_QUESTION_INDEX = "extra_question_index";

		const int TIME_OUT_MS = 100;
		const string TAG = "UpdateQuestionService";

		private GoogleApiClient google_api_client;
		public UpdateQuestionService () : base()
		{
		}

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
			google_api_client.BlockingConnect (TIME_OUT_MS, TimeUnit.Milliseconds);
			Android.Net.Uri dataItemUri = intent.Data;
			if (!google_api_client.IsConnected) {
				Log.Error (TAG, "Failed to update data item " + dataItemUri +
				" because client is disconnected from Google Play Services");
				return;
			}
			var dataItemResult = WearableClass.DataApi.GetDataItem (
				google_api_client, dataItemUri).Await ().JavaCast<IDataApiDataItemResult> ();

			var putDataMapRequest = PutDataMapRequest.CreateFromDataMapItem (
				DataMapItem.FromDataItem (dataItemResult.DataItem));
			var dataMap = putDataMapRequest.DataMap;

			//update quiz status variables
			int questionIndex = intent.GetIntExtra (EXTRA_QUESTION_INDEX, -1);
			bool chosenAnswerCorrect = intent.GetBooleanExtra (EXTRA_QUESTION_CORRECT, false);
			dataMap.PutInt (Constants.QUESTION_INDEX, questionIndex);
			dataMap.PutBoolean (Constants.CHOSEN_ANSWER_CORRECT, chosenAnswerCorrect);
			dataMap.PutBoolean (Constants.QUESTION_WAS_ANSWERED, true);
			PutDataRequest request = putDataMapRequest.AsPutDataRequest ();
			WearableClass.DataApi.PutDataItem (google_api_client, request).Await ();

			//remove this question notification
			((NotificationManager)GetSystemService (NotificationService)).Cancel (questionIndex);
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

