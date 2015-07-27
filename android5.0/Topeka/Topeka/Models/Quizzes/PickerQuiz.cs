using Android.OS;

namespace Topeka.Models.Quizzes
{
	public class PickerQuiz : Quiz<int>
	{
		public int Min { get; }

		public int Max { get; }

		public int Step { get; }

		public PickerQuiz (string question, int answer, int min, int max, int step, bool solved) : base (question, answer, solved)
		{
			Min = min;
			Max = max;
			Step = step;
			QuizType = QuizType.Picker;
		}

		public PickerQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.ReadInt ();
			Min = inObj.ReadInt ();
			Max = inObj.ReadInt ();
			Step = inObj.ReadInt ();
			QuizType = QuizType.Picker;
		}

		public override string GetStringAnswer ()
		{
			return Answer.ToString ();
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteInt (Answer);
			dest.WriteInt (Min);
			dest.WriteInt (Max);
			dest.WriteInt (Step);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			var that = obj as PickerQuiz;

			if (that == null ||
				!base.Equals (obj) ||
				Min != that.Min ||
				Max != that.Max)
				return false;
			
			return Step == that.Step;
		}

		public override int GetHashCode ()
		{
			int result = base.GetHashCode ();
			result = 31 * result + Min;
			result = 31 * result + Max;
			result = 31 * result + Step;
			return result;
		}
	}
}