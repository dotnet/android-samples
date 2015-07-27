using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class AlphaPickerQuizView : AbsQuizView<AlphaPickerQuiz>
	{
		const string KeySelection = "SELECTION";

		TextView currentSelection;
		SeekBar seekBar;
		List<string> alphabet;

		protected override bool IsAnswerCorrect =>
			Quiz.IsAnswerCorrect (currentSelection.Text);

		List<string> Alphabet =>
			alphabet = alphabet ?? Resources.GetStringArray (Resource.Array.alphabet).ToList ();

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutString (KeySelection, currentSelection.Text);
				return bundle;
			}
			set {
				if (value == null)
					return;

				var userInput = value.GetString (KeySelection, Alphabet[0]);
				seekBar.Progress = Alphabet.IndexOf (userInput);
			}
		}

		public AlphaPickerQuizView(Context context, Category category, AlphaPickerQuiz quiz) : base(context, category, quiz)
		{
		}

		protected override View CreateQuizContentView ()
		{
			var layout = (ScrollView)LayoutInflater.Inflate (Resource.Layout.quiz_layout_picker, this, false);
			currentSelection = layout.FindViewById<TextView>(Resource.Id.seekbar_progress);
			currentSelection.Text = Alphabet[0];
			seekBar = layout.FindViewById<SeekBar>(Resource.Id.seekbar);
			seekBar.Max = Alphabet.Count - 1;
			seekBar.ProgressChanged += (sender, e) => {
				currentSelection.Text = Alphabet[e.Progress];
				AllowAnswer();
			};
			return layout;
		}
	}
}

