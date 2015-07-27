using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public abstract class TextInputQuizView<T> : AbsQuizView<T> where T : Quiz
	{
		InputMethodManager InputMethodManager =>
			(InputMethodManager)Context.GetSystemService (Context.InputMethodService);

		protected TextInputQuizView (Context context, Category category, T quiz) : base (context, category, quiz)
		{
		}

		protected EditText CreateEditText ()
		{
			var editText = (EditText)LayoutInflater.Inflate (Resource.Layout.quiz_edit_text, this, false);
			editText.AfterTextChanged += (sender, e) => AllowAnswer (!string.IsNullOrEmpty (e.Editable.ToString ()));
			editText.EditorAction += OnEditorAction;
			return editText;
		}

		protected override void SubmitAnswer ()
		{
			HideKeyboard (this);
			base.SubmitAnswer ();
		}

		void HideKeyboard (View view)
		{
			var inputMethodManager = InputMethodManager;
			inputMethodManager.HideSoftInputFromWindow (view.WindowToken, 0);
		}

		public void OnEditorAction (object sender, TextView.EditorActionEventArgs e)
		{
			var v = (TextView)sender;
			if (string.IsNullOrEmpty (v.Text))
				return;
			
			AllowAnswer ();

			if (e.ActionId != ImeAction.Done)
				return;
			
			SubmitAnswer ();
			HideKeyboard (v);
		}
	}
}

