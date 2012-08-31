using Android.App;
using Android.OS;
using Android.Support.V4.App;

namespace com.xamarin.sample.fragments.supportlib
{
	[Activity(Label = "Fragments Walkthrough", MainLauncher = true, Icon = "@drawable/launcher")]
	public class MainActivity : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
		}
	}
}