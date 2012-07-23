using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Opengl;

namespace GLNativeES20
{
	// the ConfigurationChanges flags set here keep the EGL context
	// from being destroyed whenever the device is rotated or the
	// keyboard is shown (highly recommended for all GL apps)
	[Activity (Label = "GLNativeES20",
				ConfigurationChanges=ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
				MainLauncher = true)]
	public class GLNativeES20Activity : Activity
	{
		private GLSurfaceView mGLView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create a GLSurfaceView instance and set it
			// as the ContentView for this Activity
			mGLView = new MyGLSurfaceView (this);
			SetContentView (mGLView);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// The following call pauses the rendering thread.
			// If your OpenGL application is memory intensive,
			// you should consider de-allocating objects that
			// consume significant memory here.
			mGLView.OnPause ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// The following call resumes a paused rendering thread.
			// If you de-allocated graphic objects for onPause()
			// this is a good place to re-allocate them.
			mGLView.OnResume ();
		}
	}
}


