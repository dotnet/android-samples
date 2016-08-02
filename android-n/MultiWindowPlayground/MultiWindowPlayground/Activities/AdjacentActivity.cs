
using Android.App;
using Android.OS;

namespace MultiWindowPlayground
{
	[Activity(TaskAffinity = "")]
	public class AdjacentActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Create your application here
			SetDescription(Resource.String.activity_adjacent_description);
			SetBackgroundColor(Resource.Color.teal);
		}
	}
}

