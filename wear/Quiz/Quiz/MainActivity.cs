using System;
using System.IO;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Net;
using Android.Util;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Common.Data;
using Android.Gms.Wearable;
using Org.Json;
using Java.Lang;
using Java.Interop;

using ConfigChanges = Android.Content.PM.ConfigChanges;

namespace Quiz
{
	[Activity (Label = "Quiz", MainLauncher = true, 
		Icon = "@drawable/ic_launcher", 
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	public class MainActivity : Activity,IDataApiDataListener,IMessageApiMessageListener,
		GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		public const string TAG = "ExampleQuizApp";
		const string QUIZ_JSON_FILE = "Quiz.json";

		//various ui components
		private EditText question_edit_text;
		private EditText choice_a_edit_text;
		private EditText choice_b_edit_text;
		private EditText choice_c_edit_text;
		private EditText choice_d_edit_text;
		private RadioGroup choices_radio_group;
		private TextView quiz_status;
		private LinearLayout quiz_buttons;
		private LinearLayout questions_container;
		private Button read_quiz_from_file_button;
		private Button reset_quiz_button;
		private Button new_quiz_button;

		public GoogleApiClient google_api_client;
		private Java.Util.PriorityQueue future_questions;
		private int question_index;
		public bool has_question_been_asked = false;

		//data to display in end report
		private int num_correct = 0;
		private int num_incorrect = 0;
		private int num_skipped = 0;

		private Dictionary<int,int> radio_id_to_index;

		public List<IDataEvent> events;

		public Android.Net.Uri dataItemUri;

		public MainActivity ()
		{
			var temp = new Dictionary<int,int> ();
			temp [Resource.Id.choice_a_radio] = 0;
			temp [Resource.Id.choice_b_radio] = 1;
			temp [Resource.Id.choice_c_radio] = 2;
			temp [Resource.Id.choice_d_radio] = 3;
			radio_id_to_index = temp;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);

			google_api_client = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();

			future_questions = new Java.Util.PriorityQueue (10);

			question_edit_text = FindViewById <EditText> (Resource.Id.question_text);
			choice_a_edit_text = FindViewById <EditText> (Resource.Id.choice_a_text);
			choice_b_edit_text = FindViewById <EditText> (Resource.Id.choice_b_text);
			choice_c_edit_text = FindViewById <EditText> (Resource.Id.choice_c_text);
			choice_d_edit_text = FindViewById <EditText> (Resource.Id.choice_d_text);
			choices_radio_group = FindViewById <RadioGroup> (Resource.Id.choices_radio_group);
			quiz_status = FindViewById <TextView> (Resource.Id.quiz_status);
			quiz_buttons = FindViewById<LinearLayout> (Resource.Id.quiz_buttons);
			questions_container = FindViewById<LinearLayout> (Resource.Id.questions_container);
			read_quiz_from_file_button = FindViewById<Button> (Resource.Id.read_quiz_from_file_button);
			reset_quiz_button = FindViewById<Button> (Resource.Id.reset_quiz_button);
			new_quiz_button = FindViewById<Button> (Resource.Id.new_quiz_button);

			read_quiz_from_file_button.Click += delegate {
				ReadQuizFromFile (read_quiz_from_file_button);
			};

			reset_quiz_button.Click += delegate {
				ResetQuiz ();
			};
			new_quiz_button.Click += delegate {
				NewQuiz ();
			};

		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (!google_api_client.IsConnected)
				google_api_client.Connect ();
		}

		protected override void OnStop ()
		{
			WearableClass.DataApi.RemoveListener (google_api_client, this);
			WearableClass.MessageApi.RemoveListener (google_api_client, this);

			var dataMap = new DataMap ();
			dataMap.PutInt (Constants.NUM_CORRECT, num_correct);
			dataMap.PutInt (Constants.NUM_INCORRECT, num_incorrect);
			if (has_question_been_asked)
				num_skipped++;
			num_skipped += future_questions.Size ();
			dataMap.PutInt (Constants.NUM_SKIPPED, num_skipped);
			if (num_correct + num_incorrect + num_skipped > 0)
				SendMessageToWearable (Constants.QUIZ_EXITED_PATH, dataMap.ToByteArray ());
			ClearQuizStatus ();
			base.OnStop ();
		}

		public void OnConnected (Bundle connectionHint)
		{
			WearableClass.DataApi.AddListener (google_api_client, this);
			WearableClass.MessageApi.AddListener (google_api_client, this);
		}

		public void OnConnectionSuspended (int cause)
		{
			//ignore
		}

		public void OnConnectionFailed (ConnectionResult result)
		{
			Log.Error (TAG, "Failed to connect to Google Play Services");
		}


		public void OnMessageReceived (IMessageEvent messageEvent)
		{
			if (messageEvent.Path.Equals (Constants.RESET_QUIZ_PATH)) {
				RunOnUiThread (new Runnable (new Action (delegate() {
					ResetQuiz ();
				})));
			}
		}

		public class Question : Java.Lang.Object, Java.Lang.IComparable
		{
			public string question;
			private int questionIndex;
			private string[] answers;
			private int correctAnswerIndex;

			public Question (string question, int questionIndex, string[] answers, int correctAnswerIndex)
			{
				this.question = question;
				this.questionIndex = questionIndex;
				this.answers = answers;
				this.correctAnswerIndex = correctAnswerIndex;
			}

			public static Question FromJSon (JSONObject questionObject, int questionIndex)
			{
				try {
					string question = questionObject.GetString (JsonUtils.JSON_FIELD_QUESTION);
					JSONArray answersJsonArray = questionObject.GetJSONArray (JsonUtils.JSON_FIELD_ANSWERS);
					string[] answers = new string[JsonUtils.NUM_ANSWER_CHOICES];
					for (int j = 0; j < answersJsonArray.Length (); j++) {
						answers [j] = answersJsonArray.GetString (j);
					}
					int correctIndex = questionObject.GetInt (JsonUtils.JSON_FIELD_CORRECT_INDEX);
					return new Question (question, questionIndex, answers, correctIndex);
				} catch (JSONException) {
					return null;
				}
			}

			public int CompareTo (Java.Lang.Object obj)
			{
				if (obj is Question)
					return this.questionIndex - (obj as Question).questionIndex;
				return this.ToString ().CompareTo (obj.ToString ());
			}

			public int CompareTo (Question that)
			{
				return this.questionIndex - that.questionIndex;
			}

			public PutDataRequest ToPutDataRequest ()
			{
				PutDataMapRequest request = PutDataMapRequest.Create ("/question/" + questionIndex);
				DataMap dataMap = request.DataMap;
				dataMap.PutString (Constants.QUESTION, question);
				dataMap.PutInt (Constants.QUESTION_INDEX, questionIndex);
				dataMap.PutStringArray (Constants.ANSWERS, answers);
				dataMap.PutInt (Constants.CORRECT_ANSWER_INDEX, correctAnswerIndex);
				return request.AsPutDataRequest ();
			}
		}

		public void ReadQuizFromFile (View view)
		{
			ClearQuizStatus ();
			JSONObject jsonObject = JsonUtils.LoadJsonFile (this, QUIZ_JSON_FILE);
			JSONArray jsonArray = jsonObject.GetJSONArray (JsonUtils.JSON_FIELD_QUESTIONS);
			for (int i = 0; i < jsonArray.Length (); i++) {
				JSONObject questionObject = jsonArray.GetJSONObject (i);
				Question question = Question.FromJSon (questionObject, question_index++);
				AddQuestionDataItem (question);
				SetNewQuestionStatus (question.question);
			}
		}

		[Export ("AddQuestion")]
		public void AddQuestion (View view)
		{
			string question = question_edit_text.Text;
			string[] answers = new string[4];
			answers [0] = choice_a_edit_text.Text;
			answers [1] = choice_b_edit_text.Text;
			answers [2] = choice_c_edit_text.Text;
			answers [3] = choice_d_edit_text.Text;

			int correctAnswerIndex = radio_id_to_index [choices_radio_group.CheckedRadioButtonId];

			AddQuestionDataItem (new Question (question, question_index++, answers, correctAnswerIndex));
			SetNewQuestionStatus (question);

			question_edit_text.SetText ("", TextView.BufferType.Normal);
			choice_a_edit_text.SetText ("", TextView.BufferType.Normal);
			choice_b_edit_text.SetText ("", TextView.BufferType.Normal);
			choice_c_edit_text.SetText ("", TextView.BufferType.Normal);
			choice_d_edit_text.SetText ("", TextView.BufferType.Normal);
		}

		public void AddQuestionDataItem (Question question)
		{
			if (!has_question_been_asked) {
				WearableClass.DataApi.PutDataItem (google_api_client, question.ToPutDataRequest ());
				SetHasQuestionBeenAsked (true);
			} else {
				future_questions.Add (question);
			}
		}

		public void SetNewQuestionStatus (string question)
		{
			quiz_status.Visibility = ViewStates.Visible;
			quiz_buttons.Visibility = ViewStates.Visible;
			LayoutInflater inflater = LayoutInflater.From (this);
			View questionStatusElem = inflater.Inflate (Resource.Layout.question_status_element, null, false);
			((TextView)questionStatusElem.FindViewById (Resource.Id.question)).SetText (question, TextView.BufferType.Normal);
			((TextView)questionStatusElem.FindViewById (Resource.Id.status)).SetText ("This question has not yet been answered.", TextView.BufferType.Normal);
			questions_container.AddView (questionStatusElem);
		}

		public void OnDataChanged (DataEventBuffer dataEvents)
		{
			var events = FreezableUtils.FreezeIterable (dataEvents);
			dataEvents.Close ();
			RunOnUiThread (() => {
				foreach (var ev in events) {
					var e = ((Java.Lang.Object)ev).JavaCast<IDataEvent> ();
					if (e.Type == DataEvent.TypeChanged) {
						var dataMap = DataMapItem.FromDataItem (e.DataItem).DataMap;
						var questionWasAnswered = dataMap.GetBoolean (Constants.QUESTION_WAS_ANSWERED);
						var questionWasDeleted = dataMap.GetBoolean (Constants.QUESTION_WAS_DELETED);
						if (questionWasAnswered) {
							int questionIndex = dataMap.GetInt (Constants.QUESTION_INDEX);
							bool questionCorrect = dataMap.GetBoolean (Constants.CHOSEN_ANSWER_CORRECT);
							UpdateQuestionStatus (questionIndex, questionCorrect);
							AskNextQuestionIfExists ();
						} else if (questionWasDeleted) {
							int questionIndex = dataMap.GetInt (Constants.QUESTION_INDEX);
							MarkQuestionLeftBlank (questionIndex);
							AskNextQuestionIfExists ();
						}
					}
				}
			});
		}

		public void UpdateQuestionStatus (int questionIndex, bool questionCorrect)
		{
			LinearLayout questionStatusElement = (LinearLayout)
				questions_container.GetChildAt (questionIndex);
			TextView questionText = (TextView)questionStatusElement.FindViewById (Resource.Id.question);
			TextView questionStatus = (TextView)questionStatusElement.FindViewById (Resource.Id.status);
			if (questionCorrect) {
				questionText.SetTextColor (Color.Green);
				questionStatus.SetText (Resource.String.question_correct, TextView.BufferType.Normal);
				num_correct++;
			} else {
				questionText.SetTextColor (Color.Red);
				questionStatus.SetText (Resource.String.question_incorrect, TextView.BufferType.Normal);
				num_incorrect++;
			}
		}

		public void MarkQuestionLeftBlank (int index)
		{
			LinearLayout questionStatusElement = (LinearLayout)questions_container.GetChildAt (index);
			if (questionStatusElement != null) {
				TextView questionText = (TextView)questionStatusElement.FindViewById (Resource.Id.question);
				TextView questionStatus = (TextView)questionStatusElement.FindViewById (Resource.Id.status);
				if (questionStatus.Text.Equals (GetString (Resource.String.question_unanswered))) {
					questionText.SetTextColor (Color.Yellow);
					questionStatus.SetText (Resource.String.question_left_blank, TextView.BufferType.Normal);
					num_skipped++;
				}
			}
		}

		public void AskNextQuestionIfExists ()
		{
			if (future_questions.IsEmpty) {
				var dataMap = new DataMap ();
				dataMap.PutInt (Constants.NUM_CORRECT, num_correct);
				dataMap.PutInt (Constants.NUM_INCORRECT, num_incorrect);
				dataMap.PutInt (Constants.NUM_SKIPPED, num_skipped);
				SendMessageToWearable (Constants.QUIZ_ENDED_PATH, dataMap.ToByteArray ());
				SetHasQuestionBeenAsked (false);
			} else {
				WearableClass.DataApi.PutDataItem (google_api_client, (future_questions.Remove () as Question).ToPutDataRequest ());
				SetHasQuestionBeenAsked (true);
			}
		}

		public void SendMessageToWearable (string path, byte[] data)
		{
			WearableClass.NodeApi.GetConnectedNodes(google_api_client)
				.SetResultCallback (new SendMessageResultCallback (this, path, data));
		}

		private class SendMessageResultCallback : Java.Lang.Object,IResultCallback
		{
			MainActivity act;
			string path;
			byte[] data;

			public SendMessageResultCallback (MainActivity activity, string path, byte[] data)
			{
				act = activity;
				this.path = path;
				this.data = data;
			}

			public void OnResult (Java.Lang.Object obj)
			{
				var nodes = Android.Runtime.Extensions.JavaCast<INodeApiGetConnectedNodesResult> (obj);
				foreach (INode node in nodes.Nodes) {
					WearableClass.MessageApi.SendMessage (act.google_api_client, node.Id, path, data);
				}
				if (path.Equals (Constants.QUIZ_EXITED_PATH) && act.google_api_client.IsConnected)
					act.google_api_client.Disconnect ();
			}
			/*
		public void OnResult(INodeApiGetConnectedNodesResult nodes)
		{
			foreach (INode node in nodes.Nodes) {
				WearableClass.MessageApi.SendMessage (act.google_api_client, node.Id, act.path, act.data);
			}

			if (act.path.Equals (Constants.QUIZ_EXITED_PATH) && act.google_api_client.IsConnected) {
				act.google_api_client.Disconnect ();
			}
		}
		*/
		}

		public void ResetQuiz ()
		{
			for (int i = 0; i < questions_container.ChildCount; i++) {
				var questionStatusElement = (LinearLayout)questions_container.GetChildAt (i);
				var questionText = (TextView)questionStatusElement.FindViewById (Resource.Id.question);
				var questionStatus = (TextView)questionStatusElement.FindViewById (Resource.Id.status);
				questionText.SetTextColor (Color.White);
				questionStatus.SetText (Resource.String.question_unanswered, TextView.BufferType.Normal);
			}
			if (google_api_client.IsConnected) {
				WearableClass.DataApi.GetDataItems (google_api_client)
					.SetResultCallback (new ResetQuizResultCallback (this));
			} else {
				Log.Error (TAG, "Failed to reset data items because client is disconnected from" +
				"Google Play Services");
			}
			SetHasQuestionBeenAsked (false);
			num_correct = 0;
			num_incorrect = 0;
			num_skipped = 0;

		}

		private class ResetQuizResultCallback : Java.Lang.Object,IResultCallback
		{
			MainActivity act;

			public ResetQuizResultCallback (MainActivity activity)
			{
				act = activity;
			}

			public void OnResult (Java.Lang.Object obj)
			{
				if (obj is DataItemBuffer) {
					var result = Android.Runtime.Extensions.JavaCast<DataItemBuffer> (obj);
					if (result.Status.IsSuccess) {
						var dataItemList = FreezableUtils.FreezeIterable (result);
						result.Close ();
						act.ResetDataItems (dataItemList);
					} else {
						if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug))
							Log.Debug (MainActivity.TAG, "Reset quiz: failed to get Data Items to reset");
					}
					result.Close ();
				}
			}

			public void OnResult (DataItemBuffer result)
			{
				if (result.Status.IsSuccess) {
					var dataItemList = FreezableUtils.FreezeIterable (result);
					result.Close ();
					act.ResetDataItems (dataItemList);
				} else {

					if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug))
						Log.Debug (MainActivity.TAG, "Reset quiz: failed to get Data Items to reset");
				}
				result.Close ();
			}
		}

		public void ResetDataItems (System.Collections.IList dataItemList)
		{
			if (google_api_client.IsConnected) {
				foreach (var item in dataItemList) {
					var dataItem = ((Java.Lang.Object)item).JavaCast<IDataItem> ();
					Android.Net.Uri dataItemUri = dataItem.Uri;
					WearableClass.DataApi.GetDataItem (google_api_client, dataItemUri).SetResultCallback (new ResetDataItemCallback (this));
				}
			} else {
				Log.Error (TAG, "Failed to reset data items because client is disconnected from Google Play Services");
			}
		}

		private class ResetDataItemCallback : Java.Lang.Object,IResultCallback
		{
			MainActivity act;

			public ResetDataItemCallback (MainActivity activity)
			{
				act = activity;
			}

			public void OnResult (Java.Lang.Object obj)
			{
				var dataItemResult = Android.Runtime.Extensions.JavaCast<IDataApiDataItemResult> (obj);
				if (dataItemResult.Status.IsSuccess) {
					var request = PutDataMapRequest.CreateFromDataMapItem (
						              DataMapItem.FromDataItem (dataItemResult.DataItem));
					var dataMap = request.DataMap;
					dataMap.PutBoolean (Constants.QUESTION_WAS_ANSWERED, false);
					dataMap.PutBoolean (Constants.QUESTION_WAS_DELETED, false);
					if (!act.has_question_been_asked && dataMap.GetInt (Constants.QUESTION_INDEX) == 0) {
						//ask the first question now.
						WearableClass.DataApi.PutDataItem (act.google_api_client, request.AsPutDataRequest ());
						act.SetHasQuestionBeenAsked (true);
					} else {
						//enqueue future questions
						act.future_questions.Add (new Question (dataMap.GetString (Constants.QUESTION),
							dataMap.GetInt (Constants.QUESTION_INDEX), dataMap.GetStringArray (Constants.ANSWERS),
							dataMap.GetInt (Constants.CORRECT_ANSWER_INDEX)));
					}
				} else {
					Log.Error (TAG, "Failed to reset data item " + dataItemResult.DataItem.Uri);
				}
			}

			public void OnResult (IDataApiDataItemResult dataItemResult)
			{
				if (dataItemResult.Status.IsSuccess) {
					var request = PutDataMapRequest.CreateFromDataMapItem (
						              DataMapItem.FromDataItem (dataItemResult.DataItem));
					var dataMap = request.DataMap;
					dataMap.PutBoolean (Constants.QUESTION_WAS_ANSWERED, false);
					dataMap.PutBoolean (Constants.QUESTION_WAS_DELETED, false);
					if (!act.has_question_been_asked && dataMap.GetInt (Constants.QUESTION_INDEX) == 0) {
						//ask the first question now.
						WearableClass.DataApi.PutDataItem (act.google_api_client, request.AsPutDataRequest ());
						act.SetHasQuestionBeenAsked (true);
					} else {
						//enqueue future questions
						act.future_questions.Add (new Question (dataMap.GetString (Constants.QUESTION),
							dataMap.GetInt (Constants.QUESTION_INDEX), dataMap.GetStringArray (Constants.ANSWERS),
							dataMap.GetInt (Constants.CORRECT_ANSWER_INDEX)));
					}
				} else {
					Log.Error (TAG, "Failed to reset data item " + dataItemResult.DataItem.Uri);
				}
			}


		}

		public void NewQuiz ()
		{
			ClearQuizStatus ();
			if (google_api_client.IsConnected) {
				WearableClass.DataApi.GetDataItems (google_api_client)
					.SetResultCallback (new NewQuizResultCallback (this));
			} else {
				Log.Error (TAG, "Failed to delete data items because client is disconnected from Google Play Services.");
			}
		}

		public class NewQuizResultCallback : Java.Lang.Object, IResultCallback
		{
			MainActivity act;

			public NewQuizResultCallback (MainActivity activity)
			{
				act = activity;
			}

			public void OnResult (Java.Lang.Object obj)
			{
				var result = Android.Runtime.Extensions.JavaCast<DataItemBuffer> (obj);
				if (result.Status.IsSuccess) {
					var dataItemUriList = new List<Android.Net.Uri> ();
					//TODO clean loop up, can't foreach the loop easily...
					var iterator = result.Iterator ();
					while (iterator.HasNext) {
						var current = iterator.Next ().JavaCast<IDataItem> ();
						dataItemUriList.Add (current.Uri);
					}
					result.Close ();
					act.DeleteDataItems (dataItemUriList);
				} else {
					if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
						Log.Debug (MainActivity.TAG, "Clear quiz: failed to get Data Items for deletion");
					}
				}
				result.Close ();
			}

			public void OnResult (DataItemBuffer result)
			{
				if (result.Status.IsSuccess) {
					List<Android.Net.Uri> dataItemUriList = new List<Android.Net.Uri> ();
					var iterator = result.Iterator ();
					while (iterator.HasNext) {
						var dataItem = iterator.Next ().JavaCast<IDataItem> ();
						dataItemUriList.Add (dataItem.Uri);
					}
					result.Close ();
					act.DeleteDataItems (dataItemUriList);
				} else {
					if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
						Log.Debug (MainActivity.TAG, "Clear quiz: failed to get Data Items for deletion");
					}
				}
				result.Close ();
			}
		
		}

		public void ClearQuizStatus ()
		{
			questions_container.RemoveAllViews ();
			quiz_status.Visibility = ViewStates.Invisible;
			quiz_buttons.Visibility = ViewStates.Invisible;
			SetHasQuestionBeenAsked (false);
			future_questions.Clear ();
			question_index = 0;
			num_correct = 0;
			num_incorrect = 0;
			num_skipped = 0;
		}

		public void DeleteDataItems (List<Android.Net.Uri> dataItemUriList)
		{
			if (google_api_client.IsConnected) {
				foreach (Android.Net.Uri dataItemUri in dataItemUriList) {
					this.dataItemUri = dataItemUri;
					WearableClass.DataApi.DeleteDataItems (google_api_client, dataItemUri)
						.SetResultCallback (new DeleteResultCallback (this));
				}
			} else {
				Log.Error (TAG, "Failed to delete data items because client is disconnected from" +
				"Google Play Services");
			}
		}

		private class DeleteResultCallback : Java.Lang.Object,IResultCallback
		{
			MainActivity act;

			public DeleteResultCallback (MainActivity activity)
			{
				act = activity;
			}

			public void OnResult (Java.Lang.Object obj)
			{
				if (obj is IDataApiDeleteDataItemsResult) {
					if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
						if (Android.Runtime.Extensions.JavaCast<IDataApiDeleteDataItemsResult> (obj).Status.IsSuccess) {
							Log.Debug (MainActivity.TAG, "Successfully deleted data item " + act.dataItemUri);
						} else {
							Log.Debug (MainActivity.TAG, "Failed to delete data item " + act.dataItemUri);
						}
					}
				}
			}

			public void OnResult (IDataApiDeleteDataItemsResult result)
			{
				if (Log.IsLoggable (MainActivity.TAG, LogPriority.Debug)) {
					if (result.Status.IsSuccess) {
						Log.Debug (MainActivity.TAG, "Successfully deleted data item " + act.dataItemUri);
					} else {
						Log.Debug (MainActivity.TAG, "Failed to delete data item " + act.dataItemUri);
					}
				}
			}
		}

		public void SetHasQuestionBeenAsked (bool value)
		{
			has_question_been_asked = value;
			//only let user click on Reset or Read from file if they have answered all the questions
			read_quiz_from_file_button.Enabled = (!has_question_been_asked);
			reset_quiz_button.Enabled = !has_question_been_asked;
		}


	}
	/*
	public class MyRunnable2 : Java.Lang.Object,IRunnable
	{
		MainActivity act;
		public MyRunnable2(MainActivity activity)
		{
			act = activity;
		}
		public void Run()
		{
			foreach (IDataEvent e in act.events) {
				if (e.GetType ().Equals(DataEvent.TypeChanged)) {
					DataMap dataMap = DataMapItem.FromDataItem (e.DataItem).DataMap;
					bool questionWasAnswered = dataMap.GetBoolean (Constants.QUESTION_WAS_ANSWERED);
					bool questionWasDeleted = dataMap.GetBoolean (Constants.QUESTION_WAS_DELETED);
					if (questionWasAnswered) {
						int questionIndex = dataMap.GetInt (Constants.QUESTION_INDEX);
						bool questionCorrect = dataMap.GetBoolean (Constants.CHOSEN_ANSWER_CORRECT);
						act.UpdateQuestionStatus (questionIndex, questionCorrect);
						act.AskNextQuestionIfExists ();

					} else if (questionWasDeleted) {
						int questionIndex = dataMap.GetInt (Constants.QUESTION_INDEX);
						act.MarkQuestionLeftBlank (questionIndex);
						act.AskNextQuestionIfExists ();

					}
				}
			}
		}
	}
	*/










}


