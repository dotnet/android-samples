using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Larvalabs.Svgandroid;

namespace SvgAndroidSample
{
	[Activity (Label = "SvgAndroidSample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			ImageView imageView = new ImageView (this);
			// Set the background color to white
			imageView.SetBackgroundColor (Android.Graphics.Color.White);
			// Parse the SVG file from the resource
			SVG svg = SVGParser.GetSVGFromAsset (Assets, "svg/gradients.svg");
			// Get a drawable from the parsed SVG and set it as the drawable for the ImageView
			imageView.SetImageDrawable (svg.CreatePictureDrawable ());
			// Set the ImageView as the content view for the Activity
			SetContentView (imageView);
		}
	}
}


