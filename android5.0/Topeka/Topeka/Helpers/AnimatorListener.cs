using System;
using Android.Animation;
using Android.Content;

namespace Topeka.Helpers
{
	class AnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
	{
		public Context Context { get; set; }

		public EventHandler<AnimatorEventArgs> AnimationCancel { get; set; }

		public EventHandler<AnimatorEventArgs> AnimationEnd { get; set; }

		public EventHandler<AnimatorEventArgs> AnimationRepeat { get; set; }

		public EventHandler<AnimatorEventArgs> AnimationStart { get; set; }

		public AnimatorListener (Context context)
		{
			Context = context;
		}

		public void OnAnimationCancel (Animator animation)
		{
			AnimationCancel?.Invoke (Context, new AnimatorEventArgs (animation));
		}

		public void OnAnimationEnd (Animator animation)
		{
			AnimationEnd?.Invoke (Context, new AnimatorEventArgs (animation));
		}

		public void OnAnimationRepeat (Animator animation)
		{
			AnimationRepeat?.Invoke (Context, new AnimatorEventArgs (animation));
		}

		public void OnAnimationStart (Animator animation)
		{
			AnimationStart?.Invoke (Context, new AnimatorEventArgs (animation));
		}
	}

	class AnimatorEventArgs : EventArgs
	{
		public Animator Animator { get; set; }

		public AnimatorEventArgs (Animator animation)
		{
			Animator = animation;
		}
	}
}