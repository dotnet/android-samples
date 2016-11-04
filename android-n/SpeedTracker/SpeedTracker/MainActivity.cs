using Android.App;
using Android.Widget;
using Android.OS;

namespace SpeedTracker
{
	[Activity(Label = "SpeedTracker", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			// SetContentView (Resource.Layout.Main);
		}
	}
}

