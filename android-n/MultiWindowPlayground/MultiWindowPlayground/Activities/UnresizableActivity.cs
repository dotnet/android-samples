
using Android.App;
using Android.OS;

namespace MultiWindowPlayground
{
	// TODO Use attribute to set ResizeableActivity = false
	[Activity(Name = "com.xamarin.multiwindowplayground.UnresizableActivity")]
	public class UnresizableActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Create your application here
			SetDescription(Resource.String.activity_description_unresizable);
			SetBackgroundColor(Resource.Color.purple);
		}
	}
}

