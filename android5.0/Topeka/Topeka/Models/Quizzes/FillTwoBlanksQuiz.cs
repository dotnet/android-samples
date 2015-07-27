using System;
using System.Linq;

using Android.OS;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class FillTwoBlanksQuiz : Quiz<string[]>
	{
		public FillTwoBlanksQuiz (string question, string[] answer, bool solved) : base (question, answer, solved)
		{
			QuizType = QuizType.FillTwoBlanks;
		}

		public FillTwoBlanksQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.CreateStringArray ();
			QuizType = QuizType.FillTwoBlanks;
		}

		public override string GetStringAnswer ()
		{
			return AnswerHelper.GetAnswer (Answer);
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteStringArray (Answer);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			var quiz = obj as FillTwoBlanksQuiz;

			if (quiz == null ||
				Answer != null ? !Answer.SequenceEqual (quiz.Answer) : quiz.Answer != null)
				return false;

			return Question == quiz.Question;
		}

		public override int GetHashCode ()
		{
			var result = base.GetHashCode ();
			result = 31 * result + Answer.GetHashCode ();
			return result;
		}
	}
}