using System;
using Topeka.Models.Quizzes;
using Android.Widget;
using Topeka.Helpers;
using Android.Content;
using Android.OS;
using Topeka.Adapters;

namespace Topeka.Widgets.Quizzes
{
	public class SelectItemQuizView : AbsQuizView<SelectItemQuiz>
	{
		const string KeyAnswers = "ANSWERS";

		bool[] answers;
		ListView listView;

		bool[] Answers {
			get {
				if (null == answers) {
					answers = new bool[Quiz.Options.Length];
				}
				return answers;
			}
		}

		public SelectItemQuizView (Context context, Category category, SelectItemQuiz quiz) : base (context, category, quiz)
		{
			answers = Answers;
		}

		protected override Android.Views.View CreateQuizContentView ()
		{
			listView = new ListView (Context);
			listView.Divider = null;
			listView.SetSelector (Resource.Drawable.selector_button);
			listView.Adapter = new OptionsQuizAdapter (Quiz.Options, Resource.Layout.item_answer_start, Context, true);
			listView.ChoiceMode = ChoiceMode.Single;
			listView.ItemClick += (sender, e) => {
				AllowAnswer ();
				ToggleAnswerFor (e.Position);
			};
			return listView;
		}

		protected override bool IsAnswerCorrect {
			get {
				var checkedItemPositions = listView.CheckedItemPositions;
				var answer = Quiz.Answer;
				return AnswerHelper.IsAnswerCorrect (checkedItemPositions, answer);
			}
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				bundle.PutBooleanArray (KeyAnswers, answers);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				answers = value.GetBooleanArray (KeyAnswers);
				if (answers == null) {
					return;
				}
				var adapter = listView.Adapter;
				for (int i = 0; i < answers.Length; i++) {
					listView.PerformItemClick (listView.GetChildAt (i), i, adapter.GetItemId (i));
				}
			}
		}

		void ToggleAnswerFor (int answerId)
		{
			Answers [answerId] = !answers [answerId];
		}
	}
}

