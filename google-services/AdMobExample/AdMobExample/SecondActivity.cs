using System;
using Android.Support.V7.App;
using Android.App;
using Android;

namespace AdMobExample
{
	[Activity(Label="@string/second_activity_title", ParentActivity=typeof(MainActivity))]
	public class SecondActivity : AppCompatActivity
	{
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.second_activity);
		}
	}
}

