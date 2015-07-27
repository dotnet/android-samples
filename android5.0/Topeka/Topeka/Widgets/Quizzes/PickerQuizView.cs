using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Widgets.Quizzes
{
	public class PickerQuizView : AbsQuizView<PickerQuiz>
	{
		const string KeyAnswer = "ANSWER";

		TextView currentSelection;
		SeekBar seekBar;
		int step;
		int min;
		int progress;

		protected override bool IsAnswerCorrect =>
			Quiz.IsAnswerCorrect (progress);

		public override Bundle UserInput {
			get {
				var bundle = new Bundle ();
				bundle.PutInt (KeyAnswer, progress);
				return bundle;
			}
			set {
				if (value == null)
					return;

				seekBar.Progress = value.GetInt (KeyAnswer) - min;
			}
		}

		public PickerQuizView (Context context, Category category, PickerQuiz quiz) : base (context, category, quiz)
		{
		}

		protected override View CreateQuizContentView ()
		{
			InitStep ();
			min = Quiz.Min;
			var layout = (ScrollView)LayoutInflater.Inflate (Resource.Layout.quiz_layout_picker, this, false);
			currentSelection = layout.FindViewById<TextView> (Resource.Id.seekbar_progress);
			currentSelection.Text = min.ToString ();
			seekBar = layout.FindViewById<SeekBar> (Resource.Id.seekbar);
			seekBar.Max = GetSeekBarMax ();
			seekBar.ProgressChanged += (sender, e) => {
				SetCurrentSelectionText (min + e.Progress);
				AllowAnswer ();
			};
			return layout;
		}

		void SetCurrentSelectionText (int progress)
		{
			this.progress = progress / step * step;
			currentSelection.Text = this.progress.ToString ();
		}

		void InitStep ()
		{
			step = Quiz.Step == 0 ? 1 : Quiz.Step;
		}

		int GetSeekBarMax ()
		{
			int absMin = Math.Abs (Quiz.Min);
			int absMax = Math.Abs (Quiz.Max);
			int realMin = Math.Min (absMin, absMax);
			int realMax = Math.Max (absMin, absMax);
			return realMax - realMin;
		}
	}
}

