
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MultiWindowPlayground
{
	[Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.Orientation,
	          LaunchMode = LaunchMode.SingleInstance, TaskAffinity = "")]
	public class CustomConfigurationChangeActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_logging);

			// Create your application here
			SetDescription(Resource.String.activity_custom_description);
			SetBackgroundColor(Resource.Color.cyan);
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);

			// Note: The implementation in LoggingActivity logs the output to the new configuration.
			// This callback is received whenever the configuration is updated, for example when the
			// size of this Activity is changed.
		}
	}
}

