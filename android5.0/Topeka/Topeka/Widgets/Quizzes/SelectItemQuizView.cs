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
	public class SelectItemQuizView : AbsQuizView<SelectItemQuiz>
	{
		const string KeyAnswers = "ANSWERS";

		bool[] answers;
		ListView listView;

		bool[] Answers =>
			answers = answers ?? new bool[Quiz.Options.Length];

		protected override bool IsAnswerCorrect =>
			AnswerHelper.IsAnswerCorrect (listView.CheckedItemPositions, Quiz.Answer);

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				bundle.PutBooleanArray (KeyAnswers, answers);
				return bundle;
			}
			set {
				if (value == null)
					return;

				answers = value.GetBooleanArray (KeyAnswers);
				if (answers == null)
					return;

				var adapter = listView.Adapter;

				for (int i = 0; i < answers.Length; i++)
					listView.PerformItemClick (listView.GetChildAt (i), i, adapter.GetItemId (i));
			}
		}

		public SelectItemQuizView (Context context, Category category, SelectItemQuiz quiz) : base (context, category, quiz)
		{
			answers = Answers;
		}

		protected override View CreateQuizContentView ()
		{
			listView = new ListView (Context) {
				Divider = null,
				Adapter = new OptionsQuizAdapter (Quiz.Options, Resource.Layout.item_answer_start, Context, true),
				ChoiceMode = ChoiceMode.Single
			};
			listView.SetSelector (Resource.Drawable.selector_button);
			listView.ItemClick += (sender, e) => {
				AllowAnswer ();
				ToggleAnswerFor (e.Position);
			};
			return listView;
		}

		void ToggleAnswerFor (int answerId)
		{
			Answers [answerId] = !answers [answerId];
		}
	}
}

