using System.Linq;

using Android.OS;

namespace Topeka.Models.Quizzes
{
	public class OptionsQuiz<T> : Quiz<int[]>
	{
		public T[] Options { get; set; }

		public OptionsQuiz (string question, int[] answer, T[] options, bool solved) : base (question, answer, solved)
		{
			Options = options;
		}

		public OptionsQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.CreateIntArray ();
		}

		public override bool IsAnswerCorrect (int[] answer)
		{
			return Answer.SequenceEqual (answer);
		}

		public override int DescribeContents ()
		{
			return 0;
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteIntArray (Answer);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			var that = obj as OptionsQuiz<T>;

			if (that == null ||
				!Answer.SequenceEqual (that.Answer))
				return false;

			return Options.SequenceEqual (that.Options);
		}

		public override int GetHashCode ()
		{
			int result = base.GetHashCode ();
			result = 31 * result + Options.GetHashCode ();
			return result;
		}

		public override string GetStringAnswer ()
		{
			return Answer.ToString ();
		}
	}
}