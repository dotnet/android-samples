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
using Xamarin.NineOldAndroids.Views;

using ViewPropertyAnimator = Xamarin.NineOldAndroids.Views.ViewPropertyAnimator;

namespace NineOldAndroidsTest
{
	[Activity (Label = "VPADemo")]
	[IntentFilter (new string [] {Intent.ActionMain}, Categories = new string [] { Demos.SampleCategory })]
	public class VPADemo : Activity {

		static ViewPropertyAnimator Animate (View view)
		{
			return ViewPropertyAnimator.Animate (view);
		}

		protected override void OnCreate(Bundle savedInstanceState) 
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.vpademo);

			LinearLayout container = (LinearLayout) FindViewById(Resource.Id.container);

			Button fadeOut = (Button) FindViewById(Resource.Id.fadeOut);
			Button fadeIn = (Button) FindViewById(Resource.Id.fadeIn);
			Button moveOver = (Button) FindViewById(Resource.Id.moveOver);
			Button moveBack = (Button) FindViewById(Resource.Id.moveBack);
			Button rotate = (Button) FindViewById(Resource.Id.rotate);
			Button animatingButton = (Button) FindViewById(Resource.Id.animatingButton);

			// Set long default duration for the animator, for the purposes of this demo
			Animate(animatingButton).SetDuration(2000);

			fadeOut.Click += delegate {
					Animate(animatingButton).Alpha(0);
			};

			fadeIn.Click += delegate {
					Animate(animatingButton).Alpha(1);
			};

			moveOver.Click += delegate {
					int xValue = container.Width - animatingButton.Width;
					int yValue = container.Height - animatingButton.Height;
					Animate(animatingButton).X(xValue).Y(yValue);
			};

			moveBack.Click += delegate {
					Animate(animatingButton).X(0).Y(0);
			};

			rotate.Click += delegate {
					Animate(animatingButton).RotationYBy(720);
			};
		}
	}
}