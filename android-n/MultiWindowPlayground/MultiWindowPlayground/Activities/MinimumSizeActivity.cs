using Android.Content.PM;
using Android.App;
using Android.OS;

namespace MultiWindowPlayground
{
	//TODO Use LaunchMode and Layout ActivityAttributes
	//[Activity (LaunchMode = LaunchMode.SingleInstance)]
	[Activity(Name = "com.xamarin.multiwindowplayground.MinimumSizeActivity")]
	public class MinimumSizeActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Create your application here
			SetDescription(Resource.String.activity_minimum_description);
			SetBackgroundColor(Resource.Color.pink);
		}
	}
}

