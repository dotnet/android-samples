using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace Mono.Samples.HelloWorld
{
	[Activity (Label = "Hello World Demo", MainLauncher = true)]
	[IntentFilter(new[] { Intent.ActionView },
		Categories = new[] {
			Intent.CategoryDefault,
			Intent.CategoryBrowsable
		},
		DataScheme = "http",
		DataHost = "example.com"
	)]
	public class HelloAndroid : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);
		}
	}
}
