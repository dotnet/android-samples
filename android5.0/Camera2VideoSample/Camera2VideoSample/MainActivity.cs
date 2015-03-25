using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Camera2VideoSample
{
	[Activity (Label = "Camera2VideoSample", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme="@style/AppTheme")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_camera);
			if (null == bundle) {
				FragmentManager.BeginTransaction ()
					.Replace (Resource.Id.container, Camera2VideoFragment.newInstance ())
					.Commit ();
			}

		}
	}
}


