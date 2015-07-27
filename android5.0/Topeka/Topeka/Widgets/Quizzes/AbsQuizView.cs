using System;

using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Topeka.Activities;
using Topeka.Helpers;
using Topeka.Models.Quizzes;
using Topeka.Widgets.Fab;

namespace Topeka.Widgets.Quizzes
{
	public abstract class AbsQuizView : FrameLayout
	{
		protected readonly int minHeightTouchTarget;
		protected readonly int spacingDouble;
		protected readonly Category mCategory;
		protected readonly ITimeInterpolator fastOutSlowInInterpolator;
		protected readonly ITimeInterpolator linearOutSlowInInterpolator;
		protected readonly int colorAnimationDuration;
		protected readonly int iconAnimationDuration;
		protected readonly int scaleAnimationDuration;
		protected TextView questionView;
		protected CheckableFab submitAnswer;

		public int QuizId { get; protected set; }

		public Quiz Quiz { get; set; }

		protected LayoutInflater LayoutInflater { get; set; }

		protected abstract bool IsAnswerCorrect { get; }

		public abstract Bundle UserInput { get; set; }

		protected bool Answered { get; set; }

		protected AbsQuizView (Context context, Category category) : base (context)
		{
			mCategory = category;
			spacingDouble = Resources.GetDimensionPixelSize (Resource.Dimension.spacing_double);
			minHeightTouchTarget = Resources.GetDimensionPixelSize (Resource.Dimension.min_height_touch_target);
			fastOutSlowInInterpolator = AnimationUtils.LoadInterpolator (Context, Android.Resource.Interpolator.FastOutSlowIn);
			linearOutSlowInInterpolator = AnimationUtils.LoadInterpolator (Context, Android.Resource.Interpolator.LinearOutSlowIn);
			colorAnimationDuration = 400;
			iconAnimationDuration = 300;
			scaleAnimationDuration = 200;
		}

		protected void AddContentView (ViewGroup container, View quizContentView)
		{
			var layoutParams = new LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			container.AddView (questionView, layoutParams);
			container.AddView (quizContentView, layoutParams);
			AddView (container, layoutParams);
		}

		protected void AddFloatingActionButton ()
		{
			var fabSize = Resources.GetDimensionPixelSize (Resource.Dimension.size_fab);
			var bottomOfQuestionView = FindViewById (Resource.Id.question_view).Bottom;
			var fabLayoutParams = new LayoutParams (fabSize, fabSize, GravityFlags.End | GravityFlags.Top);
			var halfAFab = fabSize / 2;
			fabLayoutParams.SetMargins (0, // left
				bottomOfQuestionView - halfAFab, //top
				0, // right
				spacingDouble); // bottom
			fabLayoutParams.MarginEnd = spacingDouble;
			AddView (submitAnswer, fabLayoutParams);
		}

		protected void ResizeView ()
		{
			var widthHeightRatio = Height / (float)Width;

			Animate ()
                .ScaleY (.5f / widthHeightRatio)
                .SetDuration (300)
                .SetStartDelay (750)
                .Start ();
			Animate ()
                .ScaleX (.5f)
                .SetDuration (300)
                .SetStartDelay (800)
                .Start ();
		}

		protected void AnimateFabBackgroundColor (int backgroundColor)
		{
			var fabColorAnimator = ObjectAnimator.OfArgb (submitAnswer, "backgroundColor", Color.White, backgroundColor);
			fabColorAnimator.SetDuration (colorAnimationDuration).SetInterpolator (fastOutSlowInInterpolator);
			fabColorAnimator.Start ();
		}

		public Color GetForegroundColor (FrameLayout obj)
		{
			return ((ColorDrawable)obj.Foreground).Color;
		}

		public void SetForegroundColor (FrameLayout obj, Color value)
		{
			var foreground = obj.Foreground as ColorDrawable;
			if (foreground != null)
				foreground.Color = value;
			else
				obj.Foreground = new ColorDrawable (value);
		}
	}

	public abstract class AbsQuizView<T> : AbsQuizView where T : Quiz
	{
		public new T Quiz { get; private set; }

		protected AbsQuizView (Context context, Category category, T quiz) : base (context, category)
		{
			Quiz = quiz;
			QuizId = quiz.Id;
			submitAnswer = GetSubmitButton (context);
			LayoutInflater = LayoutInflater.From (context);
			SetUpQuestionView ();
			var container = CreateContainerLayout (context);
			var quizContentView = GetInitializedContentView ();
			AddContentView (container, quizContentView);
			EventHandler<LayoutChangeEventArgs> handler = null;
			handler = ((sender, e) => {
				((View)sender).LayoutChange -= handler;
				AddFloatingActionButton ();
			});
			LayoutChange += handler;
		}

		void SetUpQuestionView ()
		{
			questionView = (TextView)LayoutInflater.Inflate (Resource.Layout.question, this, false);
			questionView.Text = Quiz.Question;
		}

		static LinearLayout CreateContainerLayout (Context context)
		{
			return new LinearLayout (context) {
				Id = Resource.Id.absQuizViewContainer,
				Orientation = Orientation.Vertical
			};
		}

		View GetInitializedContentView ()
		{
			var quizContentView = CreateQuizContentView ();
			quizContentView.Id = Resource.Id.quiz_content;
			quizContentView.SaveEnabled = true;
			SetDefaultPadding (quizContentView);
			(quizContentView as ViewGroup)?.SetClipToPadding (false);
			
			quizContentView.SetMinimumHeight (Resources.GetDimensionPixelSize (Resource.Dimension.min_height_question));
			return quizContentView;
		}

		CheckableFab GetSubmitButton (Context context)
		{
			if (submitAnswer == null) {
				submitAnswer = new CheckableFab (context) {
					Id = Resource.Id.submitAnswer,
					Visibility = ViewStates.Gone,
					ScaleY = 0,
					ScaleX = 0
				};
				submitAnswer.Click += (sender, e) => SubmitAnswer ((View)sender);
			}
			return submitAnswer;
		}

		void SetDefaultPadding (View view)
		{
			view.SetPadding (spacingDouble, spacingDouble, spacingDouble, spacingDouble);
		}
		
		protected abstract View CreateQuizContentView ();

		protected void AllowAnswer (bool answered)
		{
			if (submitAnswer == null)
				return;
			
			float targetScale = answered ? 1f : 0f;
			if (answered)
				submitAnswer.Visibility = ViewStates.Visible;
			
			submitAnswer.Animate ().ScaleX (targetScale).ScaleY (targetScale)
				.SetInterpolator (fastOutSlowInInterpolator);
			Answered = answered;
		}

		protected void AllowAnswer ()
		{
			if (!Answered)
				AllowAnswer (true);
		}

		protected virtual void SubmitAnswer ()
		{
			SubmitAnswer (FindViewById (Resource.Id.submitAnswer));
		}

		void SubmitAnswer (View v)
		{
			var answerCorrect = IsAnswerCorrect;
			Quiz.Solved = true;
			PerformScoreAnimation (answerCorrect);
		}

		void PerformScoreAnimation (bool answerCorrect)
		{
			submitAnswer.Checked = answerCorrect;

			var backgroundColor = Resources.GetColor (answerCorrect ? Resource.Color.green : Resource.Color.red);
			AnimateFabBackgroundColor (backgroundColor);
			HideFab ();
			ResizeView ();
			MoveViewOffScreen (answerCorrect);

			AnimateForegroundColor (backgroundColor);
		}

		void HideFab ()
		{
			submitAnswer.Animate ()
				.SetDuration (scaleAnimationDuration)
				.SetStartDelay (iconAnimationDuration * 2)
				.ScaleX (0f)
				.ScaleY (0f)
				.SetInterpolator (linearOutSlowInInterpolator)
				.Start ();
		}

		void AnimateForegroundColor (int targetColor)
		{
			var foregroundAnimator = ObjectAnimator.OfArgb (this, "FOREGROUND_COLOR", Color.White, targetColor);
			foregroundAnimator.SetDuration (200).SetInterpolator (linearOutSlowInInterpolator);
			foregroundAnimator.StartDelay = 750;
			foregroundAnimator.Start ();
		}

		void MoveViewOffScreen (bool answerCorrect)
		{
			var runnable = new Runnable ();
			runnable.RunAction += (sender, e) => {
				mCategory.SetScore (Quiz, answerCorrect);
				(Context as QuizActivity)?.Proceed ();
			};

			Animate ()
                .SetDuration (200)
                .SetStartDelay (1200)
                .SetInterpolator (linearOutSlowInInterpolator)
				.WithEndAction (runnable)
				.Start ();
		}
	}
}

