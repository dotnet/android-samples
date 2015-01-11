﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Transitions;
namespace AndroidLSamples
{


	[Activity (Label = "Move Image 2", ParentActivity=typeof(HomeActivity))]			
	public class AnimationsActivityMoveImage2 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.Lollipop) {
				//Will request content Transitions with the Move Image Transition
				//This can also be specified in the Style
				//the rest is handled by the system with the shared viewname
				Window.RequestFeature (WindowFeatures.ContentTransitions);
				Window.SharedElementEnterTransition = new ChangeImageTransform ();
				Window.SharedElementExitTransition = new ChangeImageTransform();
			}
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_animations_2);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
		}
	}
}

