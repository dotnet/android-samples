using System;

using Android.App;
using Android.OS;

namespace Mono.Samples.HelloWorld
{
	[Activity (Label = "Hello World Demo", MainLauncher = true)]
	public class HelloAndroid : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);
		}
	}
}
