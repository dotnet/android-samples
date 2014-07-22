using System;
using System.Text;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Content;
using Android.Util;
using System.Threading.Tasks;
using Android.Graphics;
using System.Threading;

namespace GLSharedContext20
{
	class PaintingView : AndroidGameView
	{
		const int ATTRIB_VERTEX = 0;
		const int ATTRIB_TEXCOORD = 1;

		IGraphicsContext backgroundContext;
		Bitmap bitmap;
		object lockobject = new object();
		int program;
		int uniformTextureLocation;
		int textureid;

		/// <summary>
		/// A Queue of Actions which should be run in the primary graphihcs context
		/// in the situation where secondary contexts are not supported.
		/// </summary>
		Queue<Action> actions = new Queue<Action>();

		public PaintingView (Context context) : base (context)
		{
			Initialize ();
		}

		public PaintingView (Context context, IAttributeSet attrs) 
			: base (context, attrs)
		{
			Initialize ();
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			Initialize ();
		}

		void Initialize() {
			AutoSetContextOnRenderFrame = false;
		}

		// This gets called when the drawing surface is ready
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			try {
				// Clear the current Context
				GraphicsContext.MakeCurrent (null);
				// Create a secondary context using the same information the primary 
				// context was created with
				backgroundContext = new AndroidGraphicsContext(GraphicsMode, WindowInfo, GraphicsContext, ContextRenderingApi, GraphicsContextFlags.Embedded);
			}catch {
				// secondary context not supported
				backgroundContext = null;
			}

			MakeCurrent();

			var vertexShader = LoadShader(ShaderType.VertexShader, vertexShaderCode);
			var fragmentShader = LoadShader(ShaderType.FragmentShader, fragmentShaderCode);

			program = GL.CreateProgram ();             // create empty OpenGL Program
			GL.AttachShader (program, vertexShader);   // add the vertex shader to program
			GL.AttachShader (program, fragmentShader); // add the fragment shader to program

			GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
			GL.BindAttribLocation (program, ATTRIB_TEXCOORD, "texcoord");

			GL.LinkProgram (program);                  // create OpenGL program executables

			uniformTextureLocation = GL.GetUniformLocation (program, "texture");

			if (vertexShader != 0) {
				GL.DetachShader (program, vertexShader);
				GL.DeleteShader (vertexShader);
			}

			if (fragmentShader != 0) {
				GL.DetachShader (program, fragmentShader);
				GL.DeleteShader (fragmentShader);
			}

			GL.Viewport (0, 0, Width, Height);

			// Run the render loop
			Run ();

			Task.Factory.StartNew (() => {
				//Thread.Sleep(500);
				// load the bitmap 
				bitmap = BitmapFactory.DecodeResource (Context.Resources, Resource.Drawable.f_spot);

				// the device may or may not support a background Context. But rather than 
				// duplicating this code we just create an Action which we can invoke on this
				// background thread later or queue to be executed on the rendering thread.
				Action acton = new Action (() => {
					GL.Enable (EnableCap.Texture2D);
					GL.GenTextures(1, out textureid);
					GL.ActiveTexture (TextureUnit.Texture0);
					GL.BindTexture (TextureTarget.Texture2D, textureid);
					// setup texture parameters
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
					Android.Opengl.GLUtils.TexImage2D ((int)TextureTarget.Texture2D, 0, bitmap, 0); 
					// make sure the texture is pushed to the GPU.
					GL.Flush();

					// make sure we free resources
					bitmap.Recycle();
					bitmap.Dispose();
					bitmap = null;
				});


				// take a lock so the main rendering thread does not try to draw anything
				// there are other ways to do this, but its is probably the simplest
				lock (lockobject) {
					if (backgroundContext != null) {
						// Clear the current context bound to the Display 
						backgroundContext.MakeCurrent (null);
						// make this context active
						backgroundContext.MakeCurrent (WindowInfo);
						// do our processing
						acton.Invoke ();
						// clear the current context again so we don't error on the main thread
						backgroundContext.MakeCurrent (null);
					} else {
						// Secondary Context's are not supported on this device
						// queue the action for execution later.
						actions.Enqueue (acton);
					}
				}

			});
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		//
		// In this particular case, we demonstrate how to set
		// the graphics mode and fallback in case the device doesn't
		// support the defaults
		protected override void CreateFrameBuffer ()
		{
			ContextRenderingApi = GLVersion.ES2;
			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("GLSharedContext20", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLSharedContext20", "{0}", ex);
			}

			// this is a graphics setting that sets everything to the lowest mode possible so
			// the device returns a reliable graphics setting.
			try {
				Log.Verbose ("GLSharedContext20", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (0, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLSharedContext20", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}

		// This gets called on each frame render
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			// you only need to call this if you have delegates
			// registered that you want to have called
			base.OnRenderFrame (e);

			lock (lockobject) {

				MakeCurrent ();

				GL.ClearColor (0.5f, 0.5f, 0.5f, 1.0f);
				GL.Clear (ClearBufferMask.ColorBufferBit);

				if (textureid > 0) {
					GL.ActiveTexture (TextureUnit.Texture0);
					GL.BindTexture (TextureTarget.Texture2D, textureid);
					GL.UseProgram (program);

					GL.Uniform1 (uniformTextureLocation, 0);

					GL.EnableVertexAttribArray (ATTRIB_VERTEX);
					GL.EnableVertexAttribArray (ATTRIB_TEXCOORD);

					GL.VertexAttribPointer (ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), square_vertices);
					GL.VertexAttribPointer (ATTRIB_TEXCOORD, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), square_texcoords);

					GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);
					GL.DisableVertexAttribArray (ATTRIB_VERTEX);
					GL.DisableVertexAttribArray (ATTRIB_TEXCOORD);
				}

				SwapBuffers ();

				// Process any pending actions for the primary context.
				while (actions.Count > 0) {
					var action = actions.Dequeue ();
					action.Invoke ();
				}
			}
		}

		float[] square_vertices = {
			-0.5f, -0.5f,
			0.5f, -0.5f,
			-0.5f, 0.5f, 
			0.5f, 0.5f,
		};

		float[] square_texcoords = {
			1, 0,
			0, 0,
			1, 1, 
			0, 1,
		};

		string vertexShaderCode = @"
attribute vec2 position;
attribute vec2 texcoord;

varying vec2 textureCoordinate;

void main() {
	// the matrix must be included as a modifier of gl_Position
	gl_Position = vec4(position, 0.0, 1.0);
	textureCoordinate = texcoord;
}
";

		string fragmentShaderCode = @"
precision mediump float;
varying vec2 textureCoordinate;
uniform sampler2D texture;
void main() {
	gl_FragColor = texture2D (texture, textureCoordinate);
}
";

		int LoadShader (ShaderType type, string source)
		{
			int shader = GL.CreateShader (type);
			if (shader == 0)
				throw new InvalidOperationException ("Unable to create shader");

			int length = 0;
			GL.ShaderSource (shader, 1, new string [] {source}, (int[])null);
			GL.CompileShader (shader);

			int compiled = 0;
			GL.GetShader (shader, ShaderParameter.CompileStatus, out compiled);
			if (compiled == 0) {
				length = 0;
				GL.GetShader (shader, ShaderParameter.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetShaderInfoLog (shader, length, out length, log);
					System.Diagnostics.Debug.WriteLine (log);
				}

				GL.DeleteShader (shader);
				throw new InvalidOperationException ("Unable to compile shader of type : " + type.ToString ());
			}

			return shader;
		}
	}
}

