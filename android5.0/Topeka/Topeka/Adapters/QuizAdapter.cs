using System;
using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using Topeka.Helpers;

using Topeka.Models.Quizzes;
using Topeka.Widgets.Quizzes;

namespace Topeka.Adapters
{
	public class QuizAdapter : BaseAdapter
	{
		readonly Context context;
		readonly List<Quiz> quizzes;
		readonly Category category;
		readonly int viewTypeCount;
		List<string> quizTypes;

		public override int Count => quizzes.Count;

		public override int ViewTypeCount => viewTypeCount;

		public override bool HasStableIds => true;

		public QuizAdapter (Context context, Category category)
		{
			this.context = context;
			this.category = category;
			quizzes = category.Quizzes;
			viewTypeCount = CalculateViewTypeCount ();
		}

		int CalculateViewTypeCount ()
		{
			var tmpTypes = new List<string> ();
			for (int i = 0; i < quizzes.Count; i++)
				tmpTypes.Add (quizzes [i].QuizType.JsonName);

			quizTypes = new List<string> (tmpTypes);
			return quizTypes.Count;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return quizzes [position];
		}

		public override long GetItemId (int position)
		{
			return quizzes [position].Id;
		}

		public override int GetItemViewType (int position)
		{
			return quizTypes.IndexOf (((QuizType)GetItem (position)).JsonName);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var quiz = (Quiz)GetItem (position);
			if (convertView is AbsQuizView && ((AbsQuizView)convertView).Quiz.Equals (quiz))
					return convertView;

			convertView = GetViewInternal (quiz);
			return convertView;
		}

		AbsQuizView GetViewInternal (Quiz quiz)
		{
			if (quiz == null)
				throw new InvalidOperationException ("Quiz must not be null");
			return CreateViewFor (quiz);
		}

		AbsQuizView CreateViewFor (Quiz quiz)
		{
			if (quiz.QuizType == QuizType.AlphaPicker)
				return new AlphaPickerQuizView (context, category, (AlphaPickerQuiz)quiz);
			if (quiz.QuizType == QuizType.FillBlank)
				return new FillBlankQuizView (context, category, (FillBlankQuiz)quiz);
			if (quiz.QuizType == QuizType.FillTwoBlanks)
				return new FillTwoBlanksQuizView (context, category, (FillTwoBlanksQuiz)quiz);
			if (quiz.QuizType == QuizType.FourQuarter)
				return new FourQuarterQuizView (context, category, (FourQuarterQuiz)quiz);
			if (quiz.QuizType == QuizType.MultiSelect)
				return new MultiSelectQuizView (context, category, (MultiSelectQuiz)quiz);
			if (quiz.QuizType == QuizType.Picker)
				return new PickerQuizView (context, category, (PickerQuiz)quiz);
			if (quiz.QuizType == QuizType.SingleSelect || quiz.QuizType == QuizType.SingleSelectItem)
				return new SelectItemQuizView (context, category, (SelectItemQuiz)quiz);
			if (quiz.QuizType == QuizType.ToggleTranslate)
				return new ToggleTranslateQuizView (context, category, (ToggleTranslateQuiz)quiz);
			if (quiz.QuizType == QuizType.TrueFalse)
				return new TrueFalseQuizView (context, category, (TrueFalseQuiz)quiz);
			throw new InvalidOperationException ("Quiz of type " + quiz.QuizType + " can not be displayed.");
		}
	}
}

