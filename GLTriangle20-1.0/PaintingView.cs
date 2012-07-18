using System;
using System.Runtime.InteropServices;
using System.Text;

using OpenTK;
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
		int mProgramHandle;
		int mColorHandle;
		int mPositionHandle;
		int mMVPMatrixHandle;
		float [] vertices;

		Matrix4 mProjectionMatrix;
		Matrix4 mViewMatrix;
		Matrix4 mModelViewProjectionMatrix;

		// Set color with red, green, blue and alpha (opacity) values
	    float [] color;

		public PaintingView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Init ();
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			Init ();
		}

		void Init ()
		{
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		protected override void CreateFrameBuffer ()
		{
			ContextRenderingApi = GLVersion.ES2;

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("GLTriangle", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLTriangle", "{0}", ex);
			}

			// this is a graphics setting that sets everything to the lowest mode possible so
			// the device returns a reliable graphics setting.
			try {
				Log.Verbose ("GLTriangle", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (0, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLTriangle", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}

		// This gets called when the drawing surface has been created
		// There is already a GraphicsContext and Surface at this point,
		// following the standard OpenTK/GameWindow logic
		//
		// Android will only render when it refreshes the surface for
		// the first time, so if you don't call Run, you need to hook
		// up the Resize delegate or override the OnResize event to
		// get the updated bounds and re-call your rendering code.
		// This will also allow non-Run-loop code to update the screen
		// when the device is rotated.
		protected override void OnLoad (EventArgs e)
		{
			// This is completely optional and only needed
			// if you've registered delegates for OnLoad
			base.OnLoad (e);

			viewportHeight = Height; viewportWidth = Width;

			// Set our triangle's vertices
			vertices = new float [] {
					0.0f, 0.5f, 0.0f,
					-0.5f, -0.5f, 0.0f,
					0.5f, -0.5f, 0.0f
				};

			// Set color with red, green, blue and alpha (opacity) values
	    	color = new float [] { 0.63671875f, 0.76953125f, 0.22265625f, 1.0f };

			// Vertex and fragment shaders
			string vertexShaderSrc = "uniform mat4 uMVPMatrix;   \n" +
							  "attribute vec4 vPosition;    \n" +
							  "void main()                  \n" +
							  "{                            \n" +
							  "   gl_Position = vPosition;  \n" +
							  "}                            \n";

			string fragmentShaderSrc = "precision mediump float;             \n" +
							   "uniform vec4 vColor;                         \n" +
							   "void main()                                  \n" +
							   "{                                            \n" +
							   "  gl_FragColor = vColor;  \n" +
							   "}                                            \n";

			int vertexShader = LoadShader (All.VertexShader, vertexShaderSrc );
			int fragmentShader = LoadShader (All.FragmentShader, fragmentShaderSrc );
			mProgramHandle = GL.CreateProgram();
			if (mProgramHandle == 0)
				throw new InvalidOperationException ("Unable to create program");

			GL.AttachShader (mProgramHandle, vertexShader);
			GL.AttachShader (mProgramHandle, fragmentShader);

			GL.BindAttribLocation (mProgramHandle, 0, "vPosition");
			GL.LinkProgram (mProgramHandle);

			int linked;
			GL.GetProgram (mProgramHandle, All.LinkStatus, out linked);
			if (linked == 0) {
				// link failed
				int length;
				GL.GetProgram (mProgramHandle, All.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetProgramInfoLog (mProgramHandle, length, out length, log);
					Log.Debug ("GL2", "Couldn't link program: " + log.ToString ());
				}

				GL.DeleteProgram (mProgramHandle);
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
			GL.GetShader (shader, All.CompileStatus, out compiled);
			if (compiled == 0) {
				length = 0;
				GL.GetShader (shader, All.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetShaderInfoLog (shader, length, out length, log);
					Log.Debug ("GL2", "Couldn't compile shader: " + log.ToString ());
				}

				GL.DeleteShader (shader);
				throw new InvalidOperationException ("Unable to compile shader of type : " + type.ToString ());
			}

			return shader;
		}

		void RenderTriangle ()
		{
			GL.ClearColor (0.7f, 0.7f, 0.7f, 1);
			GL.Clear (ClearBufferMask.ColorBufferBit);


			// Set the camera position (View matrix)
		    mViewMatrix = Matrix4.LookAt(0, 0, -3, 0f, 0f, 0f, 0f, 1.0f, 0.0f);

		    // Calculate the projection and view transformation
		    mModelViewProjectionMatrix = Matrix4.Mult(mProjectionMatrix, mViewMatrix);

			GL.UseProgram (mProgramHandle);

			// get handle to vertex shader's vPosition member
		    mPositionHandle = GL.GetAttribLocation(mProgramHandle, new StringBuilder("vPosition"));

		    // Enable a handle to the triangle vertices
			GL.EnableVertexAttribArray (mPositionHandle);

			// Prepare the triangle coordinate data
			GL.VertexAttribPointer (0, 3, All.Float, false, 0, vertices);

			// get handle to fragment shader's vColor member
		    mColorHandle = GL.GetUniformLocation(mProgramHandle, new StringBuilder("vColor"));

		    // Set color for drawing the triangle
	        GL.Uniform4(mColorHandle, 1, color);

			// get handle to shape's transformation matrix
		    mMVPMatrixHandle = GL.GetUniformLocation(mProgramHandle, new StringBuilder("uMVPMatrix"));

		    // Apply the projection and view transformation
		    GL.UniformMatrix4(mMVPMatrixHandle, false, ref mModelViewProjectionMatrix);

			GL.DrawArrays (All.Triangles, 0, 3);

			// Disable vertex array
        	GL.DisableVertexAttribArray(mPositionHandle);

			SwapBuffers ();
		}

		// this is called whenever android raises the SurfaceChanged event
		protected override void OnResize (EventArgs e)
		{
			viewportHeight = Height;
			viewportWidth = Width;

			// the surface change event makes your context
			// not be current, so be sure to make it current again
			MakeCurrent ();

			// Adjust the viewport based on geometry changes,
	        // such as screen rotation
	        GL.Viewport(0, 0, viewportWidth, viewportHeight);

	        float ratio = (float) viewportWidth / viewportHeight;

	        // this projection matrix is applied to object coordinates
	        // in the onDrawFrame() method
	        mProjectionMatrix = OpenTK.Matrix4.Frustum(-ratio, ratio, -1, 1, 3, 7);

			RenderTriangle ();
		}
	}
}
