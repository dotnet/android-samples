using System;
using Android.Animation;
using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;

using CommonSampleLibrary;

namespace RevealEffectBasic
{
	public class RevealEffectBasicFragment : Fragment
	{
		const string TAG = "RevealEffectBasicFragment";
		View rootView;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			rootView = inflater.Inflate (Resource.Layout.reveal_effect_basic, container, false);

			View button = rootView.FindViewById (Resource.Id.button);

			// Set a listener to reveal the view on ACTION_DOWN.
			button.SetOnTouchListener (new MyOnTouchListener (this));
			return rootView;
		}

		private class MyOnTouchListener : Java.Lang.Object,View.IOnTouchListener
		{
			RevealEffectBasicFragment f;

			public MyOnTouchListener(RevealEffectBasicFragment frag)
			{
				f=frag;
			}

			public bool OnTouch(View view, MotionEvent motionEvent)
			{
				if (motionEvent.ActionMasked == MotionEventActions.Down) {
					View shape = f.rootView.FindViewById (Resource.Id.circle);
					/* Create a reveal ValueAnimator that starts clipping the view from
					 * the top left corner until the whole view is covered. */
					Animator animator = ViewAnimationUtils.CreateCircularReveal (shape, 0, 0, 0,
						                         (float)Math.Sqrt (Math.Pow (shape.Width, 2) + Math.Pow (shape.Height, 2)));

					// Set a natural ease-in/ease-out interpolator
					animator.SetInterpolator (new AccelerateDecelerateInterpolator ());

					animator.Start ();
					Log.Info (TAG, "Reveal effect shown");
					return false;
				}
				return false;
			}
		}
	}
}

