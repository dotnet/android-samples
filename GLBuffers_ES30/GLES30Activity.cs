using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.GLBufferES30
{
	[Activity (Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/app_gltriangle",
#if __ANDROID_11__
		HardwareAccelerated=false,
#endif
		ConfigurationChanges = ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class GLES30Activity : Activity
	{
        GLES30RenderView view;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            view = new GLES30RenderView(this);

            this.SetContentView(view);

		}

        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}
