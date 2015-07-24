using System;
using Topeka.Models.Quizzes;
using Android.Widget;
using Android.Views;
using Topeka.Helpers;
using Android.OS;
using Topeka.Adapters;
using Android.Content;

namespace Topeka.Widgets.Quizzes
{
	public class ToggleTranslateQuizView : AbsQuizView<ToggleTranslateQuiz>
	{
		const string KeyAnswers = "ANSWERS";

		bool[] answers;
		ListView listView;

		public ToggleTranslateQuizView(Context context, Category category, ToggleTranslateQuiz quiz) : base(context, category, quiz)
		{
			answers = new bool[quiz.Options.Length];
		}

        protected override View CreateQuizContentView()
        {
            listView = new ListView(Context);
            listView.Divider = null;
            listView.SetSelector(Resource.Drawable.selector_button);
            listView.Adapter = new OptionsQuizAdapter(Quiz.ReadableOptions, Resource.Layout.item_answer);
            listView.ChoiceMode = ChoiceMode.Multiple;
            listView.ItemClick += (sender, e) =>
            {
                ToggleAnswerFor(e.Position);
                var button = e.View as CompoundButton;
                if (button != null)
                {
                    button.Checked = answers[e.Position];
                }
                AllowAnswer();
            };
            return listView;
        }

		protected override bool IsAnswerCorrect {
			get {
				var checkedItemPositions = listView.CheckedItemPositions;
				var answer = Quiz.Answer;
				return AnswerHelper.IsAnswerCorrect(checkedItemPositions, answer);
			}
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutBooleanArray(KeyAnswers, answers);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				answers = value.GetBooleanArray(KeyAnswers);
				if (answers == null) {
					return;
				}
				var adapter = listView.Adapter;
				for (int i = 0; i < answers.Length; i++) {
					listView.PerformItemClick(listView.GetChildAt(i), i, adapter.GetItemId(i));
				}
			}
		}

		void ToggleAnswerFor(int answerId) {
			answers[answerId] = !answers[answerId];
		}
	}
}

