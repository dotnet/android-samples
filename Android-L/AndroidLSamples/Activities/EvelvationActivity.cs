using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views.Animations;
using Android.Views;
using Android.Animation;

namespace AndroidLSamples
{
	[Activity (Label = "Elevation Sample", ParentActivity=typeof(HomeActivity))]			
	public class ElevationActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_elevation);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			var view = FindViewById(Resource.Id.floating_shape_2);

			view.Touch += (sender, e) => {
				e.Handled = true;
				if(e.Event.ActionMasked == MotionEventActions.Up){
					view.Animate().TranslationZ(0);
				}
				else if(e.Event.ActionMasked == MotionEventActions.Down){
					view.Animate().TranslationZ(120);
				}
				else{
					e.Handled = false;
				}
			};
		}
	}
}

