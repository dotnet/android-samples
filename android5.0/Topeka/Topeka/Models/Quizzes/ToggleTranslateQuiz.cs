using System;
using System.Linq;

using Android.OS;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class ToggleTranslateQuiz : OptionsQuiz<string>
	{
		string[] readableOptions;

		public string[] ReadableOptions {
			get {
				if (readableOptions == null) {
					readableOptions = new string[Options.Length / 2];
					for (int i = 0; i < readableOptions.Length; i++) {
						var index = i * 2;
						readableOptions [i] = CreateReadablePair (Options [index], Options [index + 1]);
					}
				}
				return readableOptions;
			}
		}

		public ToggleTranslateQuiz (string question, int[] answer, string[] options, bool solved) : base (question, answer, options, solved)
		{
			QuizType = QuizType.ToggleTranslate;
		}

		public ToggleTranslateQuiz (Parcel inObj) : base (inObj)
		{
			Answer = inObj.CreateIntArray ();
			inObj.ReadStringArray (Options);
			QuizType = QuizType.ToggleTranslate;
		}

		public override string GetStringAnswer ()
		{
			return AnswerHelper.GetAnswer (Answer, ReadableOptions);
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteIntArray (Answer);
			dest.WriteStringArray (Options);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;
            
			var that = obj as ToggleTranslateQuiz;

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

		string CreateReadablePair (string option1, string option2)
		{
			return string.Format ("{0} <> {1}", option1, option2);
		}
	}
}

