using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
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
			GLContextVersion = EAGLRenderingAPI.OpenGLES1;
			rateOfRotationPS = new float [] { 30, 45, 60 };
			rot = new float [] { 0, 0, 0};
		}

		// This gets called when the drawing surface is ready
		protected override void OnLoad (EventArgs e)
		{
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

			// Run the render loop
			Run (30);
		}

		void RenderCube ()
		{
			GL.Enable(All.CullFace);
			GL.MatrixMode (All.Projection);
			GL.LoadIdentity ();
			GL.Ortho (-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);

			GL.MatrixMode (All.Modelview);
			GL.LoadIdentity ();
			GL.Rotate (rot[0], 1.0f, 0.0f, 0.0f);
			GL.Rotate (rot[1], 0.0f, 1.0f, 0.0f);
			GL.Rotate (rot[2], 0.0f, 1.0f, 0.0f);

			GL.ClearColor (0, 0, 0, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			GL.VertexPointer(3, All.Float, 0, cube);
			GL.EnableClientState (All.VertexArray);
			GL.ColorPointer (4, All.Float, 0, cubeColors);
			GL.EnableClientState (All.ColorArray);
			GL.DrawElements(All.Triangles, 36, All.UnsignedByte, triangles);

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
