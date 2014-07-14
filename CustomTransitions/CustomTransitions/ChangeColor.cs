using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Transitions;
using Android.Animation;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace CustomTransitions
{
	public class ChangeColor : Transition
	{
		private const string PROPNAME_BACKGROUND = "customtransition:change_color:background";

		public ChangeColor() : base()
		{
		}

		public ChangeColor(IntPtr pointer, JniHandleOwnership transfer) : base(pointer,transfer)
		{
		}

		//Capture the property values of views for later use
		private void CaptureValues(TransitionValues values) 
		{

			values.Values [PROPNAME_BACKGROUND] = values.View.Background;
		}

		public override void CaptureStartValues (TransitionValues transitionValues)
		{
			CaptureValues (transitionValues);
		}

		//Capture the value of the background drawable property in the ending Scene
		public override void CaptureEndValues (TransitionValues transitionValues)
		{
			CaptureValues (transitionValues);
		}

		//Create an animation for each target that is in both the starting and ending Scene that interpolates between the start and end color. 
		public override Animator CreateAnimator (ViewGroup sceneRoot, TransitionValues startValues, TransitionValues endValues)
		{
			//Check if the targets are in both the starting and ending scenes
			if (null == startValues || null == endValues) {
				return null;
			}

			View view = endValues.View;

			//store the starting and ending background properties
			Drawable startBackground = (Drawable)startValues.Values [PROPNAME_BACKGROUND];
			Drawable endBackground = (Drawable)endValues.Values [PROPNAME_BACKGROUND];

			//Ignore a target that isn't a drawable
			if (startBackground is ColorDrawable && endBackground is ColorDrawable) {
				var startColor = (ColorDrawable)startBackground;
				var endColor = (ColorDrawable)endBackground;
				int i = Color.Rgb (startColor.Color.R, startColor.Color.G, startColor.Color.B);
				int u = Color.Rgb (endColor.Color.R, endColor.Color.G, endColor.Color.B);

				//make an animation if the starting and ending colors are different
				if (startColor.Color != endColor.Color) {

					//make an animator to apply to the targets to create the animation
					ValueAnimator animator = ValueAnimator.OfObject (new ArgbEvaluator (), i, u);
					animator.Update+= (object sender, ValueAnimator.AnimatorUpdateEventArgs e) => 
					{
						var value = (int)animator.AnimatedValue;
						if(!value.Equals(null)){
							view.SetBackgroundColor(new Color(value));
						}
					};

					return animator;
				}
			}
			return null;

		}

	}
}

