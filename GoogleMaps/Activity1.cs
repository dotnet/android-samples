using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace GoogleMaps
{
	[Activity(Label = "Managed Maps", MainLauncher = true)]
	public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.MyButton);
			
			button.Click += delegate {
				StartActivity (new Intent (this, Java.Lang.Class.ForName ("mono.samples.googlemaps.MyMapActivity")));
			};
		}
	}
}


