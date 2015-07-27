using Android.OS;

namespace Topeka.Models.Quizzes
{
	public class AlphaPickerQuiz : Quiz<string>
	{
		public AlphaPickerQuiz (string question, string answer, bool solved) : base (question, answer, solved)
		{
			QuizType = QuizType.AlphaPicker;
		}

		public AlphaPickerQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.ReadString ();
			QuizType = QuizType.AlphaPicker;
		}

		public override string GetStringAnswer ()
		{
			return Answer;
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteString (Answer);
		}
	}
}

