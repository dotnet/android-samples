using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Camera2Basic
{
	[Activity (Label = "Camera2Basic", MainLauncher = true, Icon = "@drawable/icon")]
	public class CameraActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ActionBar.Hide ();
			SetContentView (Resource.Layout.activity_camera);

			if (bundle == null) {
				FragmentManager.BeginTransaction ().Replace (Resource.Id.container, Camera2BasicFragment.NewInstance ()).Commit ();
			}
		}
	}
}


