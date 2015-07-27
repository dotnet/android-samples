using System;

using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Transitions;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Topeka.Adapters;
using Topeka.Fragments;
using Topeka.Helpers;
using Topeka.Persistence;
using Topeka.Widgets.Fab;

namespace Topeka.Activities
{
	[Activity (WindowSoftInputMode = SoftInput.AdjustPan)]
	public class QuizActivity : Activity
	{
		const string Tag = "QuizActivity";
		const string ImageCategory = "image_category_";
		const string StateIsPlaying = "isPlaying";
		const int Undefined = -1;
		const string FragmentTag = "Quiz";
       
		QuizFragment quizFragment;
		Toolbar toolbar;
		ImageView quizFab;
		IInterpolator interpolator;
		ImageView icon;
		Animator circularReveal;

		string categoryId;
		bool savedStateIsPlaying;

		public static Intent GetStartIntent (Context context, Category category)
		{
			var starter = new Intent (context, typeof(QuizActivity));
			starter.PutExtra (Category.TAG, category.Id);
			return starter;
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			var sharedElementEnterTransition = TransitionInflater.From (this).InflateTransition (Resource.Transition.quiz_enter);
			Window.SharedElementEnterTransition = sharedElementEnterTransition;

			categoryId = Intent.GetStringExtra (Category.TAG);
			interpolator = AnimationUtils.LoadInterpolator (this, Android.Resource.Interpolator.FastOutSlowIn);
			if (savedInstanceState != null)
				savedStateIsPlaying = savedInstanceState.GetBoolean (StateIsPlaying);
			Populate (categoryId);
			base.OnCreate (savedInstanceState);
		}

		protected override void OnResume ()
		{
			if (savedStateIsPlaying) {
				quizFragment = (QuizFragment)FragmentManager.FindFragmentByTag (FragmentTag);
				FindViewById (Resource.Id.quiz_fragment_container).Visibility = ViewStates.Visible;
			}
			base.OnResume ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean (StateIsPlaying, quizFab.Visibility == ViewStates.Gone);
			base.OnSaveInstanceState (outState);
		}

		public override void OnBackPressed ()
		{
			if (icon == null || quizFab == null) {
				base.OnBackPressed ();
				return;
			}
            
			icon.Animate ()
                .ScaleX (.7f)
                .ScaleY (.7f)
                .Alpha (0f)
                .SetInterpolator (interpolator)
                .Start ();

			var listener = new AnimatorListener (this);
			listener.AnimationEnd += (sender, e) => {
				if (IsFinishing || IsDestroyed)
					return;
				base.OnBackPressed ();
			};

			quizFab.Animate ()
                .ScaleX (0f)
                .ScaleY (0f)
                .SetInterpolator (interpolator)
                .SetStartDelay (100)
                .SetListener (listener)
				.Start ();
		}

		void StartQuizFromClickOn (View view)
		{
			InitQuizFragment ();
			FragmentManager.BeginTransaction ().Replace (Resource.Id.quiz_fragment_container, quizFragment, FragmentTag).Commit ();
			View fragmentContainer = FindViewById (Resource.Id.quiz_fragment_container);
			int centerX = (view.Left + view.Right) / 2;
			int centerY = (view.Top + view.Bottom) / 2;
			int finalRadius = Math.Max (fragmentContainer.Width, fragmentContainer.Height);
			circularReveal = ViewAnimationUtils.CreateCircularReveal (
				fragmentContainer, centerX, centerY, 0, finalRadius);
			fragmentContainer.Visibility = ViewStates.Visible;
			view.Visibility = ViewStates.Gone;

			EventHandler handler = null;
			handler += (sender, e) => {
				icon.Visibility = ViewStates.Gone;
				circularReveal.AnimationEnd -= handler;
			};
			circularReveal.AnimationEnd += handler;

			circularReveal.Start ();

			toolbar.Elevation = 0;
		}

		public void ElevateToolbar ()
		{
			toolbar.Elevation = Resources.GetDimension (Resource.Dimension.elevation_header);
		}

		void InitQuizFragment ()
		{
			quizFragment = QuizFragment.Create (categoryId);
			quizFragment.CategorySolved += (outerSender, outerArgs) => {
				ElevateToolbar ();
				if (circularReveal != null && circularReveal.IsRunning) {
					EventHandler handler = null;
					handler = (innerSender, innerArgs) => {
						quizFab.SetImageResource (Resource.Drawable.ic_tick);
						quizFab.Id = Resource.Id.quiz_done;
						quizFab.Visibility = ViewStates.Visible;
						quizFab.ScaleX = 0f;
						quizFab.ScaleY = 0f;
						quizFab.Animate ()
                                .ScaleX (1)
                                .ScaleY (1)
                                .SetInterpolator (interpolator)
                                .SetListener (null)
                                .Start ();
						circularReveal.AnimationEnd -= handler;
					};
					circularReveal.AnimationEnd += handler;
				} else {
					quizFab.SetImageResource (Resource.Drawable.ic_tick);
					quizFab.Id = Resource.Id.quiz_done;
					quizFab.Visibility = ViewStates.Visible;
					quizFab.ScaleX = 0f;
					quizFab.ScaleY = 0f;
					quizFab.Animate ()
                            .ScaleX (1)
                            .ScaleY (1)
                            .SetInterpolator (interpolator)
                            .SetListener (null)
                            .Start ();
				}
			};
			toolbar.Elevation = 0;
		}

		public void Proceed ()
		{
			SubmitAnswer ();
		}

		void SubmitAnswer ()
		{
			ElevateToolbar ();
			if (!quizFragment.ShowNextPage ()) {
				quizFragment.ShowSummary ();
				return;
			}
			toolbar.Elevation = 0;
		}

		void Populate (string categoryId)
		{
			if (string.IsNullOrEmpty (categoryId)) {
				Log.Warn (Tag, "Didn't find a category. Finishing");
				Finish ();
			}
			var category = TopekaDatabaseHelper.GetCategoryWith (this, categoryId);
			SetTheme (category.Theme.StyleId);
			InitLayout (category.Id);
			InitToolbar (category);
		}

		void InitLayout (string categoryId)
		{
			SetContentView (Resource.Layout.activity_quiz);
			icon = FindViewById<ImageView> (Resource.Id.icon);
			int resId = Resources.GetIdentifier (ImageCategory + categoryId, CategoryAdapter.Drawable, ApplicationContext.PackageName);
			icon.SetImageResource (resId);
			icon.SetImageResource (resId);
			icon.Animate ()
                .ScaleX (1)
                .ScaleY (1)
                .Alpha (1)
                .SetInterpolator (interpolator)
                .SetStartDelay (300)
                .Start ();
			quizFab = FindViewById<FloatingActionButton> (Resource.Id.fab_quiz);
			quizFab.SetImageResource (Resource.Drawable.ic_play);
			quizFab.Visibility = savedStateIsPlaying ? ViewStates.Gone : ViewStates.Visible;
			quizFab.Click += (sender, e) => {
				var view = (View)sender;
				switch (view.Id) {
				case Resource.Id.fab_quiz:
					StartQuizFromClickOn (view);
					break;
				case Resource.Id.submitAnswer:
					SubmitAnswer ();
					break;
				case Resource.Id.quiz_done:
					FinishAfterTransition ();
					break;
				case Undefined:
					var contentDescription = view.ContentDescription;
					if (contentDescription != null && contentDescription == GetString (Resource.String.up)) {
						OnBackPressed ();
						break;
					}
					break;
				default:
					throw new InvalidOperationException (
						"OnClick has not been implemented for " + Resources.GetResourceName (view.Id));
				}
			};
			quizFab.Animate ()
                .ScaleX (1)
                .ScaleY (1)
                .SetInterpolator (interpolator)
                .SetStartDelay (400)
                .Start ();
		}

		void InitToolbar (Category category)
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.toolbar_activity_quiz);
			toolbar.Title = category.Name;
			toolbar.NavigationOnClick += (sender, e) => {
				var view = (View)sender;
				switch (view.Id) {
				case Resource.Id.fab_quiz:
					StartQuizFromClickOn (view);
					break;
				case Resource.Id.submitAnswer:
					SubmitAnswer ();
					break;
				case Resource.Id.quiz_done:
					FinishAfterTransition ();
					break;
				case Undefined:
					var contentDescription = view.ContentDescription;
					if (contentDescription != null && contentDescription == GetString (Resource.String.up)) {
						OnBackPressed ();
						break;
					}
					break;
				default:
					throw new InvalidOperationException (
						"OnClick has not been implemented for " + Resources.GetResourceName (view.Id));
				}
			};

			if (savedStateIsPlaying)
				toolbar.Elevation = 0;
		}
	}
}

