using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.GLCube
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_glcube",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class GLCubeActivity : Activity
	{
		public GLCubeActivity (IntPtr handle) : base (handle)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			// - should match filename res/layout/main.xml ?
			SetContentView (Resource.layout.main);

			// Load the view
			FindViewById (Resource.id.paintingview);
		}
	}
}
