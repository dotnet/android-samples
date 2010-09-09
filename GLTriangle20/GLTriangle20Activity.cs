using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.GLTriangle20
{
	public class GLTriangle20Activity : Activity
	{
		public GLTriangle20Activity (IntPtr handle) : base (handle)
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
