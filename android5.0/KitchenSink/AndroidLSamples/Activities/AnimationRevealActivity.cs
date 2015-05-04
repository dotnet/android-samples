
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Animation;

namespace AndroidLSamples
{
	[Activity (Label = "Reveal Demo")]			
	public class AnimationRevealActivity : Activity
	{
		View myView;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_reveal);
			// Create your application here

			if ((int)Build.VERSION.SdkInt < 20) {
				Toast.MakeText (this, "This sample is not compatible with anything other than Android L", ToastLength.Long).Show ();
				Finish ();
			}
			myView = FindViewById(Resource.Id.my_view);
			FindViewById<Button>(Resource.Id.animate).Click += (sender, e) => {
				if(myView.Visibility == ViewStates.Visible)
					Hide();
				else
					Reveal();
			};

			FindViewById<Button>(Resource.Id.animate_bottom).Click += (sender, e) => {
				if(myView.Visibility == ViewStates.Visible)
					Hide(true);
				else
					Reveal(true);
			};

		}

		private void Reveal(bool bottom = false)
		{
			myView.Visibility  = ViewStates.Visible;
		
			int cx, cy = 0;
			if (bottom) {
				// get the bottom left
				cx = myView.Left;
				cy = (myView.Top + myView.Bottom);
			}
			else
			{
				// get the center for the clipping circle
			 	cx = (myView.Left + myView.Right) / 2;
				cy = (myView.Top + myView.Bottom) / 2;
			}

			// get the final radius for the clipping circle
			int finalRadius = myView.Width;

			// create and start the animator for this view
			// (the start radius is zero)
			var anim =
				ViewAnimationUtils.CreateCircularReveal(myView, cx, cy, 0, finalRadius);
			anim.Start();
		}

		private void Hide(bool bottom = false)
		{
			int cx, cy = 0;
			if (bottom) {
				// get the bottom left
				cx = myView.Left;
				cy = (myView.Top + myView.Bottom);
			}
			else
			{
				// get the center for the clipping circle
				cx = (myView.Left + myView.Right) / 2;
				cy = (myView.Top + myView.Bottom) / 2;
			}
				

			// get the initial radius for the clipping circle
			int initialRadius = myView.Width;

			// create the animation (the final radius is zero)
			var anim =
				ViewAnimationUtils.CreateCircularReveal(myView, cx, cy, initialRadius, 0);

			// make the view invisible when the animation is done
			anim.AnimationEnd += (sender, e) => {
				myView.Visibility  = ViewStates.Invisible;
			};
		
			// start the animation
			anim.Start();
		}
	}
}

