using System;
using System.Runtime.InteropServices;
using System.Text;

using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Util;
using Android.Views;
using Android.Content;

// Render a triangle using OpenGLES 2.0

namespace Mono.Samples.GLTriangle20 {

	class PaintingView : AndroidGameView
	{
		int viewportWidth, viewportHeight;
		int program;
		float [] vertices;

		public PaintingView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		// This gets called when the drawing surface is ready
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			// Select OpenGLES 2.0
			// This must be done before the call to CreateFrameBuffer ()
			GLContextVersion = GLContextVersion.Gles2_0;

			CreateFrameBuffer ();
			MakeCurrent ();

			viewportHeight = Height; viewportWidth = Width;

			// Vertex and fragment shaders
			string vertexShaderSrc =  "attribute vec4 vPosition;    \n" + 
							  "void main()                  \n" +
							  "{                            \n" +
							  "   gl_Position = vPosition;  \n" +
							  "}                            \n";

			string fragmentShaderSrc = "precision mediump float;\n" +
		      					   "void main()                                  \n" +
		      					   "{                                            \n" +
		      					   "  gl_FragColor = vec4 (1.0, 0.0, 0.0, 1.0);  \n" +
		      					   "}                                            \n";

			int vertexShader = LoadShader (All.VertexShader, vertexShaderSrc );
			int fragmentShader = LoadShader (All.FragmentShader, fragmentShaderSrc );
			program = GL.CreateProgram();
			if (program == 0)
				throw new InvalidOperationException ("Unable to create program");

			GL.AttachShader (program, vertexShader);
			GL.AttachShader (program, fragmentShader);

			GL.BindAttribLocation (program, 0, "vPosition");
			GL.LinkProgram (program);

			int linked = 0;
			GL.GetProgram (program, All.LinkStatus, ref linked);
			if (linked == 0) {
				// link failed
				int length = 0;
				GL.GetProgram (program, All.InfoLogLength, ref length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetProgramInfoLog (program, length, ref length, log);
					Log.Debug ("GL2", "Couldn't link program: " + log.ToString ());
				}

				GL.DeleteProgram (program);
				throw new InvalidOperationException ("Unable to link program");
			}

			RenderTriangle ();
		}

		int LoadShader (All type, string source)
		{
			int shader = GL.CreateShader (type);
			if (shader == 0)
				throw new InvalidOperationException ("Unable to create shader");

			int length = 0;
			GL.ShaderSource (shader, 1, new string [] {source}, (int[])null);
			GL.CompileShader (shader);

			int compiled = 0;
			GL.GetShader (shader, All.CompileStatus, ref compiled);
			if (compiled == 0) {
				length = 0;
				GL.GetShader (shader, All.InfoLogLength, ref length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetShaderInfoLog (shader, length, ref length, log);
					Log.Debug ("GL2", "Couldn't compile shader: " + log.ToString ());
				}

				GL.DeleteShader (shader);
				throw new InvalidOperationException ("Unable to compile shader of type : " + type.ToString ());
			}

			return shader;
		}

		void RenderTriangle ()
		{
			vertices = new float [] {
					0.0f, 0.5f, 0.0f,
					-0.5f, -0.5f, 0.0f,
					0.5f, -0.5f, 0.0f
				};

			GL.ClearColor (0.7f, 0.7f, 0.7f, 1);
			GL.Clear ((int)All.ColorBufferBit);

			GL.Viewport (0, 0, viewportWidth, viewportHeight);
			GL.UseProgram (program);

			GL.VertexAttribPointer (0, 3, All.Float, false, 0, vertices);
			GL.EnableVertexAttribArray (0);

			GL.DrawArrays (All.Triangles, 0, 3);

			SwapBuffers ();
		}

	}
}
