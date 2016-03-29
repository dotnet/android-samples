
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MultiWindowPlayground
{
	[Activity(LaunchMode = LaunchMode.SingleInstance, TaskAffinity = "")]
	public class BasicActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Set the color and description
			SetDescription(Resource.String.activity_description_basic);
			SetBackgroundColor(Resource.Color.gray);
		}
	}
}

