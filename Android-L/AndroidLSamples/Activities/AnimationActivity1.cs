using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Transitions;

namespace AndroidLSamples
{
	[Activity (Label = "Explode", ParentActivity=typeof(HomeActivity))]
	public class AnimationsActivity1 : Activity
	{
	
		int count;
		protected override void OnCreate (Bundle bundle)
		{
			if ((int)Build.VERSION.SdkInt >= 20) {
				//Will request content Transitions with the Explode Transition
				//This can also be specified in the Style
				Window.RequestFeature (WindowFeatures.ContentTransitions);
				Window.EnterTransition = new Explode ();
				Window.ExitTransition = new Explode ();
				Window.AllowExitTransitionOverlap = true;
				Window.AllowEnterTransitionOverlap = true;
			}
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_animations_1);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			var xamarin = FindViewById<ImageView> (Resource.Id.xamarin);
			xamarin.Click += (sender, e) => 
			{
				var intent = new Intent(this, typeof(AnimationsActivity2));
				StartActivity(intent);
			};

		

		}
	}
}


