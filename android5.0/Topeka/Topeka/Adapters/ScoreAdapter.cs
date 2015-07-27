using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Adapters
{
	public class ScoreAdapter : BaseAdapter
	{
		readonly Category category;
		readonly int count;
		readonly List<Quiz> quizList;

		Drawable successIcon;
		Drawable failedIcon;

		public ScoreAdapter (Category category)
		{
			this.category = category;
			quizList = this.category.Quizzes;
			count = quizList.Count;
		}

		public override int Count {
			get {
				return count;
			}
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return quizList [position];
		}

		public override long GetItemId (int position)
		{
			return (position > count || position < 0) ? AbsListView.InvalidPosition : position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
				convertView = CreateView (parent);

			var quiz = (Quiz)GetItem (position);
			var viewHolder = (ViewHolder)convertView.Tag;
			viewHolder.mQuizView.Text = quiz.Question;
			viewHolder.mAnswerView.Text = quiz.GetStringAnswer ();
			SetSolvedStateForQuiz (viewHolder.mSolvedState, position);
			return convertView;
		}

		void SetSolvedStateForQuiz (ImageView solvedState, int position)
		{
			var context = solvedState.Context;
			Drawable tintedImage;
			tintedImage = (category.IsSolvedCorrectly ((Quiz)GetItem (position))) ? GetSuccessIcon (context) : GetFailedIcon (context);

			solvedState.SetImageDrawable (tintedImage);
		}

		Drawable GetSuccessIcon (Context context)
		{
			return successIcon = successIcon ?? LoadAndTint (context, Resource.Drawable.ic_tick, Resource.Color.theme_green_primary);
		}

		Drawable GetFailedIcon (Context context)
		{
			return failedIcon = failedIcon ?? LoadAndTint (context, Resource.Drawable.ic_cross, Resource.Color.theme_red_primary);
		}

		Drawable LoadAndTint (Context context, int drawableId, int tintColor)
		{
			Drawable imageDrawable = context.GetDrawable (drawableId);
			if (imageDrawable == null)
				throw new InvalidOperationException ("The drawable with id " + drawableId + " does not exist");

			imageDrawable.SetTint (context.Resources.GetColor (tintColor));
			return imageDrawable;
		}

		View CreateView (ViewGroup parent)
		{
			View convertView;
			LayoutInflater inflater = LayoutInflater.From (parent.Context);
			var scorecardItem = (ViewGroup)inflater.Inflate (Resource.Layout.item_scorecard, parent, false);
			convertView = scorecardItem;
			ViewHolder holder = new ViewHolder (scorecardItem);
			convertView.Tag = holder;
			return convertView;
		}

		class ViewHolder : Java.Lang.Object
		{
			internal TextView mAnswerView;
			internal TextView mQuizView;
			internal ImageView mSolvedState;

			public ViewHolder (ViewGroup scorecardItem)
			{
				mQuizView = scorecardItem.FindViewById<TextView> (Resource.Id.quiz);
				mAnswerView = scorecardItem.FindViewById<TextView> (Resource.Id.answer);
				mSolvedState = scorecardItem.FindViewById<ImageView> (Resource.Id.solved_state);
			}
		}
	}
}

