using System;
using System.Linq;

using Android.OS;

using Java.Interop;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class FourQuarterQuiz : OptionsQuiz<string>
	{
		[ExportField ("CREATOR")]
		public static new Creator<FourQuarterQuiz> InitializeCreator ()
		{
			var creator = new Creator<FourQuarterQuiz> ();
			creator.Created += (sender, e) => e.Result = new FourQuarterQuiz (e.Source);
			return creator;
		}

		public FourQuarterQuiz (string question, int[] answer, string[] options, bool solved) : base (question, answer, options, solved)
		{
			QuizType = QuizType.FourQuarter;
		}

		public FourQuarterQuiz (Parcel inObj) : base (inObj)
		{
			Options = inObj.CreateStringArray ();
			QuizType = QuizType.FourQuarter;
		}

		public override string GetStringAnswer ()
		{
			return AnswerHelper.GetAnswer (Answer, Options);
		}

		public override int DescribeContents ()
		{
			return 0;
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			var options = Options;
			dest.WriteStringArray (options);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			var quiz = obj as FourQuarterQuiz;

			if (quiz == null)
				return false;

			var answer = Answer;
			var question = Question;

			if (answer != null ? !answer.SequenceEqual (quiz.Answer) : quiz.Answer != null ||
				question != quiz.Question)
				return false;

			return Options.SequenceEqual (quiz.Options);
		}

		public override int GetHashCode ()
		{
			int result = base.GetHashCode ();
			result = 31 * result + Options.GetHashCode ();
			result = 31 * result + Answer.GetHashCode ();
			return result;
		}
	}
}