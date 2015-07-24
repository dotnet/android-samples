using System;
using System.Linq;
using Android.OS;
using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class ToggleTranslateQuiz : OptionsQuiz<string>
	{
		string[] readableOptions;

		public string[] ReadableOptions
        {
            get
            {
                if (readableOptions == null)
                {
                    var options = Options;
                    readableOptions = new string[options.Length / 2];
                    for (int i = 0; i < readableOptions.Length; i++)
                    {
                        var index = i * 2;
                        readableOptions[i] = CreateReadablePair(options[index], options[index + 1]);
                    }
                }
                return readableOptions;
            }
        }

		public ToggleTranslateQuiz (string question, int[] answer, string[] options, bool solved) : base(question, answer, options, solved)
		{
			QuizType = QuizType.ToggleTranslate;
		}

		public ToggleTranslateQuiz(Parcel inObj) : base(inObj)
		{
			Answer = inObj.CreateIntArray ();
			inObj.ReadStringArray (Options);
			QuizType = QuizType.ToggleTranslate;
		}

		public override string GetStringAnswer ()
		{
			return AnswerHelper.GetAnswer (Answer, ReadableOptions);
		}

		string CreateReadablePair(string option1, string option2) {
			return option1 + " <> " + option2;
		}

		public override void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel (dest, flags);
			dest.WriteIntArray(Answer);
            dest.WriteStringArray(Options);
		}

		public override bool Equals (object obj)
		{
			if (this == obj) {
				return true;
			}
            
			var that = obj as ToggleTranslateQuiz;

			if (that == null) {
				return false;
			}


			if (!Answer.SequenceEqual(that.Answer)) {
				return false;
			}

			if (!Options.SequenceEqual(that.Options)) {
				return false;
			}

			return true;
		}

		public override int GetHashCode ()
		{
			int result = base.GetHashCode ();
			result = 31 * result + Options.GetHashCode();
			return result;
		}
	}
}

