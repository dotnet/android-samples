using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using Topeka.Activities;
using Topeka.Adapters;
using Topeka.Helpers;

namespace Topeka.Fragments
{
	public class CategorySelectionFragment : Fragment
	{
		CategoryAdapter categoryAdapter;

		public static CategorySelectionFragment Create ()
		{
			return new CategorySelectionFragment ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_categories, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			SetUpQuizGrid (view.FindViewById<GridView> (Resource.Id.categories));
			base.OnViewCreated (view, savedInstanceState);
		}

		void SetUpQuizGrid (GridView categoriesView)
		{
			categoriesView.ItemClick += (sender, e) => { 
				var activity = Activity;
				StartQuizActivityWithTransition (activity, e.View.FindViewById (Resource.Id.category_title), (Category)categoryAdapter.GetItem (e.Position));
			};
			categoryAdapter = new CategoryAdapter (Activity);
			categoriesView.Adapter = categoryAdapter;
		}

		public override void OnResume ()
		{
			categoryAdapter.NotifyDataSetChanged ();
			base.OnResume ();
		}

		void StartQuizActivityWithTransition (Activity activity, View toolbar, Category category)
		{
			var pairs = TransitionHelper.CreateSafeTransitionParticipants (activity, false, new Pair (toolbar, activity.GetString (Resource.String.transition_toolbar)));
			var sceneTransitionAnimation = ActivityOptions.MakeSceneTransitionAnimation (activity, pairs);

			// Start the activity with the participants, animating from one to the other.
			var transitionBundle = sceneTransitionAnimation.ToBundle ();
			activity.StartActivity (QuizActivity.GetStartIntent (activity, category), transitionBundle);
		}
	}
}

