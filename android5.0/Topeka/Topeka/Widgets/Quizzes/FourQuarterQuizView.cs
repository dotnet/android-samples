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

		public FourQuarterQuizView(Context context, Category category, FourQuarterQuiz quiz) : base(context, category, quiz) {}

		protected override View CreateQuizContentView ()
		{
			answerView = new GridView(Context);
			answerView.SetSelector(Resource.Drawable.selector_button);
			answerView.NumColumns = 2;
			answerView.Adapter = new OptionsQuizAdapter (Quiz.Options, Resource.Layout.item_answer);
			answerView.ItemClick += (sender, e) => {
				AllowAnswer();
				answered = e.Position;
			};
			return answerView;
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutInt(KeyAnswer, answered);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				answered = value.GetInt(KeyAnswer);
				if (answered != -1) {
					if (IsLaidOut) {
						SetUpUserInput();
					} else {
						EventHandler<LayoutChangeEventArgs> handler = null;
						handler = ((sender, e) => {
							((View)sender).LayoutChange -= handler;
							SetUpUserInput();
						});
						LayoutChange += handler;
					}
				}
			}
		}

		void SetUpUserInput() {
			answerView.PerformItemClick(answerView.GetChildAt(answered), answered,
				answerView.Adapter.GetItemId(answered));
			answerView.GetChildAt(answered).Selected = true;
			answerView.SetSelection(answered);
		}

		protected override bool IsAnswerCorrect {
			get {
				return Quiz.IsAnswerCorrect(new []{answered});
			}
		}
	}
}

