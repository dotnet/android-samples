using System;
using Topeka.Models.Quizzes;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Topeka.Helpers;
using System.Linq;
using Android.Views;
using Android.OS;

namespace Topeka.Widgets.Quizzes
{
	public class AlphaPickerQuizView : AbsQuizView<AlphaPickerQuiz>
	{
		const string KeySelection = "SELECTION";

		TextView currentSelection;
		SeekBar seekBar;
		List<string> alphabet;

		public AlphaPickerQuizView(Context context, Category category, AlphaPickerQuiz quiz) : base(context, category, quiz) {}

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

		protected override bool IsAnswerCorrect {
			get {
				return Quiz.IsAnswerCorrect (currentSelection.Text);
			}
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutString(KeySelection, currentSelection.Text);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				var userInput = value.GetString(KeySelection, Alphabet[0]);
				seekBar.Progress = Alphabet.IndexOf(userInput);
			}
		}

		List<string> Alphabet {
			get {
				if (alphabet == null)
					alphabet = Resources.GetStringArray (Resource.Array.alphabet).ToList ();
                return alphabet;
			}
		}
	}
}

