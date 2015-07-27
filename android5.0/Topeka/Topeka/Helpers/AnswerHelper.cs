using Android.Util;

namespace Topeka.Helpers
{
	public static class AnswerHelper
	{
		const string Tag = "AnswerHelper";

		public static string GetAnswer (string[] answers)
		{
			return string.Join ("\n", answers);
		}

		public static string GetAnswer (int[] answers, string[] options)
		{
			var readableAnswers = new string[answers.Length];
			for (int i = 0; i < answers.Length; i++)
				readableAnswers [i] = options [answers [i]];
			return GetAnswer (readableAnswers);
		}

		public static bool IsAnswerCorrect (SparseBooleanArray checkedItems, int[] answerIds)
		{
			if (checkedItems == null || answerIds == null) {
				Log.Info (Tag, "isAnswerCorrect got a null parameter input.");
				return false;
			}

			foreach (int answer in answerIds)
				if (0 > checkedItems.IndexOfKey (answer))
					return false;

			return true;
		}
	}
}

