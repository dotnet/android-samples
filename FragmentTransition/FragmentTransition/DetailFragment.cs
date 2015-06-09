using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using Android.Transitions;
using Android.Animation;

using CommonSampleLibrary;

namespace FragmentTransition
{
	public class DetailFragment : Fragment
	{
		public const string TAG = "DetailFragment";
		public const string ARG_RESOURCE_ID = "resource_id";
		public const string ARG_TITLE = "title";
		public const string ARG_X = "x";
		public const string ARG_Y = "y";
		public const string ARG_WIDTH = "width";
		public const string ARG_HEIGHT = "height";


		/**
	     * Create a new instance of DetailFragment.
	     * @param resourceId The resource ID of the Drawable image to show
	     * @param title The title of the image
	     * @param x The horizontal position of the grid item in pixel
	     * @param y The vertical position of the grid item in pixel
	     * @param width The width of the grid item in pixel
	     * @param height The height of the grid item in pixel
	     * @return a new instance of DetailFragment
	     */
		public static DetailFragment NewInstance (int resourceId, string title, int x, int y, int width, int height)
		{
			var fragment = new DetailFragment ();
			var args = new Bundle ();
			args.PutInt (ARG_RESOURCE_ID, resourceId);
			args.PutString (ARG_TITLE, title);
			args.PutInt (ARG_X, x);
			args.PutInt (ARG_Y, y);
			args.PutInt (ARG_WIDTH, width);
			args.PutInt (ARG_HEIGHT, height);
			fragment.Arguments = args;
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_detail, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			var root = (FrameLayout)view;
			var context = view.Context;
			if (context == null)
				return;
			
			// This is how the fragment looks at first. Since the transition is one-way, we don't need to make
			// this a Scene.
			var item = LayoutInflater.From (context).Inflate (Resource.Layout.item_meat_grid, root, false);
			if (item == null)
				return;
			
			Bind (item);
			// We adjust the position of the initial image with LayoutParams using the values supplied
			// as the fragment arguments.
			Bundle args = Arguments;
			FrameLayout.LayoutParams param = null;
			if (args != null) {
				param = new FrameLayout.LayoutParams (
					args.GetInt (ARG_WIDTH), args.GetInt (ARG_HEIGHT));
				param.TopMargin = args.GetInt (ARG_Y);
				param.LeftMargin = args.GetInt (ARG_X);
			}
			root.AddView (item, param);
		}

		public override void OnResume()
		{
			base.OnResume();
		}

		/**
		 * Bind the views inside of parent with the fragment arguments.
	     *
	     * @param parent The parent of views to bind.
	     */
		private void Bind (View parent)
		{
			Bundle args = Arguments;
			if (args == null)
				return;
			
			var image = parent.FindViewById<ImageView> (Resource.Id.image);
			image.SetImageResource (args.GetInt (ARG_RESOURCE_ID));

			var title = parent.FindViewById<TextView> (Resource.Id.title);
			title.Text = args.GetString (ARG_TITLE);
		}

		public override Animator OnCreateAnimator (FragmentTransit transit, bool enter, int nextAnim)
		{
			Animator animator = AnimatorInflater.LoadAnimator (Activity,
				enter ? Android.Resource.Animator.FadeIn : Android.Resource.Animator.FadeOut);
			
			// We bind a listener for the fragment transaction. We only bind it when
			// this fragment is entering.
			if (animator != null && enter)
				animator.AnimationEnd += (object sender, EventArgs e) => {
					// This method is called at the end of the animation for the fragment transaction,
					// which is perfect time to start our Transition.
					Log.Info (TAG, "Fragment animation ended. Starting a Transition.");
					Scene scene = Scene.GetSceneForLayout ((ViewGroup)View, Resource.Layout.fragment_detail_content, Activity);
					TransitionManager.Go (scene);
					// Note that we need to bind views with data after we call TransitionManager.go().
					Bind (scene.SceneRoot);
				};

			return animator;		
		}
	}
}