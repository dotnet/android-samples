using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace DirectBoot
{
	[Activity (Label = "DirectBoot", MainLauncher = true, Theme="@style/Theme.AppCompat.Light")]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			if (savedInstanceState == null)
				SupportFragmentManager.BeginTransaction ()
									  .Add (Resource.Id.container, SchedulerFragment.NewInstance ())
									  .Commit ();
		}
	}
}


