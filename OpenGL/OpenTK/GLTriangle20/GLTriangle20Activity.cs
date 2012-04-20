using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.GLTriangle20
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_gltriangle",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class GLTriangle20Activity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			// - should match filename res/layout/main.xml ?
			SetContentView (Resource.Layout.main);

			// Load the view
			FindViewById (Resource.Id.paintingview);
		}
	}
}
