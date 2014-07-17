using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES10;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Util;
using Android.Content;

namespace Mono.Samples.GLCube {

	class PaintingView : AndroidGameView
	{
		float [] rot;
		float [] rateOfRotationPS;//degrees
		int viewportWidth, viewportHeight;

		public PaintingView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			rateOfRotationPS = new float [] { 30, 45, 60 };
			rot = new float [] { 0, 0, 0};
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		protected override void CreateFrameBuffer ()
		{
			ContextRenderingApi = GLVersion.ES1;

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("GLCube", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLCube", "{0}", ex);
			}

			// this is a graphics setting that sets everything to the lowest mode possible so
			// the device returns a reliable graphics setting.
			try {
				Log.Verbose ("GLCube", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (0, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLCube", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}


		// This gets called when the drawing surface is ready
		protected override void OnLoad (EventArgs e)
		{
			// this call is optional, and meant to raise delegates
			// in case any are registered
			base.OnLoad (e);

			// UpdateFrame and RenderFrame are called
			// by the render loop. This is takes effect
			// when we use 'Run ()', like below
			UpdateFrame += delegate (object sender, FrameEventArgs args) {
				// Rotate at a constant speed
				for (int i = 0; i < 3; i ++)
					rot [i] += (float) (rateOfRotationPS [i] * args.Time);
			};

			RenderFrame += delegate {
				RenderCube ();
			};
			
			GL.Enable(All.CullFace);
			GL.ShadeModel(All.Smooth);
			
			GL.Hint(All.PerspectiveCorrectionHint, All.Nicest);

			// Run the render loop
			Run (30);
		}

		// this occurs mostly on rotation.
		protected override void OnResize (EventArgs e)
		{
			viewportWidth = Width;
			viewportHeight = Height;
		}

		void RenderCube ()
		{
			GL.Viewport(0, 0, viewportWidth, viewportHeight);
			
			GL.MatrixMode (All.Projection);
			GL.LoadIdentity ();		
			
			if ( viewportWidth > viewportHeight )
			{
				GL.Ortho(-1.5f, 1.5f, 1.0f, -1.0f, -1.0f, 1.0f);
			}
			else
			{
				GL.Ortho(-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
			}

			GL.MatrixMode (All.Modelview);
			GL.LoadIdentity ();
			GL.Rotate (rot[0], 1.0f, 0.0f, 0.0f);
			GL.Rotate (rot[1], 0.0f, 1.0f, 0.0f);
			GL.Rotate (rot[2], 0.0f, 1.0f, 0.0f);

			GL.ClearColor (0, 0, 0, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			// pin the data, so that GC doesn't move them, while used
			// by native code
			unsafe {
				fixed (float* pcube = cube, pcubeColors = cubeColors) {
					fixed (byte* ptriangles = triangles) {
						GL.VertexPointer(3, All.Float, 0, new IntPtr (pcube));
						GL.EnableClientState (All.VertexArray);
						GL.ColorPointer (4, All.Float, 0, new IntPtr (pcubeColors));
						GL.EnableClientState (All.ColorArray);
						GL.DrawElements(All.Triangles, 36, All.UnsignedByte, new IntPtr (ptriangles));
					}
				}
			}

			SwapBuffers ();
		}

		float[] cube = {
			-0.5f, 0.5f, 0.5f, // vertex[0]
			0.5f, 0.5f, 0.5f, // vertex[1]
			0.5f, -0.5f, 0.5f, // vertex[2]
			-0.5f, -0.5f, 0.5f, // vertex[3]
			-0.5f, 0.5f, -0.5f, // vertex[4]
			0.5f, 0.5f, -0.5f, // vertex[5]
			0.5f, -0.5f, -0.5f, // vertex[6]
			-0.5f, -0.5f, -0.5f, // vertex[7]
		};

		byte[] triangles = {
			1, 0, 2, // front
			3, 2, 0,
			6, 4, 5, // back
			4, 6, 7,
			4, 7, 0, // left
			7, 3, 0,
			1, 2, 5, //right
			2, 6, 5,
			0, 1, 5, // top
			0, 5, 4,
			2, 3, 6, // bottom
			3, 7, 6,
		};

		float[] cubeColors = {
			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,
			0.0f, 1.0f, 1.0f, 1.0f,
			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,
			0.0f, 1.0f, 1.0f, 1.0f,
		};
	}
}
