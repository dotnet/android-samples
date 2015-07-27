using System;
using Topeka.Models.Quizzes;
using Android.Views;
using Android.Content;
using Topeka.Helpers;
using Android.OS;

namespace Topeka.Widgets.Quizzes
{
	public class TrueFalseQuizView : AbsQuizView<TrueFalseQuiz>
	{
		const string KeySelection = "SELECTION";
		static readonly LayoutParams LayoutParams = new LayoutParams (0, ViewGroup.LayoutParams.WrapContent, (GravityFlags)1);

		bool answer;
		View answerTrue;
		View answerFalse;

		protected override bool IsAnswerCorrect =>
			Quiz.IsAnswerCorrect (answer);

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				bundle.PutBoolean (KeySelection, answer);
				return bundle;
			}
			set {
				if (value == null)
					return;

				var tmpAnswer = value.GetBoolean (KeySelection);
				PerformSelection (tmpAnswer ? answerTrue : answerFalse);
			}
		}

		static TrueFalseQuizView ()
		{
			LayoutParams.Gravity = GravityFlags.Center;
		}

		public TrueFalseQuizView (Context context, Category category, TrueFalseQuiz quiz) : base (context, category, quiz)
		{
		}

		protected override View CreateQuizContentView ()
		{
			var container = (ViewGroup)LayoutInflater.Inflate (Resource.Layout.quiz_radio_group_true_false, this, false);
			EventHandler handler = (sender, e) => {
				switch (((View)sender).Id) {
				case Resource.Id.answerTrue:
					answer = true;
					break;
				case Resource.Id.answerFalse:
					answer = false;
					break;
				}
				AllowAnswer ();
			};
			answerTrue = container.FindViewById (Resource.Id.answerTrue);
			answerTrue.Click += handler;
			answerFalse = container.FindViewById (Resource.Id.answerFalse);
			answerFalse.Click += handler;
			return container;
		}

		void PerformSelection (View selection)
		{
			selection.PerformClick ();
			selection.Selected = true;
		}
	}
}

