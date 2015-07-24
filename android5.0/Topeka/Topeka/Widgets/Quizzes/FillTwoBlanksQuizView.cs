using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class FillTwoBlanksQuizView : TextInputQuizView<FillTwoBlanksQuiz>
	{
		const string KeyAnswerOne = "ANSWER_ONE";
		const string KeyAnswerTwo = "ANSWER_TWO";
		static readonly LinearLayout.LayoutParams ChildLayoutParams = new LinearLayout.LayoutParams((int)ViewGroup.LayoutParams.MatchParent, 0, 1);
		EditText answerOne;
		EditText answerTwo;

		public FillTwoBlanksQuizView(Context context, Category category, FillTwoBlanksQuiz quiz) : base(context, category, quiz) {}


		protected override View CreateQuizContentView ()
		{
			var layout = new LinearLayout(Context);
			layout.Orientation = Orientation.Vertical;
			answerOne = CreateEditText();
			answerOne.ImeOptions = ImeAction.Next;
			answerTwo = CreateEditText();
			AddEditText(layout, answerOne);
			AddEditText(layout, answerTwo);
			return layout;
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutString(KeyAnswerOne, answerOne.Text);
				bundle.PutString(KeyAnswerTwo, answerTwo.Text);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				answerOne.Text = value.GetString(KeyAnswerOne);
				answerTwo.Text = value.GetString(KeyAnswerTwo);
			}
		}

		void AddEditText(LinearLayout layout, EditText editText) {
			layout.AddView(editText, ChildLayoutParams);
		}

		protected override bool IsAnswerCorrect {
			get {
				var partOne = GetAnswerFrom(answerOne);
				var partTwo = GetAnswerFrom(answerTwo);
				return Quiz.IsAnswerCorrect(new []{partOne, partTwo});
			}
		}

		string GetAnswerFrom(EditText view) {
			return view.Text;
		}
	}
}

