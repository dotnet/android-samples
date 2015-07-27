using System;

using Android.Content;
using Android.OS;
using Android.Widget;
using Topeka.Adapters;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class MultiSelectQuizView : AbsQuizView<MultiSelectQuiz>
	{
		const string KeyAnswer = "ANSWER";

		ListView listView;

		protected override bool IsAnswerCorrect =>
			AnswerHelper.IsAnswerCorrect (listView.CheckedItemPositions, Quiz.Answer);

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				var bundleableAnswer = GetBundleableAnswer ();
				bundle.PutBooleanArray (KeyAnswer, bundleableAnswer);
				return bundle;
			}
			set {
				if (value == null)
					return;
				
				var answers = value.GetBooleanArray (KeyAnswer);
				if (answers == null)
					return;

				for (int i = 0; i < answers.Length; i++)
					listView.SetItemChecked (i, answers [i]);
			}
		}

		public MultiSelectQuizView (Context context, Category category, MultiSelectQuiz quiz) : base (context, category, quiz)
		{
		}

		protected override Android.Views.View CreateQuizContentView ()
		{
			listView = new ListView (Context) {
				Adapter = new OptionsQuizAdapter (Quiz.Options, Android.Resource.Layout.SimpleListItemMultipleChoice),
				ChoiceMode = ChoiceMode.Multiple,
				ItemsCanFocus = false
			};

			listView.ItemClick += (sender, e) => AllowAnswer ();
			return listView;
		}

		bool[] GetBundleableAnswer ()
		{
			var checkedItemPositions = listView.CheckedItemPositions;
			var answerSize = checkedItemPositions.Size ();

			if (answerSize == 0)
				return null;
			
			var optionsSize = Quiz.Options.Length;
			var bundleableAnswer = new bool[optionsSize];
			int key;

			for (int i = 0; i < answerSize; i++) {
				key = checkedItemPositions.KeyAt (i);
				bundleableAnswer [key] = checkedItemPositions.ValueAt (i);
			}
			return bundleableAnswer;
		}
	}
}

