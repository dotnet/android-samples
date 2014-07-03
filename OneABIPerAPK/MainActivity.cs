using Android.App;
using Android.Widget;
using Android.OS;

namespace HelloWorld
{
	[Activity(Label = "HelloWorld", MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);
			int versionCode = PackageManager.GetPackageInfo(PackageName, 0).VersionCode;
			FindViewById<TextView>(Resource.Id.textView1).Text = string.Format("Version code: {0}", versionCode);

			Button button = FindViewById<Button>(Resource.Id.myButton);
			
			button.Click += delegate
			{
				button.Text = string.Format("{0} clicks!", count++);
			};
		}
	}
}


