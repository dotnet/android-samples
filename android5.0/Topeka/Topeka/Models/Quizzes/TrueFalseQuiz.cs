using Android.OS;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class TrueFalseQuiz : Quiz<bool>
	{
		public TrueFalseQuiz (string question, bool answer, bool solved) : base (question, answer, solved)
		{
			QuizType = Topeka.Models.Quizzes.QuizType.TrueFalse;
		}

		public TrueFalseQuiz (Parcel inObj) : base (inObj)
		{
			Answer = ParcelableHelper.ReadBoolean (inObj);
			QuizType = Topeka.Models.Quizzes.QuizType.TrueFalse;
		}

		public override string GetStringAnswer ()
		{
			return Answer.ToString ();
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			ParcelableHelper.WriteBoolean (dest, Answer);
		}
	}
}

