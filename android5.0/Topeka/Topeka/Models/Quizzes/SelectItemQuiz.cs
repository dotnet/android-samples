using Android.OS;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class SelectItemQuiz : OptionsQuiz<string>
	{
		public SelectItemQuiz (string question, int[] answer, string[] options, bool solved) : base (question, answer, options, solved)
		{
			QuizType = QuizType.SingleSelect;
		}

		public SelectItemQuiz (Parcel inObj) : base (inObj)
		{
			Options = inObj.CreateStringArray ();
			QuizType = QuizType.SingleSelect;
		}

		public override string GetStringAnswer ()
		{
			return AnswerHelper.GetAnswer (Answer, Options);
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteStringArray (Options);
		}
	}
}

