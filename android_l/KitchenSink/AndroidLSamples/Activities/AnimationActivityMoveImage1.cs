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
	[Activity (Label = "Move Image", ParentActivity=typeof(HomeActivity))]
	public class AnimationsActivityMoveImage1 : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.Lollipop) {
				//Will request content Transitions with the Move Image Transition
				//This can also be specified in the Style
				Window.RequestFeature (WindowFeatures.ContentTransitions);
				Window.SharedElementEnterTransition = new  ChangeImageTransform();
				Window.SharedElementExitTransition = new ChangeImageTransform();
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
				var intent = new Intent(this, typeof(AnimationsActivityMoveImage2));
				if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.Lollipop) {

					//specify the control to move and the view id
					//this is set with android:viewName="xamarin" 
					var options = ActivityOptions.MakeSceneTransitionAnimation(this, xamarin, "xamarin");
					StartActivity(intent, options.ToBundle());
				}
				else
				{
					StartActivity(intent);
				}
				 
			};

		}
	}
}


