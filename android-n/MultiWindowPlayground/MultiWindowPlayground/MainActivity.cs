
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using CommonSampleLibrary;
using Java.Interop;

namespace MultiWindowPlayground
{
	[Activity(Label = "MultiWindow Playground", MainLauncher = true, TaskAffinity = "")]
	public class MainActivity : LoggingActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			var multiDisabledMessage = FindViewById(Resource.Id.warning_multiwindow_disabled);
			// Display an additional message if the app is not in multiwindow mode.

			if (!IsInMultiWindowMode)
			{
				multiDisabledMessage.Visibility = ViewStates.Visible;
			}
			else {
				multiDisabledMessage.Visibility = ViewStates.Gone;
			}
		}

		[Export]
		public void OnStartUnresizableClick(View view)
		{
			Log.Debug(logTag, "** starting UnresizableActivity");

			// This activity is marked as 'unresizable' in the AndroidManifest. We need to specify the
			// FLAG_ACTIVITY_NEW_TASK flag here to launch it into a new task stack, otherwise the
			// properties from the root activity would have been inherited (which was here marked as
			// resizable by default).

			var intent = new Intent(this, typeof(UnresizableActivity));
			intent.AddFlags(ActivityFlags.NewTask);

			StartActivity(intent);
		}

		[Export]
		public void OnStartMinimumSizeActivity(View view)
		{
			Log.Debug(logTag, "** starting MinimumSizeActivity");

			StartActivity(new Intent(this, typeof(MinimumSizeActivity)));
		}

		[Export]
		public void OnStartAdjacentActivity(View view)
		{
			Log.Debug(logTag, "** starting AdjacentActivity");

			// Start this activity adjacent to the focused activity (ie. this activity) if possible.
			// Note that this flag is just a hint to the system and may be ignored. For example,
			// if the activity is launched within the same task, it will be launched on top of the
			// previous activity that started the Intent. That's why the Intent.FLAG_ACTIVITY_NEW_TASK
			// flag is specified here in the intent - this will start the activity in a new task.

			// TODO Add the Intent.FLAG_ACTIVITY_LAUNCH_ADJACENT flag
			var intent = new Intent(this, typeof(AdjacentActivity));
			intent.AddFlags((ActivityFlags)ActivityFlags.LaunchAdjacent | ActivityFlags.NewTask);

			StartActivity(intent);
		}

		[Export]
		public void OnStartLaunchBoundsActivity(View view)
		{
			Log.Debug(logTag, "** starting LaunchBoundsActivity");

			// Define the bounds in which the Activity will be launched into.
			var bounds = new Rect(500, 300, 100, 0);

			// Set the bounds as an activity option.
			ActivityOptions options = ActivityOptions.MakeBasic();
			options.SetLaunchBounds(bounds);

			// Start the LaunchBoundsActivity with the specified options
			var intent = new Intent(this, typeof(LaunchBoundsActivity));
			StartActivity(intent, options.ToBundle());

		}

		[Export]
		public void OnStartBasicActivity(View view)
		{
			Log.Debug(logTag, "** starting BasicActivity");

			// Start an Activity with the default options in the 'singleTask' launch mode as defined
			// by the ActivityAttribute.
			StartActivity(new Intent(this, typeof(BasicActivity)));
		}

		[Export]
		public void OnStartCustomConfigurationActivity(View view)
		{
			Log.Debug(logTag, "** starting CustomConfigurationChangeActivity");

			// Start an Activity that handles all configuration changes itself.
			StartActivity(new Intent(this, typeof(CustomConfigurationChangeActivity)));
		}
	}
}
