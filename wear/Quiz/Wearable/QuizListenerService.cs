using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Text;
using Android.Text.Style;
using Android.Util;

using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Common.Data;
using Android.Gms.Wearable;

using Java.Util;
using Java.Util.Concurrent;
using Java.Interop;

namespace Wearable
{
	[Service]
	[IntentFilter (new string[]{ "com.google.android.gms.wearable.BIND_LISTENER" })]
	public class QuizListenerService : WearableListenerService
	{
		const string TAG = "QuizSample";
		const int QUIZ_REPORT_NOTIF_ID = -1;
		//never used by question notifications
		IMap question_num_to_drawable_id;

		public QuizListenerService ()
		{
			var temp = new HashMap (4);
			temp.Put (0, Resource.Drawable.ic_choice_a);
			temp.Put (1, Resource.Drawable.ic_choice_b);
			temp.Put (2, Resource.Drawable.ic_choice_c);
			temp.Put (3, Resource.Drawable.ic_choice_d);
			question_num_to_drawable_id = temp;
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
		}

		public override void OnDataChanged (DataEventBuffer eventBuffer)
		{
			var events = FreezableUtils.FreezeIterable (eventBuffer);
			eventBuffer.Close ();

			var google_api_client = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.Build ();

			var connectionResult = google_api_client.BlockingConnect (Constants.CONNECT_TIMEOUT_MS,
				                       TimeUnit.Milliseconds);

			if (!connectionResult.IsSuccess) {
				Log.Error (TAG, "QuizListenerService failed to connect to GoogleApiClient.");
				return;
			}

			foreach (var ev in events) {
				var e = ((Java.Lang.Object)ev).JavaCast<IDataEvent> ();
				if (e.Type == DataEvent.TypeChanged) {
					var dataItem = e.DataItem;
					var dataMap = DataMapItem.FromDataItem (dataItem).DataMap;
					if (dataMap.GetBoolean (Constants.QUESTION_WAS_ANSWERED)
					    || dataMap.GetBoolean (Constants.QUESTION_WAS_DELETED)) {
						continue;
					}

					string question = dataMap.GetString (Constants.QUESTION);
					int questionIndex = dataMap.GetInt (Constants.QUESTION_INDEX);
					int questionNum = questionIndex + 1;
					string[] answers = dataMap.GetStringArray (Constants.ANSWERS);
					int correctAnswerIndex = dataMap.GetInt (Constants.CORRECT_ANSWER_INDEX);
					Intent deleteOperation = new Intent (this, typeof(DeleteQuestionService));
					deleteOperation.SetData (dataItem.Uri);
					PendingIntent deleteIntent = PendingIntent.GetService (this, 0,
						                             deleteOperation, PendingIntentFlags.UpdateCurrent);
					//first page of notification contains question as Big Text.
					var bigTextStyle = new Notification.BigTextStyle ()
						.SetBigContentTitle (GetString (Resource.String.question, questionNum))
						.BigText (question);
					var builder = new Notification.Builder (this)
						.SetStyle (bigTextStyle)
						.SetSmallIcon (Resource.Drawable.ic_launcher)
						.SetLocalOnly (true)
						.SetDeleteIntent (deleteIntent);

					//add answers as actions
					var wearableOptions = new Notification.WearableExtender ();
					for (int i = 0; i < answers.Length; i++) {
						Notification answerPage = new Notification.Builder (this)
							.SetContentTitle (question)
							.SetContentText (answers [i])
							.Extend (new Notification.WearableExtender ()
								.SetContentAction (i))
							.Build ();

						bool correct = (i == correctAnswerIndex);
						var updateOperation = new Intent (this, typeof(UpdateQuestionService));
						//Give each intent a unique action.
						updateOperation.SetAction ("question_" + questionIndex + "_answer_" + i);
						updateOperation.SetData (dataItem.Uri);
						updateOperation.PutExtra (UpdateQuestionService.EXTRA_QUESTION_INDEX, questionIndex);
						updateOperation.PutExtra (UpdateQuestionService.EXTRA_QUESTION_CORRECT, correct);
						var updateIntent = PendingIntent.GetService (this, 0, updateOperation,
							                   PendingIntentFlags.UpdateCurrent);
						Notification.Action action = new Notification.Action.Builder (
							                             (int)question_num_to_drawable_id.Get (i), (string)null, updateIntent)
							.Build ();
						wearableOptions.AddAction (action).AddPage (answerPage);
					}
					builder.Extend (wearableOptions);
					Notification notification = builder.Build ();
					((NotificationManager)GetSystemService (NotificationService))
						.Notify (questionIndex, notification);
				} else if (e.Type == DataEvent.TypeDeleted) {
					Android.Net.Uri uri = e.DataItem.Uri;
					//URIs are in the form of "/question/0", "/question/1" etc.
					//We use the question index as the notification id.
					int notificationId = Java.Lang.Integer.ParseInt (uri.LastPathSegment);
					((NotificationManager)GetSystemService (NotificationService))
						.Cancel (notificationId);
				}

				((NotificationManager)GetSystemService (NotificationService))
					.Cancel (QUIZ_REPORT_NOTIF_ID);
			}
			google_api_client.Disconnect ();
		}

		public override void OnMessageReceived (IMessageEvent messageEvent)
		{
			string path = messageEvent.Path;
			if (path.Equals (Constants.QUIZ_EXITED_PATH)) {
				((NotificationManager)GetSystemService (NotificationService)).CancelAll ();
			}
			if (path.Equals (Constants.QUIZ_ENDED_PATH) || path.Equals (Constants.QUIZ_EXITED_PATH)) {
				var dataMap = DataMap.FromByteArray (messageEvent.GetData ());
				int numCorrect = dataMap.GetInt (Constants.NUM_CORRECT);
				int numIncorrect = dataMap.GetInt (Constants.NUM_INCORRECT);
				int numSkipped = dataMap.GetInt (Constants.NUM_SKIPPED);

				var builder = new Notification.Builder (this)
					.SetContentTitle (GetString (Resource.String.quiz_report))
					.SetSmallIcon (Resource.Drawable.ic_launcher)
					.SetLocalOnly (true);
				var quizReportText = new SpannableStringBuilder ();
				AppendColored (quizReportText, numCorrect.ToString (), Resource.Color.dark_green);
				quizReportText.Append (" " + GetString (Resource.String.correct) + "\n");
				AppendColored (quizReportText, numIncorrect.ToString (), Resource.Color.dark_red);
				quizReportText.Append (" " + GetString (Resource.String.incorrect) + "\n");
				AppendColored (quizReportText, numSkipped.ToString (), Resource.Color.dark_yellow);
				quizReportText.Append (" " + GetString (Resource.String.skipped) + "\n");

				builder.SetContentText (quizReportText);
				if (!path.Equals (Constants.QUIZ_EXITED_PATH)) {
					builder.AddAction (Resource.Drawable.ic_launcher,
						GetString (Resource.String.reset_quiz), GetResetQuizPendingIntent ());
				}
				((NotificationManager)GetSystemService (NotificationService))
					.Notify (QUIZ_REPORT_NOTIF_ID, builder.Build ());
			}
		}

		private void AppendColored (SpannableStringBuilder builder, string text, int colorResId)
		{
			builder.Append (text);
			builder.SetSpan (new ForegroundColorSpan (Resources.GetColor (colorResId)),
				builder.Length () - text.Length, builder.Length (), 0);
		}

		private PendingIntent GetResetQuizPendingIntent ()
		{
			var intent = new Intent (QuizReportActionService.ACTION_RESET_QUIZ)
				.SetClass (this, typeof(QuizReportActionService));
			return PendingIntent.GetService (this, 0, intent, PendingIntentFlags.UpdateCurrent);

		}
	}
}

