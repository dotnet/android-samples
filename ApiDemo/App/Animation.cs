using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "@string/activity_animation")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class AnimationDemo : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.activity_animation);

			var button = FindViewById <Button> (Resource.Id.fade_animation);
			button.Click += OnFadeClicked;
			button = FindViewById <Button> (Resource.Id.zoom_animation);
			button.Click += OnZoomClicked;
		}

		void OnFadeClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof (Controls1)));
			OverridePendingTransition(Resource.Animation.fade, Resource.Animation.hold);
		}

		void OnZoomClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof (Controls1)));
			OverridePendingTransition(Resource.Animation.zoom_enter, Resource.Animation.zoom_exit);
		}
	}
}

