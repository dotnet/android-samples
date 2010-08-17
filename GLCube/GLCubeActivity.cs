using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.GLCube
{
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
			SetContentView (R.layout.main);

			// Load the view
			FindViewById (R.id.paintingview);
		}
	}
}
