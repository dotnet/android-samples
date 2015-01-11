using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.TexturedCube
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_texturedcube",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class TexturedCubeActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (Resource.Layout.main);
		}
	}
}
