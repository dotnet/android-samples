using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class FillBlankQuizView : TextInputQuizView<FillBlankQuiz>
	{
		const string KeyAnswer = "ANSWER";

		EditText answerView;

		protected override bool IsAnswerCorrect =>
			Quiz.IsAnswerCorrect(answerView.Text);

		public FillBlankQuizView(Context context, Category category, FillBlankQuiz quiz) : base(context, category, quiz)
		{
		}

		protected override View CreateQuizContentView ()
		{
			var start = Quiz.Start;
			var end = Quiz.End;
			if (null != start || null != end)
				return GetStartEndView (start, end);

			if (null == answerView)
				answerView = CreateEditText ();

			return answerView;
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutString(KeyAnswer, answerView.Text);
				return bundle;
			}
			set {
				if (value == null)
					return;
				
				answerView.Text = value.GetString(KeyAnswer);
			}
		}

		View GetStartEndView(string start, string end) {
			var container = (LinearLayout) LayoutInflater.Inflate(Resource.Layout.quiz_fill_blank_with_surroundings, this, false);
			answerView = container.FindViewById<EditText>(Resource.Id.quiz_edit_text);
			answerView.AfterTextChanged += (sender, e) => AllowAnswer (!string.IsNullOrEmpty (e.Editable.ToString()));
			answerView.EditorAction += OnEditorAction;
			TextView startView = container.FindViewById<TextView>(Resource.Id.start);
			SetExistingContentOrHide(startView, start);

			TextView endView = container.FindViewById<TextView>(Resource.Id.end);
			SetExistingContentOrHide(endView, end);

			return container;
		}

		static void SetExistingContentOrHide(TextView view, string content)
		{
			if (null == content)
				view.Visibility = ViewStates.Gone;
			else
				view.Text = content;
		}
	}
}

