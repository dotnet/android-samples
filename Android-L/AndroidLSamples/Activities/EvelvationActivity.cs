using Android.App;
using Android.OS;

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
		}
	}
}

