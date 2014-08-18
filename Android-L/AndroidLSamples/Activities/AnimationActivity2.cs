using Android.App;
using Android.OS;
using Android.Views;
using Android.Transitions;
namespace AndroidLSamples
{
	[Activity (Label = "Explode 2", ParentActivity=typeof(HomeActivity))]			
	public class AnimationsActivity2 : Activity
	{
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
			SetContentView (Resource.Layout.activity_animations_2);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
		}
	}
}

