using System;
using System.Drawing;
using System.Runtime.InteropServices;

using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using OpenTK;

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace Mono.Samples.TexturedCube {

	class PaintingView : AndroidGameView
	{
		float prevx, prevy;
		float downx, downy;
		float xangle, yangle;
		bool touch = false;
		int textureId;
		Context context;

		const int UNIFORM_PROJECTION = 0;
		const int UNIFORM_TEXTURE = 1;
		const int UNIFORM_TEX_DEPTH = 2;
		const int UNIFORM_LIGHT = 3;
		const int UNIFORM_VIEW = 4;
		const int UNIFORM_NORMAL_MATRIX = 5;
		const int UNIFORM_COUNT = 6;
		int[] uniforms = new int [UNIFORM_COUNT];
		const int ATTRIB_VERTEX = 0;
		const int ATTRIB_NORMAL = 1;
		const int ATTRIB_TEXCOORD = 2;
		const int ATTRIB_COUNT = 3;
		int vbo, vbi;
		bool UseTexture = false;

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
			context = Context;
			xangle = 45;
			yangle = 45;

			Resize += delegate {
				SetupProjection ();
				RenderCube ();
			};
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		protected override void CreateFrameBuffer ()
		{
			ContextRenderingApi = GLVersion.ES3;

			try {
				Log.Verbose ("TexturedCube", "Loading with high quality settings");

				GraphicsMode = new GraphicsMode (new ColorFormat (32), 24, 0, 0); 
				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("TexturedCube", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// Fallback modes
			// If the first attempt at initializing the surface with a default graphics
			// mode fails, then the app can try different configurations. Devices will
			// support different modes, and what is valid for one might not be valid for
			// another. If all options fail, you can set all values to 0, which will
			// ask for the first available configuration the device has without any
			// filtering.
			// After a successful call to base.CreateFrameBuffer(), the GraphicsMode
			// object will have its values filled with the actual values that the
			// device returned.


			// This is a setting that asks for any available 16-bit color mode with no
			// other filters. It passes 0 to the buffers parameter, which is an invalid
			// setting in the default OpenTK implementation but is valid in some
			// Android implementations, so the AndroidGraphicsMode object allows it.
			try {
				Log.Verbose ("TexturedCube", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (16, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// this is a setting that doesn't specify any color values. Certain devices
			// return invalid graphics modes when any color level is requested, and in
			// those cases, the only way to get a valid mode is to not specify anything,
			// even requesting a default value of 0 would return an invalid mode.
			try {
				Log.Verbose ("TexturedCube", "Loading with no Android settings");
				GraphicsMode = new AndroidGraphicsMode (0, 4, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}

		protected override void OnLoad (EventArgs e)
		{
			GL.ClearColor (0, 0, 0, 1);

			GL.ClearDepth (1.0f);
			GL.Enable (EnableCap.DepthTest);
			GL.DepthFunc (DepthFunction.Lequal);
			GL.Enable (EnableCap.CullFace);
			GL.CullFace (CullFaceMode.Back);

			textureId = GL.GenTexture ();
			LoadTexture (context, Resource.Drawable.texture1, textureId);
			LoadShaders (ShaderSource ("vsh"), ShaderSource ("fsh"), out programTexture);
			LoadShaders (ShaderSource ("vsh"), ShaderSource ("fsh", "Plain"), out programPlain);
			ToggleTexture ();
			SetupProjection ();
			InitModel ();
			RenderCube ();
		}

		string ShaderSource (string extension, string suffix = "")
		{
			return new System.IO.StreamReader (Context.Assets.Open (String.Format ("Resources/Shader{0}.{1}", suffix, extension))).ReadToEnd ();
		}

		internal void InitModel ()
		{
			GL.GenBuffers (1, out vbo);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(CubeModel.vertices.Length * sizeof(float)), CubeModel.vertices, BufferUsage.StaticDraw);
			GL.BindBuffer (BufferTarget.ArrayBuffer, 0);

			GL.GenBuffers (1, out vbi);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vbi);
			GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr)(CubeModel.faceIndexes.Length * sizeof(ushort)), CubeModel.faceIndexes, BufferUsage.StaticDraw);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			base.OnTouchEvent (e);
			if (e.Action == MotionEventActions.Down) {
				downx = prevx = e.GetX ();
				downy = prevy = e.GetY ();
				touch = true;
			}
			if (e.Action == MotionEventActions.Move) {
			    float e_x = e.GetX ();
			    float e_y = e.GetY ();

			    float xdiff = (prevx - e_x);
			    float ydiff = (prevy - e_y);
				xangle = xangle + ydiff/200;
				yangle = yangle + xdiff/200;
			    prevx = e_x;
			    prevy = e_y;
			}
			if (System.Math.Abs (downx - e.GetX ()) > 5 || System.Math.Abs (downy - e.GetY ()) > 5)
				touch = false;
			if (e.Action == MotionEventActions.Move)
				SetupProjection ();
			else if (e.Action == MotionEventActions.Up && touch)
				ToggleTexture ();

			RenderCube ();

			return true;
		}

		void ToggleTexture ()
		{
			UseTexture = !UseTexture;
			currentProgram = UseTexture ? programTexture : programPlain;
		}

		protected override void OnUnload (EventArgs e)
		{
			GL.DeleteTexture (textureId);
		}

		void RenderCube ()
		{
			MakeCurrent ();
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Use shader program.
			GL.UseProgram (currentProgram);

			// Update uniform value.
			GL.UniformMatrix4 (uniforms [UNIFORM_PROJECTION], false, ref projection);
			GL.UniformMatrix4 (uniforms [UNIFORM_VIEW], false, ref view);
			GL.UniformMatrix4 (uniforms [UNIFORM_NORMAL_MATRIX], false, ref normalMatrix);
			GL.Uniform3 (uniforms [UNIFORM_LIGHT], 25f, 25f, 28f);

			DrawModel ();

			SwapBuffers ();
		}

		internal void DrawModel ()
		{
			GL.ActiveTexture (TextureUnit.Texture0);
			GL.BindTexture (TextureTarget.Texture2D, textureId);
			GL.Uniform1 (uniforms [UNIFORM_TEXTURE], 0);

			// Update attribute values.
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.VertexAttribPointer (ATTRIB_VERTEX, 3, VertexAttribPointerType.Float, false, sizeof(float)*8, IntPtr.Zero);
			GL.EnableVertexAttribArray (ATTRIB_VERTEX);

			GL.VertexAttribPointer (ATTRIB_NORMAL, 3, VertexAttribPointerType.Float, false, sizeof(float)*8, new IntPtr (sizeof (float)*3));
			GL.EnableVertexAttribArray (ATTRIB_NORMAL);

			GL.VertexAttribPointer (ATTRIB_TEXCOORD, 3, VertexAttribPointerType.Float, false, sizeof(float)*8, new IntPtr (sizeof (float)*6));
			GL.EnableVertexAttribArray (ATTRIB_TEXCOORD);

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vbi);
			GL.DrawElementsInstanced (PrimitiveType.Triangles, CubeModel.faceIndexes.Length, DrawElementsType.UnsignedShort, IntPtr.Zero, 24);

			GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			GL.DeleteTexture (textureId);
		}

		public static float ToRadians (float degrees)
		{
			//pi/180
			//FIXME: precalc pi/180
			return (float) (degrees * (System.Math.PI/180.0));
		}


		void LoadTexture (Context context, int resourceId, int tex_id)
		{
			GL.BindTexture (TextureTarget.Texture2D, tex_id);

			// setup texture parameters
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.NearestMipmapLinear);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			Bitmap b = BitmapFactory.DecodeResource (context.Resources, resourceId);

			Android.Opengl.GLUtils.TexImage2D ((int)All.Texture2D, 0, b, 0); 
			b.Recycle ();
			GL.GenerateMipmap (TextureTarget.Texture2D);
		}

		internal Matrix4 view = new Matrix4 ();
		internal Matrix4 normalMatrix = new Matrix4 ();
		internal Matrix4 projection = new Matrix4 ();

		int programTexture, programPlain, currentProgram;

		internal void SetupProjection ()
		{
			if (Width <= 0 || Height <= 0)
				return;

			Matrix4 model = Matrix4.Mult (Matrix4.CreateRotationX (-xangle), Matrix4.CreateRotationZ (-yangle));

			float aspect = (float)Width / Height;
			if (aspect > 1) {
				Matrix4 scale = Matrix4.Scale (aspect);
				model = Matrix4.Mult (model, scale);
			}
			view = Matrix4.Mult (model, Matrix4.LookAt (0, -70, 5, 0, 10, 0, 0, 1, 0));
			GL.Viewport (0, 0, Width, Height);
			projection = Matrix4.CreatePerspectiveFieldOfView (OpenTK.MathHelper.DegreesToRadians (42.0f), aspect, 1.0f, 200.0f);
			projection = Matrix4.Mult (view, projection);
			normalMatrix = Matrix4.Invert (view);
			normalMatrix.Transpose ();
		}

		internal bool LoadShaders (string vertShaderSource, string fragShaderSource, out int program)
		{
			int vertShader, fragShader;

			// Create shader program.
			program = GL.CreateProgram ();

			// Create and compile vertex shader.
			if (!CompileShader (ShaderType.VertexShader, vertShaderSource, out vertShader)) {
				Console.WriteLine ("Failed to compile vertex shader");
				return false;
			}
			// Create and compile fragment shader.
			if (!CompileShader (ShaderType.FragmentShader, fragShaderSource, out fragShader)) {
				Console.WriteLine ("Failed to compile fragment shader");
				return false;
			}

			// Attach vertex shader to program.
			GL.AttachShader (program, vertShader);

			// Attach fragment shader to program.
			GL.AttachShader (program, fragShader);

			// Bind attribute locations.
			// This needs to be done prior to linking.
			GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
			GL.BindAttribLocation (program, ATTRIB_NORMAL, "normal");
			GL.BindAttribLocation (program, ATTRIB_TEXCOORD, "texcoord");

			// Link program.
			if (!LinkProgram (program)) {
				Console.WriteLine ("Failed to link program: {0:x}", program);

				if (vertShader != 0)
					GL.DeleteShader (vertShader);

				if (fragShader != 0)
					GL.DeleteShader (fragShader);

				if (program != 0) {
					GL.DeleteProgram (program);
					program = 0;
				}
				return false;
			}

			// Get uniform locations.
			uniforms [UNIFORM_PROJECTION] = GL.GetUniformLocation (program, "projection");
			uniforms [UNIFORM_VIEW] = GL.GetUniformLocation (program, "view");
			uniforms [UNIFORM_NORMAL_MATRIX] = GL.GetUniformLocation (program, "normalMatrix");
			uniforms [UNIFORM_TEXTURE] = GL.GetUniformLocation (program, "text");
			uniforms [UNIFORM_TEX_DEPTH] = GL.GetUniformLocation (program, "texDepth");
			uniforms [UNIFORM_LIGHT] = GL.GetUniformLocation (program, "light");

			// Release vertex and fragment shaders.
			if (vertShader != 0) {
				GL.DetachShader (program, vertShader);
				GL.DeleteShader (vertShader);
			}

			if (fragShader != 0) {
				GL.DetachShader (program, fragShader);
				GL.DeleteShader (fragShader);
			}

			return true;
		}

		internal void DestroyShaders ()
		{
			if (programTexture != 0) {
				GL.DeleteProgram (programTexture);
				programTexture = 0;
			}
			if (programPlain != 0) {
				GL.DeleteProgram (programPlain);
				programPlain = 0;
			}
		}

		static bool CompileShader (ShaderType type, string src, out int shader)
		{
			shader = GL.CreateShader (type);
			GL.ShaderSource (shader, src);
			GL.CompileShader (shader);

			#if DEBUG || true
			int logLength = 0;
			GL.GetShader (shader, ShaderParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				//GL.GetShaderInfoLog (shader, logLength, out logLength, infoLog);
				Console.WriteLine ("Shader compile log:\n{0}", GL.GetShaderInfoLog (shader));
			}
			#endif

			int status = 0;
			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);
			if (status == 0) {
				GL.DeleteShader (shader);
				return false;
			}

			return true;
		}

		internal static bool LinkProgram (int prog)
		{
			GL.LinkProgram (prog);

			#if DEBUG
			int logLength = 0;
			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
			if (logLength > 0)
				Console.WriteLine ("Program link log:\n{0}", GL.GetProgramInfoLog (prog));
			#endif
			int status = 0;
			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
			if (status == 0)
				return false;

			return true;
		}

		static void CheckGLError ()
		{
			ErrorCode code = GL.GetErrorCode ();
			if (code != ErrorCode.NoError) {
				Console.WriteLine ("GL Error {0}", code);
			}
		}

		static bool ValidateProgram (int prog)
		{
			GL.ValidateProgram (prog);
			CheckGLError ();

			int logLength = 0;
			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
			CheckGLError ();
			if (logLength > 0) {
				var infoLog = new System.Text.StringBuilder (logLength);
				GL.GetProgramInfoLog (prog, logLength, out logLength, infoLog);
				//Console.WriteLine ("Program validate log:\n{0}", infoLog);
			}

			int status = 0;
			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
			CheckGLError ();
			if (status == 0)
				return false;

			return true;
		}
	}
}
