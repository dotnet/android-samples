using System;

using Android.Animation;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;

using CommonSampleLibrary;

namespace RevealEffectBasic
{
	public class RevealEffectBasicFragment : Fragment
	{
		static readonly string TAG = "RevealEffectBasicFragment";

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate (Resource.Layout.reveal_effect_basic, container, false);
			View button = rootView.FindViewById (Resource.Id.button);

			// Set a listener to reveal the view on ACTION_DOWN.
			button.Click += delegate {
				View shape = rootView.FindViewById (Resource.Id.circle);

				/* Create a reveal ValueAnimator that starts clipping the view from
					 * the top left corner until the whole view is covered. */
				Animator animator = ViewAnimationUtils.CreateCircularReveal (shape, 0, 0, 0,
					(float)Math.Sqrt (Math.Pow (shape.Width, 2) + Math.Pow (shape.Height, 2)));

				// Set a natural ease-in/ease-out interpolator
				animator.SetInterpolator (new AccelerateDecelerateInterpolator ());

				animator.Start ();

				Log.Info (TAG, "Reveal effect shown");
			};

			return rootView;
		}
	}
}

