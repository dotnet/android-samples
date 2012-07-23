using System;

using Android.Views;
using Android.Content;
using Android.Util;
using Android.Opengl;

namespace GLNativeES20
{
	class MyGLSurfaceView : GLSurfaceView
	{
		private MyGLRenderer mRenderer;
		private const float TOUCH_SCALE_FACTOR = 180.0f / 320;

		public MyGLSurfaceView (Context context) : base (context)
		{
			// Create an OpenGL ES 2.0 context.
			SetEGLContextClientVersion (2);

			// Set the Renderer for drawing on the GLSurfaceView
			mRenderer = new MyGLRenderer ();
			SetRenderer (mRenderer);

			// Render the view only when there is a change in the drawing data
			this.RenderMode = Rendermode.WhenDirty;
		}
	}
}

