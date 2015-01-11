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
using Xamarin.NineOldAndroids.Animations;
using Xamarin.NineOldAndroids.Views;

namespace NineOldAndroidsTest
{
	[Activity (Label = "Toggles")]
	[IntentFilter (new string [] {Intent.ActionMain}, Categories = new string [] { Demos.SampleCategory })]
	public class Toggles : Activity 
	{
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.toggles);

			View target = FindViewById(Resource.Id.target);
			int duration = 2 * 1000;

			FindViewById(Resource.Id.tx).Click += delegate {
					ObjectAnimator.OfFloat(target, "translationX", 0, 50, -50, 0).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.ty).Click += delegate {
					ObjectAnimator.OfFloat(target, "translationY", 0, 50, -50, 0).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.sx).Click += delegate {
					ObjectAnimator.OfFloat(target, "scaleX", 1, 2, 1).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.sy).Click += delegate {
					ObjectAnimator.OfFloat(target, "scaleY", 1, 2, 1).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.a).Click += delegate {
					ObjectAnimator.OfFloat(target, "alpha", 1, 0, 1).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.rx).Click += delegate {
					ObjectAnimator.OfFloat(target, "rotationX", 0, 180, 0).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.ry).Click += delegate {
					ObjectAnimator.OfFloat(target, "rotationY", 0, 180, 0).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.rz).Click += delegate {
					ObjectAnimator.OfFloat(target, "rotation", 0, 180, 0).SetDuration(duration).Start();
			};
			FindViewById(Resource.Id.pivot_zero_zero).Click += delegate {
					ViewHelper.SetPivotX(target, 0);
					ViewHelper.SetPivotY(target, 0);
			};
			FindViewById(Resource.Id.pivot_center).Click += delegate {
					ViewHelper.SetPivotX(target, target.Width / 2f);
					ViewHelper.SetPivotY(target, target.Height / 2f);
			};
			FindViewById(Resource.Id.pivot_width_height).Click += delegate {
					ViewHelper.SetPivotX(target, target.Width);
					ViewHelper.SetPivotY(target, target.Height);
			};
		}
	}
}

