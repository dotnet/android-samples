using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.TexturedCube
{
	public class TexturedCubeActivity : Activity
	{
		public TexturedCubeActivity (IntPtr handle) : base (handle)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (R.layout.main);

			PaintingView glp = (PaintingView)FindViewById (R.id.paintingview);
		}
	}
}
