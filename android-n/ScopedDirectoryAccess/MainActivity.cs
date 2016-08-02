using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace ScopedDirectoryAccess
{
	[Activity (Label = "ScopedDirectoryAccess", MainLauncher = true, Theme= "@style/Theme.AppCompat.Light")]
	public class MainActivity : AppCompatActivity
	{
		readonly string FRAGMENT_TAG = typeof(ScopedDirectoryAccessFragment).Name;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			if (savedInstanceState == null)
				FragmentManager.BeginTransaction ()
				               .Add (Resource.Id.container, ScopedDirectoryAccessFragment.NewInstance (), FRAGMENT_TAG)
				               .Commit ();
		}
	}
}

