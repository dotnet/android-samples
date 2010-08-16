using System;

using Android.App;
using Android.OS;

namespace Mono.Samples.HelloWorld
{
	public class HelloAndroid : Activity
	{

		public HelloAndroid (IntPtr handle) : base (handle)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (R.layout.main);
		}
	}
}
