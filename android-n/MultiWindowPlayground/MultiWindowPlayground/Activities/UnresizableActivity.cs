
using Android.App;
using Android.OS;

namespace MultiWindowPlayground
{
	[Activity(ResizeableActivity = false, TaskAffinity = "")]
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

