using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Topeka.Adapters;
using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class ToggleTranslateQuizView : AbsQuizView<ToggleTranslateQuiz>
	{
		const string KeyAnswers = "ANSWERS";

		bool[] answers;
		ListView listView;

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

		public ToggleTranslateQuizView (Context context, Category category, ToggleTranslateQuiz quiz) : base (context, category, quiz)
		{
			answers = new bool[quiz.Options.Length];
		}

		protected override View CreateQuizContentView ()
		{
			listView = new ListView (Context);
			listView.Divider = null;
			listView.SetSelector (Resource.Drawable.selector_button);
			listView.Adapter = new OptionsQuizAdapter (Quiz.ReadableOptions, Resource.Layout.item_answer);
			listView.ChoiceMode = ChoiceMode.Multiple;
			listView.ItemClick += (sender, e) => {
				ToggleAnswerFor (e.Position);
				var button = e.View as CompoundButton;
				if (button != null)
					button.Checked = answers [e.Position];
				
				AllowAnswer ();
			};
			return listView;
		}

		void ToggleAnswerFor (int answerId)
		{
			answers [answerId] = !answers [answerId];
		}
	}
}

