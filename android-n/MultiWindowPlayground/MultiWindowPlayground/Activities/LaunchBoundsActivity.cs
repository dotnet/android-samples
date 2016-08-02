
using Android.App;
using Android.OS;

namespace MultiWindowPlayground
{
	[Activity(TaskAffinity = "")]
	public class LaunchBoundsActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Create your application here
			SetDescription(Resource.String.activity_bounds_description);
			SetBackgroundColor(Resource.Color.lime);
		}
	}
}

