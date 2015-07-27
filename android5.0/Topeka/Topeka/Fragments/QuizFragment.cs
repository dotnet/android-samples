using System;

using Android;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Topeka.Adapters;
using Topeka.Helpers;
using Topeka.Persistence;
using Topeka.Widgets.Quizzes;

namespace Topeka.Fragments
{
	public class QuizFragment : Fragment
	{
		const string KeyUserInput = "USER_INPUT";

		TextView progressText;
		int quizSize;
		ProgressBar progressBar;
		Category category;
		AdapterViewAnimator quizView;
		ScoreAdapter scoreAdapter;
		QuizAdapter quizAdapter;

		public event EventHandler<EventArgs> CategorySolved;

		ScoreAdapter ScoreAdapter =>
			scoreAdapter = scoreAdapter ?? new ScoreAdapter (category);

		QuizAdapter QuizAdapter =>
			quizAdapter = quizAdapter ?? new QuizAdapter (Activity, category);

		public static QuizFragment Create (string categoryId)
		{
			if (string.IsNullOrEmpty (categoryId))
				throw new InvalidOperationException ("The category can not be null");

			var args = new Bundle ();
			args.PutString (Category.TAG, categoryId);
			return new QuizFragment { Arguments = args };
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			var categoryId = Arguments.GetString (Category.TAG);
			category = TopekaDatabaseHelper.GetCategoryWith (Activity, categoryId);
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var theme = category.Theme;
			var context = new ContextThemeWrapper (Activity, theme.StyleId);
			var themedInflater = LayoutInflater.From (context);
			return themedInflater.Inflate (Resource.Layout.fragment_quiz, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			quizView = view.FindViewById<AdapterViewAnimator> (Resource.Id.quiz_view);
			DecideOnViewToDisplay ();
			quizView.SetInAnimation (Activity, Resource.Animator.slide_in_bottom);
			quizView.SetOutAnimation (Activity, Resource.Animator.slide_out_top);
			var avatar = view.FindViewById<AvatarView> (Resource.Id.avatar);
			SetAvatarDrawable (avatar);
			InitProgressToolbar (view);
			base.OnViewCreated (view, savedInstanceState);
		}

		void InitProgressToolbar (View view)
		{
			var firstUnsolvedQuizPosition = category.GetFirstUnsolvedQuizPosition ();
			var quizzes = category.Quizzes;
			quizSize = quizzes.Count;
			progressText = view.FindViewById<TextView> (Resource.Id.progress_text);
			progressBar = view.FindViewById<ProgressBar> (Resource.Id.progress);
			progressBar.Max = quizSize;

			SetProgress (firstUnsolvedQuizPosition);
		}

		void SetProgress (int currentQuizPosition)
		{
			if (!IsAdded)
				return;

			progressText.Text = string.Format (GetString (Resource.String.quiz_of_quizzes), currentQuizPosition, quizSize);
			progressBar.Progress = currentQuizPosition;
		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			var focusedChild = quizView.FocusedChild;
			var viewGroup = focusedChild as ViewGroup;
			if (viewGroup != null) {
				var currentView = viewGroup.GetChildAt (0);
				var qView = currentView as AbsQuizView;
				if (qView != null)
					outState.PutBundle (KeyUserInput, qView.UserInput);
			}
			base.OnSaveInstanceState (outState);
		}

		public override void OnViewStateRestored (Bundle savedInstanceState)
		{
			RestoreQuizState (savedInstanceState);
			base.OnViewStateRestored (savedInstanceState);
		}

		public bool ShowNextPage ()
		{
			if (quizView == null)
				return false;

			var nextItem = quizView.DisplayedChild + 1;
			SetProgress (nextItem);
			var count = quizView.Adapter.Count;
			if (nextItem < count) {
				quizView.ShowNext ();
				TopekaDatabaseHelper.UpdateCategory (Activity, category);
				return true;
			}
			MarkCategorySolved ();
			return false;
		}

		public void ShowSummary ()
		{
			var scorecardView = View.FindViewById<ListView> (Resource.Id.scorecard);
			scoreAdapter = ScoreAdapter;
			scorecardView.Adapter = scoreAdapter;
			scorecardView.Visibility = ViewStates.Visible;
			quizView.Visibility = ViewStates.Gone;
		}

		protected virtual void OnCategorySolved (EventArgs e)
		{
			CategorySolved?.Invoke (this, e);
		}

		void SetAvatarDrawable (AvatarView avatarView)
		{
			var player = PreferencesHelper.GetPlayer (Activity);
			avatarView.SetImageResource ((int)player.Avatar);
		}

		void DecideOnViewToDisplay ()
		{
			var isSolved = category.Solved;
			if (isSolved) {
				ShowSummary ();
				OnCategorySolved (new EventArgs ());
			} else {
				quizView.Adapter = QuizAdapter;
				quizView.SetSelection (category.GetFirstUnsolvedQuizPosition ());
			}
		}

		void MarkCategorySolved ()
		{
			category.Solved = true;
			TopekaDatabaseHelper.UpdateCategory (Activity, category);
		}

		void RestoreQuizState (Bundle savedInstanceState)
		{
			if (null == savedInstanceState)
				return;

			EventHandler<View.LayoutChangeEventArgs> handler = null;
			handler += (sender, e) => {
				quizView.LayoutChange -= handler;
				var currentChild = quizView.GetChildAt (0);
				var viewGroup = currentChild as ViewGroup;
				if (viewGroup == null)
					return;

				var potentialQuizView = viewGroup.GetChildAt (0);
				var absQuizView = potentialQuizView as AbsQuizView;
				if (absQuizView != null)
					absQuizView.UserInput = savedInstanceState.GetBundle (KeyUserInput);
			};
			quizView.LayoutChange += handler;
		}
	}
}

