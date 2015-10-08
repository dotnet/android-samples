using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;

using Java.Util.Concurrent;
using Java.Interop;

namespace Wearable
{
	[Service]
	public class QuizReportActionService : IntentService
	{
		public const string ACTION_RESET_QUIZ = "quiz.RESET_QUIZ";
		const string TAG = "QuizReportActionReciever";

		public QuizReportActionService () : base ()
		{
		}

		protected override void OnHandleIntent (Intent intent)
		{
			if (intent.Action.Equals (ACTION_RESET_QUIZ)) {
				var google_api_client = new GoogleApiClient.Builder (this)
					.AddApi (WearableClass.API)
					.Build ();

				ConnectionResult result = google_api_client.BlockingConnect (Constants.CONNECT_TIMEOUT_MS,
					TimeUnit.Milliseconds);
				if (!result.IsSuccess) {
					Log.Error (TAG, "QuizListenerService failed to connect to GoogleApiClient.");
					return;
				}

				var nodes = WearableClass.NodeApi.GetConnectedNodes (google_api_client).Await ().JavaCast<INodeApiGetConnectedNodesResult>();
				foreach (INode node in nodes.Nodes) {
					WearableClass.MessageApi.SendMessage (google_api_client, node.Id, Constants.RESET_QUIZ_PATH,
						new byte[0]);
				}

			}
		}
	}
}

