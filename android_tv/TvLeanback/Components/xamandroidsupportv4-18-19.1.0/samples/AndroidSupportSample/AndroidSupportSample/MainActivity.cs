using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.App;

using Android.Support.V4.App;

namespace com.xamarin.sample.fragments.supportlib
{
	[Activity (Label = "AndroidSupportSample", MainLauncher = true)]
	public class MainActivity : Android.Support.V4.App.FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
		}
	}
}


