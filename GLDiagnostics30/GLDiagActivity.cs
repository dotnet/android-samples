using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.GLDiag30
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_gldiag",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class GLDiagActivity : Activity
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

		protected override void OnPause ()
		{
			base.OnPause ();
			var view = FindViewById<PaintingView> (Resource.Id.paintingview);
			view.Pause ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			var view = FindViewById<PaintingView> (Resource.Id.paintingview);
			view.Resume ();
		}
	}
}
