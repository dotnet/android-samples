using Android.OS;

namespace Topeka.Models.Quizzes
{
	public class FillBlankQuiz : Quiz<string>
	{
		public string Start { get; private set; }

		public string End { get; private set; }

		public FillBlankQuiz (string question, string answer, string start, string end, bool solved) : base (question, answer, solved)
		{
			Start = start;
			End = end;
			QuizType = QuizType.FillBlank;
		}

		public FillBlankQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.ReadString ();
			Start = inObj.ReadString ();
			End = inObj.ReadString ();
			QuizType = QuizType.FillBlank;
		}

		public override string GetStringAnswer ()
		{
			return Answer;
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteString (Answer);
			dest.WriteString (Start);
			dest.WriteString (End);
		}
	}
}
