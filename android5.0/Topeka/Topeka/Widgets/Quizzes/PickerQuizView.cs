using System;
using Topeka.Models.Quizzes;
using Android.Widget;
using Android.Content;
using Topeka.Helpers;
using Android.OS;
using Android.Views;

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

		public PickerQuizView(Context context, Category category, PickerQuiz quiz) : base(context, category, quiz) {}

		protected override View CreateQuizContentView ()
		{
			InitStep();
			min = Quiz.Min;
			var layout = (ScrollView) LayoutInflater.Inflate(Resource.Layout.quiz_layout_picker, this, false);
			currentSelection = layout.FindViewById<TextView>(Resource.Id.seekbar_progress);
			currentSelection.Text = min.ToString();
			seekBar = layout.FindViewById<SeekBar>(Resource.Id.seekbar);
			seekBar.Max = GetSeekBarMax();
			seekBar.ProgressChanged += (sender, e) => {
				SetCurrentSelectionText(min + e.Progress);
				AllowAnswer();
			};
			return layout;
		}

		void SetCurrentSelectionText(int progress) {
			this.progress = progress / step * step;
            currentSelection.Text = this.progress.ToString();
		}

		protected override bool IsAnswerCorrect {
			get {
				return Quiz.IsAnswerCorrect(progress);
			}
		}

		void InitStep() {
			int tmpStep = Quiz.Step;
			step = tmpStep == 0 ? 1 : tmpStep;
		}

		public override Bundle UserInput {
			get {
				var bundle = new Bundle();
				bundle.PutInt(KeyAnswer, progress);
				return bundle;
			}
			set {
				if (value == null) {
					return;
				}
				seekBar.Progress = value.GetInt(KeyAnswer) - min;
			}
		}

		int GetSeekBarMax() {
			int absMin = Math.Abs(Quiz.Min);
			int absMax = Math.Abs(Quiz.Max);
			int realMin = Math.Min(absMin, absMax);
			int realMax = Math.Max(absMin, absMax);
			return realMax - realMin;
		}
	}
}

