using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.Tetrahedron
{
	[Activity (Label = "@string/app_name", MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class TetrahedronActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (Resource.Layout.main);
		}
	}
}
