using System;
using Android.Content;
using Org.Json;
using System.IO;
using Java.Lang;

namespace Quiz
{
	public class JsonUtils
	{
		public const string JSON_FIELD_QUESTIONS = "questions";
		public const string JSON_FIELD_QUESTION = "question";
		public const string JSON_FIELD_ANSWERS = "answers";
		public const string JSON_FIELD_CORRECT_INDEX = "correctIndex";
		public const int NUM_ANSWER_CHOICES = 4;

		public JsonUtils ()
		{
		}

		public static JSONObject LoadJsonFile(Context context, string fileName)
		{
			var stream = context.Assets.Open(fileName);
			var sr = new StreamReader(stream);
			var data = sr.ReadToEnd();
			stream.Close ();
			return new JSONObject(data);
		}
	}
}

