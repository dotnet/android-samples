using System;
using Topeka.Models.Quizzes;
using Android.Widget;
using Topeka.Helpers;
using Android.OS;
using Topeka.Adapters;
using Android.Content;

namespace Topeka.Widgets.Quizzes
{
	public class MultiSelectQuizView : AbsQuizView<MultiSelectQuiz>
	{
		const string KeyAnswer = "ANSWER";

		ListView listView;

		public MultiSelectQuizView(Context context, Category category, MultiSelectQuiz quiz) : base(context, category, quiz) {}

		protected override Android.Views.View CreateQuizContentView ()
		{
			listView = new ListView(Context);
			listView.Adapter = new OptionsQuizAdapter(Quiz.Options, Android.Resource.Layout.SimpleListItemMultipleChoice);
			listView.ChoiceMode = ChoiceMode.Multiple;
			listView.ItemsCanFocus = false;
			listView.ItemClick += (sender, e) => AllowAnswer();
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
				var bundleableAnswer = GetBundleableAnswer();
				bundle.PutBooleanArray(KeyAnswer, bundleableAnswer);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				var answers = value.GetBooleanArray(KeyAnswer);
				if (answers == null) {
					return;
				}
				for (int i = 0; i < answers.Length; i++) {
					listView.SetItemChecked(i, answers[i]);
				}
			}
		}

		bool[] GetBundleableAnswer() {
			var checkedItemPositions = listView.CheckedItemPositions;
			var answerSize = checkedItemPositions.Size();
			if (answerSize == 0) {
				return null;
			}
			var optionsSize = Quiz.Options.Length;
			var bundleableAnswer = new bool[optionsSize];
			int key;
			for (int i = 0; i < answerSize; i++) {
				key = checkedItemPositions.KeyAt(i);
				bundleableAnswer[key] = checkedItemPositions.ValueAt(i);
			}
			return bundleableAnswer;
		}
	}
}

