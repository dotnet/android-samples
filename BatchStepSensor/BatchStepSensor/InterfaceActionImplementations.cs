using System;
using Android.Animation;
using Android.Views;
using BatchStepSensor.CardStream;
using Android.Hardware;

namespace BatchStepSensor
{
	public class AnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
	{
		public Action<Animator> OnAnimationCancelAction, OnAnimationEndAction, OnAnimationRepeatAction, OnAnimationStartAction;
		public void OnAnimationCancel (Animator animation)
		{
			if (OnAnimationCancelAction != null) {
				OnAnimationCancelAction (animation);
			}
		}

		public void OnAnimationEnd (Animator animation)
		{
			if (OnAnimationEndAction != null) {
				OnAnimationEndAction (animation);
			}
		}

		public void OnAnimationRepeat (Animator animation)
		{
			if (OnAnimationRepeatAction != null) {
				OnAnimationRepeatAction (animation);
			}
		}

		public void OnAnimationStart (Animator animation)
		{
			if (OnAnimationStartAction != null) {
				OnAnimationStartAction (animation);
			}
		}
	}

	public class OnClickListener : Java.Lang.Object, View.IOnClickListener
	{
		public Action<View> OnClickAction;
		public void OnClick (View v)
		{
			if (OnClickAction != null) {
				OnClickAction (v);
			}
		}
	}

	public class TransitionListener : Java.Lang.Object, LayoutTransition.ITransitionListener
	{
		public Action<LayoutTransition, ViewGroup, View, LayoutTransitionType> EndTransitionAction, StartTransitionAction;
		public void EndTransition (LayoutTransition transition, ViewGroup container, View view, LayoutTransitionType transitionType)
		{
			if (EndTransitionAction != null) {
				EndTransitionAction (transition, container, view, transitionType);
			}
		}

		public void StartTransition (LayoutTransition transition, ViewGroup container, View view, LayoutTransitionType transitionType)
		{
			if (StartTransitionAction != null) {
				StartTransitionAction (transition, container, view, transitionType);
			}
		}
	}

	public class OnHierarchyChangeListener : Java.Lang.Object, ViewGroup.IOnHierarchyChangeListener
	{
		public Action<View, View> OnChildViewAddedAction, OnChildViewRemovedAction;
		public void OnChildViewAdded (View parent, View child)
		{
			if (OnChildViewAddedAction != null) {
				OnChildViewAddedAction (parent, child);
			}
		}

		public void OnChildViewRemoved (View parent, View child)
		{
			if (OnChildViewRemovedAction != null) {
				OnChildViewRemovedAction (parent, child);
			}
		}
	}

	public class OnDismissListener : Java.Lang.Object, CardStreamLinearLayout.OnDismissListener
	{
		public Action<string> OnDismissAction;
		public void OnDismiss (string tag)
		{
			if (OnDismissAction != null) {
				OnDismissAction (tag);
			}
		}
	}

	public class SensorEventListener : Java.Lang.Object, ISensorEventListener
	{
		public Action<Sensor, SensorStatus> OnAccuracyChangedAction;
		public Action<SensorEvent> OnSensorChangedAction;
		public void OnAccuracyChanged (Sensor sensor, SensorStatus accuracy)
		{
			if (OnAccuracyChangedAction != null)
				OnAccuracyChangedAction (sensor, accuracy);
		}

		public void OnSensorChanged (SensorEvent e)
		{
			if (OnSensorChangedAction != null)
				OnSensorChangedAction (e);
		}
	}
}

