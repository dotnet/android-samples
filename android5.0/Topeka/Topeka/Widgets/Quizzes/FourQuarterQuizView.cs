using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Topeka.Adapters;
using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class FourQuarterQuizView : AbsQuizView<FourQuarterQuiz>
	{
		const string KeyAnswer = "ANSWER";

		int answered = -1;
		GridView answerView;

		protected override bool IsAnswerCorrect =>
			Quiz.IsAnswerCorrect (new []{ answered });

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				bundle.PutInt (KeyAnswer, answered);
				return bundle;
			}
			set {
				if (value == null)
					return;

				answered = value.GetInt (KeyAnswer);
				if (answered != -1) {
					if (IsLaidOut) {
						SetUpUserInput ();
					} else {
						EventHandler<LayoutChangeEventArgs> handler = null;
						handler = ((sender, e) => {
							((View)sender).LayoutChange -= handler;
							SetUpUserInput ();
						});
						LayoutChange += handler;
					}
				}
			}
		}

		public FourQuarterQuizView (Context context, Category category, FourQuarterQuiz quiz) : base (context, category, quiz)
		{
		}

		protected override View CreateQuizContentView ()
		{
			answerView = new GridView (Context) {
				NumColumns = 2,
				Adapter = new OptionsQuizAdapter (Quiz.Options, Resource.Layout.item_answer)
			};
			answerView.SetSelector (Resource.Drawable.selector_button);
			answerView.ItemClick += (sender, e) => {
				AllowAnswer ();
				answered = e.Position;
			};

			return answerView;
		}

		void SetUpUserInput ()
		{
			answerView.PerformItemClick (answerView.GetChildAt (answered), answered,answerView.Adapter.GetItemId (answered));
			answerView.GetChildAt (answered).Selected = true;
			answerView.SetSelection (answered);
		}
	}
}

